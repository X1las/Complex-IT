using DataServiceLayer;
using DataServiceLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebServiceLayer.Models;

[ApiController]
[Route("api/crew")]
public class CrewController : ControllerBase
{
    private readonly CrewDataService _crewService;

    public CrewController()
    {
        _crewService = new CrewDataService();
    }

    // GET: api/crew
    [HttpGet]
    public IActionResult GetAllCrewMembers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        var (crewMembers, totalCount) = _crewService.GetCrew();

        if (!string.IsNullOrEmpty(search))
        {
            var (searchedCrew, searchedCount) = _crewService.SearchCrew(search, crewMembers);
            crewMembers = searchedCrew ?? new List<Crew>();
            totalCount = searchedCount;
        }

        var crew = crewMembers
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        if (crew.Count == 0)
        {
            return NotFound("No crew members found.");
        }

        var CrewModelDTO = crew.Select(c => new CrewModel
        {
            CrewId = c.CrewId,
            Fullname = c.Fullname ?? string.Empty,
            BirthYear = c.BirthYear,
            DeathYear = c.DeathYear,
            AverageRating = c.AverageRating,
            Url = $"/api/crew/{c.CrewId}"
        }).ToList();

        var TotalPages = (int)Math.Ceiling((double)totalCount / pageSize);
        var response = new PagedResultDto<CrewModel>
        {
            Items = CrewModelDTO,
            CurrentPage = page,
            PageSize = pageSize,
            TotalItems = totalCount,
            TotalPages = TotalPages,
        };

        return Ok(response);
    }

    // GET: api/crew/{id}
    [HttpGet("{id}")]
    public IActionResult GetCrewById(string id)
    {
        var crew = _crewService.GetCrew(id);
        if (crew == null)
        {
            return NotFound($"Crew member with ID {id} not found.");
        }

        var crewModel = new CrewModel
        {
            CrewId = crew.CrewId,
            Fullname = crew.Fullname ?? string.Empty,
            BirthYear = crew.BirthYear,
            DeathYear = crew.DeathYear,
            AverageRating = crew.AverageRating,
            Url = $"/api/crew/{crew.CrewId}"
        };

        return Ok(crewModel);
    }

    // GET: api/crew/{id}/titles
    [HttpGet("{id}/titles")]
    public IActionResult GetCrewTitles(string id)
    {
        // Input validation
        var crew = _crewService.GetCrew(id);
        if (crew == null)
        {
            return NotFound($"Crew member with ID {id} not found.");
        }

        var (titles, totalCount) = _crewService.GetCrewTitles(id);

        if (titles == null || totalCount == 0)
        {
            return NotFound($"No titles found for crew member with ID {id}.");
        }

        var titlesModel = titles.Select(t => new CrewTitlesModel
        {
            TitleId = t.TitleId,
            Title = t.Title,
            TitleType = t.TitleType,
            Year = t.Year,
            Rating = t.Rating,
            Url = $"/api/titles/{t.TitleId}"
        }).ToList();

        var TotalPages = (int)Math.Ceiling((double)totalCount / 10); // Default page size 10
        var response = new PagedResultDto<CrewTitlesModel>
        {
            Items = titlesModel,
            CurrentPage = 1,
            PageSize = 10,
            TotalItems = totalCount,
            TotalPages = TotalPages,
        };
        
        return Ok(response);
    }
}