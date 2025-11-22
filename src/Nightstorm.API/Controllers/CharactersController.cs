using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nightstorm.API.DTOs.Characters;
using Nightstorm.Core.Constants;
using Nightstorm.Core.Entities;
using Nightstorm.Core.Interfaces.Repositories;
using Nightstorm.Core.Interfaces.Services;
using Nightstorm.Data.Contexts;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Nightstorm.API.Controllers;

/// <summary>
/// API controller for character management operations.
/// Optimized for high concurrency (100s of requests/second).
/// All endpoints require JWT authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize] // Require authentication for all endpoints
public class CharactersController : ControllerBase
{
    private readonly ICharacterRepository _characterRepository;
    private readonly ICharacterStatsService _statsService;
    private readonly RpgContext _context;
    private readonly ILogger<CharactersController> _logger;

    public CharactersController(
        ICharacterRepository characterRepository,
        ICharacterStatsService statsService,
        RpgContext context,
        ILogger<CharactersController> logger)
    {
        _characterRepository = characterRepository;
        _statsService = statsService;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Gets the current authenticated user's character.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current user's character</returns>
    /// <response code="200">Character found</response>
    /// <response code="404">User has no character</response>
    [HttpGet("me")]
    [ProducesResponseType(typeof(CharacterDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CharacterDetailResponse>> GetMyCharacter(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var character = await _context.Characters
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (character == null)
        {
            return NotFound(new { message = "You don't have a character yet. Create one with POST /api/characters" });
        }

        return Ok(MapToDetailResponse(character));
    }

    /// <summary>
    /// Gets a character by ID (any user can view).
    /// </summary>
    /// <param name="id">Character ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Character details</returns>
    /// <response code="200">Character found</response>
    /// <response code="404">Character not found</response>
    [HttpGet("{id:guid}")]
    [AllowAnonymous] // Public endpoint - anyone can view characters
    [ProducesResponseType(typeof(CharacterDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CharacterDetailResponse>> GetCharacter(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var character = await _characterRepository.GetByIdAsync(id, cancellationToken);

        if (character == null)
        {
            return NotFound(new { message = "Character not found" });
        }

        return Ok(MapToDetailResponse(character));
    }

    /// <summary>
    /// Gets a character by Discord user ID (backward compatibility).
    /// </summary>
    /// <param name="discordUserId">Discord user ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Character details</returns>
    /// <response code="200">Character found</response>
    /// <response code="404">Character not found</response>
    [HttpGet("discord/{discordUserId:long}")]
    [AllowAnonymous] // Public endpoint
    [ProducesResponseType(typeof(CharacterDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CharacterDetailResponse>> GetCharacterByDiscordId(
        [FromRoute] ulong discordUserId,
        CancellationToken cancellationToken)
    {
        var character = await _characterRepository.GetByDiscordUserIdAsync(discordUserId, cancellationToken);

        if (character == null)
        {
            return NotFound(new { message = "Character not found for this Discord user" });
        }

        return Ok(MapToDetailResponse(character));
    }

    /// <summary>
    /// Gets all characters with pagination (public leaderboard).
    /// </summary>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of characters</returns>
    /// <response code="200">List of characters</response>
    [HttpGet]
    [AllowAnonymous] // Public leaderboard
    [ProducesResponseType(typeof(PagedResponse<CharacterResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<CharacterResponse>>> GetCharacters(
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var allCharacters = await _characterRepository.GetAllAsync(cancellationToken);
        var charactersList = allCharacters.ToList();
        
        var totalCount = charactersList.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        
        var pagedCharacters = charactersList
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToResponse)
            .ToList();

        return Ok(new PagedResponse<CharacterResponse>
        {
            Items = pagedCharacters,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < totalPages
        });
    }

    /// <summary>
    /// Gets top characters by level for leaderboard (public).
    /// </summary>
    /// <param name="count">Number of characters (default: 10, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of top characters</returns>
    /// <response code="200">List of top characters</response>
    [HttpGet("leaderboard")]
    [AllowAnonymous] // Public leaderboard
    [ProducesResponseType(typeof(IEnumerable<CharacterResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CharacterResponse>>> GetLeaderboard(
        [FromQuery, Range(1, 100)] int count = 10,
        CancellationToken cancellationToken = default)
    {
        var topCharacters = await _characterRepository.GetTopByLevelAsync(count, cancellationToken);
        return Ok(topCharacters.Select(MapToResponse));
    }

    /// <summary>
    /// Gets characters in a guild with pagination (public).
    /// </summary>
    /// <param name="guildId">Guild ID</param>
    /// <param name="pageNumber">Page number (default: 1)</param>
    /// <param name="pageSize">Page size (default: 50, max: 100)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of guild members</returns>
    /// <response code="200">List of guild members</response>
    [HttpGet("guild/{guildId:guid}")]
    [AllowAnonymous] // Public guild roster
    [ProducesResponseType(typeof(PagedResponse<CharacterResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResponse<CharacterResponse>>> GetGuildCharacters(
        [FromRoute] Guid guildId,
        [FromQuery, Range(1, int.MaxValue)] int pageNumber = 1,
        [FromQuery, Range(1, 100)] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        var (characters, totalCount) = await _characterRepository.GetByGuildIdAsync(
            guildId, pageNumber, pageSize, cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        return Ok(new PagedResponse<CharacterResponse>
        {
            Items = characters.Select(MapToResponse),
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            HasPreviousPage = pageNumber > 1,
            HasNextPage = pageNumber < totalPages
        });
    }

    /// <summary>
    /// Creates a new character for the authenticated user.
    /// Users can only have one character.
    /// </summary>
    /// <param name="request">Character creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created character</returns>
    /// <response code="201">Character created successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="409">User already has a character or name is taken</response>
    [HttpPost]
    [ProducesResponseType(typeof(CharacterDetailResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CharacterDetailResponse>> CreateCharacter(
        [FromBody] CreateCharacterRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        // Validate name length
        if (request.Name.Length < CharacterConstants.MinNameLength || 
            request.Name.Length > CharacterConstants.MaxNameLength)
        {
            return BadRequest(new { message = $"Character name must be between {CharacterConstants.MinNameLength} and {CharacterConstants.MaxNameLength} characters" });
        }

        // Check if user already has a character
        var existingCharacter = await _context.Characters
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (existingCharacter != null)
        {
            return Conflict(new { message = "You already have a character" });
        }

        // Check if name is taken
        var nameTaken = await _characterRepository.IsNameTakenAsync(request.Name, null, cancellationToken);
        if (nameTaken)
        {
            return Conflict(new { message = "Character name is already taken" });
        }

        // Get user for DiscordUserId backward compatibility
        var user = await _context.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user == null)
        {
            return Unauthorized(new { message = "User not found" });
        }

        // Create character
        var character = new Character
        {
            UserId = userId,
            DiscordUserId = user.DiscordId ?? 0, // Backward compatibility
            Name = request.Name,
            Class = request.Class,
            Level = CharacterConstants.DefaultLevel,
            Experience = CharacterConstants.DefaultExperience,
            Gold = CharacterConstants.DefaultGold
        };

        // Initialize stats based on class
        character.InitializeStats(_statsService);

        // Save to database
        await _characterRepository.AddAsync(character, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Character created: {CharacterId} for user {UserId}",
            character.Id, userId);

        return CreatedAtAction(
            nameof(GetCharacter),
            new { id = character.Id },
            MapToDetailResponse(character));
    }

    /// <summary>
    /// Updates the authenticated user's character.
    /// Users can only update their own character.
    /// </summary>
    /// <param name="request">Update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated character</returns>
    /// <response code="200">Character updated successfully</response>
    /// <response code="400">Invalid request</response>
    /// <response code="404">Character not found</response>
    /// <response code="409">Character name already taken</response>
    [HttpPut("me")]
    [ProducesResponseType(typeof(CharacterDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CharacterDetailResponse>> UpdateMyCharacter(
        [FromBody] UpdateCharacterRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var character = await _context.Characters
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (character == null)
        {
            return NotFound(new { message = "You don't have a character" });
        }

        // Update name if provided
        if (!string.IsNullOrWhiteSpace(request.Name) && request.Name != character.Name)
        {
            if (request.Name.Length < CharacterConstants.MinNameLength || 
                request.Name.Length > CharacterConstants.MaxNameLength)
            {
                return BadRequest(new { message = $"Character name must be between {CharacterConstants.MinNameLength} and {CharacterConstants.MaxNameLength} characters" });
            }

            var nameTaken = await _characterRepository.IsNameTakenAsync(request.Name, character.Id, cancellationToken);
            if (nameTaken)
            {
                return Conflict(new { message = "Character name is already taken" });
            }

            character.Name = request.Name;
        }

        // Update guild if provided
        if (request.GuildId.HasValue)
        {
            character.GuildId = request.GuildId.Value;
        }

        await _characterRepository.UpdateAsync(character, cancellationToken: cancellationToken);

        _logger.LogInformation("Character updated: {CharacterId} by user {UserId}", character.Id, userId);

        return Ok(MapToDetailResponse(character));
    }

    /// <summary>
    /// Deletes the authenticated user's character (soft delete).
    /// Users can only delete their own character.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>No content</returns>
    /// <response code="204">Character deleted successfully</response>
    /// <response code="404">Character not found</response>
    [HttpDelete("me")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteMyCharacter(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        if (userId == Guid.Empty)
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var character = await _context.Characters
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);

        if (character == null)
        {
            return NotFound(new { message = "You don't have a character" });
        }

        _characterRepository.Remove(character);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Character deleted: {CharacterId} by user {UserId}", character.Id, userId);

        return NoContent();
    }

    // Helper methods

    /// <summary>
    /// Extracts user ID from JWT claims.
    /// </summary>
    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
    }

    private static CharacterResponse MapToResponse(Character character) => new()
    {
        Id = character.Id,
        DiscordUserId = character.DiscordUserId,
        Name = character.Name,
        Class = character.Class,
        Level = character.Level,
        Experience = character.Experience,
        CurrentHealth = character.CurrentHealth,
        MaxHealth = character.MaxHealth,
        CurrentMana = character.CurrentMana,
        MaxMana = character.MaxMana,
        Gold = character.Gold,
        GuildId = character.GuildId,
        CreatedAt = character.CreatedAt,
        UpdatedAt = character.UpdatedAt
    };

    private static CharacterDetailResponse MapToDetailResponse(Character character) => new()
    {
        Id = character.Id,
        DiscordUserId = character.DiscordUserId,
        Name = character.Name,
        Class = character.Class,
        Level = character.Level,
        Experience = character.Experience,
        CurrentHealth = character.CurrentHealth,
        MaxHealth = character.MaxHealth,
        CurrentMana = character.CurrentMana,
        MaxMana = character.MaxMana,
        Gold = character.Gold,
        GuildId = character.GuildId,
        CreatedAt = character.CreatedAt,
        UpdatedAt = character.UpdatedAt,
        Strength = character.Strength,
        Dexterity = character.Dexterity,
        Constitution = character.Constitution,
        Intelligence = character.Intelligence,
        Wisdom = character.Wisdom,
        Spirit = character.Spirit,
        Luck = character.Luck
    };
}
