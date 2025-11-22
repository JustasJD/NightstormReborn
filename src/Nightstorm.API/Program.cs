using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Nightstorm.API.Services;
using Nightstorm.Core.Interfaces.Repositories;
using Nightstorm.Core.Interfaces.Services;
using Nightstorm.Core.Services;
using Nightstorm.Data.Contexts;
using Nightstorm.Data.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddOpenApi();

// Configure PostgreSQL DbContext with connection pooling for high concurrency
builder.Services.AddDbContext<RpgContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        npgsqlOptions =>
        {
            npgsqlOptions.MigrationsAssembly("Nightstorm.Data");
            npgsqlOptions.EnableRetryOnFailure(maxRetryCount: 3); // Retry on transient failures
            npgsqlOptions.CommandTimeout(30); // 30 second timeout
        });

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(); // Log parameter values in dev
        options.EnableDetailedErrors(); // Detailed error messages in dev
    }
});

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

        // Support JWT from query string (for SignalR future use)
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

// Register repositories - Scoped for per-request instances
builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();

// Register services - Singleton for stateless services
builder.Services.AddSingleton<ICharacterStatsService, CharacterStatsService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

// Configure CORS for Bot/Web clients
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBotAndWeb", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add controllers with JSON options for high performance
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // Keep PascalCase
        options.JsonSerializerOptions.WriteIndented = false; // Minimize payload size
    });

// Add response compression for reduced bandwidth
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

// Add output caching for frequently-accessed read-only data
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(10)));
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseDeveloperExceptionPage();
}

app.UseResponseCompression();
app.UseOutputCache();

app.UseHttpsRedirection();
app.UseCors("AllowBotAndWeb");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint with database connectivity test
app.MapGet("/api/health", async (RpgContext db) =>
{
    var canConnect = await db.Database.CanConnectAsync();
    return Results.Ok(new 
    { 
        status = canConnect ? "healthy" : "unhealthy",
        database = "PostgreSQL",
        timestamp = DateTime.UtcNow
    });
})
.WithName("HealthCheck")
.WithOpenApi()
.CacheOutput(); // Cache health check for 10 seconds

// Detailed health check with stats
app.MapGet("/api/health/detailed", async (RpgContext db) =>
{
    try
    {
        var canConnect = await db.Database.CanConnectAsync();
        
        if (!canConnect)
        {
            return Results.Json(new { status = "unhealthy", reason = "Cannot connect to database" }, statusCode: 503);
        }

        var characterCount = await db.Characters.CountAsync();
        var userCount = await db.Users.CountAsync();
        var migrations = await db.Database.GetAppliedMigrationsAsync();

        return Results.Ok(new
        {
            status = "healthy",
            database = "PostgreSQL",
            timestamp = DateTime.UtcNow,
            stats = new
            {
                totalUsers = userCount,
                totalCharacters = characterCount,
                appliedMigrations = migrations.Count()
            }
        });
    }
    catch (Exception ex)
    {
        return Results.Json(new 
        { 
            status = "unhealthy", 
            error = ex.Message,
            timestamp = DateTime.UtcNow
        }, statusCode: 503);
    }
})
.WithName("DetailedHealthCheck")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
