using DataServiceLayer;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace WebServiceLayer;

[Route("api/titles")]
[ApiController]
public class TitleController : ControllerBase
{
    private readonly DataService _dataService;
    private readonly LinkGenerator _generator;
    private readonly IMapper _mapper;

    public TitleController(
        DataService dataService,
        LinkGenerator generator,
        IMapper mapper)
    {
        _dataService = dataService;
        _generator = generator;
        _mapper = mapper;
    }

    [HttpGet("{id}", Name = nameof(GetTitle))]
    public IActionResult GetTitle(string id)
    {
        var title = _dataService.GetTitleById(id);

        if (title == null)
        {
            return NotFound();
        }

        var model = CreateTitleModel(title);

        return Ok(model);
    }

    private TitleModel CreateTitleModel(Titles title)
    {
        var model = _mapper.Map<TitleModel>(title);
        model.Url = _generator.GetUriByName(
            HttpContext,
            nameof(GetTitle),
            new { id = title.Id }) ?? string.Empty;

        return model;
    }

}