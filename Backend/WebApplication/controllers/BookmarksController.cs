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

    private IActionResult? ValidateUserAccess(string requestedUsername)
    {
        var authenticatedUsername = User?.Identity?.Name;
        
        if (string.IsNullOrWhiteSpace(authenticatedUsername))
        {
            return Unauthorized(new ErrorResponseDto { Error = "Unable to determine authenticated user" });
        }

        if (authenticatedUsername != requestedUsername)
        {
            return Forbid(); // Sends a 403
        }

        return null;
    }

    // GET: api/users/{username}/bookmarks
    [HttpGet]
    public async Task<IActionResult> GetUserBookmarks(string username,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var validationResult = ValidateUserAccess(username);
        if (validationResult != null) return validationResult;
        try
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(username))
            {
                return BadRequest(new ErrorResponseDto { Error = "Username is required" });
            }

            var (bookmarks, totalCount) = await Task.Run(() => _bookmarkService.GetUserBookmarks(username));

            if (bookmarks == null || bookmarks.Count == 0)
            {
                return NotFound(new ErrorResponseDto { Error = "No bookmarks found for the user" });
            }

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
            var paginatedBookmarks = bookmarks
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Create DTO
            var bookmarkDtos = paginatedBookmarks.Select(b => new BookmarkDisplayItemDto
            {
                Url = $"{Request.Scheme}://{Request.Host}/api/users/{username}/bookmarks/{b.TitleId}",
                TitleId = b.TitleId,
            }).ToList();

            _logger.LogInformation("User {Username} retrieved bookmarks", username);
            var response = new PagedResultDto<BookmarkDisplayItemDto>
            {
                Items = bookmarkDtos,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalCount,
                TotalPages = totalPages
            };

            return Ok(response);
        }
        catch (Exception ex)
        {
        _logger.LogError(ex, "Error retrieving bookmarks for user");
        return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving bookmarks" });
        }
    }

    // POST: api/users/{username}/bookmarks
    [HttpPost]
    public async Task<IActionResult> AddBookmark(UserBookmarkDto model)
    {
        // Input validation - check for null model first
        if (model == null)
        {
            return BadRequest(new ErrorResponseDto { Error = "Request body is required" });
        }
        
        var validationResult = ValidateUserAccess(model.Username);
        if (validationResult != null) return validationResult;
        
        try
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(model.Username))
            {
                return BadRequest(new ErrorResponseDto { Error = "Username is required" });
            }

            if (string.IsNullOrWhiteSpace(model.TitleId))
            {
                return BadRequest(new ErrorResponseDto { Error = "TitleId is required" });
            }

            if (await Task.Run(() => _bookmarkService.BookmarkExists(model.Username, model.TitleId)))
                return Conflict(new ErrorResponseDto { Error = "Bookmark already exists" });

            await Task.Run(() => _bookmarkService.AddBookmark(model.Username, model.TitleId));

            // Get the created bookmark
            var bookmark = await Task.Run(() => _bookmarkService.GetBookmark(model.Username, model.TitleId));

            if (bookmark == null)
            {
                return StatusCode(500, new ErrorResponseDto { Error = "Failed to retrieve created bookmark" });
            }

            _logger.LogInformation("User {Username} added bookmark for title {TitleId}", model.Username, model.TitleId);

            return Created();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding bookmark for user");
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while adding bookmark" });
        }
    }

    // DELETE: api/users/{username}/bookmarks/{titleId}
    [HttpDelete("{titleId}")]
    public async Task<IActionResult> RemoveBookmark(UserBookmarkDto model)
    {
        // Input validation - check for null model first
        if (model == null)
        {
            return BadRequest(new ErrorResponseDto { Error = "Request body is required" });
        }
        
        var validationResult = ValidateUserAccess(model.Username);
        if (validationResult != null) return validationResult;

        try
        {
            if (string.IsNullOrWhiteSpace(model.Username))
            {
                return BadRequest(new ErrorResponseDto { Error = "Username is required" });
            }
            if (string.IsNullOrWhiteSpace(model.TitleId))
            {
                return BadRequest(new ErrorResponseDto { Error = "TitleId is required" });
            }

            await Task.Run(() => _bookmarkService.RemoveBookmark(model.Username, model.TitleId));

            _logger.LogInformation("User {Username} removed bookmark for title {TitleId}", model.Username, model.TitleId);

            var bookmark = await Task.Run(() => _bookmarkService.GetBookmark(model.Username, model.TitleId));
            if (bookmark == null)
            {
                return Ok();
            }
            else
            {
                return StatusCode(500, new ErrorResponseDto { Error = "Failed to delete bookmark" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing bookmark for user");
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while removing bookmark" });
        }
    }
}