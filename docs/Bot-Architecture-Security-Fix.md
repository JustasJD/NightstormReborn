# ?? CRITICAL SECURITY FIX - Bot Architecture

## ? **PROBLEM IDENTIFIED**

The original architecture document incorrectly suggested that **Nightstorm.Bot** would have direct database access. This is a **severe security and architecture violation**.

---

## ? **CORRECTED ARCHITECTURE**

### **Who Can Access Database?**

| Component | Database Access | API Access | Redis Access | Why |
|-----------|----------------|------------|--------------|-----|
| **Nightstorm.API** | ? Full (Read/Write) | N/A | ? Read/Write | Primary data gateway |
| **Nightstorm.GameEngine** | ? Full (Read/Write) | ? No | ? Read/Write | Game state management |
| **Nightstorm.Bot** | ? **NEVER** | ? HTTP Only | ? Pub/Sub Only (Read) | Discord interface only |
| **Web/Mobile Clients** | ? Never | ? HTTP Only | ? No | Users |

---

## ?? **Why Bot NEVER Gets Database Access**

### **Security Risks**

1. **Token Compromise = Full DB Access**
   - Discord bot token leaked ? Attacker has full database access
   - API key leaked ? Only limited to API endpoints with auth

2. **No Authentication Layer**
   - Direct DB access bypasses JWT validation
   - Bot could impersonate any user
   - No rate limiting

3. **No Audit Trail**
   - API logs all requests (who, what, when)
   - Direct DB access = invisible changes

4. **SQL Injection Risk**
   - If bot constructs raw queries = potential injection
   - API uses parameterized queries + validation

---

### **Architecture Violations**

1. **Breaks Single Responsibility Principle**
   - Bot should only handle Discord UI/UX
   - Data access should be centralized in API

2. **Tight Coupling**
   - Bot depends on exact DB schema
   - Schema change = bot breaks
   - API provides abstraction layer

3. **Duplicate Validation Logic**
   - Business rules duplicated in bot + API
   - Inconsistencies guaranteed

4. **Can't Scale**
   - Multiple bot instances = connection pool exhaustion
   - API handles connection pooling properly

---

## ? **CORRECT Bot Architecture**

```
???????????????????????????????????????????????????????
?  Discord Bot (Nightstorm.Bot)                       ?
?                                                     ?
?  ?????????????????????????????????????????????     ?
?  ?  Slash Commands (/travel, /register)      ?     ?
?  ?????????????????????????????????????????????     ?
?                  ?                                  ?
?                  ?                                  ?
?  ?????????????????????????????????????????????     ?
?  ?  ApiClient Service                        ?     ?
?  ?  - HTTP calls to API                      ?     ?
?  ?  - JWT token authentication              ?     ?
?  ?  - Retry logic                            ?     ?
?  ?????????????????????????????????????????????     ?
?                  ?                                  ?
?                  ? HTTPS                            ?
???????????????????????????????????????????????????????
                   ?
                   ?
????????????????????????????????????????????????????????
?  Nightstorm.API (REST API)                           ?
?                                                      ?
?  ??????????????????????????????????????????????     ?
?  ?  Controllers                               ?     ?
?  ?  - Validate requests                       ?     ?
?  ?  - Check JWT authorization                 ?     ?
?  ?  - Call repositories                       ?     ?
?  ??????????????????????????????????????????????     ?
?                   ?                                  ?
?                   ?                                  ?
?  ??????????????????????????????????????????????     ?
?  ?  PostgreSQL Database                       ?     ?
?  ?  - Full CRUD access                        ?     ?
?  ??????????????????????????????????????????????     ?
????????????????????????????????????????????????????????
```

---

## ?? **Bot Implementation - Correct Way**

### **1. Bot Project Dependencies**

```xml
<!-- Nightstorm.Bot.csproj -->
<ItemGroup>
  <!-- Discord library -->
  <PackageReference Include="Discord.Net" Version="3.14.1" />
  
  <!-- HTTP client for API calls -->
  <PackageReference Include="Microsoft.Extensions.Http" Version="9.0.0" />
  
  <!-- Redis for Pub/Sub notifications (read-only) -->
  <PackageReference Include="StackExchange.Redis" Version="2.7.20" />
  
  <!-- ? NO Entity Framework -->
  <!-- ? NO Npgsql.EntityFrameworkCore.PostgreSQL -->
  <!-- ? NO Nightstorm.Data reference -->
</ItemGroup>

<ItemGroup>
  <!-- Only reference Core for DTOs and enums -->
  <ProjectReference Include="..\Nightstorm.Core\Nightstorm.Core.csproj" />
</ItemGroup>
```

---

### **2. API Client Service (NEW)**

```csharp
// Nightstorm.Bot/Services/ApiClient.cs
public class ApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiClient> _logger;
    private readonly string _apiBaseUrl;
    private readonly string _serviceToken; // JWT token for bot service account
    
    public ApiClient(
        HttpClient httpClient, 
        IConfiguration config,
        ILogger<ApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _apiBaseUrl = config["Api:BaseUrl"] ?? "http://nightstorm-api:8080";
        _serviceToken = config["Api:ServiceToken"] ?? throw new Exception("Missing Api:ServiceToken");
        
        // Set JWT token for all requests
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _serviceToken);
    }
    
    // Travel endpoints
    public async Task<TravelResult> InitiateTravelAsync(Guid characterId, string destinationZoneId)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"{_apiBaseUrl}/api/travel/{destinationZoneId}", 
            new { CharacterId = characterId });
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TravelResult>();
    }
    
    // Combat registration
    public async Task<RegistrationResult> RegisterForCombatAsync(Guid characterId)
    {
        var response = await _httpClient.PostAsync(
            $"{_apiBaseUrl}/api/game/nightstorm/register", 
            JsonContent.Create(new { CharacterId = characterId }));
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<RegistrationResult>();
    }
    
    // Get character by Discord user ID
    public async Task<CharacterDto?> GetCharacterByDiscordIdAsync(ulong discordUserId)
    {
        var response = await _httpClient.GetAsync(
            $"{_apiBaseUrl}/api/characters/discord/{discordUserId}");
        
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<CharacterDto>();
    }
}
```

---

### **3. Redis Subscriber (Read-Only Notifications)**

```csharp
// Nightstorm.Bot/Services/RedisSubscriber.cs
public class RedisSubscriber : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly DiscordSocketClient _discordClient;
    private readonly ILogger<RedisSubscriber> _logger;
    
    public RedisSubscriber(
        IConnectionMultiplexer redis,
        DiscordSocketClient discordClient,
        ILogger<RedisSubscriber> logger)
    {
        _redis = redis;
        _discordClient = discordClient;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var subscriber = _redis.GetSubscriber();
        
        // Subscribe to game events (READ-ONLY)
        await subscriber.SubscribeAsync("game:travel:complete", async (channel, message) =>
        {
            var eventData = JsonSerializer.Deserialize<TravelCompleteEvent>(message);
            await HandleTravelCompleteAsync(eventData);
        });
        
        await subscriber.SubscribeAsync("game:nightstorm:triggered", async (channel, message) =>
        {
            var eventData = JsonSerializer.Deserialize<NightstormEvent>(message);
            await HandleNightstormEventAsync(eventData);
        });
        
        await subscriber.SubscribeAsync("game:combat:turn", async (channel, message) =>
        {
            var eventData = JsonSerializer.Deserialize<CombatTurnEvent>(message);
            await HandleCombatTurnAsync(eventData);
        });
        
        _logger.LogInformation("Redis subscriber started for game events");
    }
    
    private async Task HandleTravelCompleteAsync(TravelCompleteEvent eventData)
    {
        // Remove "Travelling" role
        // Assign zone role
        // Send DM notification
    }
}
```

---

### **4. Discord Command Handler (Uses API Client)**

```csharp
// Nightstorm.Bot/Handlers/TravelCommandHandler.cs
public class TravelCommandHandler : InteractionModuleBase<SocketInteractionContext>
{
    private readonly ApiClient _apiClient;
    
    public TravelCommandHandler(ApiClient apiClient)
    {
        _apiClient = apiClient;
    }
    
    [SlashCommand("travel", "Travel to another zone")]
    public async Task TravelAsync(
        [Summary("zone", "Destination zone (e.g., A1, B3)")] string zoneId)
    {
        await DeferAsync(); // Show "Bot is thinking..."
        
        try
        {
            // Get character via API (NOT database!)
            var character = await _apiClient.GetCharacterByDiscordIdAsync(Context.User.Id);
            
            if (character == null)
            {
                await FollowupAsync("? You don't have a character! Use `/create-character` first.");
                return;
            }
            
            // Initiate travel via API (NOT database!)
            var result = await _apiClient.InitiateTravelAsync(character.Id, zoneId);
            
            // Assign Discord role
            var travellingRole = Context.Guild.Roles.FirstOrDefault(r => r.Name == "Travelling");
            if (travellingRole != null)
            {
                await (Context.User as IGuildUser)?.AddRoleAsync(travellingRole);
            }
            
            await FollowupAsync(
                $"?? Travelling to **{zoneId}**...\n" +
                $"?? ETA: **{result.TravelTimeSeconds}s**\n" +
                $"?? Entry fee on arrival: **{result.EntryFee}g**");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "API request failed for travel");
            await FollowupAsync("? Travel failed! Please try again later.");
        }
    }
}
```

---

## ?? **Security Best Practices**

### **1. Bot Service Account JWT Token**

```json
// appsettings.json (Nightstorm.Bot)
{
  "Api": {
    "BaseUrl": "http://nightstorm-api:8080",
    "ServiceToken": "eyJhbGc..." // Long-lived JWT for bot service account
  },
  "Discord": {
    "Token": "YOUR_DISCORD_BOT_TOKEN"
  }
}
```

**Generate service token in API:**

```csharp
// Create a special "Bot Service Account" user
var botServiceAccount = new User
{
    Username = "bot-service-account",
    Email = "bot@nightstorm.internal",
    IsActive = true
};

// Generate JWT with long expiration (1 year)
var token = _jwtTokenService.GenerateToken(
    botServiceAccount, 
    expirationDays: 365);
```

---

### **2. API Rate Limiting for Bot**

```csharp
// Nightstorm.API - Rate limit bot requests
app.UseRateLimiter(new RateLimiterOptions
{
    GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var userId = context.User.GetUserId();
        var isBotServiceAccount = userId == botServiceAccountId;
        
        // Bot gets higher rate limits
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: userId.ToString(),
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = isBotServiceAccount ? 1000 : 100, // 1000 req/min for bot
                Window = TimeSpan.FromMinutes(1)
            });
    })
});
```

---

## ?? **Comparison - Wrong vs Right**

| Aspect | ? Bot with DB Access | ? Bot with API Access |
|--------|----------------------|----------------------|
| **Security** | Bot token leak = Full DB access | Bot token leak = Limited API access |
| **Authentication** | None | JWT token required |
| **Validation** | Must duplicate logic | API handles all validation |
| **Audit Logging** | Impossible | Every action logged |
| **Schema Changes** | Bot breaks | API provides abstraction |
| **Connection Pooling** | Bot handles poorly | API optimized |
| **Rate Limiting** | None | API enforces limits |
| **Testing** | Must mock database | Mock HTTP responses |
| **Deployment** | Needs DB credentials | Only needs API URL + token |

---

## ? **Summary - The Correct Way**

1. **Bot** ? HTTP calls ? **API** ? Database
2. **GameEngine** ? Writes to Database ? Publishes Redis events ? **Bot** listens (read-only)
3. **Bot** subscribes to Redis Pub/Sub for notifications (read-only)
4. **Bot** uses JWT service account token for API authentication
5. **Bot** NEVER has connection string to database

---

## ?? **Implementation Checklist**

- [ ] Remove any database references from `Nightstorm.Bot.csproj`
- [ ] Create `ApiClient` service in bot project
- [ ] Create bot service account in API
- [ ] Generate long-lived JWT token for bot
- [ ] Store token in bot's `appsettings.json` (environment variable in production)
- [ ] Implement Redis Pub/Sub subscriber in bot (read-only)
- [ ] Update all Discord command handlers to use `ApiClient`
- [ ] Add rate limiting for bot service account in API
- [ ] Add audit logging for bot actions in API
- [ ] Test bot token rotation (token expiry handling)

---

**This is the ONLY correct architecture for bot integration.** ??
