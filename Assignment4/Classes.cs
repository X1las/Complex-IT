

namespace Assignment4;

public class Category
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }

    public int UnitPrice { get; set; }
    public int UnitsInStock { get; set; }
    public string? QuantityPerUnit { get; set; }
    public Category? Category { get; set; }
}

public class DataService
{
    public List<Category> Categories { get; set; } = new List<Category>();
    public List<Product> Products { get; set; } = new List<Product>();

    public List<Category> GetCategories()
    {
        return Categories;
    }
    public Category? GetCategory(int id)
    {
        return Categories.FirstOrDefault(c => c.Id == id);
    }
    public Category CreateCategory(string name, string description)
    {
        
    }
    public bool DeleteCategory(int id)
    {

    }

    public bool UpdateCategory(int id, string name, string description)
    {

    }

    public Product? GetProduct(int id)
    {

    }
}