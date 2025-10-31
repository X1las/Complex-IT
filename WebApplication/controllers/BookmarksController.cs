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

    public BookmarksController()
    {
        _bookmarkService = new BookmarkDataService();
    }

    // GET: api/users/{username}/bookmarks
    [HttpGet]
    public IActionResult GetUserBookmarks(
        string username,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest(new ErrorResponseDto {Error = "Username is required" });

        var (bookmarks, totalCount) = _bookmarkService.GetUserBookmarks(username);

        if (bookmarks == null || bookmarks.Count == 0)
            return NotFound(new ErrorResponseDto {Error = "No bookmarks found for the user" });

        // Create DTO
        var bookmarkDtos = bookmarks.Select(b => new BookmarkDto
        {
            Url = Url.Action(nameof(GetUserBookmarks), new { username }) ?? string.Empty,
            TitleId = b.TitleId,

        }).ToList();

        // Create paged result
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var response = new PagedResultDto<BookmarkDto>
        {
            Items = bookmarkDtos,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalCount,
            TotalPages = totalPages,
        };

        // Return paged result
        return Ok(response);
    }

    // POST: api/users/{username}/bookmarks
    [HttpPost]
    public IActionResult AddBookmark(string username,
        [FromBody] CreateBookmarkDto request)
    {
        // Input validation
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest(new ErrorResponseDto {Error = "Username is required" });

        if (request == null || string.IsNullOrWhiteSpace(request.TitleId))
            return BadRequest(new ErrorResponseDto {Error = "TitleId is required" });

        if (_bookmarkService.BookmarkExists(username, request.TitleId))
            return Conflict(new ErrorResponseDto {Error = "Bookmark already exists" });

        var success = _bookmarkService.AddBookmark(username, request.TitleId);

        if (!success)
            return StatusCode(500, new ErrorResponseDto {Error = "Failed to create bookmark" });

        // Get the created bookmark
        var bookmark = _bookmarkService.GetBookmark(username, request.TitleId);

        if (bookmark == null)
            return StatusCode(500, new ErrorResponseDto {Error = "Failed to retrieve created bookmark" });

        return Created();
    }

    // DELETE: api/users/{username}/bookmarks/{titleId}
    [HttpDelete("{titleId}")]
    public IActionResult RemoveBookmark(string username, string titleId)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(titleId))
            return BadRequest(new ErrorResponseDto {Error = "Username and TitleId are required" });

        var success = _bookmarkService.RemoveBookmark(username, titleId);

        if (!success)
            return NotFound(new ErrorResponseDto {Error = "Bookmark not found" });

        return Ok();
    }
}