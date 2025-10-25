using DataServiceLayer;
using DataServiceLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebServiceLayer;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly BookmarkDataService _bookmarkService;

    public UserController()
    {
        _bookmarkService = new BookmarkDataService();
    }

    // GET: api/users/{username}/bookmarks
    [HttpGet("{username}/bookmarks")]
    public IActionResult GetBookmarks(
        string username,
        [FromQuery] int page = 0,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest(new { message = "Username is required" });

        var (bookmarks, totalCount) = _bookmarkService.GetUserBookmarks(username, page, pageSize);

        if (bookmarks == null || bookmarks.Count == 0)
            return NotFound(new { message = "No bookmarks found" });

        // Map to DTOs
        var bookmarkDtos = bookmarks.Select(b => new BookmarkDto
        {
            TitleId = b.TitleId,
            Url = Url.Action(nameof(GetBookmarks), new { username }) ?? string.Empty
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
    [HttpPost("{username}/bookmarks")]
    public IActionResult AddBookmark(string username, [FromBody] CreateBookmarkDto req)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest(new { message = "Username is required" });

        if (req == null || string.IsNullOrWhiteSpace(req.TitleId))
            return BadRequest(new { message = "TitleId is required" });

        // Check if bookmark already exists
        if (_bookmarkService.BookmarkExists(username, req.TitleId))
            return Conflict(new { message = "Bookmark already exists" });

        // Add the bookmark
        var success = _bookmarkService.AddBookmark(username, req.TitleId);

        if (!success)
            return StatusCode(500, new { message = "Failed to create bookmark" });

        // Get the created bookmark
        var bookmark = _bookmarkService.GetBookmark(username, req.TitleId);

        if (bookmark == null)
            return StatusCode(500, new { message = "Failed to retrieve created bookmark" });

        var bookmarkDto = new BookmarkDto
        {
            TitleId = bookmark.TitleId,
            Url = Url.Action(nameof(GetBookmarks), new { username }) ?? string.Empty
        };

        var uri = Url.Action(nameof(GetBookmarks), new { username }) ?? string.Empty;
        return Created(uri, bookmarkDto);
    }

    // DELETE: api/users/{username}/bookmarks/{titleId}
    [HttpDelete("{username}/bookmarks/{titleId}")]
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
    [HttpGet("{username}/bookmarks/{titleId}")]
    public IActionResult GetBookmark(string username, string titleId)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(titleId))
            return BadRequest(new { message = "Username and TitleId are required" });

        var bookmark = _bookmarkService.GetBookmark(username, titleId);

        if (bookmark == null)
            return NotFound(new { message = "Bookmark not found" });

        var bookmarkDto = new BookmarkDto
        {
            TitleId = bookmark.TitleId,
            Url = Url.Action(nameof(GetBookmarks), new { username }) ?? string.Empty
        };

        return Ok(bookmarkDto);
    }

    // GET: api/users/{username}/bookmarks/count
    [HttpGet("{username}/bookmarks/count")]
    public IActionResult GetBookmarkCount(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest(new { message = "Username is required" });

        var count = _bookmarkService.GetBookmarkCount(username);

        return Ok(new { count });
    }
}