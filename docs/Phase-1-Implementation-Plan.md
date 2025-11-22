# ?? Phase 1: Game Engine Implementation Plan

**Status:** ?? STARTING  
**Prerequisites:** ? Phase 0 Complete (All critical fixes implemented)  
**Estimated Time:** ~40 hours  

---

## ?? **Overview**

We're building a **turn-based RPG game engine** with:
- **81 zones** in a 9×9 grid (A1 to I9)
- **Nightstorm events** (10v10 PvE combat) spawning randomly in zones
- **Travel system** between zones (90 seconds per zone)
- **Zone treasuries** that accumulate entry fees
- **Guild ownership** of zones with daily treasury withdrawals

---

## ??? **Implementation Phases**

### **Phase 1.1: Core Game Entities** (~8 hours)
Create the fundamental game state entities

1. **PlayerState** (~1.5h)
   - Current location (InZone, Travelling, InCombat)
   - Current/destination zone
   - Travel start/end times
   - Combat registration status

2. **NightstormEvent** (~2h)
   - Event status (Scheduled, Registration, InProgress, Complete)
   - Zone where event occurs
   - Scheduled time
   - Registration list (max 10 players)
   - Combat instance reference

3. **CombatInstance** (~2h)
   - Participants (players + monsters)
   - Turn order and current turn
   - Combat log
   - Status and result

4. **TravelLog** (~1h)
   - Track player movements
   - Origin/destination zones
   - Start/completion times

5. **ZoneTreasury** (~1.5h)
   - Gold accumulation from entry fees
   - Guild ownership
   - Withdrawal history

---

### **Phase 1.2: Game Engine Services** (~12 hours)

1. **TravelService** (~3h)
   - Initiate travel between zones
   - Check travel completion
   - Collect zone entry fees
   - Update player location

2. **NightstormService** (~4h)
   - Schedule random Nightstorm events
   - Handle player registration
   - Start combat when 10 players or time limit
   - Process combat results

3. **CombatEngineService** (~5h)
   - Generate monsters based on zone danger tier
   - Calculate turn order
   - Process player/monster actions
   - Determine winner
   - Distribute rewards

---

### **Phase 1.3: Worker Service** (~8 hours)

1. **GameEngineWorker** (~4h)
   - Background service that runs game loops
   - Process travel completions every second
   - Process combat turns every 2 seconds
   - Update zone treasuries every 5 seconds
   - Save player states every 30 seconds

2. **Quartz Jobs** (~4h)
   - NightstormSchedulerJob (one per zone, 81 total)
   - Random scheduling (10-30 minutes)
   - Auto-reschedule after event completes

---

### **Phase 1.4: API Integration** (~6 hours)

1. **TravelController** (~2h)
   - POST /api/travel/start
   - GET /api/travel/status
   - POST /api/travel/cancel (if not started)

2. **NightstormController** (~2h)
   - GET /api/nightstorm/active
   - POST /api/nightstorm/register
   - GET /api/nightstorm/{id}/status

3. **CombatController** (~2h)
   - GET /api/combat/{id}/status
   - GET /api/combat/{id}/log
   - POST /api/combat/{id}/action (player turn)

---

### **Phase 1.5: Event System** (~4 hours)

1. **Redis Pub/Sub Events** (~2h)
   - TravelStarted
   - TravelCompleted
   - NightstormScheduled
   - NightstormRegistrationOpen
   - CombatStarted
   - CombatTurnProcessed
   - CombatEnded

2. **SignalR Hubs** (~2h)
   - Real-time notifications to web clients
   - Bot subscribes to events

---

### **Phase 1.6: Testing & Polish** (~2 hours)

1. **Integration Tests**
   - Travel system end-to-end
   - Nightstorm scheduling
   - Combat processing

2. **Load Testing**
   - 100 players traveling
   - 50 simultaneous combats
   - 1000 concurrent players

---

## ?? **Phase 1.1: Core Entities - Detailed Plan**

Let's start with the entities first. Here's what we'll create:

### **1. PlayerState Entity**

```csharp
public class PlayerState : BaseEntity
{
    // Identity
    public Guid CharacterId { get; set; }
    public Character Character { get; set; }
    
    // Location
    public PlayerLocation Location { get; set; } // InZone, Travelling, InCombat
    public Guid CurrentZoneId { get; set; }
    public Zone CurrentZone { get; set; }
    
    // Travel
    public Guid? DestinationZoneId { get; set; }
    public Zone? DestinationZone { get; set; }
    public DateTime? TravelStartedAt { get; set; }
    public DateTime? TravelEndsAt { get; set; }
    
    // Combat
    public Guid? CurrentCombatId { get; set; }
    public CombatInstance? CurrentCombat { get; set; }
    public Guid? RegisteredEventId { get; set; }
    public NightstormEvent? RegisteredEvent { get; set; }
}

public enum PlayerLocation
{
    InZone = 0,
    Travelling = 1,
    InCombat = 2
}
```

### **2. NightstormEvent Entity**

```csharp
public class NightstormEvent : BaseEntity
{
    // Event Info
    public Guid ZoneId { get; set; }
    public Zone Zone { get; set; }
    public DateTime ScheduledAt { get; set; }
    public EventStatus Status { get; set; }
    
    // Registration
    public DateTime? RegistrationOpenedAt { get; set; }
    public DateTime? RegistrationClosesAt { get; set; }
    public int MaxParticipants { get; set; } = 10;
    
    // Participants
    public ICollection<PlayerState> RegisteredPlayers { get; set; }
    
    // Combat
    public Guid? CombatInstanceId { get; set; }
    public CombatInstance? CombatInstance { get; set; }
    
    // Results
    public bool? PlayerVictory { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public enum EventStatus
{
    Scheduled = 0,
    Registration = 1,
    InProgress = 2,
    Victory = 3,
    Defeat = 4,
    Cancelled = 5
}
```

### **3. CombatInstance Entity**

```csharp
public class CombatInstance : BaseEntity
{
    // Combat Info
    public Guid NightstormEventId { get; set; }
    public NightstormEvent NightstormEvent { get; set; }
    public CombatStatus Status { get; set; }
    
    // Participants
    public ICollection<CombatParticipant> Participants { get; set; }
    
    // Turn Management
    public int CurrentTurn { get; set; }
    public Guid? CurrentActorId { get; set; } // CharacterId or MonsterId
    
    // Combat Log
    public ICollection<CombatLogEntry> CombatLog { get; set; }
    
    // Results
    public bool? PlayerVictory { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class CombatParticipant : BaseEntity
{
    public Guid CombatInstanceId { get; set; }
    public CombatInstance CombatInstance { get; set; }
    
    public ParticipantType Type { get; set; } // Player or Monster
    public Guid EntityId { get; set; } // CharacterId or MonsterId
    
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
    public bool IsAlive { get; set; }
    public int InitiativeRoll { get; set; }
}

public enum CombatStatus
{
    Pending = 0,
    InProgress = 1,
    Victory = 2,
    Defeat = 3
}

public enum ParticipantType
{
    Player = 0,
    Monster = 1
}
```

### **4. CombatLogEntry**

```csharp
public class CombatLogEntry : BaseEntity
{
    public Guid CombatInstanceId { get; set; }
    public CombatInstance CombatInstance { get; set; }
    
    public int Turn { get; set; }
    public Guid ActorId { get; set; }
    public string ActorName { get; set; }
    
    public CombatActionType ActionType { get; set; }
    public Guid? TargetId { get; set; }
    public string? TargetName { get; set; }
    
    public int? Damage { get; set; }
    public bool? IsCritical { get; set; }
    public bool? IsMiss { get; set; }
    
    public string Description { get; set; }
}

public enum CombatActionType
{
    Attack = 0,
    Skill = 1,
    Heal = 2,
    Defend = 3,
    Flee = 4
}
```

### **5. TravelLog**

```csharp
public class TravelLog : BaseEntity
{
    public Guid CharacterId { get; set; }
    public Character Character { get; set; }
    
    public Guid OriginZoneId { get; set; }
    public Zone OriginZone { get; set; }
    
    public Guid DestinationZoneId { get; set; }
    public Zone DestinationZone { get; set; }
    
    public DateTime StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    public long EntryFeePaid { get; set; }
    public TravelStatus Status { get; set; }
}

public enum TravelStatus
{
    InProgress = 0,
    Completed = 1,
    Cancelled = 2
}
```

### **6. ZoneTreasury**

```csharp
public class ZoneTreasury : BaseEntity
{
    public Guid ZoneId { get; set; }
    public Zone Zone { get; set; }
    
    public long CurrentGold { get; set; }
    public long TotalCollected { get; set; }
    public long TotalWithdrawn { get; set; }
    
    public DateTime? LastWithdrawalAt { get; set; }
    public Guid? LastWithdrawnByGuildId { get; set; }
    
    public ICollection<TreasuryTransaction> Transactions { get; set; }
}

public class TreasuryTransaction : BaseEntity
{
    public Guid ZoneTreasuryId { get; set; }
    public ZoneTreasury ZoneTreasury { get; set; }
    
    public TransactionType Type { get; set; }
    public long Amount { get; set; }
    
    public Guid? CharacterId { get; set; }
    public Character? Character { get; set; }
    
    public Guid? GuildId { get; set; }
    public Guild? Guild { get; set; }
    
    public string Description { get; set; }
}

public enum TransactionType
{
    EntryFee = 0,
    Withdrawal = 1
}
```

---

## ?? **Let's Start Implementation!**

We'll create entities first, then services, then the worker.

**Ready to start with PlayerState entity?**

---

## ?? **Progress Tracking**

```
Phase 1: Game Engine Implementation
???????????????????????????????????????????

Phase 1.1: Core Entities [??????????] 0% (0h / 8h)
  ? PlayerState entity
  ? NightstormEvent entity
  ? CombatInstance entity
  ? TravelLog entity
  ? ZoneTreasury entity
  ? EF Core configurations
  ? Database migration

Phase 1.2: Services [??????????] 0% (0h / 12h)
  ? TravelService
  ? NightstormService
  ? CombatEngineService

Phase 1.3: Worker Service [??????????] 0% (0h / 8h)
  ? GameEngineWorker
  ? Quartz jobs

Phase 1.4: API Integration [??????????] 0% (0h / 6h)
  ? Controllers
  ? DTOs

Phase 1.5: Events [??????????] 0% (0h / 4h)
  ? Redis Pub/Sub
  ? SignalR

Phase 1.6: Testing [??????????] 0% (0h / 2h)
  ? Integration tests
  ? Load tests

???????????????????????????????????????????
Overall Progress: [??????????] 0% (0h / 40h)
```

---

**Let's begin! Say "start with entities" to create the first entity!** ????
