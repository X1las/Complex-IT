namespace Assignment4;

// We need a way to implement First() and Last() on all lists

public class Category
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}

/*
public class Category
{
    public int categoryId { get; set; }
    public string categoryName { get; set; }
    public string categoryDescription { get; set; }
    public ICollection<Product> Products { get; set; }
}
*/

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

/*

Joachim:
public class Product
{
    public required int productId { get; set; }
    public required string productName { get; set; }
    public required int SupplierId { get; set; }
    public required int CategoryId { get; set; }
    public required string quantityperunit { get; set; }
    public required int UnitPrice { get; set; }
    public required int UnitsInStock { get; set; }
    public required Category Category { get; set; }
}

public class Product
{
    public required int productId { get; set; }
    public required string productName { get; set; }
    public required int SupplierId { get; set; }
    public required int CategoryId { get; set; }
    public required string quantityperunit { get; set; }
    public required int UnitPrice { get; set; }
    public required int UnitsInStock { get; set; }
    public required Category Category { get; set; }
}
*/

public class Order
{
    public int Id { get; set; }
    public DateTime Date { get; set; }
    public DateTime Required { get; set; }
    public OrderDetails OrderDetails { get; set; }
    public String? ShipName { get; set; }
    public String? ShipCity { get; set; }
}

/*
public class Order
{
    public required int orderid { get; set; }
    public required string customerid { get; set; }
    public int employeeid { get; set; }
    public DateOnly orderdate { get; set; }
    public DateOnly requiredate { get; set; }
    public DateOnly shippeddate { get; set; }
    public int freight { get; set; }
    public required string shipname { get; set; }
    public required string shipaddress { get; set; }
    public string shipcity { get; set; }
    public string shippostalcode { get; set; }
    public string shipcountry { get; set; }

    public ICollection<OrderDetails> OrderDetails { get; set; }

}
*/

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

/*
public class OrderDetails
{
    public required string orderId { get; set; }
    public required int productId { get; set; }
    public decimal unitPrice { get; set; }
    public int quantity { get; set; }
    public float discount { get; set; }
}
*/