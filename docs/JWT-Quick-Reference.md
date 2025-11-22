# JWT Authentication - Quick Reference

## ?? What We've Completed

### ? Phase 1: Database Schema (DONE)
1. Created `User` entity with multi-platform support
2. Updated `Character` entity with `UserId` 
3. Added EF Core configuration for `User`
4. Updated `RpgContext` with Users DbSet

**Next Step:** Create migration

---

## ?? Implementation Order Recommendation

### **Quick Start (Minimum Viable):**
1. Add JWT packages
2. Create JWT service
3. Add AuthController
4. Create migration
5. Test with Discord

### **Full Implementation:**
Follow all 6 phases in the main guide

---

## ?? Key Files Created

| File | Status | Purpose |
|------|--------|---------|
| `User.cs` | ? Created | Platform-agnostic user entity |
| `Character.cs` | ? Updated | Added UserId + User navigation |
| `UserConfiguration.cs` | ? Created | EF Core configuration |
| `RpgContext.cs` | ? Updated | Added Users DbSet |
| `IJwtTokenService.cs` | ?? Documented | JWT token generation interface |
| `JwtTokenService.cs` | ?? Documented | JWT implementation |
| `IPasswordHasher.cs` | ?? Documented | Password hashing interface |
| `PasswordHasher.cs` | ?? Documented | PBKDF2 password hashing |
| `AuthController.cs` | ?? Documented | Auth endpoints |
| `IUserRepository.cs` | ?? Documented | User repository interface |
| `UserRepository.cs` | ?? Documented | User repository implementation |

---

## ?? Quick Commands

### Add Required Packages
```bash
dotnet add src/Nightstorm.API package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0
dotnet add src/Nightstorm.API package System.IdentityModel.Tokens.Jwt --version 8.0.0
```

### Create Migration
```bash
dotnet ef migrations add AddUserAuthentication --project src\Nightstorm.Data --startup-project src\Nightstorm.API
```

### Apply Migration
```bash
dotnet ef database update --project src\Nightstorm.Data --startup-project src\Nightstorm.API
```

### Build & Test
```bash
dotnet build
dotnet run --project src\Nightstorm.API
```

---

## ?? Architecture Overview

```
Discord User ? Bot ? JWT Token ? API ? Database
Web User    ? App ? JWT Token ? API ? Database
Mobile User ? App ? JWT Token ? API ? Database
                      ?
              Same authentication,
              same character data!
```

---

## ?? Security Configuration

**appsettings.json additions:**
```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyMinimum32Characters!",
    "Issuer": "NightstormAPI",
    "Audience": "NightstormClients",
    "ExpiresInMinutes": 10080
  }
}
```

**?? Production:** Store JWT:Key in Azure Key Vault!

---

## ?? New API Endpoints

| Endpoint | Method | Auth Required | Purpose |
|----------|--------|---------------|---------|
| `/api/auth/discord` | POST | No | Discord user registration/login |
| `/api/auth/register` | POST | No | Email/password registration |
| `/api/auth/login` | POST | No | Email/password login |
| `/api/auth/me` | GET | Yes | Current user info |
| `/api/characters/me` | GET | Yes | Current user's character |

---

## ?? Discord Bot Changes

### Before (No Auth):
```csharp
var response = await _httpClient.GetAsync("/api/characters/...");
```

### After (With JWT):
```csharp
var token = await _tokenCache.GetOrCreateTokenAsync(discordUserId, username);
_httpClient.DefaultRequestHeaders.Authorization = 
    new AuthenticationHeaderValue("Bearer", token);
var response = await _httpClient.GetAsync("/api/characters/me");
```

---

## ?? Testing Steps

1. **Test Discord Auth:**
   ```bash
   curl -X POST https://localhost:7001/api/auth/discord \
     -H "Content-Type: application/json" \
     -d '{"discordId":123456789,"discordUsername":"TestUser"}'
   ```

2. **Test Character Creation with JWT:**
   ```bash
   curl -X POST https://localhost:7001/api/characters \
     -H "Authorization: Bearer YOUR_TOKEN_HERE" \
     -H "Content-Type: application/json" \
     -d '{"name":"Aragorn","class":1}'
   ```

3. **Test Get Current User:**
   ```bash
   curl https://localhost:7001/api/auth/me \
     -H "Authorization: Bearer YOUR_TOKEN_HERE"
   ```

---

## ? Performance Optimizations Included

- ? Token caching in Bot (7-day TTL)
- ? AsNoTracking for read-only queries
- ? Indexed User lookups (DiscordId, Email, Username)
- ? Stateless JWT (no database lookup per request)
- ? PBKDF2 password hashing (100K iterations)

---

## ?? Migration Path for Existing Data

If you have existing characters:

1. Run migration (creates Users table, adds UserId column)
2. Run data migration script (creates Users from existing DiscordUserIds)
3. Update Characters with UserId
4. Test backward compatibility

---

## ?? Additional Resources

- Full implementation guide: `docs/JWT-Authentication-Implementation-Guide.md`
- Architecture fix documentation: `docs/Architecture-Fix.md`
- PostgreSQL setup: `docs/PostgreSQL-Setup-Summary.md`

---

## ?? What to Implement Next?

**Option A: Minimal (Get it working)** - 2-3 hours
- Add JWT packages
- Create JWT service & AuthController
- Create migration
- Test with Postman

**Option B: Full Implementation** - 1-2 days
- All 6 phases
- Bot integration
- Comprehensive testing
- Production-ready

**Which would you like to tackle first?** ??
