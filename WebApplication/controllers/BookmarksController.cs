using DataServiceLayer;
using DataServiceLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebServiceLayer.Models;

[ApiController]
[Route("api/users/{username}/bookmarks")]
public class BookmarksController : ControllerBase
{
    private readonly BookmarkServices _bookmarkService;

    public BookmarksController()
    {
        _bookmarkService = new BookmarkServices();
    }

    // GET: api/users/{username}/bookmarks
    [HttpGet]
    public IActionResult GetUserBookmarks(
        string username,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest(new { message = "Username is required" });

        var (bookmarks, totalCount) = _bookmarkService.GetUserBookmarks(username, page, pageSize);

        // Map to DTOs
        var bookmarkDtos = bookmarks.Select(b => new BookmarkDto
        {
            Url = Url.Action(nameof(GetUserBookmarks), new { username }) ?? string.Empty,
            TitleId = b.TitleId,
        
        }).ToList();

        return Ok(new
        {
            data = bookmarkDtos,
            totalCount,
            page,
            pageSize
        });
    }

    // POST: api/users/{username}/bookmarks
    [HttpPost]
    public IActionResult AddBookmark(string username, [FromBody] CreateBookmarkDto request)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest(new { message = "Username is required" });

        if (request == null || string.IsNullOrWhiteSpace(request.TitleId))
            return BadRequest(new { message = "TitleId is required" });

        if (_bookmarkService.BookmarkExists(username, request.TitleId))
            return Conflict(new { message = "Bookmark already exists" });

        var success = _bookmarkService.AddBookmark(username, request.TitleId);

        if (!success)
            return StatusCode(500, new { message = "Failed to create bookmark" });

        // Get the created bookmark
        var bookmark = _bookmarkService.GetBookmark(username, request.TitleId);

        if (bookmark == null)
            return StatusCode(500, new { message = "Failed to retrieve created bookmark" });

        var bookmarkDto = new BookmarkDto
        {
            Url = Url.Action(nameof(GetUserBookmarks), new { username }) ?? string.Empty,
            TitleId = bookmark.TitleId,
        };

        var uri = Url.Action(nameof(GetUserBookmarks), new { username }) ?? string.Empty;
        return Created(uri, bookmarkDto);
    }

    // DELETE: api/users/{username}/bookmarks/{titleId}
    [HttpDelete("{titleId}")]
    public IActionResult RemoveBookmark(string username, string titleId)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(titleId))
            return BadRequest(new { message = "Username and TitleId are required" });

        var success = _bookmarkService.RemoveBookmark(username, titleId);

        if (!success)
            return NotFound(new { message = "Bookmark not found" });

        return Ok(new { message = "Bookmark removed successfully" });
    }

    // GET: api/users/{username}/bookmarks/{titleId}
    [HttpGet("{titleId}")]
    public IActionResult GetBookmark(string username, string titleId)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(titleId))
            return BadRequest(new { message = "Username and TitleId are required" });

        var bookmark = _bookmarkService.GetBookmark(username, titleId);

        if (bookmark == null)
            return NotFound(new { message = "Bookmark not found" });

        var bookmarkDto = new BookmarkDto
        {
            Url = Url.Action(nameof(GetUserBookmarks), new { username }) ?? string.Empty,
            TitleId = bookmark.TitleId,
        };

        return Ok(bookmarkDto);
    }

    // GET: api/users/{username}/bookmarks/count
    [HttpGet("count")]
    public IActionResult GetBookmarkCount(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest(new { message = "Username is required" });

        var count = _bookmarkService.GetBookmarkCount(username);

        return Ok(new { count });
    }
}