# JWT Multi-Platform Authentication Implementation Guide

## ?? Overview

This guide covers implementing JWT authentication to support Discord, Web, and Mobile platforms while maintaining backward compatibility with existing Discord-only functionality.

---

## ?? Implementation Phases

### **Phase 1: Database Schema Updates** ? COMPLETED
- [x] Created `User` entity (platform-agnostic)
- [x] Updated `Character` entity with `UserId`
- [x] Added `UserConfiguration` for EF Core
- [x] Updated `RpgContext` with Users DbSet
- [ ] Create and run migration

### **Phase 2: Authentication Infrastructure** ?? NEXT
- [ ] Add JWT configuration
- [ ] Create JWT token service
- [ ] Create password hashing service
- [ ] Add authentication middleware

### **Phase 3: API Endpoints** ?? PENDING
- [ ] Create AuthController (register/login)
- [ ] Update CharactersController with [Authorize]
- [ ] Add "me" endpoints (current user)

### **Phase 4: Repositories & Services** ??? PENDING
- [ ] Create IUserRepository
- [ ] Create UserRepository implementation
- [ ] Update CharacterRepository for UserId

### **Phase 5: Discord Bot Integration** ?? PENDING
- [ ] Create token cache service
- [ ] Update bot commands to use JWT
- [ ] Add auto-registration for Discord users

---

## ?? Required NuGet Packages

Add these to `Nightstorm.API` project:

```bash
dotnet add src/Nightstorm.API package Microsoft.AspNetCore.Authentication.JwtBearer --version 9.0.0
dotnet add src/Nightstorm.API package System.IdentityModel.Tokens.Jwt --version 8.0.0
```

---

## ?? Phase 2: Authentication Infrastructure

### **2.1: JWT Configuration in appsettings.json**

Add to `src/Nightstorm.API/appsettings.json`:

```json
{
  "Jwt": {
    "Key": "YourSuperSecretKeyHere-MinimumLength32Characters!",
    "Issuer": "NightstormAPI",
    "Audience": "NightstormClients",
    "ExpiresInMinutes": 10080
  }
}
```

**Note:** For production, store `Jwt:Key` in Azure Key Vault or environment variables!

---

### **2.2: JWT Token Service Interface**

Create `src/Nightstorm.Core/Interfaces/Services/IJwtTokenService.cs`:

```csharp
namespace Nightstorm.Core.Interfaces.Services;

/// <summary>
/// Service for generating and validating JWT tokens.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT token for a user.
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="username">Username</param>
    /// <param name="platform">Platform identifier (discord/web/mobile)</param>
    /// <returns>JWT token string</returns>
    string GenerateToken(Guid userId, string username, string platform);

    /// <summary>
    /// Validates a JWT token and returns user ID.
    /// </summary>
    /// <param name="token">JWT token</param>
    /// <returns>User ID if valid, null otherwise</returns>
    Guid? ValidateToken(string token);
}
```

---

### **2.3: JWT Token Service Implementation**

Create `src/Nightstorm.API/Services/JwtTokenService.cs`:

```csharp
using Microsoft.IdentityModel.Tokens;
using Nightstorm.Core.Interfaces.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Nightstorm.API.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<JwtTokenService> _logger;

    public JwtTokenService(IConfiguration configuration, ILogger<JwtTokenService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateToken(Guid userId, string username, string platform)
    {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim("platform", platform),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                int.Parse(_configuration["Jwt:ExpiresInMinutes"]!)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public Guid? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userIdClaim = jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value;

            return Guid.Parse(userIdClaim);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to validate JWT token");
            return null;
        }
    }
}
```

---

### **2.4: Password Hashing Service Interface**

Create `src/Nightstorm.Core/Interfaces/Services/IPasswordHasher.cs`:

```csharp
namespace Nightstorm.Core.Interfaces.Services;

/// <summary>
/// Service for securely hashing and verifying passwords.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a password.
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// Verifies a password against a hash.
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);
}
```

---

### **2.5: Password Hashing Service Implementation**

Create `src/Nightstorm.API/Services/PasswordHasher.cs`:

```csharp
using Nightstorm.Core.Interfaces.Services;
using System.Security.Cryptography;

namespace Nightstorm.API.Services;

/// <summary>
/// Password hashing service using BCrypt-style PBKDF2.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int SaltSize = 16; // 128 bits
    private const int KeySize = 32; // 256 bits
    private const int Iterations = 100000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        // Combine salt + hash for storage
        var hashBytes = new byte[SaltSize + KeySize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);

        return Convert.ToBase64String(hashBytes);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var hashBytes = Convert.FromBase64String(passwordHash);

        // Extract salt
        var salt = new byte[SaltSize];
        Array.Copy(hashBytes, 0, salt, 0, SaltSize);

        // Extract hash
        var storedHash = new byte[KeySize];
        Array.Copy(hashBytes, SaltSize, storedHash, 0, KeySize);

        // Compute hash of input password
        var computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);

        // Compare hashes
        return CryptographicOperations.FixedTimeEquals(computedHash, storedHash);
    }
}
```

---

### **2.6: Configure Services in Program.cs**

Add to `src/Nightstorm.API/Program.cs`:

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

// After builder creation, add:

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        // Support JWT from query string (for SignalR)
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

// Register authentication services
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// ... existing services ...

// Before app.Run(), add:
app.UseAuthentication();
app.UseAuthorization(); // Already there
```

---

## ?? Phase 3: API Endpoints

### **3.1: Authentication DTOs**

Create `src/Nightstorm.API/DTOs/Auth/AuthDtos.cs`:

```csharp
namespace Nightstorm.API.DTOs.Auth;

/// <summary>
/// Request to register a Discord user.
/// </summary>
public record RegisterDiscordRequest
{
    public required ulong DiscordId { get; init; }
    public required string DiscordUsername { get; init; }
}

/// <summary>
/// Request to register with email/password.
/// </summary>
public record RegisterEmailRequest
{
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required string Password { get; init; }
}

/// <summary>
/// Request to login with email/password.
/// </summary>
public record LoginRequest
{
    public required string Email { get; init; }
    public required string Password { get; init; }
}

/// <summary>
/// Authentication response with JWT token.
/// </summary>
public record AuthResponse
{
    public required string AccessToken { get; init; }
    public required string TokenType { get; init; } = "Bearer";
    public required int ExpiresIn { get; init; }
    public required Guid UserId { get; init; }
    public required string Username { get; init; }
}
```

---

### **3.2: Authentication Controller**

Create `src/Nightstorm.API/Controllers/AuthController.cs`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nightstorm.API.DTOs.Auth;
using Nightstorm.Core.Entities;
using Nightstorm.Core.Interfaces.Services;
using Nightstorm.Data.Contexts;

namespace Nightstorm.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly RpgContext _context;
    private readonly IJwtTokenService _jwtService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        RpgContext context,
        IJwtTokenService jwtService,
        IPasswordHasher passwordHasher,
        IConfiguration configuration,
        ILogger<AuthController> logger)
    {
        _context = context;
        _jwtService = jwtService;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
        _logger = logger;
    }

    /// <summary>
    /// Registers or authenticates a Discord user.
    /// </summary>
    [HttpPost("discord")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> RegisterDiscord(
        [FromBody] RegisterDiscordRequest request,
        CancellationToken cancellationToken)
    {
        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.DiscordId == request.DiscordId, cancellationToken);

        User user;

        if (existingUser != null)
        {
            // Update last login
            existingUser.LastLoginAt = DateTime.UtcNow;
            existingUser.DiscordUsername = request.DiscordUsername;
            user = existingUser;
        }
        else
        {
            // Create new user
            user = new User
            {
                Username = request.DiscordUsername,
                DiscordId = request.DiscordId,
                DiscordUsername = request.DiscordUsername,
                IsActive = true,
                LastLoginAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Generate JWT token
        var token = _jwtService.GenerateToken(user.Id, user.Username, "discord");
        var expiresIn = int.Parse(_configuration["Jwt:ExpiresInMinutes"]!) * 60;

        _logger.LogInformation(
            "Discord user authenticated: {UserId} (Discord: {DiscordId})",
            user.Id, request.DiscordId);

        return Ok(new AuthResponse
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = expiresIn,
            UserId = user.Id,
            Username = user.Username
        });
    }

    /// <summary>
    /// Registers a new user with email/password.
    /// </summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterEmailRequest request,
        CancellationToken cancellationToken)
    {
        // Validate
        if (request.Password.Length < 8)
        {
            return BadRequest(new { message = "Password must be at least 8 characters" });
        }

        // Check if username exists
        if (await _context.Users.AnyAsync(u => u.Username == request.Username, cancellationToken))
        {
            return Conflict(new { message = "Username already taken" });
        }

        // Check if email exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            return Conflict(new { message = "Email already registered" });
        }

        // Create user
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsActive = true,
            LastLoginAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync(cancellationToken);

        // Generate token
        var token = _jwtService.GenerateToken(user.Id, user.Username, "web");
        var expiresIn = int.Parse(_configuration["Jwt:ExpiresInMinutes"]!) * 60;

        _logger.LogInformation("New user registered: {UserId} ({Username})", user.Id, user.Username);

        return CreatedAtAction(nameof(GetCurrentUser), null, new AuthResponse
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = expiresIn,
            UserId = user.Id,
            Username = user.Username
        });
    }

    /// <summary>
    /// Authenticates a user with email/password.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email, cancellationToken);

        if (user == null || user.PasswordHash == null)
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid email or password" });
        }

        if (!user.IsActive)
        {
            return Unauthorized(new { message = "Account is disabled" });
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        // Generate token
        var token = _jwtService.GenerateToken(user.Id, user.Username, "web");
        var expiresIn = int.Parse(_configuration["Jwt:ExpiresInMinutes"]!) * 60;

        _logger.LogInformation("User logged in: {UserId}", user.Id);

        return Ok(new AuthResponse
        {
            AccessToken = token,
            TokenType = "Bearer",
            ExpiresIn = expiresIn,
            UserId = user.Id,
            Username = user.Username
        });
    }

    /// <summary>
    /// Gets the current authenticated user's information.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);

        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(new
        {
            user.Id,
            user.Username,
            user.Email,
            user.DiscordId,
            user.LastLoginAt,
            user.CreatedAt
        });
    }
}
```

---

## ??? Phase 4: Repositories

### **4.1: User Repository Interface**

Create `src/Nightstorm.Core/Interfaces/Repositories/IUserRepository.cs`:

```csharp
using Nightstorm.Core.Entities;

namespace Nightstorm.Core.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByDiscordIdAsync(ulong discordId, CancellationToken cancellationToken = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
```

### **4.2: User Repository Implementation**

Create `src/Nightstorm.Data/Repositories/UserRepository.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using Nightstorm.Core.Entities;
using Nightstorm.Core.Interfaces.Repositories;
using Nightstorm.Data.Contexts;

namespace Nightstorm.Data.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(RpgContext context) : base(context)
    {
    }

    public async Task<User?> GetByDiscordIdAsync(ulong discordId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.DiscordId == discordId, cancellationToken);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AsNoTracking()
            .AnyAsync(u => u.Email == email, cancellationToken);
    }
}
```

---

## ??? Phase 5: Database Migration

### **5.1: Create Migration**

Run in CMD:

```cmd
dotnet ef migrations add AddUserAuthentication --project src\Nightstorm.Data --startup-project src\Nightstorm.API
```

### **5.2: Review Migration**

The migration should:
- Create `Users` table
- Add `UserId` column to `Characters` table
- Add foreign key relationship
- Create indexes on User fields

### **5.3: Data Migration Script (Optional)**

If you have existing characters with `DiscordUserId`, create a data migration:

```sql
-- Create users from existing Discord users
INSERT INTO "Users" ("Id", "Username", "DiscordId", "DiscordUsername", "IsActive", "CreatedAt", "IsDeleted")
SELECT 
    gen_random_uuid(),
    CONCAT('DiscordUser_', "DiscordUserId"),
    "DiscordUserId",
    "Name",
    true,
    "CreatedAt",
    false
FROM "Characters"
WHERE "DiscordUserId" IS NOT NULL
GROUP BY "DiscordUserId", "Name", "CreatedAt";

-- Update characters with UserId
UPDATE "Characters" c
SET "UserId" = u."Id"
FROM "Users" u
WHERE c."DiscordUserId" = u."DiscordId";
```

### **5.4: Apply Migration**

```cmd
dotnet ef database update --project src\Nightstorm.Data --startup-project src\Nightstorm.API
```

---

## ?? Phase 6: Discord Bot Integration

### **6.1: Token Cache Service**

Create in Bot project: `src/Nightstorm.Bot/Services/TokenCacheService.cs`:

```csharp
using System.Collections.Concurrent;

namespace Nightstorm.Bot.Services;

public class TokenCacheService
{
    private readonly ConcurrentDictionary<ulong, TokenCacheEntry> _cache = new();
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public TokenCacheService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<string> GetOrCreateTokenAsync(ulong discordId, string discordUsername)
    {
        if (_cache.TryGetValue(discordId, out var entry) && entry.ExpiresAt > DateTime.UtcNow)
        {
            return entry.Token;
        }

        // Request new token from API
        var apiUrl = _configuration["GameApi:BaseUrl"];
        var response = await _httpClient.PostAsJsonAsync($"{apiUrl}/api/auth/discord", new
        {
            DiscordId = discordId,
            DiscordUsername = discordUsername
        });

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception("Failed to authenticate with API");
        }

        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        
        var cacheEntry = new TokenCacheEntry
        {
            Token = authResponse!.AccessToken,
            ExpiresAt = DateTime.UtcNow.AddSeconds(authResponse.ExpiresIn - 300) // 5 min buffer
        };

        _cache[discordId] = cacheEntry;
        return cacheEntry.Token;
    }

    private class TokenCacheEntry
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    private record AuthResponse(string AccessToken, int ExpiresIn);
}
```

---

## ? Testing Checklist

### Authentication Endpoints
- [ ] POST /api/auth/discord (Discord registration)
- [ ] POST /api/auth/register (Email registration)
- [ ] POST /api/auth/login (Email login)
- [ ] GET /api/auth/me (Current user info)

### Character Endpoints (with JWT)
- [ ] GET /api/characters/me (Current user's character)
- [ ] POST /api/characters (Create - requires auth)
- [ ] PUT /api/characters/{id} (Update - requires auth)
- [ ] DELETE /api/characters/{id} (Delete - requires auth)

### Discord Bot
- [ ] /register command works
- [ ] Token caching works
- [ ] API calls include JWT
- [ ] Character creation via bot works

---

## ?? Summary

This implementation provides:

? **Multi-platform support** - Discord, Web, Mobile  
? **Backward compatibility** - Existing Discord users migrated  
? **Secure authentication** - JWT tokens, password hashing  
? **Scalable architecture** - Ready for horizontal scaling  
? **Production-ready** - Following .NET 9 best practices  

---

## ?? Next Steps

1. Review this guide
2. Choose which phases to implement first
3. I'll help implement each phase step-by-step
4. Test thoroughly before deploying

**Which phase would you like to implement first?** ??
