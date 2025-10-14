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
    public int Id { get; set; }
    public string? Name
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
    public decimal Price { get; set; }
    public int CategoryId { get; set; }

    public int Count { get; set; }

    public int UnitPrice { get; set; }
    public int UnitsInStock { get; set; }
    public string? QuantityPerUnit { get; set; }
    public Category? Category { get; set; }
    public string? CategoryName { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public string CustomerId { get; set; }
    public int EmplyeeId { get; set; }
    public DateTime Date { get; set; }
    public DateTime Required { get; set; }
    public DateTime ShippedDate { get; set; }
    public int Freight { get; set; }
    public string ShipName { get; set; }
    public string ShipAddress { get; set; }
    public string ShipCity { get; set; }
    public string ShipPostalCode { get; set; }
    public string ShipCountry { get; set; }

    public ICollection<OrderDetails> OrderDetails { get; set; }
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