using DataServiceLayer;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Linq;

namespace WebServiceLayer;

[Route("api/users")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly DataService _dataService;
    private readonly LinkGenerator _generator;
    private readonly IMapper _mapper;
    private readonly ImdbContext _context;

    public UserController(
        DataService DSUser,
        LinkGenerator generator,
        IMapper mapper,
        ImdbContext context)
    {
        _dataService = DSUser;
        _generator = generator;
        _mapper = mapper;
        _context = context;
    }

    [HttpGet("{username}/bookmarks")]
    public IActionResult GetBookmarks(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            return BadRequest();

        var bookmarks = _dataService.GetUserBookmarksByUsername(_context.Bookmark, username);
        if (bookmarks.Count == 0)
            return NotFound();

        var models = bookmarks.Select(CreateBookmarkModel).ToList();
        return Ok(models);
    }

    [HttpPost("{username}/bookmarks")]
    public IActionResult AddBookmark(string username, [FromBody] CreateBookmarkRequest req)
    {
        if (string.IsNullOrWhiteSpace(username) || req == null || string.IsNullOrWhiteSpace(req.TitleId))
            return BadRequest();

        // check for samme (username, titleId)
        var exists = _context.Bookmark.Any(b => b.Username == username && b.TitleId == req.TitleId);
        if (exists)
            return Conflict("Bookmark already exists");

        var bookmark = new Bookmarks { Username = username, TitleId = req.TitleId };
        var success = _dataService.CreateUserBookmark(_context.Bookmark, bookmark);

        if (!success)
            return StatusCode(500);

        var model = CreateBookmarkModel(bookmark);
        var uri = _generator.GetUriByName(HttpContext, nameof(GetBookmarks), new { username }) ?? string.Empty;
        return Created(uri, model);
    }

    private BookmarkModel CreateBookmarkModel(Bookmarks bookmark)
    {
        var model = _mapper.Map<BookmarkModel>(bookmark);
        model.Url = _generator.GetUriByName(HttpContext, nameof(GetBookmarks), new { username = bookmark.Username }) ?? string.Empty;
        return model;
    }
}