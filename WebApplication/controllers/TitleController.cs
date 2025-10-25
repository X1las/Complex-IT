using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
using WebServiceLayer.Models;

namespace WebServiceLayer;

[Route("api/titles")]
[ApiController]
public class TitleController : ControllerBase
{
    private readonly TitleServices _titleService;

    public TitleController()
    {
        _titleService = new TitleServices();
    }

    // GET: api/titles
    [HttpGet]
    public IActionResult GetTitles(
        [FromQuery] int? userId = null)
    {
        var (titles, totalCount) = _titleService.GetTitles(userId);

        if (titles == null || titles.Count == 0)
            return NotFound(new { message = "No titles found" });

        var titleDtos = titles.Select(t => new TitleModel
        {
            Id = t.Id,
            Title = t.Title ?? string.Empty,
            TitleType = t.TitleType ?? string.Empty,
            Year = t.StartYear ?? string.Empty,
            Rating = t.Rating,
            Url = Url.Action(nameof(GetTitle), new { id = t.Id }) ?? string.Empty
        }).ToList();

        return Ok(new
        {
            data = titleDtos,
            totalCount,
        });
    }

    // GET: api/titles/{id}
    [HttpGet("{id}")]
    public IActionResult GetTitle(string id, [FromQuery] int? userId = null)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { message = "Title ID is required" });

        var title = _titleService.GetTitle(id, userId);

        if (title == null)
            return NotFound(new { message = "Title not found" });

        var titleDto = new TitleModel
        {
            Id = title.Id,
            Title = title.Title ?? string.Empty,
            TitleType = title.TitleType ?? string.Empty,
            Year = title.StartYear ?? string.Empty,
            Rating = title.Rating,
            Url = Url.Action(nameof(GetTitle), new { id = title.Id }) ?? string.Empty
        };

        return Ok(titleDto);
    }

    // GET: api/titles/search
    [HttpGet("search")]
    public IActionResult SearchTitles(
        [FromQuery] string query,
        [FromQuery] int? userId = null)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest(new { message = "Search query is required" });

        var (results, totalCount) = _titleService.SearchTitles(query, userId);

        if (results == null || results.Count == 0)
            return NotFound(new { message = "No titles found matching the search query" });

        var searchDtos = results.Select(t => new TitleSearchModel
        {
            Id = t.Id,
            Title = t.Title ?? string.Empty,
            TitleType = t.TitleType,
            Year = t.StartYear ?? string.Empty,
            Rating = t.Rating,
            Url = Url.Action(nameof(GetTitle), new { id = t.Id }) ?? string.Empty
        }).ToList();

        return Ok(new
        {
            data = searchDtos,
            query
        });
    }

    // GET: api/titles/{id}/genres
    [HttpGet("{id}/genres")]
    public IActionResult GetTitleGenres(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { message = "Title ID is required" });

        if (!_titleService.TitleExists(id))
            return NotFound(new { message = "Title not found" });

        var genres = _titleService.GetTitleGenres(id);

        if (genres == null || genres.Count == 0)
            return NotFound(new { message = "No genres found for this title" });

        return Ok(new { data = genres });
    }

    // GET: api/titles/{id}/cast
    [HttpGet("{id}/cast")]
    public IActionResult GetTitleCast(string id, [FromQuery] int limit = 10)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { message = "Title ID is required" });

        if (!_titleService.TitleExists(id))
            return NotFound(new { message = "Title not found" });

        var cast = _titleService.GetTitleCast(id, limit);

        if (cast == null || cast.Count == 0)
            return NotFound(new { message = "No cast found for this title" });

        return Ok(new { data = cast });
    }

    // GET: api/titles/genre/{genre}
    [HttpGet("genre/{genre}")]
    public IActionResult GetTitlesByGenre(
        string genre
        )
    {
        if (string.IsNullOrWhiteSpace(genre))
            return BadRequest(new { message = "Genre is required" });

        var (titles, totalCount) = _titleService.GetTitlesByGenre(genre);

        if (titles == null || titles.Count == 0)
            return NotFound(new { message = $"No titles found for genre '{genre}'" });

        var titleDtos = titles.Select(t => new TitleModelShort
        {
            Id = t.Id,
            Title = t.Title ?? string.Empty,
            TitleType = t.TitleType,
            Year = t.StartYear ?? string.Empty,
            Rating = t.Rating,
        }).ToList();

        return Ok(new
        {
            data = titleDtos,
            totalCount,
            genre
        });
    }

    // GET: api/titles/{id}/episodes
    [HttpGet("{id}/episodes")]
    public IActionResult GetTitleEpisodes(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new { message = "Series ID is required" });

        if (!_titleService.TitleExists(id))
            return NotFound(new { message = "Series not found" });

        var episodes = _titleService.GetTitleEpisodes(id);

        if (episodes == null || episodes.Count == 0)
            return NotFound(new { message = "No episodes found for this series" });

        var episodeDtos = episodes.Select(e => new TitleEpisodesModel
        {
            EpisodessId = e.EpisodeId,
            SeriesId = e.SeriesId ?? string.Empty,
            SeasonNumber = e.SeasonNumber,
            EpisodeNumber = e.EpisodeNumber,
            EpisodeTitle = e.EpisodeId, // You may need to join with Titles table to get actual title
            Url = Url.Action(nameof(GetTitle), new { id = e.EpisodeId }) ?? string.Empty
        }).ToList();

        return Ok(new { data = episodeDtos });
    }
}