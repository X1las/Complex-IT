namespace WebServiceLayer;

public class ProductModel
{
    public string? Url { get; set; }
<<<<<<< HEAD
    public required string Name { get; set; }
    public int UnitPrice { get; set; }
    public required string? CategoryName { get; set; }
=======
    public string? Name { get; set; }
    public int UnitPrice { get; set; }
    public string? CategoryName { get; set; }
>>>>>>> 1aa6250150feadf49d4305c12ae1141b60045214
    public string? CategoryUrl { get; set; }
}

public class CategoryModel
{
    public int Id { get; set; }
    public string? Url { get; set; }
    public string? Name { get; set; }
    public required string? Description { get; set; }
}

public class CreateCategoryModel
{
<<<<<<< HEAD
    public required string? Name { get; set; }
    public required string? Description { get; set; }
=======
    public string? Name { get; set; }
    public string? Description { get; set; }
>>>>>>> 1aa6250150feadf49d4305c12ae1141b60045214
}