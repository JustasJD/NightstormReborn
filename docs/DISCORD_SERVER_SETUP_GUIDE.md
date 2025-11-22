# Discord Server Setup Guide for Nightstorm RPG

This guide will help you configure your Discord server for the Nightstorm RPG bot.

---

## ?? **Prerequisites**

1. **Discord Server** with administrator permissions
2. **Discord Bot** created at [Discord Developer Portal](https://discord.com/developers/applications)
3. **PostgreSQL Database** installed locally or hosted
4. **.NET 9 SDK** installed

---

## ?? **Part 1: Discord Bot Setup**

### **Step 1: Create Discord Application**

1. Go to [Discord Developer Portal](https://discord.com/developers/applications)
2. Click **"New Application"**
3. Name it **"Nightstorm RPG"**
4. Go to **"Bot"** tab
5. Click **"Add Bot"**
6. **Copy the Bot Token** (you'll need this later)
7. Enable these **Privileged Gateway Intents**:
   - ? Server Members Intent
   - ? Message Content Intent

### **Step 2: Bot Permissions**

Required permissions (use permission calculator):
```
Permissions Integer: 8589934592
Or select these permissions:
- Read Messages/View Channels
- Send Messages
- Embed Links
- Attach Files
- Read Message History
- Add Reactions
- Use Slash Commands
- Manage Roles
```

### **Step 3: Invite Bot to Server**

1. Go to **"OAuth2"** ? **"URL Generator"**
2. Select scopes:
   - ? `bot`
   - ? `applications.commands`
3. Select permissions (from Step 2)
4. Copy the generated URL
5. Open URL in browser and invite bot to your server

---

## ?? **Part 2: Server Channel Setup**

### **Required Channels:**

Create these channels in your Discord server:

```
?? INFORMATION
??? #welcome          (Read-only, bot sends greeting)
??? #rules            (Read-only, emoji reaction for verification)
??? #announcements    (Optional, admin announcements)

?? CHARACTER
??? #character-creation (Where users create characters)
??? #character-info     (Optional, character guides)

?? GAMEPLAY
??? #game-general    (Main gameplay channel)
??? #combat          (Combat encounters)
??? #trading         (Player trading, future)
??? #leaderboard     (Rankings, future)

??? SUPPORT
??? #support         (Help and questions)
??? #bot-commands    (Optional, command testing)
```

### **Channel Permissions Setup:**

#### **#rules Channel:**
```
@everyone:
? View Channel
? Read Message History
? Add Reactions
? Send Messages
```

#### **#character-creation Channel:**
```
@everyone:
? View Channel

@Verified:
? View Channel
? Read Message History
? Use Application Commands
? Send Messages (bot-only)
```

#### **Game Channels (#game-general, #combat, etc.):**
```
@everyone:
? View Channel

@Adventurer:
? View Channel
? Send Messages
? Use Application Commands
```

---

## ?? **Part 3: Role Setup**

### **Create These Roles:**

1. **@Verified**
   - Color: Gray (#95a5a6)
   - Position: Below @everyone
   - Permissions: None special (just unlocks #character-creation)

2. **@Adventurer**
   - Color: Green (#2ecc71)
   - Position: Above @Verified
   - Permissions: None special (unlocks game channels)

### **Role Hierarchy:**
```
@Admin (highest)
@Moderator
@Adventurer
@Verified
@everyone (lowest)
```

**Important:** Bot's role must be **above** @Verified and @Adventurer to assign them!

---

## ??? **Part 4: Database Setup (PostgreSQL)**

### **Option A: Local PostgreSQL**

1. **Install PostgreSQL** from [postgresql.org](https://www.postgresql.org/download/)
2. During installation, set a password for `postgres` user
3. Open pgAdmin or command line
4. Create database:
   ```sql
   CREATE DATABASE NightstormDb;
   ```

### **Option B: Hosted PostgreSQL (Recommended for Production)**

Popular options:
- [Neon](https://neon.tech/) - Free tier available
- [Supabase](https://supabase.com/) - Free tier available
- [ElephantSQL](https://www.elephantsql.com/) - Free tier available
- [Heroku Postgres](https://www.heroku.com/postgres) - Free tier available

After creating database, you'll get a connection string like:
```
postgresql://username:password@hostname:5432/database_name
```

---

## ?? **Part 5: Bot Configuration**

### **Step 1: Get Channel and Role IDs**

Enable **Developer Mode** in Discord:
- Settings ? Advanced ? Developer Mode ?

Then **right-click** on channels/roles and **"Copy ID"**

### **Step 2: Configure appsettings.json**

Edit `src/Nightstorm.Bot/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=NightstormDb;Username=postgres;Password=YOUR_PASSWORD"
  },
  "DiscordBot": {
    "Token": "YOUR_BOT_TOKEN_FROM_STEP_1",
    "GuildId": YOUR_SERVER_ID,
    "Channels": {
      "WelcomeChannelId": WELCOME_CHANNEL_ID,
      "RulesChannelId": RULES_CHANNEL_ID,
      "CharacterCreationChannelId": CHAR_CREATION_CHANNEL_ID,
      "GameChannelId": GAME_GENERAL_CHANNEL_ID,
      "SupportChannelId": SUPPORT_CHANNEL_ID
    },
    "Roles": {
      "VerifiedRoleId": VERIFIED_ROLE_ID,
      "AdventurerRoleId": ADVENTURER_ROLE_ID
    }
  }
}
```

**Example with real IDs:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=NightstormDb;Username=postgres;Password=mySecurePass123"
  },
  "DiscordBot": {
    "Token": "TokenGoesHere",
    "GuildId": 1234567890123456789,
    "Channels": {
      "WelcomeChannelId": 1234567890123456789,
      "RulesChannelId": 1234567890123456790,
      "CharacterCreationChannelId": 1234567890123456791,
      "GameChannelId": 1234567890123456792,
      "SupportChannelId": 1234567890123456793
    },
    "Roles": {
      "VerifiedRoleId": 1234567890123456794,
      "AdventurerRoleId": 1234567890123456795
    }
  }
}
```

---

## ?? **Part 6: Initialize Database**

Run these commands from solution directory:

```bash
# Restore packages
dotnet restore

# Create database migration
dotnet ef migrations add InitialCreate --project src/Nightstorm.Data --startup-project src/Nightstorm.Bot

# Apply migration (creates tables)
dotnet ef database update --project src/Nightstorm.Data --startup-project src/Nightstorm.Bot
```

---

## ?? **Part 7: Run the Bot**

```bash
cd src/Nightstorm.Bot
dotnet run
```

You should see:
```
[Info] Nightstorm RPG Bot Starting...
[Info] Connected to Discord as Nightstorm RPG#1234
[Info] Registered 5 slash commands
[Info] Bot is ready!
```

---

## ?? **Part 8: Setup Rules Channel**

Post this message in **#rules** channel:

```
?? **Nightstorm RPG - Server Rules**

1. Be respectful to all players
2. No cheating or exploiting bugs
3. No spam or advertising
4. Follow Discord's Terms of Service
5. Have fun and enjoy the adventure!

? **React with ?? below to confirm you've read and agree to these rules.**

You'll then be able to create your character in #character-creation!
```

Add a ?? reaction to this message. The bot will detect reactions and assign @Verified role.

---

## ?? **Part 9: Test the Flow**

1. **Join as test user**
2. Go to #rules
3. Click ?? emoji
4. Go to #character-creation
5. Use `/character create` command
6. Fill out character modal
7. Confirm character
8. Verify @Adventurer role assigned
9. Access #game-general

---

## ?? **Troubleshooting**

### **Bot offline?**
- Check bot token is correct
- Verify bot has permission to connect
- Check firewall isn't blocking connection

### **Slash commands not appearing?**
- Wait 1 hour (Discord caches commands)
- Or kick and re-invite bot
- Check bot has `applications.commands` scope

### **Role not being assigned?**
- Check bot's role is **above** @Verified and @Adventurer
- Check bot has "Manage Roles" permission
- Check role IDs are correct in appsettings.json

### **Database connection failed?**
- Verify PostgreSQL is running
- Check connection string is correct
- Test connection with pgAdmin or psql
- Check firewall allows port 5432

### **Character creation fails?**
- Check database migration ran successfully
- Verify all tables exist
- Check bot logs for error messages

---

## ?? **Support**

If you encounter issues:
1. Check bot logs in console
2. Verify all IDs are correct (common mistake!)
3. Ensure bot role hierarchy is correct
4. Test with a fresh Discord account

---

## ?? **You're Done!**

Your Nightstorm RPG server is now fully configured and ready for players!

**Next steps:**
- Invite players
- Monitor bot logs
- Adjust channel permissions as needed
- Add more channels (trading, guilds, etc.) later

---

**Quick Reference Card:**

```
Setup Checklist:
? Bot created and invited
? 5 channels created
? 2 roles created (@Verified, @Adventurer)
? Role hierarchy set (bot role above player roles)
? Channel permissions configured
? appsettings.json filled out
? Database created and migrated
? Bot started successfully
? Test user verified flow works
```

Save this guide for reference! ??
