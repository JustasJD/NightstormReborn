# ?? Phase 0: Critical Fixes - Implementation Plan

**Status:** ?? IN PROGRESS  
**Started:** Now  
**Estimated Time:** ~16 hours  
**Goal:** Fix 5 critical architectural flaws before entity creation

---

## ?? Implementation Checklist

### **Fix 1: Optimistic Concurrency Control** ?? ~2 hours
- [x] Add `RowVersion` to BaseEntity
- [ ] Update EF Core configurations
- [ ] Add migration for RowVersion
- [ ] Update Repository pattern to handle concurrency exceptions
- [ ] Add retry logic for concurrent updates

### **Fix 2: Redis Distributed Locking** ?? ~4 hours
- [ ] Install StackExchange.Redis package (already installed)
- [ ] Create `IDistributedLockService` interface
- [ ] Implement `RedisDistributedLockService` with RedLock algorithm
- [ ] Add lock acquisition helpers
- [ ] Add unit tests for distributed locking

### **Fix 3: Parallel Combat Processing** ?? ~3 hours
- [ ] Create `ICombatOrchestratorService` interface
- [ ] Implement parallel processing with SemaphoreSlim
- [ ] Add combat turn batching
- [ ] Add performance metrics
- [ ] Add integration tests

### **Fix 4: Transaction Management** ?? ~4 hours
- [ ] Create `ITransactionManager` interface
- [ ] Implement transaction wrapper service
- [ ] Add retry queue for failed transactions
- [ ] Update all state-changing operations to use transactions
- [ ] Add idempotency checks

### **Fix 5: Quartz.NET Persistence** ?? ~3 hours
- [ ] Install Quartz.Extensions.DependencyInjection
- [ ] Install Quartz.Serialization.Json
- [ ] Configure Quartz with PostgreSQL persistence
- [ ] Create job recovery service (reschedule missing jobs on startup)
- [ ] Add health checks for Quartz scheduler

---

## ??? Files to Create

### **Nightstorm.Core**
```
Entities/
  ??? BaseEntity.cs (UPDATE - add RowVersion)

Interfaces/Services/
  ??? IDistributedLockService.cs (NEW)
  ??? ITransactionManager.cs (NEW)
  ??? ICombatOrchestratorService.cs (NEW)
  ??? IRetryQueueService.cs (NEW)

Exceptions/
  ??? ConcurrencyException.cs (NEW)
  ??? DistributedLockException.cs (NEW)
```

### **Nightstorm.Data**
```
Services/
  ??? RedisDistributedLockService.cs (NEW)
  ??? TransactionManager.cs (NEW)
  ??? RetryQueueService.cs (NEW)

Migrations/
  ??? YYYYMMDD_AddRowVersionToConcurrentEntities.cs (NEW)
```

### **Nightstorm.API**
```
Program.cs (UPDATE - register new services)
```

---

## ?? Implementation Details

### **Fix 1: Optimistic Concurrency Control**

#### **Step 1.1: Update BaseEntity**
```csharp
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    
    /// <summary>
    /// Row version for optimistic concurrency control.
    /// Automatically updated by database on every modification.
    /// </summary>
    [Timestamp]
    public byte[] RowVersion { get; set; } = Array.Empty<byte>(); // NEW
}
```

#### **Step 1.2: Update Repository Pattern**
```csharp
public interface IRepository<T> where T : BaseEntity
{
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    // Add retry logic in implementation
}

// Implementation
public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
{
    const int maxRetries = 3;
    int retryCount = 0;
    
    while (retryCount < maxRetries)
    {
        try
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync(cancellationToken);
            return entity;
        }
        catch (DbUpdateConcurrencyException ex)
        {
            retryCount++;
            if (retryCount >= maxRetries)
            {
                throw new ConcurrencyException(
                    $"Failed to update {typeof(T).Name} after {maxRetries} retries", ex);
            }
            
            // Reload from database and retry
            var entry = ex.Entries.Single();
            await entry.ReloadAsync(cancellationToken);
        }
    }
    
    throw new InvalidOperationException("Should never reach here");
}
```

---

### **Fix 2: Redis Distributed Locking**

#### **Step 2.1: Create Interface**
```csharp
public interface IDistributedLockService
{
    /// <summary>
    /// Acquires a distributed lock with the specified key.
    /// Returns null if lock cannot be acquired within timeout.
    /// </summary>
    Task<IDistributedLock?> AcquireLockAsync(
        string key, 
        TimeSpan timeout, 
        CancellationToken cancellationToken = default);
}

public interface IDistributedLock : IAsyncDisposable
{
    string Key { get; }
    DateTime AcquiredAt { get; }
    TimeSpan Duration { get; }
}
```

#### **Step 2.2: Implement RedLock**
```csharp
public class RedisDistributedLockService : IDistributedLockService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisDistributedLockService> _logger;
    
    public async Task<IDistributedLock?> AcquireLockAsync(
        string key, 
        TimeSpan timeout, 
        CancellationToken cancellationToken = default)
    {
        var lockKey = $"lock:{key}";
        var lockValue = Guid.NewGuid().ToString();
        var db = _redis.GetDatabase();
        
        var acquired = await db.StringSetAsync(
            lockKey, 
            lockValue, 
            timeout, 
            When.NotExists);
        
        if (!acquired)
        {
            _logger.LogWarning($"Failed to acquire lock for key: {key}");
            return null;
        }
        
        return new RedisDistributedLock(db, lockKey, lockValue, timeout, _logger);
    }
}

internal class RedisDistributedLock : IDistributedLock
{
    private readonly IDatabase _db;
    private readonly string _key;
    private readonly string _value;
    private readonly ILogger _logger;
    
    public string Key { get; }
    public DateTime AcquiredAt { get; }
    public TimeSpan Duration { get; }
    
    public async ValueTask DisposeAsync()
    {
        try
        {
            // Only delete if we still own the lock (compare value)
            var script = @"
                if redis.call('get', KEYS[1]) == ARGV[1] then
                    return redis.call('del', KEYS[1])
                else
                    return 0
                end";
            
            await _db.ScriptEvaluateAsync(
                script, 
                new RedisKey[] { _key }, 
                new RedisValue[] { _value });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to release lock: {_key}");
        }
    }
}
```

---

### **Fix 3: Parallel Combat Processing**

#### **Step 3.1: Create Orchestrator Interface**
```csharp
public interface ICombatOrchestratorService
{
    /// <summary>
    /// Processes all active combats in parallel with throttling.
    /// </summary>
    Task ProcessActiveCombatsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Processes a single combat turn.
    /// </summary>
    Task ProcessCombatTurnAsync(Guid combatId, CancellationToken cancellationToken = default);
}
```

#### **Step 3.2: Implement Parallel Processing**
```csharp
public class CombatOrchestratorService : ICombatOrchestratorService
{
    private readonly SemaphoreSlim _combatSemaphore = new(10); // Max 10 concurrent combats
    
    public async Task ProcessActiveCombatsAsync(CancellationToken cancellationToken)
    {
        var activeCombats = await GetActiveCombatsAsync();
        
        // Process in parallel batches
        var tasks = activeCombats.Select(combat => 
            ProcessWithThrottlingAsync(combat.Id, cancellationToken));
        
        await Task.WhenAll(tasks);
    }
    
    private async Task ProcessWithThrottlingAsync(Guid combatId, CancellationToken cancellationToken)
    {
        await _combatSemaphore.WaitAsync(cancellationToken);
        try
        {
            await ProcessCombatTurnAsync(combatId, cancellationToken);
        }
        finally
        {
            _combatSemaphore.Release();
        }
    }
}
```

---

### **Fix 4: Transaction Management**

#### **Step 4.1: Create Transaction Manager**
```csharp
public interface ITransactionManager
{
    Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> action, 
        CancellationToken cancellationToken = default);
    
    Task ExecuteInTransactionAsync(
        Func<Task> action, 
        CancellationToken cancellationToken = default);
}

public class TransactionManager : ITransactionManager
{
    private readonly RpgContext _context;
    private readonly IRetryQueueService _retryQueue;
    private readonly ILogger<TransactionManager> _logger;
    
    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<Task<T>> action, 
        CancellationToken cancellationToken = default)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var result = await action();
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Transaction failed, rolling back");
            throw;
        }
    }
}
```

---

### **Fix 5: Quartz.NET Persistence**

#### **Step 5.1: Configure Quartz**
```csharp
// Program.cs
services.AddQuartz(q =>
{
    q.UsePersistentStore(store =>
    {
        store.UsePostgres(postgresOptions =>
        {
            postgresOptions.ConnectionString = connectionString;
            postgresOptions.TablePrefix = "qrtz_";
        });
        store.UseJsonSerializer();
        store.UseClustering();
    });
    
    q.UseDefaultThreadPool(tp =>
    {
        tp.MaxConcurrency = 10;
    });
});

services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});
```

#### **Step 5.2: Job Recovery Service**
```csharp
public class QuartzJobRecoveryService : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for scheduler to start
        await Task.Delay(5000, stoppingToken);
        
        // Verify all 81 zones have scheduled jobs
        var zones = await _zoneRepository.GetAllAsync();
        var scheduledJobs = await _scheduler.GetJobKeys(
            GroupMatcher<JobKey>.GroupEquals("nightstorm-group"));
        
        foreach (var zone in zones)
        {
            var jobKey = new JobKey($"nightstorm_{zone.Id}", "nightstorm-group");
            if (!scheduledJobs.Contains(jobKey))
            {
                _logger.LogWarning($"Missing Nightstorm job for zone {zone.ZoneId}, rescheduling...");
                await RescheduleNightstormAsync(zone.Id);
            }
        }
    }
}
```

---

## ?? Testing Strategy

### **Unit Tests**
- [ ] Concurrency exception handling
- [ ] Distributed lock acquisition/release
- [ ] Combat parallel processing throttling
- [ ] Transaction rollback scenarios

### **Integration Tests**
- [ ] Concurrent updates to same entity (race condition)
- [ ] Multiple processes acquiring same lock
- [ ] Combat processing with 50+ simultaneous combats
- [ ] Transaction failure and retry

### **Load Tests**
- [ ] 1000 concurrent travel initiations
- [ ] 100 simultaneous combat registrations
- [ ] 50 active combats processing in parallel

---

## ?? Success Criteria

### **Fix 1: Concurrency Control**
- ? No data corruption under concurrent load
- ? Automatic retry on concurrency conflicts
- ? All entities have RowVersion tracking

### **Fix 2: Distributed Locking**
- ? Only 10 players max can register for combat
- ? No race conditions on critical operations
- ? Lock automatically released on timeout

### **Fix 3: Parallel Combat**
- ? 50 combats process in 2-3 seconds (not 100s)
- ? Throttling prevents database overload
- ? No deadlocks or resource exhaustion

### **Fix 4: Transactions**
- ? Reward distribution is atomic
- ? Failed transactions don't corrupt state
- ? Retry queue handles transient failures

### **Fix 5: Quartz Persistence**
- ? Jobs survive server restart
- ? Missing jobs automatically rescheduled
- ? All 81 zones always have scheduled events

---

## ?? Implementation Order

1. **Fix 1** (Concurrency) - Foundation for all others
2. **Fix 2** (Locking) - Required for combat registration
3. **Fix 4** (Transactions) - Required for combat completion
4. **Fix 3** (Parallel Combat) - Performance optimization
5. **Fix 5** (Quartz) - Last, depends on entities existing

---

**Let's begin with Fix 1! ??**
