using DataServiceLayer;
using DataServiceLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace WebServiceLayer.Models;

[ApiController]
[Route("api/crew")]
public class CrewController : ControllerBase
{
    private readonly CrewDataService _crewService;
    private readonly ILogger<CrewController> _logger;

    public CrewController(ILogger<CrewController> logger)
    {
        _crewService = new CrewDataService();
        _logger = logger;
    }

    // GET: api/crew
    [HttpGet]
    public async Task<IActionResult> GetAllCrewMembers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? search = null)
    {
        try
        {
            var (crewMembers, totalCount) = await Task.Run(() => _crewService.GetCrew());

            if (!string.IsNullOrEmpty(search))
            {
                var (searchedCrew, searchedCount) = await Task.Run(() => _crewService.SearchCrew(search, crewMembers));
                crewMembers = searchedCrew ?? new List<Crew>();
                totalCount = searchedCount;
            }

            var crew = crewMembers
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            if (crew.Count == 0)
            {
                return NotFound(new ErrorResponseDto { Error = "No crew members found" });
            }

            var CrewModelDTO = crew.Select(c => new CrewModel
            {
                CrewId = c.CrewId,
                Fullname = c.Fullname ?? string.Empty,
                BirthYear = c.BirthYear,
                DeathYear = c.DeathYear,
                AverageRating = c.AverageRating,
                Url = $"{Request.Scheme}://{Request.Host}/api/crew/{c.CrewId}"
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving crew members");
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving crew members" });
        }
    }

    // GET: api/crew/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCrewById(string id)
    {
        try
        {
            var crew = await Task.Run(() => _crewService.GetCrew(id));
            if (crew == null)
            {
                return NotFound(new ErrorResponseDto { Error = $"Crew member with ID {id} not found" });
            }

            var crewModel = new CrewModel
            {
                CrewId = crew.CrewId,
                Fullname = crew.Fullname ?? string.Empty,
                BirthYear = crew.BirthYear,
                DeathYear = crew.DeathYear,
                AverageRating = crew.AverageRating,
                Url = $"{Request.Scheme}://{Request.Host}/api/crew/{crew.CrewId}"
            };

            return Ok(crewModel);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving crew member {CrewId}", id);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving crew member" });
        }
    }

    // GET: api/crew/{id}/titles
    [HttpGet("{id}/titles")]
    public async Task<IActionResult> GetCrewTitles(string id)
    {
        try
        {
            // Input validation
            var crew = await Task.Run(() => _crewService.GetCrew(id));
            if (crew == null)
            {
                return NotFound(new ErrorResponseDto { Error = $"Crew member with ID {id} not found" });
            }

            var (titles, totalCount) = await Task.Run(() => _crewService.GetCrewTitles(id));

            if (titles == null || totalCount == 0)
            {
                return NotFound(new ErrorResponseDto { Error = $"No titles found for crew member with ID {id}" });
            }

            var titlesModel = titles.Select(t => new CrewTitlesModel
            {
                TitleId = t.TitleId,
                Title = t.Title,
                TitleType = t.TitleType,
                Year = t.Year,
                Rating = t.Rating,
                Url = $"{Request.Scheme}://{Request.Host}/api/titles/{t.TitleId}"
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving titles for crew member {CrewId}", id);
            return StatusCode(500, new ErrorResponseDto { Error = "An error occurred while retrieving crew titles" });
        }
    }
}