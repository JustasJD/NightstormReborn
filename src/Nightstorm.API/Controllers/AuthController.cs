using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nightstorm.API.DTOs.Auth;
using Nightstorm.Core.Entities;
using Nightstorm.Core.Interfaces.Services;
using Nightstorm.Data.Contexts;
using System.Security.Claims;

namespace Nightstorm.API.Controllers;

/// <summary>
/// Authentication controller for user registration and login.
/// Supports Discord, email/password authentication.
/// </summary>
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
    /// Creates new user if doesn't exist, otherwise returns token for existing user.
    /// </summary>
    /// <param name="request">Discord user information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JWT token and user information</returns>
    /// <response code="200">Successfully authenticated</response>
    /// <response code="400">Invalid request</response>
    [HttpPost("discord")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> RegisterDiscord(
        [FromBody] RegisterDiscordRequest request,
        CancellationToken cancellationToken)
    {
        // Validate request
        if (request.DiscordId == 0)
        {
            return BadRequest(new { message = "Invalid Discord ID" });
        }

        if (string.IsNullOrWhiteSpace(request.DiscordUsername))
        {
            return BadRequest(new { message = "Discord username is required" });
        }

        // Check if user already exists
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.DiscordId == request.DiscordId, cancellationToken);

        User user;

        if (existingUser != null)
        {
            // Update last login and username
            existingUser.LastLoginAt = DateTime.UtcNow;
            existingUser.DiscordUsername = request.DiscordUsername;
            user = existingUser;
            
            _logger.LogInformation("Discord user logged in: {UserId} (Discord: {DiscordId})", 
                user.Id, request.DiscordId);
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
            
            _logger.LogInformation("New Discord user registered: {Username} (Discord: {DiscordId})", 
                request.DiscordUsername, request.DiscordId);
        }

        await _context.SaveChangesAsync(cancellationToken);

        // Generate JWT token
        var token = _jwtService.GenerateToken(user.Id, user.Username, "discord");
        var expiresIn = int.Parse(_configuration["Jwt:ExpiresInMinutes"]!) * 60;

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
    /// <param name="request">Registration information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JWT token and user information</returns>
    /// <response code="201">User created successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="409">Username or email already exists</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterEmailRequest request,
        CancellationToken cancellationToken)
    {
        // Validate password length
        if (request.Password.Length < 8)
        {
            return BadRequest(new { message = "Password must be at least 8 characters long" });
        }

        // Validate username length
        if (request.Username.Length < 3 || request.Username.Length > 50)
        {
            return BadRequest(new { message = "Username must be between 3 and 50 characters" });
        }

        // Check if username exists
        if (await _context.Users.AnyAsync(u => u.Username == request.Username, cancellationToken))
        {
            return Conflict(new { message = "Username is already taken" });
        }

        // Check if email exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            return Conflict(new { message = "Email is already registered" });
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

        _logger.LogInformation("New user registered: {UserId} ({Username})", user.Id, user.Username);

        // Generate token
        var token = _jwtService.GenerateToken(user.Id, user.Username, "web");
        var expiresIn = int.Parse(_configuration["Jwt:ExpiresInMinutes"]!) * 60;

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
    /// <param name="request">Login credentials</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JWT token and user information</returns>
    /// <response code="200">Successfully authenticated</response>
    /// <response code="401">Invalid credentials or account disabled</response>
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
            _logger.LogWarning("Login attempt with invalid email: {Email}", request.Email);
            return Unauthorized(new { message = "Invalid email or password" });
        }

        if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login attempt with invalid password for user: {UserId}", user.Id);
            return Unauthorized(new { message = "Invalid email or password" });
        }

        if (!user.IsActive)
        {
            _logger.LogWarning("Login attempt for disabled account: {UserId}", user.Id);
            return Unauthorized(new { message = "Account is disabled" });
        }

        // Update last login
        user.LastLoginAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("User logged in: {UserId}", user.Id);

        // Generate token
        var token = _jwtService.GenerateToken(user.Id, user.Username, "web");
        var expiresIn = int.Parse(_configuration["Jwt:ExpiresInMinutes"]!) * 60;

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
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current user information</returns>
    /// <response code="200">User information retrieved</response>
    /// <response code="401">Not authenticated</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(UserInfoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UserInfoResponse>> GetCurrentUser(CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (userIdClaim == null || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized();
        }

        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);

        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(new UserInfoResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            DiscordId = user.DiscordId,
            LastLoginAt = user.LastLoginAt,
            CreatedAt = user.CreatedAt
        });
    }
}
