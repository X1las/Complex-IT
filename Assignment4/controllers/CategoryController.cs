using DataServiceLayer;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;

namespace WebServiceLayer;

[Route("api/categories")]
[ApiController]
public class CategoriesController : ControllerBase
{
    private readonly DataService _dataService;
    private readonly LinkGenerator _generator;
    private readonly IMapper _mapper;

    public CategoriesController(
        DataService dataService,
        LinkGenerator generator,
        IMapper mapper)
    {
        _dataService = dataService;
        _generator = generator;
        _mapper = mapper;
    }

    [HttpGet(Name = nameof(GetCategories))]
    public IActionResult GetCategories()
    {
        var categories = _dataService.GetCategories()
            .Select(x => CreateCategoryModel(x));

        return Ok(categories);
    }

    [HttpGet("{id}", Name = nameof(GetCategory))]
    public IActionResult GetCategory(int id)
    {
        var category = _dataService.GetCategory(id);

        if (category == null)
        {
            return NotFound();
        }

        var model = CreateCategoryModel(category);

        return Ok(model);
    }

    

    [HttpPost]
    public IActionResult CreateCategory(CreateCategoryModel creationModel)
    {
        var category = creationModel.Adapt<Category>();

        _dataService.CreateCategory(category);

        return Created();
    }

    [HttpDelete("{id}")]
    public IActionResult DeleteCategory(int id)
    {
        if (_dataService.DeleteCategory(id))
        {
            return NoContent();
        }

        return NotFound();
    }

    private CategoryModel CreateCategoryModel(Category category)
    {
        var model = _mapper.Map<CategoryModel>(category);
        model.Url = GetUrl(nameof(GetCategory), new { id = category.Id });
        return model;
    }

    private string? GetUrl(string endpointName, object values)
    {
        return _generator.GetUriByName(HttpContext, endpointName, values);
    }
}