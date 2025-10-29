using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebServiceLayer.Models;

namespace WebServiceLayer;

[Route("api/titles")]
[ApiController]
public class TitleController : ControllerBase
{
    private readonly TitleDataService _titleService;

    public TitleController(TitleDataService titleService)
    {
        _titleService = titleService;
    }

    // GET: api/titles
    [HttpGet]
    public IActionResult GetTitles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        List<Titles> titlesList;
        int totalCount;

        if (!string.IsNullOrWhiteSpace(search))
        {
            (titlesList, totalCount) = _titleService.SearchTitles(search);
        }
        else
        {
            (titlesList, totalCount) = _titleService.GetTitles();
        }
        
        // Calculate pagination
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        var titles = titlesList
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        if (titles == null || titles.Count == 0)
            return NotFound(new ErrorResponseDto { Error = "No titles found" });

        // Creating Title DTO
        var titleDtos = titles.Select(t => new TitleModelShort
        {
            Id = t.Id,
            Title = t.Title ?? string.Empty,
            TitleType = t.TitleType ?? string.Empty,
            Year = t.StartYear ?? string.Empty,
            Rating = t.Rating ?? 0,
            Url = Url.Action(nameof(GetTitle), new { id = t.Id }) ?? string.Empty
        }).ToList();
        
        // Converting Title DTO to Paginated Response
        var response = new PagedResultDto<TitleModelShort>
        {
            Items = titleDtos,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalCount,
            TotalPages = totalPages,
        };

        return Ok(response);
    }

    // GET: api/titles/{id}
    [HttpGet("{id}")]
    public IActionResult GetTitle(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new ErrorResponseDto { Error = "Title ID is required" }); // Fix 4: Use ErrorResponseDto consistently

        var title = _titleService.GetTitle(id);

        if (title == null)
            return NotFound(new ErrorResponseDto { Error = "Title not found" });

        var titleDto = new TitleModel
        {
            Id = title.Id,
            Title = title.Title ?? string.Empty,
            TitleType = title.TitleType ?? string.Empty,
            Year = title.StartYear ?? string.Empty,
            Rating = title.Rating ?? 0,
            Plot = title.Plot ?? string.Empty,
            PosterUrl = _titleService.GetTitlePoster(title.Id)?.Poster ?? string.Empty,
            WebsiteUrl = _titleService.GetTitleWebsite(title.Id)?.Website ?? string.Empty,
            StartYear = title.StartYear ?? string.Empty,
            EndYear = title.EndYear ?? string.Empty,
            ReleaseDate = title.Release_Date ?? string.Empty,
            RuntimeMinutes = _titleService.GetTitleRuntime(title.Id)?.Runtime ?? string.Empty,
            IsAdult = title.IsAdult.HasValue ? (title.IsAdult.Value ? "Yes" : "No") : "Unknown",
            Votes = title.Votes ?? 0
        };

        return Ok(titleDto);
    }

    // GET: api/titles/{id}/genres
    [HttpGet("{id}/genres")]
    public IActionResult GetTitleGenres(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new ErrorResponseDto { Error = "Title ID is required" });

        var title = _titleService.GetTitle(id);

        if (title == null)
            return NotFound(new ErrorResponseDto { Error = "Title not found" });

        var genres = _titleService.GetTitleGenres(id);

        if (genres == null || genres.Count == 0)
            return NotFound(new ErrorResponseDto { Error = "No genres found for this title" });

        return Ok(genres);
    }

    // GET: api/titles/{id}/crew
    [HttpGet("{id}/crew")]
    public IActionResult GetTitleCrew(string id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new ErrorResponseDto { Error = "Title ID is required" });

        var title = _titleService.GetTitle(id);

        if (title == null)
            return NotFound(new ErrorResponseDto { Error = "Title not found" });

        var cast = _titleService.GetTitleCrew(id);

        if (cast == null || cast.Count == 0)
            return NotFound(new ErrorResponseDto { Error = "No cast found for this title" });

        var totalCount = cast.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var paginatedCast = cast
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new TitleCrewModel
            {
                CrewId = c.CrewId,
                TitleId = c.TitleId
            })
            .ToList();

        var response = new PagedResultDto<TitleCrewModel>
        {
            Items = paginatedCast,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalCount,
            TotalPages = totalPages,
        };

        return Ok(response);
    }

    // GET: api/titles/genres
    [HttpGet("genres")]
    public IActionResult GetGenres()
    {
        var genres = _titleService.GetAllGenres();
        return Ok(genres);
    }

    [HttpGet("genres/{genre}")]
    public IActionResult GetTitlesBySpecificGenre(string genre,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        // Add validation
        if (string.IsNullOrWhiteSpace(genre))
            return BadRequest(new ErrorResponseDto { Error = "Genre is required" });

        var (titles, totalCount) = _titleService.GetTitlesByGenre(genre);

        if (titles == null || titles.Count == 0)
            return NotFound(new ErrorResponseDto { Error = $"No titles found for genre '{genre}'" });

        
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var paginatedTitles = titles
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var titleDtos = paginatedTitles.Select(t => new TitleModelShort
        {
            Id = t.Id,
            Title = t.Title ?? string.Empty,
            TitleType = t.TitleType ?? string.Empty,
            Year = t.StartYear ?? string.Empty,
            Rating = t.Rating ?? 0,
            Url = Url.Action(nameof(GetTitle), new { id = t.Id }) ?? string.Empty
        }).ToList();

        var response = new PagedResultDto<TitleModelShort>
        {
            Items = titleDtos,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalCount,
            TotalPages = totalPages,
        };

        return Ok(response);
    }

    // GET: api/titles/{id}/episodes
    [HttpGet("{id}/episodes")]
    public IActionResult GetTitleEpisodes(string id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new ErrorResponseDto { Error = "Series ID is required" });

        var title = _titleService.GetTitle(id);

        if (title == null)
            return NotFound(new ErrorResponseDto { Error = "Series not found" });

        var episodes = _titleService.GetTitleEpisodes(id);

        if (episodes == null || episodes.Count == 0)
            return NotFound(new ErrorResponseDto { Error = "No episodes found for this title" });

        var episodeDtos = episodes.Select(e => new TitleEpisodesModel
        {
            EpisodessId = e.EpisodeId,
            SeriesId = e.SeriesId ?? string.Empty,
            SeasonNumber = e.SeasonNumber,
            EpisodeNumber = e.EpisodeNumber,
            EpisodeTitle = _titleService.GetTitle(e.EpisodeId)?.Title ?? string.Empty,
            Url = Url.Action(nameof(GetTitle), new { id = e.EpisodeId }) ?? string.Empty
        }).ToList();

        var totalCount = episodeDtos.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var paginatedEpisodes = episodeDtos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var response = new PagedResultDto<TitleEpisodesModel>
        {
            Items = paginatedEpisodes,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalCount,
            TotalPages = totalPages,
        };



        return Ok(response);
    }

    [HttpGet("{id}/alternates")]
    public IActionResult GetAlternateTitles(string id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var alternateTitles = _titleService.GetTitleAlternates(id);

        if (alternateTitles == null || alternateTitles.Count == 0)
            return NotFound(new ErrorResponseDto { Error = "No alternate titles found" });

        var totalCount = alternateTitles.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var paginatedTitles = alternateTitles
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        
        var titleDtos = paginatedTitles.Select(t => new TitleAltsModel
        {
            AltsTitle = t.AltsTitle ?? string.Empty,
            Types = t.Types,
            IsOriginalTitle = t.IsOriginalTitle
        }).ToList();

        var response = new PagedResultDto<TitleAltsModel>
        {
            Items = titleDtos,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalCount,
            TotalPages = totalPages,
        };

        return Ok(response);
    }

    [HttpGet("{id}/regions")]
    public IActionResult GetTitleRegions(string id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var titleRegions = _titleService.GetTitleRegions(id);

        if (titleRegions == null || titleRegions.Count == 0)
            return NotFound(new ErrorResponseDto { Error = "No regions found for this title" });

        var totalCount = titleRegions.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var paginatedRegions = titleRegions
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var regionDtos = paginatedRegions.Select(r => new TitleRegionModel
        {
            Region = r.Region ?? string.Empty,
            Language = _titleService.GetTitleRegionDetails(r.Region)?.Language ?? string.Empty
        }).ToList();

        var response = new PagedResultDto<TitleRegionModel>
        {
            Items = regionDtos,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalCount,
            TotalPages = totalPages,
        };

        return Ok(response);
    }
}