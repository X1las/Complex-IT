
namespace Assignment4;
public class DataService        
{
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();

    public ICollection<Order> Orders { get; set; } = new List<Order>();
    public ICollection<OrderDetails> OrderDetails { get; set; } = new List<OrderDetails>();

    public ICollection<Category> GetCategories()
    {
        return Categories;
    }
    public Category? GetCategory(int id)
    {
        return Categories.FirstOrDefault(c => c.Id == id);
    }
    public Category CreateCategory(string name, string description)
    {
        var category = new Category
        {
            Id = Categories.Count > 0 ? Categories.Max(c => c.Id) + 1 : 1,
            Name = name,
            Description = description
        };
        Categories.Add(category);
        return category;
    }
    public bool DeleteCategory(int id)
    {
        var category = GetCategory(id);
        if (category != null)
        {
            Categories.Remove(category);
            return true;
        }
        return false;
    }
    public bool UpdateCategory(int id, string name, string description)
    {
        var category = GetCategory(id);
        if (category != null)
        {
            category.Name = name;
            category.Description = description;
            return true;
        }
        return false;
    }
    public Product? GetProduct(int id)
    {
        return Products.FirstOrDefault(p => p.Id == id);
    }
    public ICollection<Product> GetProductByCategory(int categoryId)
    {
        return Products.Where(p => p.CategoryId == categoryId).ToList();
    }
    public ICollection<Product> GetProductByName(string name)
    {
        return Products.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
    }
    public Order? GetOrder(int id)
    {
        return Orders.FirstOrDefault(o => o.Id == id);
    }
    public ICollection<Order> GetOrders()
    {
        return Orders.ToList();
    }
    public ICollection<OrderDetails> GetOrderDetailsByOrderId(int orderId)
    {
        return OrderDetails.Where(od => od.OrderId == orderId.ToString()).ToList();
    }
    public ICollection<OrderDetails> GetOrderDetailsByProductId(int productId)
    {
        return OrderDetails.Where(od => od.ProductId == productId).ToList();
    }
}