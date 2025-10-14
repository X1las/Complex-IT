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
    private string name;
    public required int Id { get; set; }
    public required string? Name
    {
        get => name;
        set => name = value;
    }

    public string ProductName {
        get => name;
        set => name = value;
    }   

    public string? Description { get; set; }
    public int SupplierId { get; set; }
    public required double Price { get; set; }
    public int CategoryId { get; set; }

    public int Count { get; set; }

    public double UnitPrice { get; set; }
    public int UnitsInStock { get; set; }
    public string? QuantityPerUnit { get; set; }
    public Category? Category { get; set; }
    public string? CategoryName { get; set; }

    public Product(int id = 0, string name = "", double price = 0.0, int unitsInStock = 0, string quantityPerUnit = "")
    {
        Id = id;
        Name = name;
        Price = price;
        UnitsInStock = unitsInStock;
        QuantityPerUnit = quantityPerUnit;
    }
}

public class Order
{
    public required int Id { get; set; }
    public required string CustomerId { get; set; }
    public int EmplyeeId { get; set; }
    public DateTime Date { get; set; }
    public DateTime Required { get; set; }
    public DateTime ShippedDate { get; set; }
    public int Freight { get; set; }
    public required string ShipName { get; set; }
    public required string ShipAddress { get; set; }
    public required string ShipCity { get; set; }
    public required string ShipPostalCode { get; set; }
    public required string ShipCountry { get; set; }

    public Order(int id = 0, string customerId = "", string shipName = "", string shipAddress = "", string shipCity = "", string shipPostalCode = "", string shipCountry = "")
    {
        Id = id;
        CustomerId = customerId;
        ShipName = shipName;
        ShipAddress = shipAddress;
        ShipCity = shipCity;
        ShipPostalCode = shipPostalCode;
        ShipCountry = shipCountry;
    }

    public required ICollection<OrderDetails> OrderDetails { get; set; }
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