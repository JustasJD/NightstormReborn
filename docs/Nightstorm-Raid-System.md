# ?? Nightstorm Event - Raid System

## ?? **Event Outcomes**

### **1. Victory (EventStatus.Victory = 3)**
**Players defended successfully!**
- At least 1 player registered
- Players defeated all monsters
- Treasury safe
- Players receive rewards

### **2. Defeat (EventStatus.Defeat = 4)**
**Players fought but lost**
- At least 1 player registered
- All players died in combat
- Treasury safe (players tried to defend)
- No rewards

### **3. Raid (EventStatus.Raid = 5)** ?? **PUNISHABLE**
**No one defended - monsters raided the town!**
- **Zero players registered**
- Registration time expired
- Monsters automatically raid the treasury
- **Treasury loses gold as penalty**
- Guild notified: "Your town was raided! No defenders showed up!"

#### **Raid Penalty Calculation:**
```csharp
// Example: 10-30% of current treasury
var penaltyPercentage = Random.Next(10, 31); // 10-30%
var penaltyAmount = (treasury.CurrentGold * penaltyPercentage) / 100;

// Apply penalty
treasury.CurrentGold -= penaltyAmount;

// Record in event
nightstormEvent.RaidPenaltyAmount = penaltyAmount;
nightstormEvent.Status = EventStatus.Raid;
```

### **4. Cancelled (EventStatus.Cancelled = 6)** ? **NON-PUNISHABLE**
**System cancelled - no penalties**
- Server shutdown
- Database error
- Invalid game state
- Admin intervention

#### **Cancellation Reasons:**
```csharp
nightstormEvent.Status = EventStatus.Cancelled;
nightstormEvent.CancellationReason = "Server maintenance";
// OR
nightstormEvent.CancellationReason = "Database connection lost";
// OR
nightstormEvent.CancellationReason = "Admin cancelled event";
```

**No penalties applied!**

---

## ?? **Event Flow**

```
???????????????????????????????????????????????????????????
? Scheduled (EventStatus.Scheduled)                       ?
? - Event created and scheduled                           ?
? - Waiting for scheduled time                            ?
???????????????????????????????????????????????????????????
                      ?
                      ?
???????????????????????????????????????????????????????????
? Registration (EventStatus.Registration)                 ?
? - Players can register (max 10)                         ?
? - Registration window: 10 minutes                       ?
???????????????????????????????????????????????????????????
         ?                    ?                  ?
         ? Players            ? No players       ? System error
         ? registered         ? registered       ?
         ?                    ?                  ?
??????????????????   ?????????????????   ???????????????
?   InProgress   ?   ?     RAID      ?   ?  Cancelled  ?
?  (Combat On)   ?   ?  (Undefended) ?   ? (No Penalty)?
??????????????????   ?????????????????   ???????????????
         ?                   ?
         ?                   ? Treasury loses gold
         ?                   ?
         ?           ?????????????????????
         ?           ? Guild notification?
         ?           ? "Town was raided!"?
         ?           ?????????????????????
         ?
         ?
??????????????????
? Victory/Defeat ?
?  (Battle End)  ?
??????????????????
```

---

## ?? **Treasury Impact**

| Outcome | Treasury Impact | Notification |
|---------|----------------|--------------|
| **Victory** | ? No change | "Town successfully defended!" |
| **Defeat** | ? No change (players tried) | "Defenders fell, but town safe!" |
| **Raid** | ? Loses 10-30% gold | "RAID! No defenders - treasury plundered!" |
| **Cancelled** | ? No change | "Event cancelled (system issue)" |

---

## ?? **Example Scenarios**

### **Scenario 1: Successful Defense**
```
Time: 12:00 - Event scheduled
Time: 12:05 - Registration opens
Time: 12:06 - 5 players register
Time: 12:15 - Registration closes, combat starts
Time: 12:20 - Players win
Result: Victory ?
Treasury: Safe (no change)
```

### **Scenario 2: Heroic Defeat**
```
Time: 12:00 - Event scheduled
Time: 12:05 - Registration opens
Time: 12:08 - 3 players register
Time: 12:15 - Combat starts
Time: 12:18 - All players die
Result: Defeat ?
Treasury: Safe (players tried to defend)
```

### **Scenario 3: Raid (No Defenders)**
```
Time: 12:00 - Event scheduled
Time: 12:05 - Registration opens
Time: 12:15 - No players registered, time expired
Result: RAID ??
Treasury: Before: 100,000 gold
         Penalty: -20,000 gold (20%)
         After: 80,000 gold
Notification: "Zone A5 was raided! Treasury lost 20,000 gold!"
```

### **Scenario 4: System Cancelled**
```
Time: 12:00 - Event scheduled
Time: 12:05 - Registration opens
Time: 12:07 - Server needs restart for hotfix
Result: Cancelled (system)
Treasury: Safe (no penalty for system issues)
CancellationReason: "Server maintenance"
```

---

## ?? **Notifications**

### **Guild Notifications (Bot/Discord):**

```csharp
// Victory
"?? Victory! Zone {zone} successfully defended against Nightstorm!"

// Defeat
"?? Brave defenders fell in Zone {zone}, but the treasury is safe!"

// Raid (punishable)
"?? RAID ALERT! Zone {zone} was undefended! 
Treasury plundered: -{raidPenalty} gold"

// Cancelled (non-punishable)
"?? Nightstorm event in Zone {zone} cancelled due to: {reason}"
```

---

## ?? **Implementation Logic**

### **When Registration Closes:**

```csharp
if (nightstormEvent.ShouldBeRaided) // No players registered
{
    // Calculate raid penalty
    var treasury = await _treasuryRepository.GetByZoneIdAsync(zoneId);
    var penaltyPercentage = Random.Next(10, 31); // 10-30%
    var penaltyAmount = (treasury.CurrentGold * penaltyPercentage) / 100;
    
    // Apply penalty
    treasury.CurrentGold -= penaltyAmount;
    
    // Record raid
    nightstormEvent.Status = EventStatus.Raid;
    nightstormEvent.RaidPenaltyAmount = penaltyAmount;
    nightstormEvent.CompletedAt = DateTime.UtcNow;
    
    // Notify guild
    await _notificationService.SendRaidAlertAsync(
        zoneId, 
        penaltyAmount,
        $"Zone {zone.Identifier} was raided! No defenders showed up.");
}
else if (nightstormEvent.CanStart) // Has players
{
    // Start combat normally
    await StartCombatAsync(nightstormEvent);
}
```

### **On Server Shutdown:**

```csharp
// Find all active events
var activeEvents = await _eventRepository
    .FindAsync(e => e.Status == EventStatus.Registration || 
                   e.Status == EventStatus.InProgress);

foreach (var ev in activeEvents)
{
    // Cancel without penalty
    ev.Status = EventStatus.Cancelled;
    ev.CancellationReason = "Server shutdown";
    ev.CompletedAt = DateTime.UtcNow;
    
    // NO treasury penalty!
}

await _eventRepository.SaveChangesAsync();
```

---

## ? **Summary**

**Punishable:** Raid (no defenders = treasury loses gold)  
**Non-Punishable:** Cancelled (system issues = no penalty)

This ensures guilds are responsible for defending their zones, but aren't penalized for server issues beyond their control!

---

**Status:** ? Raid system defined  
**Next:** EF Core configurations
