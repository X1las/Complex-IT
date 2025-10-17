namespace WebServiceLayer;

public class ProductModel
{
    public string? Url { get; set; }
    public string Name { get; set; }
    public int UnitPrice { get; set; }
    public string CategoryName { get; set; }
    public string? CategoryUrl { get; set; }
}

public class CategoryModel
{
    public string? Url { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class CreateCategoryModel
{
    public string Name { get; set; }
    public string Description { get; set; }
}