
namespace Assignment4;
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