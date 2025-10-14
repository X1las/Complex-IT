
namespace Assignment4;
public class DataService
{
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Product> Products { get; set; } = new List<Product>();

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
    public ICollection<Product> GetProductByCategory(int categoryId)
    {

    }
    public ICollection<Product> GetProductByName(string name)
    {

    }
    public Order GetOrder(int id)
    {

    }
    public ICollection<Order> GetOrders()
    {

    }
    public ICollection<OrderDetails> GetOrderDetailsByOrderId(int orderId)
    {

    }
    public ICollection<OrderDetails> GetOrderDetailsByProductId(int productId)
    {

    }
}