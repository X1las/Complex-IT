using DataServiceLayer;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WebServiceLayer.Controllers;



[Route("api/products")]
[ApiController]
public class ProductsController : ControllerBase
{
    private readonly DataService _dataService;
    private readonly LinkGenerator _generator;
    private readonly IMapper _mapper;

    public ProductsController(
        DataService dataService,
        LinkGenerator generator, 
        IMapper mapper) 
    {
        _dataService = dataService;
        _generator = generator;
        _mapper = mapper;
    }

    [HttpGet(Name = nameof(GetProducts))]
    public IActionResult GetProducts(int page = 0, int pageSize = 5)
    {
        var products = _dataService
            .GetProducts(page, pageSize)
            .Select(x => CreateProductModel(x));

        var numOfItems = _dataService.GetProductCount();
        var numPages = (int)Math.Ceiling((double)numOfItems / pageSize);

        var prev = page > 0
            ? GetUrl(nameof(GetProducts), new { page = page - 1, pageSize })
            : null;

        var next = page < numPages-1
            ? GetUrl(nameof(GetProducts), new { page = page + 1, pageSize })
            : null;

        var cur = GetUrl(nameof(GetProducts), new {page, pageSize });

        var result = new
        {
            Prev = prev,
            Next = next,
            Current = cur,
            NumberOfPages=numPages,
            NumberOfIems = numOfItems,
            Items = products
        };

        return Ok(result);
    }

    

    [HttpGet("{id}", Name = nameof(GetProduct))]
    public IActionResult GetProduct(int id)
    {
        var product = _dataService.GetProduct(id);
        if (product == null)
        {
            return NotFound();
        }
        return Ok(CreateProductModel(product));
    }

    private WebServiceLayer.ProductModel CreateProductModel(Product product)
    {
        var model = _mapper.Map<WebServiceLayer.ProductModel>(product);
        model.Url = GetUrl(nameof(GetProduct), new { id = product.Id });
        model.CategoryUrl = GetUrl(nameof(CategoriesController.GetCategory), new { product.Category.Id});
        return model;
    }

    private string? GetUrl(string endpointName, object values)
    {
        return _generator.GetUriByName(HttpContext, endpointName, values);
    }
}