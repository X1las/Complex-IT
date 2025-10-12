namespace Assignment4;

// We need a way to implement First() and Last() on all lists

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

    public int Count { get; set; }

    public int UnitPrice { get; set; }
    public int UnitsInStock { get; set; }
    public string? QuantityPerUnit { get; set; }
    public Category? Category { get; set; }
    public string? CategoryName { get; set; }
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
    public List<Product> GetProductByCategory(int categoryId)
    {

    }
    public Product GetProductByName(string name)
    {

    }
    public Order GetOrder(int id)
    {

    }
    public List<Order> GetOrders()
    {

    }
    public OrderDetails GetOrderDetailsByOrderId(int orderId)
    {

    }
    public OrderDetails GetOrderDetailsByProductId(int productId)
    {

    }
}

public class Order
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public DateTime Required { get; set; }
    public OrderDetails OrderDetails { get; set; }
    public String? ShipName { get; set; }
    public String? ShipCity { get; set; }
}

public class OrderDetails
{
    public int OrderId { get; set; }
    public int Count { get; set; }
    public Order? Order { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public double UnitPrice { get; set; }
    public int Quantity { get; set; }
    public double Discount { get; set; }
    
}