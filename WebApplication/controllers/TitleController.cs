using DataServiceLayer;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace WebServiceLayer;

[Route("api/titles")]
[ApiController]
public class TitleController : ControllerBase
{
    private readonly TitleDataService _dataService;
    private readonly LinkGenerator _generator;
    private readonly IMapper _mapper;
    private readonly ImdbContext _context;

    public TitleController(
        TitleDataService dataService,
        LinkGenerator generator,
        IMapper mapper,
        ImdbContext context)
    {
        _dataService = dataService;
        _generator = generator;
        _mapper = mapper;
        _context = context;
    }

    public IActionResult GetTitle(string id)
    {
        var title = _dataService.GetTitleById(_context, id);

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