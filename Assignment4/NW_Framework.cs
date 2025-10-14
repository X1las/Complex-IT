namespace Assignment4;

// We need a way to implement First() and Last() on all lists

public static class ICollectionExtensions
{
    public static T First<T>(this ICollection<T> collection)
    {
        if (collection == null || collection.Count == 0) throw new InvalidOperationException("Collection is empty");
        using var enumerator = collection.GetEnumerator();
        enumerator.MoveNext();
        return enumerator.Current;
    }
    public static T Last<T>(this ICollection<T> collection)
    {
        if (collection == null || collection.Count == 0) throw new InvalidOperationException("Collection is empty");
        using var enumerator = collection.GetEnumerator();
        T last = default(T);
        while (enumerator.MoveNext())
        {
            last = enumerator.Current;
        }
        return last;
    }
}

public class Category
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    public ICollection<Product> Products { get; set; }
}

public class Product
{
    private string name = "";
    public required int Id { get; set; }
    public required string Name
    {
        get => name;
        set => name = value ?? "";
    }

    public string ProductName {
        get => name;
        set => name = value ?? "";
    }   

    public string? Description { get; set; }
    public required int SupplierId { get; set; }
    public required int CategoryId { get; set; }
    public required string QuantityPerUnit { get; set; }
    public required int UnitPrice { get; set; }
    public required int UnitsInStock { get; set; }

    public int Count { get; set; }
    public double Price { get; set; }
    public required Category Category { get; set; }
    public string? CategoryName { get; set; }

    public Product(int id = 0, string name = "", int supplierId = 0, int categoryId = 0, string quantityPerUnit = "", int unitPrice = 0, int unitsInStock = 0, Category? category = null)
    {
        Id = id;
        Name = name;
        SupplierId = supplierId;
        CategoryId = categoryId;
        QuantityPerUnit = quantityPerUnit;
        UnitPrice = unitPrice;
        UnitsInStock = unitsInStock;
        Category = category ?? new Category();
    }
}

public class Order
{
    public required int Id { get; set; }
    public required string CustomerId { get; set; }
    public int EmployeeId { get; set; }
    public DateOnly OrderDate { get; set; }
    public DateOnly RequireDate { get; set; }
    public DateOnly ShippedDate { get; set; }
    public int Freight { get; set; }
    public required string ShipName { get; set; }
    public required string ShipAddress { get; set; }
    public string? ShipCity { get; set; }
    public string? ShipPostalCode { get; set; }
    public string? ShipCountry { get; set; }

    public Order(int id = 0, string customerId = "", string shipName = "", string shipAddress = "", string? shipCity = null, string? shipPostalCode = null, string? shipCountry = null)
    {
        Id = id;
        CustomerId = customerId;
        ShipName = shipName;
        ShipAddress = shipAddress;
        ShipCity = shipCity;
        ShipPostalCode = shipPostalCode;
        ShipCountry = shipCountry;
    }

    public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();
}

public class OrderDetails
{
    public required string OrderId { get; set; }
    public required int ProductId { get; set; }
    public Order? Order { get; set; }
    public Product? Product { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    public float Discount { get; set; }

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

public class OrderDetails
{
    public required string orderId { get; set; }
    public required int productId { get; set; }
    public decimal unitPrice { get; set; }
    public int quantity { get; set; }
    public float discount { get; set; }
}
*/