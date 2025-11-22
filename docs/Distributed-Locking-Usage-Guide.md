# ?? Distributed Locking - Usage Guide

## Overview

**RedisDistributedLockService** prevents race conditions in critical sections using Redis-backed locks. This ensures that only one process can execute a critical operation at a time, even across multiple servers.

---

## ?? When to Use Distributed Locking

### **Use Cases:**

| Scenario | Why Lock Needed | Without Lock | With Lock |
|----------|----------------|--------------|-----------|
| **Combat Registration** | Max 10 players | 11+ players register | Only 10 succeed |
| **Treasury Withdrawal** | Once per day per guild | Multiple withdrawals | Single withdrawal |
| **Item Auction Bidding** | Highest bidder wins | Duplicate bids | Serialized bids |
| **Resource Allocation** | Limited resources | Over-allocation | Guaranteed limit |

### **Don't Use For:**
- ? Simple database updates (use optimistic concurrency instead)
- ? Read-only operations (no conflicts possible)
- ? Operations that can handle duplicates
- ? High-frequency operations (performance cost)

---

## ?? Basic Usage

### **Example 1: Combat Registration**

```csharp
public class CombatRegistrationService
{
    private readonly IDistributedLockService _lockService;
    private readonly INightstormEventRepository _eventRepository;
    private readonly ILogger<CombatRegistrationService> _logger;

    public async Task<RegistrationResult> RegisterForCombatAsync(
        Guid eventId, 
        Guid characterId)
    {
        // Acquire lock for this specific event
        await using var lockHandle = await _lockService.AcquireLockAsync(
            $"combat:registration:{eventId}",
            TimeSpan.FromSeconds(5)); // Lock expires after 5 seconds

        if (lockHandle == null)
        {
            _logger.LogWarning(
                "Could not acquire lock for combat registration: EventId={EventId}",
                eventId);
            
            return new RegistrationResult
            {
                Success = false,
                Message = "System busy, please try again"
            };
        }

        // Lock acquired - we have exclusive access
        var registeredCount = await GetRegisteredCountAsync(eventId);

        if (registeredCount >= 10)
        {
            _logger.LogInformation(
                "Combat full: EventId={EventId}, Registered={Count}",
                eventId, registeredCount);
            
            return new RegistrationResult
            {
                Success = false,
                Message = "Combat is full (10/10 players)"
            };
        }

        // Safe to register - we hold the lock
        await AddParticipantAsync(eventId, characterId);

        _logger.LogInformation(
            "Player registered for combat: EventId={EventId}, CharacterId={CharacterId}, Position={Position}",
            eventId, characterId, registeredCount + 1);

        return new RegistrationResult
        {
            Success = true,
            Position = registeredCount + 1,
            Message = $"Registered! Position {registeredCount + 1}/10"
        };
        
        // Lock automatically released here when lockHandle disposes
    }
}
```

### **Example 2: Treasury Withdrawal (Daily Limit)**

```csharp
public class GuildTreasuryService
{
    private readonly IDistributedLockService _lockService;
    
    public async Task<WithdrawalResult> WithdrawDailyTreasuryAsync(
        Guid guildId,
        Guid zoneId)
    {
        // Lock for this specific guild + zone combination
        await using var lockHandle = await _lockService.AcquireLockAsync(
            $"treasury:withdrawal:{guildId}:{zoneId}",
            TimeSpan.FromSeconds(10));

        if (lockHandle == null)
        {
            return new WithdrawalResult
            {
                Success = false,
                Message = "Another withdrawal in progress"
            };
        }

        // Check if already withdrawn today
        var lastWithdrawal = await GetLastWithdrawalAsync(guildId, zoneId);
        
        if (lastWithdrawal.HasValue && 
            lastWithdrawal.Value.Date == DateTime.UtcNow.Date)
        {
            return new WithdrawalResult
            {
                Success = false,
                Message = "Already withdrawn today. Try again tomorrow."
            };
        }

        // Perform withdrawal
        var amount = await PerformWithdrawalAsync(guildId, zoneId);

        return new WithdrawalResult
        {
            Success = true,
            Amount = amount,
            Message = $"Withdrawn {amount} gold from zone treasury"
        };
    }
}
```

---

## ?? Advanced Usage

### **With Retry Logic**

```csharp
public async Task<bool> TryRegisterWithRetryAsync(
    Guid eventId,
    Guid characterId)
{
    // Try to acquire lock with automatic retries
    await using var lockHandle = await _lockService.AcquireLockWithRetryAsync(
        $"combat:registration:{eventId}",
        timeout: TimeSpan.FromSeconds(5),
        retryAttempts: 3,
        retryDelay: TimeSpan.FromMilliseconds(100));

    if (lockHandle == null)
    {
        _logger.LogWarning(
            "Failed to acquire lock after 3 retries: EventId={EventId}",
            eventId);
        return false;
    }

    // Lock acquired, proceed with registration
    await RegisterPlayerAsync(eventId, characterId);
    return true;
}
```

### **Checking Lock Validity**

```csharp
public async Task<bool> PerformLongOperationAsync(Guid resourceId)
{
    await using var lockHandle = await _lockService.AcquireLockAsync(
        $"resource:{resourceId}",
        TimeSpan.FromSeconds(30));

    if (lockHandle == null)
        return false;

    // Part 1 of operation
    await Step1Async();

    // Check if lock is still valid before continuing
    if (!lockHandle.IsValid)
    {
        _logger.LogWarning("Lock expired during operation");
        return false;
    }

    // Part 2 of operation
    await Step2Async();

    return true;
}
```

---

## ?? Important Considerations

### **Lock Timeout Selection**

| Operation Type | Recommended Timeout | Reason |
|----------------|-------------------|--------|
| **Quick DB query** | 2-5 seconds | Should complete quickly |
| **Multiple DB operations** | 5-10 seconds | Needs more time |
| **External API calls** | 10-30 seconds | Network delays possible |
| **Long processing** | 30-60 seconds | May need extension |

**Rule of Thumb:** Timeout should be **2-3x** the expected operation time.

### **Lock Granularity**

```csharp
// ? BAD: Too coarse - locks entire system
await _lockService.AcquireLockAsync("combat", timeout);

// ? GOOD: Fine-grained - locks specific event
await _lockService.AcquireLockAsync($"combat:registration:{eventId}", timeout);

// ? BETTER: Very specific - locks specific operation on specific resource
await _lockService.AcquireLockAsync($"treasury:withdrawal:{guildId}:{zoneId}", timeout);
```

### **Deadlock Prevention**

```csharp
// ? DANGER: Can cause deadlock if locks acquired in different order
async Task ProcessA()
{
    await using var lock1 = await _lockService.AcquireLockAsync("resourceA", ...);
    await using var lock2 = await _lockService.AcquireLockAsync("resourceB", ...);
    // Process...
}

async Task ProcessB()
{
    await using var lock2 = await _lockService.AcquireLockAsync("resourceB", ...); // Different order!
    await using var lock1 = await _lockService.AcquireLockAsync("resourceA", ...);
    // Process...
}

// ? SAFE: Always acquire locks in same order
async Task ProcessSafe()
{
    var keys = new[] { "resourceA", "resourceB" }.OrderBy(k => k).ToArray();
    
    await using var lock1 = await _lockService.AcquireLockAsync(keys[0], ...);
    await using var lock2 = await _lockService.AcquireLockAsync(keys[1], ...);
    // Process...
}
```

---

## ?? Testing Distributed Locks

### **Unit Test: Lock Acquisition**

```csharp
[Test]
public async Task AcquireLockAsync_FirstAcquisition_ShouldSucceed()
{
    // Arrange
    var lockService = new RedisDistributedLockService(_redis, _logger);

    // Act
    await using var lockHandle = await lockService.AcquireLockAsync(
        "test-lock",
        TimeSpan.FromSeconds(5));

    // Assert
    Assert.NotNull(lockHandle);
    Assert.True(lockHandle.IsValid);
    Assert.Equal("lock:test-lock", lockHandle.Key);
}
```

### **Unit Test: Lock Contention**

```csharp
[Test]
public async Task AcquireLockAsync_AlreadyLocked_ShouldReturnNull()
{
    // Arrange
    var lockService = new RedisDistributedLockService(_redis, _logger);
    
    await using var lock1 = await lockService.AcquireLockAsync(
        "test-lock",
        TimeSpan.FromSeconds(5));

    // Act
    var lock2 = await lockService.AcquireLockAsync(
        "test-lock",
        TimeSpan.FromSeconds(5));

    // Assert
    Assert.NotNull(lock1);
    Assert.Null(lock2); // Second acquisition should fail
}
```

### **Integration Test: Concurrent Registration**

```csharp
[Test]
public async Task CombatRegistration_Concurrent_OnlyTenPlayersSucceed()
{
    // Arrange: 20 players try to register simultaneously
    var players = Enumerable.Range(1, 20)
        .Select(i => Guid.NewGuid())
        .ToList();

    var eventId = Guid.NewGuid();

    // Act: All try to register at once
    var tasks = players.Select(playerId =>
        RegisterForCombatAsync(eventId, playerId));
    
    var results = await Task.WhenAll(tasks);

    // Assert: Exactly 10 should succeed
    var successCount = results.Count(r => r.Success);
    Assert.Equal(10, successCount);
    
    _logger.LogInformation(
        "Concurrent registration test: {Success} succeeded, {Failed} failed",
        successCount, 20 - successCount);
}
```

---

## ?? Performance Characteristics

### **Operation Latency:**

| Operation | Typical Latency | Notes |
|-----------|----------------|-------|
| **Lock acquisition (success)** | 1-5ms | Single Redis SET NX |
| **Lock acquisition (failure)** | 1-5ms | Redis returns false immediately |
| **Lock release** | 1-5ms | Lua script execution |
| **Lock with retry (3 attempts)** | 100-400ms | Includes backoff delays |

### **Throughput:**

- Single Redis instance: **10,000-50,000 lock ops/sec**
- With network latency: **1,000-5,000 lock ops/sec**
- Good enough for: **100-500 concurrent combats**

---

## ?? Monitoring and Debugging

### **Log Output Examples:**

```
# Successful lock acquisition
[Debug] Attempting to acquire lock: Key=lock:combat:registration:abc123, Timeout=5000ms
[Info] Successfully acquired distributed lock: Key=lock:combat:registration:abc123, Timeout=5000ms
[Debug] Distributed lock released successfully: Key=lock:combat:registration:abc123

# Lock contention
[Debug] Attempting to acquire lock: Key=lock:treasury:withdrawal:guild456:zoneA1, Timeout=10000ms
[Warning] Failed to acquire lock (already held by another process): Key=lock:treasury:withdrawal:guild456:zoneA1

# Lock expiration
[Warning] Distributed lock was already released or expired: Key=lock:combat:registration:abc123
```

### **Redis Commands for Debugging:**

```bash
# Check if lock exists
redis-cli GET lock:combat:registration:abc123

# Check lock TTL (time to live)
redis-cli TTL lock:combat:registration:abc123

# List all locks
redis-cli KEYS lock:*

# Force delete lock (use with caution!)
redis-cli DEL lock:combat:registration:abc123
```

---

## ? Best Practices

1. **Always use `await using`** - Ensures lock is released even on exceptions
2. **Set appropriate timeouts** - 2-3x expected operation time
3. **Use specific lock keys** - Include resource IDs in lock key
4. **Log lock operations** - Helps debugging contention issues
5. **Handle lock acquisition failure** - Always check if lock is null
6. **Keep critical sections short** - Minimize time holding lock
7. **Avoid nested locks** - Can cause deadlocks
8. **Use retry for transient failures** - Network blips shouldn't fail operations

---

## ?? Common Pitfalls

### **1. Forgetting to Check Null**

```csharp
// ? BAD: Will throw NullReferenceException if lock fails
await using var lock = await _lockService.AcquireLockAsync(...);
await DoSomethingCriticalAsync(); // Executes even if lock is null!

// ? GOOD: Always check
await using var lock = await _lockService.AcquireLockAsync(...);
if (lock == null)
{
    return Failure("Could not acquire lock");
}
await DoSomethingCriticalAsync();
```

### **2. Lock Timeout Too Short**

```csharp
// ? BAD: Operation takes 3 seconds but lock expires in 1 second
await using var lock = await _lockService.AcquireLockAsync("key", TimeSpan.FromSeconds(1));
await SlowOperationAsync(); // Takes 3 seconds - lock expires mid-operation!

// ? GOOD: Timeout is longer than operation
await using var lock = await _lockService.AcquireLockAsync("key", TimeSpan.FromSeconds(10));
await SlowOperationAsync(); // Lock valid for entire operation
```

### **3. Not Using Specific Keys**

```csharp
// ? BAD: All combat registrations use same lock
await using var lock = await _lockService.AcquireLockAsync("combat-registration", ...);

// ? GOOD: Each event has its own lock
await using var lock = await _lockService.AcquireLockAsync($"combat:registration:{eventId}", ...);
```

---

## ?? Related Documentation

- **Fix 1: Optimistic Concurrency** - Use for general updates
- **Fix 4: Transaction Management** - Combine with locks for multi-table operations
- **Architecture Critical Review** - Why distributed locking is needed

---

**Ready to use distributed locking to prevent race conditions! ??**
