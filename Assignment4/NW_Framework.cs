namespace Assignment4;

public class Category
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public ICollection<Product> Products { get; set; }
}

public class Product
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? ProductName { get; set; } 
    public int UnitPrice { get; set; } 
    public int UnitsInStock { get; set; }
    public string? QuantityPerUnit { get; set; }
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public string? CategoryName { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public string CustomerId { get; set; }
    public int EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public DateTime Required { get; set; }
    public string? ShipName { get; set; }
    public string? ShipCity { get; set; }
    public List<OrderDetails>? OrderDetails { get; set; }
}

public class OrderDetails
{
    public int OrderId { get; set; }
    // public int Count { get; set; }
    public Order? Order { get; set; }
    public Product? Product { get; set; }
    public int ProductId { get; set; }
    public int UnitPrice { get; set; }
    public int Quantity { get; set; }
    public int Discount { get; set; }
}
