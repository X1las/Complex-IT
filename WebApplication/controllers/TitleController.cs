using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> GetTitles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        List<Titles> titlesList;
        int totalCount;

        if (!string.IsNullOrWhiteSpace(search))
        {
            (titlesList, totalCount) = await Task.Run(() => _titleService.SearchTitles(search));
        }
        else
        {
            (titlesList, totalCount) = await Task.Run(() => _titleService.GetTitles());
        }
        
        // Calculate pagination
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        
        var titles = titlesList
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        // Return empty page with metadata instead of 404
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
    public async Task<IActionResult> GetTitle(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new ErrorResponseDto { Error = "Title ID is required" });

        var title = await Task.Run(() => _titleService.GetTitle(id));
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
    public async Task<IActionResult> GetTitleGenres(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new ErrorResponseDto { Error = "Title ID is required" });

        var title = await Task.Run(() => _titleService.GetTitle(id));

        if (title == null)
            return NotFound(new ErrorResponseDto { Error = "Title not found" });

        var genres = await Task.Run(() => _titleService.GetTitleGenres(id));
        return Ok(genres);
    }

    // GET: api/titles/{id}/crew
    [HttpGet("{id}/crew")]
    public async Task<IActionResult> GetTitleCrew(string id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new ErrorResponseDto { Error = "Title ID is required" });

        var title = await Task.Run(() => _titleService.GetTitle(id));
        if (title == null)
            return NotFound(new ErrorResponseDto { Error = "Title not found" });

        var crew = await Task.Run(() => _titleService.GetTitleCrew(id)) ?? new List<Attends>();
        var totalCount = crew.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

        // APPLY pagination before mapping (bug fix)
        var pagedCrew = crew
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var castDtos = pagedCrew.Select(c => new TitleCrewModel
        {
            CrewId = c.CrewId,
            TitleId = c.TitleId,
            Url = Url.Action("GetCrew", "Crew", new { id = c.CrewId }) ?? $"/api/crew/{c.CrewId}"
        }).ToList();

        var response = new PagedResultDto<TitleCrewModel>
        {
            Items = castDtos,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalCount,
            TotalPages = totalPages,
        };

        return Ok(response);
    }

    // GET: api/titles/genres
    [HttpGet("genres")]
    public async Task<IActionResult> GetGenres()
    {
        var genres = await Task.Run(() => _titleService.GetAllGenres());
        return Ok(genres);
    }

    [HttpGet("genres/{genre}")]
    public async Task<IActionResult> GetTitlesBySpecificGenre(string genre,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        // Add validation
        if (string.IsNullOrWhiteSpace(genre))
            return BadRequest(new ErrorResponseDto { Error = "Genre is required" });

        var (titles, totalCount) = await Task.Run(() => _titleService.GetTitlesByGenre(genre));
        titles ??= new List<Titles>();

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
    public async Task<IActionResult> GetTitleEpisodes(string id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new ErrorResponseDto { Error = "Series ID is required" });

        var title = await Task.Run(() => _titleService.GetTitle(id));
        if (title == null)
            return NotFound(new ErrorResponseDto { Error = "Series not found" });

        var episodes = await Task.Run(() => _titleService.GetTitleEpisodes(id)) ?? new List<Episodes>();

        // NOTE: possible N+1 below; consider optimizing in the service layer
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
    public async Task<IActionResult> GetAlternateTitles(string id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new ErrorResponseDto { Error = "Title ID is required" });

        var alternateTitles = await Task.Run(() => _titleService.GetTitleAlternates(id)) ?? new List<AlternateTitles>();

        var totalCount = alternateTitles.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        if (totalPages == 0) totalPages = 1;
        if (page > totalPages) page = totalPages;

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
    public async Task<IActionResult> GetTitleRegions(string id,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest(new ErrorResponseDto { Error = "Title ID is required" });

        var titleRegions = await Task.Run(() => _titleService.GetTitleRegions(id)) ?? new List<TitleRegions>();

        var totalCount = titleRegions.Count;
        var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        if (totalPages == 0) totalPages = 1;
        if (page > totalPages) page = totalPages;

        var paginatedRegions = titleRegions
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var regionDtos = paginatedRegions.Select(r => new TitleRegionModel
        {
            Region = r.Region ?? string.Empty,
            Language = string.IsNullOrWhiteSpace(r.Region) ? string.Empty : (_titleService.GetTitleRegionDetails(r.Region)?.Language ?? string.Empty)
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