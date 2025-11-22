# Nightstorm Discord Bot Setup

## Overview
This is the Discord bot foundation for the Nightstorm RPG game. The bot runs as a .NET Worker Service (background service) that operates 24/7.

## Current Status
? **Foundation Complete** - Bot infrastructure and hosting setup  
? **Combat System Implemented** - Fully balanced turn-based combat engine ready for integration  
? **Game Commands** - Slash commands and interaction handlers (next phase)  

## Architecture
The bot follows Clean Architecture principles with:
- **Configuration Layer**: `DiscordBotOptions` using Options Pattern
- **Service Interface**: `IDiscordBotService` for abstraction
- **Service Implementation**: `DiscordBotService` managing Discord client lifecycle
- **Worker Service**: `Worker` hosting the bot as a background service
- **Dependency Injection**: All services registered in `Program.cs`

## Combat System Integration (Ready)
The core combat engine has been implemented in `Nightstorm.Core` and is ready for Discord bot integration:
- **Turn-based Combat**: Complete combat calculation engine with 16 balanced character classes
- **Rock-Paper-Scissors System**: 4-quadrant type effectiveness (HeavyMelee, FastMelee, ElementalMagic, SpiritualMagic)
- **Mitigation System**: Tank-focused damage reduction (19-22% for tanks, 10-11% for DPS)
- **Critical Hit System**: Luck + DEX + Class bonuses with variable multipliers (2.0x-3.0x)
- **Balance Tested**: 16,800+ combat simulations validate class balance and type matchups

See `docs/COMBAT_SYSTEM_FINAL_REPORT.md` for detailed combat system documentation.

## Project Structure
```
Nightstorm.Bot/
??? Configuration/
?   ??? DiscordBotOptions.cs       # Bot configuration options
??? Interfaces/
?   ??? IDiscordBotService.cs      # Bot service interface
??? Services/
?   ??? DiscordBotService.cs       # Core bot service implementation
??? Program.cs                      # DI configuration & host setup
??? Worker.cs                       # Background service host
??? appsettings.json               # Production configuration
??? appsettings.Development.json   # Development configuration
```

## Configuration

### Setting Up Your Bot Token

#### 1. Create a Discord Bot
1. Go to [Discord Developer Portal](https://discord.com/developers/applications)
2. Click "New Application"
3. Give it a name (e.g., "Nightstorm RPG")
4. Go to the "Bot" section
5. Click "Add Bot"
6. Copy the bot token (keep it secret!)

#### 2. Enable Required Intents
In the Discord Developer Portal, under Bot settings:
- Enable **Server Members Intent**
- Enable **Message Content Intent**
- Enable **Presence Intent** (optional)

#### 3. Configure Bot Token (Development)

**Option A: User Secrets (Recommended for Development)**
```bash
cd src/Nightstorm.Bot
dotnet user-secrets set "DiscordBot:Token" "YOUR_BOT_TOKEN_HERE"
```

**Option B: Environment Variable**
```bash
# Windows PowerShell
$env:DiscordBot__Token = "YOUR_BOT_TOKEN_HERE"

# Windows CMD
set DiscordBot__Token=YOUR_BOT_TOKEN_HERE

# Linux/Mac
export DiscordBot__Token="YOUR_BOT_TOKEN_HERE"
```

**Option C: appsettings.Development.json (NOT recommended - never commit tokens)**
```json
{
  "DiscordBot": {
    "Token": "YOUR_BOT_TOKEN_HERE"
  }
}
```

#### 4. Production Deployment
For production, use environment variables or Azure Key Vault:
```bash
DiscordBot__Token=YOUR_BOT_TOKEN_HERE
```

### Configuration Options

| Option | Default | Description |
|--------|---------|-------------|
| `Token` | `""` | Discord bot token (required) |
| `CommandPrefix` | `"!"` | Prefix for text commands |
| `LogGatewayEvents` | `false` | Enable detailed gateway logging |
| `ActivityStatus` | `"Nightstorm RPG"` | Bot's status message |
| `ActivityType` | `"Playing"` | Activity type (Playing, Watching, Listening, etc.) |

## Running the Bot

### Development
```bash
cd src/Nightstorm.Bot
dotnet run
```

### Production
```bash
cd src/Nightstorm.Bot
dotnet run --configuration Release
```

### As Windows Service
```bash
sc create NightstormBot binPath="C:\path\to\Nightstorm.Bot.exe"
sc start NightstormBot
```

### As Linux Systemd Service
Create `/etc/systemd/system/nightstorm-bot.service`:
```ini
[Unit]
Description=Nightstorm Discord Bot
After=network.target

[Service]
Type=notify
WorkingDirectory=/path/to/bot
ExecStart=/usr/bin/dotnet /path/to/Nightstorm.Bot.dll
Restart=always
RestartSec=10
Environment=DOTNET_ENVIRONMENT=Production
Environment=DiscordBot__Token=YOUR_TOKEN_HERE

[Install]
WantedBy=multi-user.target
```

## Discord Gateway Intents
The bot is configured with the following intents:
- **Guilds**: Access to guild (server) information
- **GuildMessages**: Receive messages in guilds
- **MessageContent**: Read message content (required for content-based features)
- **GuildMembers**: Access to member information

## Dependencies
- **Discord.Net 3.18.0**: Discord API wrapper
- **Microsoft.Extensions.Hosting 9.0.11**: Background service hosting
- **Nightstorm.Core**: Game core logic and combat system
- **Nightstorm.Data**: Database access

## Scalability Notes
- **Current Setup**: Single instance, no sharding
- **Supports**: 20,000+ users across 1-3 guilds
- **Sharding**: Not required unless exceeding 2,500 guilds
- **Future**: Can add sharding support if expanding to more servers

## Development Roadmap

### ? Phase 1: Foundation (Complete)
- Bot hosting infrastructure
- Configuration management
- Discord client lifecycle
- Background service implementation

### ? Phase 2: Combat System (Complete)
- Turn-based combat engine
- Character classes and stats
- Type effectiveness system
- Balance testing and calibration

### ? Phase 3: Game Commands (Next)
- Slash command registration
- Character creation (`/character create`)
- Character stats display (`/character stats`)
- Combat commands (`/attack`, `/defend`)
- Inventory management (`/inventory`)

### ?? Phase 4: Game Features (Planned)
- Quest system integration
- Monster encounters
- Loot and item system
- Guild management
- PvP arenas

### ?? Phase 5: Advanced Features (Future)
- Boss raids
- World events
- Crafting system
- Achievement system
- Leaderboards

## Troubleshooting

### Bot Won't Start
- Check if token is configured correctly
- Verify intents are enabled in Discord Developer Portal
- Check logs for detailed error messages

### Bot Connects but Doesn't Respond
- No command handlers implemented yet (foundation only)
- Next phase will add slash commands and interactions

### Connection Issues
- Verify internet connection
- Check Discord API status: https://discordstatus.com
- Review gateway logs (set `LogGatewayEvents: true`)

## Security Best Practices
? **DO:**
- Use User Secrets for development
- Use environment variables or Key Vault for production
- Keep token secure and never share it
- Regenerate token if compromised

? **DON'T:**
- Commit tokens to source control
- Share tokens in public channels
- Store tokens in plain text files

## Logging
- **Production**: Information level and above
- **Development**: Debug level with gateway events
- **Log Output**: Includes Discord.Net events and service lifecycle

---

## Recent Updates
**January 2025** - Combat System Implementation
- Implemented full turn-based combat engine with 16 character classes
- Balanced 4-quadrant type effectiveness system (rock-paper-scissors)
- Tank-focused mitigation system with 19-22% damage reduction for tanks
- Critical hit system with class-specific multipliers (2.0x-3.0x)
- Comprehensive testing: 16,800+ combat simulations validate balance
- Production-ready combat engine awaiting Discord command integration

---

*This bot is under active development. Command handlers and game features will be implemented in upcoming phases.*
