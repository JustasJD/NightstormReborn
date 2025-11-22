# ?? Phase 0 Quick Start Guide

## ?? What Is This?

Phase 0 fixes **5 critical bugs** that would break the game in production:
1. Race conditions ? Data corruption
2. Combat overflow ? 11 players in 10-slot combat
3. Sequential processing ? Slow performance at scale
4. No transactions ? Lost rewards on crashes
5. Lost schedules ? Nightstorms disappear after restart

---

## ? Current Status (30% Complete)

```
? Architecture reviewed (18 flaws found)
? Interfaces designed (IDistributedLockService, ITransactionManager)
? BaseEntity.RowVersion added (optimistic concurrency)
? Exception classes created (ConcurrencyException, DistributedLockException)
? Build successful (no breaking changes)

? EF Core migration needed (next)
? Repository implementation needed (next)
? Redis distributed locking needed (next)
```

---

## ?? Continue From Here

### **Option 1: Complete Fix 1 (Recommended)**

```bash
# 1. Create EF Core migration
cd src/Nightstorm.Data
dotnet ef migrations add AddRowVersionToConcurrentEntities ^
  --startup-project ../Nightstorm.API

# 2. Check the migration file
# It should add RowVersion column to all tables

# 3. Apply migration
dotnet ef database update --startup-project ../Nightstorm.API
```

### **Option 2: Read Documentation First**

1. Read `docs/Architecture-Critical-Review.md` - Understand the problems
2. Read `docs/Phase-0-Implementation-Plan.md` - See the full plan
3. Read `docs/Phase-0-Session-Summary.md` - Current progress

### **Option 3: Start Phase 1 (Not Recommended)**

If you want to skip Phase 0 and create entities first:
- ?? Risk: May need to refactor entities later to add concurrency
- ?? Risk: Race conditions will exist in production
- ? Benefit: Faster to get basic functionality working

---

## ?? Files Changed Today

### **Created (7 files):**
```
src/Nightstorm.Core/Interfaces/Services/
  ??? IDistributedLockService.cs
  ??? ITransactionManager.cs

src/Nightstorm.Core/Exceptions/
  ??? ConcurrencyException.cs
  ??? DistributedLockException.cs

docs/
  ??? Architecture-Critical-Review.md
  ??? Bot-Architecture-Security-Fix.md
  ??? Phase-0-Implementation-Plan.md
  ??? Phase-0-Progress-Report-1.md
  ??? Phase-0-Session-Summary.md
```

### **Modified (1 file):**
```
src/Nightstorm.Core/Entities/BaseEntity.cs
  ??? Added RowVersion property
```

---

## ?? Next Implementation Steps

### **Step 1: EF Core Migration** (~30 min)
```csharp
// This will be generated automatically
public partial class AddRowVersionToConcurrentEntities : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<byte[]>(
            name: "RowVersion",
            table: "Characters",
            rowVersion: true,
            nullable: false);
        
        // Repeat for all tables...
    }
}
```

### **Step 2: Update Repository** (~1 hour)
```csharp
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
                    $"Failed to update after {maxRetries} retries", 
                    typeof(T), 
                    entity.Id, 
                    retryCount, 
                    ex);
            }
            
            // Reload and retry
            var entry = ex.Entries.Single();
            await entry.ReloadAsync(cancellationToken);
            
            // Merge changes (or re-apply your logic)
            var databaseValues = entry.GetDatabaseValues();
            entry.OriginalValues.SetValues(databaseValues);
        }
    }
}
```

### **Step 3: Implement Distributed Locking** (~2 hours)

See `docs/Phase-0-Implementation-Plan.md` for full implementation.

---

## ?? How to Test

### **Test Concurrency:**
```csharp
[Test]
public async Task UpdateAsync_ConcurrentUpdates_ShouldHandleGracefully()
{
    // Arrange
    var character = await _repository.GetByIdAsync(characterId);
    
    // Act - Simulate two processes updating simultaneously
    var task1 = _repository.UpdateAsync(character);
    var task2 = _repository.UpdateAsync(character);
    
    // Assert - One should succeed, one should retry
    await Task.WhenAll(task1, task2);
    
    // Verify final state is consistent
    var updated = await _repository.GetByIdAsync(characterId);
    Assert.NotNull(updated.RowVersion);
}
```

### **Test Distributed Locking:**
```csharp
[Test]
public async Task AcquireLock_AlreadyLocked_ShouldReturnNull()
{
    // Arrange
    var lock1 = await _lockService.AcquireLockAsync("test-key", TimeSpan.FromSeconds(5));
    
    // Act
    var lock2 = await _lockService.AcquireLockAsync("test-key", TimeSpan.FromSeconds(5));
    
    // Assert
    Assert.NotNull(lock1);
    Assert.Null(lock2); // Cannot acquire same lock
}
```

---

## ?? Progress Tracking

| Fix | Estimated | Actual | Status |
|-----|-----------|--------|--------|
| Fix 1: Concurrency | 2h | 0.5h | ?? 30% |
| Fix 2: Locking | 4h | 0h | ? 0% |
| Fix 3: Parallel Combat | 3h | 0h | ? 0% |
| Fix 4: Transactions | 4h | 0h | ? 0% |
| Fix 5: Quartz | 3h | 0h | ? 0% |
| **Total** | **16h** | **0.5h** | **?? 3%** |

---

## ?? Key Concepts

### **Optimistic Concurrency:**
```
Process A reads Character (RowVersion=1)
Process B reads Character (RowVersion=1)

Process A updates: Level=11 (RowVersion becomes 2)
Process B updates: Gold=1000 (RowVersion=1 doesn't match!)
? Exception thrown, B retries with fresh data
```

### **Distributed Locking:**
```
Process A acquires lock "combat:123"
Process B tries to acquire same lock ? blocked
Process A completes operation, releases lock
Process B now acquires lock and proceeds
```

### **Transaction Management:**
```
BEGIN TRANSACTION
  Update combat status
  Distribute rewards
  Update player states
COMMIT -- All succeed

-- If any step fails:
ROLLBACK -- All changes undone
```

---

## ?? Common Pitfalls

### ? Don't Do This:
```csharp
// BAD: Direct update without checking concurrency
var entity = await _repository.GetByIdAsync(id);
entity.Level = 10;
await _context.SaveChangesAsync(); // May lose updates!
```

### ? Do This:
```csharp
// GOOD: Use repository method with retry logic
var entity = await _repository.GetByIdAsync(id);
entity.Level = 10;
await _repository.UpdateAsync(entity); // Handles concurrency automatically
```

---

## ?? Need Help?

### **Questions to Ask Yourself:**
1. Do I understand why Phase 0 is needed?
   - Read: `docs/Architecture-Critical-Review.md`

2. What's the implementation plan?
   - Read: `docs/Phase-0-Implementation-Plan.md`

3. What's been done so far?
   - Read: `docs/Phase-0-Session-Summary.md`

4. How do I continue?
   - Follow steps in this guide

---

## ?? Decision Point

### **Choose Your Path:**

#### **Path A: Complete Phase 0 First (Recommended)**
- ?? Time: ~12 more hours
- ? Pro: Production-ready architecture
- ? Pro: No refactoring later
- ? Con: Slower to see features working

#### **Path B: Start Phase 1 Now (Risky)**
- ?? Time: Faster initial progress
- ? Pro: See game entities working quickly
- ? Con: Will need to refactor for concurrency
- ? Con: Race conditions in production

#### **Path C: Hybrid Approach**
- Create basic entities (Phase 1)
- Add concurrency later (Phase 0)
- ?? Warning: May need to regenerate migrations

---

## ? Checklist Before Continuing

- [ ] I've read the architecture review
- [ ] I understand the 5 critical flaws
- [ ] I know which path I'm taking (A, B, or C)
- [ ] I've verified the build is successful
- [ ] I'm ready to write code

---

**Ready? Let's build production-ready game architecture! ??**

---

## ?? Quick Commands

```bash
# Build project
dotnet build

# Create migration
dotnet ef migrations add AddRowVersionToConcurrentEntities ^
  --project src/Nightstorm.Data ^
  --startup-project src/Nightstorm.API

# Apply migration
dotnet ef database update ^
  --project src/Nightstorm.Data ^
  --startup-project src/Nightstorm.API

# Run tests
dotnet test

# Check for errors
dotnet build --no-incremental
```

---

**Last Updated:** Current Session  
**Status:** ?? Ready to Continue  
**Next Step:** Create EF Core migration for RowVersion
