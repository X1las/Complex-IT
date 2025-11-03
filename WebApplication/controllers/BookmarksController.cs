using DataServiceLayer;
using DataServiceLayer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebServiceLayer.Models;

[Authorize]
[ApiController]
[Route("api/users/{username}/bookmarks")]
public class BookmarksController : ControllerBase
{
    private readonly BookmarkDataService _bookmarkService;
    private readonly ILogger<BookmarksController> _logger;

    public BookmarksController(ILogger<BookmarksController> logger)
    {
        _bookmarkService = new BookmarkDataService();
        _logger = logger;
    }

    private string GetAuthenticatedUsername()
    {
        return User?.Identity?.Name ?? throw new UnauthorizedAccessException("User not authenticated");
    }

    // Validate that the route username matches the authenticated user
    private bool ValidateUsername(string username)
    {
        var authenticatedUsername = GetAuthenticatedUsername();
        return username.Equals(authenticatedUsername, StringComparison.OrdinalIgnoreCase);
    }

    // GET: api/users/{username}/bookmarks
    [HttpGet]
    public async Task<IActionResult> GetUserBookmarks(
    string username,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 10)
    {
        try
        {
            if (!ValidateUsername(username))
            {
            return Forbid(); // 403 Forbidden
            }

        // Input validation
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest(new ErrorResponseDto { Error = "Username is required" });

        var (bookmarks, totalCount) = await Task.Run(() => _bookmarkService.GetUserBookmarks(username));

        if (bookmarks == null || bookmarks.Count == 0)
            return NotFound(new ErrorResponseDto { Error = "No bookmarks found for the user" });

        // Calculate pagination
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        // Apply pagination
        var paginatedBookmarks = bookmarks
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Create DTO
        var bookmarkDtos = paginatedBookmarks.Select(b => new BookmarkDto
        {
            Url = $"{Request.Scheme}://{Request.Host}/api/users/{username}/bookmarks/{b.TitleId}",
            TitleId = b.TitleId,
        }).ToList();

        var response = new PagedResultDto<BookmarkDto>
        {
            Items = bookmarkDtos,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalCount,
            TotalPages = totalPages
        };

        return Ok(response);
        }
        catch (UnauthorizedAccessException)
        {
        return Unauthorized(new ErrorResponseDto { Error = "User not authenticated" });
        }
        catch (Exception ex)
        {
        _logger.LogError(ex, "Error retrieving bookmarks for user");
        return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving bookmarks" });
        }
    }

    // POST: api/users/{username}/bookmarks
    [HttpPost]
    public async Task<IActionResult> AddBookmark(string username,
        [FromBody] CreateBookmarkDto request)
    {
        try
        {
            if (!ValidateUsername(username))
            {
                return Forbid(); // 403 Forbidden
            }

            // Input validation
            if (string.IsNullOrWhiteSpace(username))
                return BadRequest(new ErrorResponseDto { Error = "Username is required" });

            if (request == null || string.IsNullOrWhiteSpace(request.TitleId))
                return BadRequest(new ErrorResponseDto { Error = "TitleId is required" });

            if (await Task.Run(() => _bookmarkService.BookmarkExists(username, request.TitleId)))
                return Conflict(new ErrorResponseDto { Error = "Bookmark already exists" });

            var success = await Task.Run(() => _bookmarkService.AddBookmark(username, request.TitleId));

            if (!success)
                return StatusCode(500, new ErrorResponseDto { Error = "Failed to create bookmark" });

            // Get the created bookmark
            var bookmark = await Task.Run(() => _bookmarkService.GetBookmark(username, request.TitleId));

            if (bookmark == null)
                return StatusCode(500, new ErrorResponseDto { Error = "Failed to retrieve created bookmark" });

            _logger.LogInformation("User {Username} added bookmark for title {TitleId}", username, request.TitleId);

            var response = new BookmarkDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{username}/bookmarks/{request.TitleId}",
                TitleId = request.TitleId
            };

            return CreatedAtAction(nameof(GetUserBookmarks), new { username }, response);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ErrorResponseDto { Error = "User not authenticated" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding bookmark for user");
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while adding bookmark" });
        }
    }

    // DELETE: api/users/{username}/bookmarks/{titleId}
    [HttpDelete("{titleId}")]
    public async Task<IActionResult> RemoveBookmark(string username, string titleId)
    {
        try
        {
            if (!ValidateUsername(username))
            {
                return Forbid(); // 403 Forbidden
            }

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(titleId))
                return BadRequest(new ErrorResponseDto { Error = "Username and TitleId are required" });

            var success = await Task.Run(() => _bookmarkService.RemoveBookmark(username, titleId));

            if (!success)
                return NotFound(new ErrorResponseDto { Error = "Bookmark not found" });

            _logger.LogInformation("User {Username} removed bookmark for title {TitleId}", username, titleId);

            return Ok(new { message = "Bookmark deleted successfully" });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ErrorResponseDto { Error = "User not authenticated" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing bookmark for user");
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while removing bookmark" });
        }
    }
}