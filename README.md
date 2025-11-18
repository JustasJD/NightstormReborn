# NightstormReborn

A modern text-based RPG game engine built with .NET 9, featuring Discord bot integration and real-time multiplayer capabilities.

## Project Overview

NightstormReborn is a complete game ecosystem that combines a powerful backend game engine with Discord bot integration, allowing players to experience an immersive text-based RPG directly through Discord or web interface. The project follows Clean Architecture principles with SOLID design patterns, ensuring maintainability, scalability, and testability.

## Architecture & Project Structure

The solution is organized into 5 distinct projects, each with a specific responsibility following the separation of concerns principle:

```
NightstormReborn/
??? src/
?   ??? Nightstorm.Core/        # Domain Layer - Core business logic
?   ??? Nightstorm.Data/         # Infrastructure Layer - Data persistence
?   ??? Nightstorm.API/          # Presentation Layer - Game Engine API
?   ??? Nightstorm.Bot/          # Presentation Layer - Discord Bot
?   ??? Nightstorm.Web/          # Presentation Layer - Web Frontend (Future)
??? Nightstorm.sln
```

---

## Project Descriptions

### 1. **Nightstorm.Core** - Domain Layer
**Type:** Class Library (.NET 9)  
**Dependencies:** None (Pure domain layer)

#### Purpose
The Core project represents the **heart of the application** - the domain layer in Clean Architecture. It contains the fundamental business logic, rules, and entities that define what the game *is*, independent of any implementation details.

#### Responsibilities
- **Domain Entities**: Core game objects like `Character`, `Monster`, `Item`, `Quest`, `Guild`, etc.
- **Value Objects**: Immutable objects that represent concepts without identity (e.g., `Position`, `Stats`, `Damage`)
- **Domain Events**: Events that represent something significant happening in the domain (e.g., `CharacterLeveledUp`, `QuestCompleted`, `ItemEquipped`)
- **Enums**: Game-specific enumerations (`CharacterClass`, `ItemRarity`, `MonsterType`, `QuestStatus`)
- **Interfaces**: Contracts for repositories and services (e.g., `ICharacterRepository`, `IGameEngine`, `ICombatService`)
- **Business Rules**: Core game mechanics, formulas, and validation logic
- **Domain Exceptions**: Custom exceptions for business rule violations

#### Key Characteristics
- **No dependencies** on other projects or external frameworks
- **Framework-agnostic** - Pure C# business logic
- **Testable** - Easy to unit test without infrastructure
- **Stable** - Changes rarely, only when game rules change
- **Shared** - Referenced by all other projects

#### Example Structure
```
Nightstorm.Core/
??? Entities/
?   ??? Character.cs
?   ??? Monster.cs
?   ??? Item.cs
?   ??? Quest.cs
?   ??? Guild.cs
??? ValueObjects/
?   ??? Stats.cs
?   ??? Position.cs
?   ??? Damage.cs
??? Enums/
?   ??? CharacterClass.cs
?   ??? ItemRarity.cs
?   ??? QuestStatus.cs
??? Interfaces/
?   ??? Repositories/
?   ?   ??? ICharacterRepository.cs
?   ?   ??? IItemRepository.cs
?   ??? Services/
?       ??? ICombatService.cs
?       ??? IQuestService.cs
??? Events/
?   ??? CharacterLeveledUp.cs
?   ??? QuestCompleted.cs
??? Exceptions/
    ??? GameRuleViolationException.cs
```

---

### 2. **Nightstorm.Data** - Infrastructure Layer (Data Persistence)
**Type:** Class Library (.NET 9)  
**Dependencies:** Nightstorm.Core, Entity Framework Core 9.0

#### Purpose
The Data project is the **data access layer** responsible for all database operations and persistence logic. It implements the repository interfaces defined in Core and manages the application's interaction with SQL Server using Entity Framework Core.

#### Responsibilities
- **DbContext**: Main `RpgContext` class that manages entity configurations and database connections
- **Entity Configurations**: Fluent API configurations for each entity (table mappings, relationships, constraints)
- **Migrations**: Database schema version control and evolution
- **Repository Implementations**: Concrete implementations of `IRepository` interfaces from Core
- **Data Seeding**: Initial data population for game content (starter items, NPCs, quests)
- **Query Optimizations**: Efficient data retrieval strategies with proper indexing
- **Database Interceptors**: Logging, soft deletes, audit trails

#### Key Technologies
- **Microsoft.EntityFrameworkCore** 9.0.0 - ORM framework
- **Microsoft.EntityFrameworkCore.SqlServer** 9.0.0 - SQL Server provider
- **Microsoft.EntityFrameworkCore.Design** 9.0.0 - Design-time support for migrations
- **Microsoft.EntityFrameworkCore.Tools** 9.0.0 - CLI tools for migrations

#### Key Characteristics
- **Abstracts database complexity** from business logic
- **Supports migrations** for database versioning
- **Optimized queries** with Include/ThenInclude for related data
- **Transaction management** through Unit of Work pattern
- **Connection pooling** for performance

#### Example Structure
```
Nightstorm.Data/
??? Contexts/
?   ??? RpgContext.cs
??? Configurations/
?   ??? CharacterConfiguration.cs
?   ??? ItemConfiguration.cs
?   ??? QuestConfiguration.cs
??? Repositories/
?   ??? CharacterRepository.cs
?   ??? ItemRepository.cs
?   ??? QuestRepository.cs
??? Migrations/
?   ??? (Auto-generated EF Core migrations)
??? Seeders/
    ??? GameDataSeeder.cs
```

#### Common Commands
```bash
# Add new migration
dotnet ef migrations add InitialCreate --project src/Nightstorm.Data

# Update database
dotnet ef database update --project src/Nightstorm.Data

# Remove last migration
dotnet ef migrations remove --project src/Nightstorm.Data
```

---

### 3. **Nightstorm.API** - Game Engine (RESTful API + SignalR)
**Type:** ASP.NET Core Web API (.NET 9)  
**Dependencies:** Nightstorm.Core, Nightstorm.Data

#### Purpose
The API project is the **game engine server** - a high-performance backend that orchestrates all game logic, manages player sessions, handles real-time combat, and provides both RESTful endpoints and WebSocket connections for game clients.

#### Responsibilities
- **RESTful API Endpoints**: HTTP endpoints for standard CRUD operations
  - Character management (create, view, update, delete)
  - Inventory management
  - Quest system
  - Guild operations
  - Leaderboards
- **SignalR Hubs**: Real-time bidirectional communication for:
  - Live combat encounters
  - Party/raid coordination
  - Real-time notifications
  - Chat systems
  - World events
- **Game Services**: Application-level business logic
  - Combat calculation engine
  - Loot generation system
  - Quest progression tracking
  - Experience and leveling system
  - Economy and trading system
- **Controllers**: HTTP request handlers
- **Middleware**: Authentication, authorization, error handling, logging
- **Background Services**: Scheduled tasks (world events, maintenance, cleanup)

#### Key Technologies
- **ASP.NET Core 9.0** - Web framework
- **Microsoft.AspNetCore.SignalR** - Real-time communication (included in framework)
- **Microsoft.AspNetCore.SignalR.Client** 10.0.0 - Client library
- **Microsoft.AspNetCore.OpenApi** 9.0.11 - OpenAPI/Swagger documentation

#### Key Features
- **RESTful design** - Standard HTTP methods and status codes
- **Real-time updates** via SignalR WebSockets
- **JWT Authentication** for secure API access
- **Rate limiting** to prevent abuse
- **API versioning** for backward compatibility
- **OpenAPI/Swagger** documentation
- **CORS configuration** for web clients
- **Health checks** for monitoring

#### Example Structure
```
Nightstorm.API/
??? Controllers/
?   ??? CharactersController.cs
?   ??? ItemsController.cs
?   ??? QuestsController.cs
?   ??? GuildsController.cs
??? Hubs/
?   ??? GameHub.cs          # Main game hub
?   ??? CombatHub.cs        # Combat encounters
?   ??? ChatHub.cs          # In-game chat
??? Services/
?   ??? CombatService.cs
?   ??? LootService.cs
?   ??? QuestService.cs
?   ??? ExperienceService.cs
??? Middleware/
?   ??? ExceptionHandlingMiddleware.cs
?   ??? RequestLoggingMiddleware.cs
??? BackgroundServices/
?   ??? WorldEventService.cs
??? DTOs/
?   ??? CharacterDto.cs
?   ??? ItemDto.cs
??? Validators/
?   ??? CreateCharacterValidator.cs
??? Program.cs
```

#### Example API Endpoints
```
GET    /api/v1/characters/{id}              # Get character details
POST   /api/v1/characters                    # Create new character
PUT    /api/v1/characters/{id}               # Update character
DELETE /api/v1/characters/{id}               # Delete character
POST   /api/v1/combat/attack                 # Execute attack
GET    /api/v1/quests/available              # List available quests
POST   /api/v1/guilds/{id}/join              # Join a guild
```

#### SignalR Hub Methods
```csharp
// Server -> Client
OnPlayerJoined(string playerId, string playerName)
OnCombatUpdate(CombatState state)
OnQuestUpdate(QuestProgress progress)

// Client -> Server
JoinParty(string partyId)
ExecuteAbility(string abilityId, string targetId)
SendChatMessage(string message)
```

---

### 4. **Nightstorm.Bot** - Discord Integration
**Type:** .NET Worker Service (.NET 9)  
**Dependencies:** Nightstorm.Core, Nightstorm.Data, Discord.Net

#### Purpose
The Bot project is the **Discord client integration** - a long-running background service that connects to Discord's API, allowing players to interact with the game directly through Discord slash commands, buttons, and messages. It acts as an alternative UI to the web interface.

#### Responsibilities
- **Discord Bot Client**: Maintains persistent connection to Discord servers
- **Slash Command Handlers**: Modern Discord slash commands for game actions
  - `/character create` - Create new character
  - `/character stats` - View character information
  - `/adventure explore` - Start exploring
  - `/combat attack` - Execute combat actions
  - `/inventory` - View and manage inventory
  - `/quest accept` - Accept quests
  - `/guild create` - Create or join guilds
- **Interaction Handlers**: Process Discord interactions (buttons, select menus, modals)
- **Notification Service**: Push notifications to Discord users
  - Quest completion alerts
  - Level up notifications
  - Guild invitations
  - Combat results
  - Item drops
- **Message Components**: Interactive Discord UI elements
  - Button-based combat interface
  - Dropdown menus for item selection
  - Modal forms for character creation
- **Integration Service**: Communicates with Nightstorm.API for game logic

#### Key Technologies
- **Discord.Net** 3.18.0 - Discord API wrapper
  - Discord.Net.Core - Core functionality
  - Discord.Net.WebSocket - Gateway connection
  - Discord.Net.Interactions - Slash commands
  - Discord.Net.Commands - Text-based commands (legacy support)
  - Discord.Net.Rest - REST API interactions
- **Microsoft.Extensions.Hosting** 9.0.0 - Background service infrastructure

#### Key Features
- **Slash commands** - Modern Discord command interface
- **Interactive components** - Buttons, dropdowns, modals
- **Ephemeral responses** - Private messages only visible to user
- **Embed messages** - Rich formatted game information
- **Auto-completion** - Type-ahead suggestions
- **User Secrets** - Secure token storage during development
- **Resilient connection** - Automatic reconnection on failures
- **Rate limit handling** - Respects Discord's rate limits

#### Example Structure
```
Nightstorm.Bot/
??? Commands/
?   ??? CharacterCommands.cs
?   ??? CombatCommands.cs
?   ??? QuestCommands.cs
?   ??? InventoryCommands.cs
??? Handlers/
?   ??? InteractionHandler.cs
?   ??? ComponentHandler.cs
?   ??? ModalHandler.cs
??? Services/
?   ??? DiscordService.cs
?   ??? NotificationService.cs
?   ??? GameApiClient.cs
??? Modules/
?   ??? GameModule.cs
??? Worker.cs                    # Main background service
??? Program.cs
```

#### Example Discord Interactions
```
/character create name:Aragorn class:Warrior
/character stats @Aragorn
/adventure explore region:DarkForest
/combat attack target:Goblin ability:PowerStrike
/inventory equip item:SteelSword
/quest list status:available
/guild create name:DragonSlayers tag:DRGN
/shop buy item:HealthPotion quantity:5
```

#### User Secrets Configuration
```bash
# Store Discord bot token securely
dotnet user-secrets set "Discord:Token" "YOUR_BOT_TOKEN_HERE" --project src/Nightstorm.Bot
dotnet user-secrets set "GameApi:BaseUrl" "https://localhost:5001" --project src/Nightstorm.Bot
```

---

### 5. **Nightstorm.Web** - Web Frontend (Future Development)
**Type:** React Application (Planned)  
**Status:** Folder created, implementation pending

#### Purpose
The Web project will be the **browser-based game client** - a modern, responsive single-page application (SPA) that provides a rich graphical interface for players who prefer playing through a web browser instead of Discord.

#### Planned Technologies
- **React** - Frontend framework
- **TypeScript** - Type-safe JavaScript
- **SignalR Client** - Real-time connection to API
- **React Router** - Client-side routing
- **Redux/Zustand** - State management
- **TailwindCSS/MUI** - UI components and styling

#### Planned Features
- Character creation wizard
- Interactive game world map
- Real-time combat interface
- Inventory management with drag-and-drop
- Quest journal and tracking
- Guild management dashboard
- Player profile and statistics
- Marketplace/trading interface
- Leaderboards and rankings
- Social features (friends, parties, chat)

#### Planned Structure
```
Nightstorm.Web/
??? src/
?   ??? components/
?   ?   ??? Character/
?   ?   ??? Combat/
?   ?   ??? Inventory/
?   ?   ??? Quest/
?   ??? pages/
?   ?   ??? Dashboard.tsx
?   ?   ??? Character.tsx
?   ?   ??? Adventure.tsx
?   ??? services/
?   ?   ??? gameApi.ts
?   ?   ??? signalRConnection.ts
?   ??? store/
?   ?   ??? gameStore.ts
?   ??? App.tsx
??? public/
??? package.json
```

---

## Technology Stack

### Backend
- **.NET 9** - Latest framework with performance improvements
- **Entity Framework Core 9** - Modern ORM with improved performance
- **ASP.NET Core** - High-performance web framework
- **SignalR** - Real-time bidirectional communication
- **SQL Server** - Relational database

### External Integrations
- **Discord.Net 3.18** - Discord bot framework

### Frontend (Planned)
- **React** - Modern UI library
- **TypeScript** - Type-safe development

---

## Architectural Principles

### Clean Architecture
The solution follows Clean Architecture (Onion Architecture) with clear dependency rules:
```
Nightstorm.Core (Domain)
    ?
Nightstorm.Data (Infrastructure)
    ?
Nightstorm.API + Nightstorm.Bot (Presentation)
```

**Dependency Rule**: Dependencies point **inward**. Core has no dependencies, Data depends on Core, and Presentation layers depend on both.

### SOLID Principles
- **Single Responsibility**: Each class has one reason to change
- **Open/Closed**: Open for extension, closed for modification
- **Liskov Substitution**: Derived classes are substitutable for base classes
- **Interface Segregation**: Many specific interfaces over one general interface
- **Dependency Inversion**: Depend on abstractions, not concretions

### Design Patterns Used
- **Repository Pattern** - Data access abstraction (in Data project)
- **Unit of Work** - Transaction management
- **CQRS** - Command Query Responsibility Segregation (planned with MediatR)
- **Domain Events** - Decoupled domain logic
- **Result Pattern** - Error handling without exceptions
- **Options Pattern** - Configuration management
- **Dependency Injection** - Loose coupling

---

## Getting Started

### Prerequisites
- .NET 9 SDK
- SQL Server (LocalDB, Express, or Full)
- Visual Studio 2022 (17.8+) or VS Code
- Discord Bot Token (for bot development)

### Setup Instructions

1. **Clone the repository**
   ```bash
   git clone https://github.com/JustasJD/NightstormReborn.git
   cd NightstormReborn
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

3. **Configure Database Connection**
   
   Update `appsettings.json` in `Nightstorm.API`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=NightstormDb;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

4. **Run Database Migrations**
   ```bash
   cd src/Nightstorm.Data
   dotnet ef database update
   ```

5. **Configure Discord Bot (Optional)**
   
   Set up user secrets for the Bot project:
   ```bash
   cd src/Nightstorm.Bot
   dotnet user-secrets init
   dotnet user-secrets set "Discord:Token" "YOUR_BOT_TOKEN"
   ```

6. **Build the solution**
   ```bash
   dotnet build
   ```

7. **Run the projects**
   
   Terminal 1 - API:
   ```bash
   cd src/Nightstorm.API
   dotnet run
   ```
   
   Terminal 2 - Bot:
   ```bash
   cd src/Nightstorm.Bot
   dotnet run
   ```

---

## Development Workflow

### Adding a New Feature

1. **Define domain entities** in `Nightstorm.Core/Entities/`
2. **Create repository interface** in `Nightstorm.Core/Interfaces/Repositories/`
3. **Implement repository** in `Nightstorm.Data/Repositories/`
4. **Add EF configuration** in `Nightstorm.Data/Configurations/`
5. **Create migration** with `dotnet ef migrations add FeatureName`
6. **Build API endpoints** in `Nightstorm.API/Controllers/`
7. **Implement Discord commands** in `Nightstorm.Bot/Commands/`
8. **Write unit tests** (future test project)

### Database Migrations

```bash
# Add new migration
dotnet ef migrations add MigrationName --project src/Nightstorm.Data --startup-project src/Nightstorm.API

# Update database
dotnet ef database update --project src/Nightstorm.Data --startup-project src/Nightstorm.API

# Rollback migration
dotnet ef database update PreviousMigrationName --project src/Nightstorm.Data --startup-project src/Nightstorm.API

# Generate SQL script
dotnet ef migrations script --project src/Nightstorm.Data --startup-project src/Nightstorm.API --output migration.sql
```

---

## Roadmap

### Phase 1: Core Foundation (Current)
- [x] Project structure setup
- [x] Package dependencies installed
- [x] Core domain entities
- [x] Database context and migrations
- [ ] Basic API endpoints
- [ ] Discord bot basic commands

### Phase 2: Game Mechanics
- [ ] Combat system
- [ ] Character progression
- [ ] Inventory management
- [ ] Quest system
- [ ] Loot generation

### Phase 3: Social Features
- [ ] Guild system
- [ ] Party/raid groups
- [ ] Friend system
- [ ] Chat functionality
- [ ] Trading system

### Phase 4: Advanced Features
- [ ] Crafting system
- [ ] Player housing
- [ ] World events
- [ ] Achievements
- [ ] Leaderboards

### Phase 5: Web Frontend
- [ ] React application setup
- [ ] Character dashboard
- [ ] Interactive map
- [ ] Real-time combat UI
- [ ] Full game interface

---

## Contributing

Contributions are welcome! Please follow these guidelines:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Follow the coding conventions in `.github/copilot-instructions.md`
4. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
5. Push to the branch (`git push origin feature/AmazingFeature`)
6. Open a Pull Request

### Coding Standards
- Follow Microsoft's .NET naming conventions
- Use PascalCase for classes, methods, properties
- Use camelCase with `_` prefix for private fields
- Write XML documentation for public APIs
- Follow SOLID principles
- Write unit tests for new features

---

## License

This project is licensed under the MIT License - see the LICENSE file for details.

---

## Authors

- **JustasJD** - *Initial work* - [GitHub](https://github.com/JustasJD)

---

## Acknowledgments

- Discord.Net community for excellent Discord integration
- Entity Framework Core team for powerful ORM
- ASP.NET Core team for high-performance web framework
- Clean Architecture principles by Robert C. Martin

---

## Support

For issues, questions, or contributions:
- Open an issue on [GitHub Issues](https://github.com/JustasJD/NightstormReborn/issues)
- Join our Discord server (coming soon)
- Check the documentation wiki (coming soon)

---

**Built with using .NET 9**