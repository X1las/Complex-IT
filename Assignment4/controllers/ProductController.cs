using DataServiceLayer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace WebServiceLayer;

[Route("api/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly DataService _dataService;
    private readonly LinkGenerator _generator;

    public ProductController(DataService dataService, LinkGenerator generator)
    {
        _dataService = dataService;
        _generator = generator;
    }

    [HttpGet("{id}", Name = nameof(GetProduct))]
    public IActionResult GetProduct(int id)
    {
        var product = _dataService.GetProduct(id);

        if (product == null)
        {
            return NotFound();
        }

        var result = new
        {
            name = product.Name,
            category = new
            {
                name = product.Category?.Name
            }
        };

        return Ok(result);
    }

    [HttpGet("category/{id}")]
    public IActionResult GetProductsByCategory(int id)
    {
        var products = _dataService.GetProductByCategory(id);

        if (products == null || products.Count == 0)
        {
            return NotFound(new List<object>());
        }

        var result = products.Select(p => new
        {
            name = p.Name,
            categoryName = p.CategoryName
        });

        return Ok(result);
    }

    [HttpGet("name/{name}")]
    public IActionResult GetProductsByName(string name)
    {
        var products = _dataService.GetProductByName(name);

        if (products == null || products.Count == 0)
        {
            return NotFound(new List<object>());
        }

        var result = products.Select(p => new
        {
            productName = p.ProductName
        });

        return Ok(result);
    }
}