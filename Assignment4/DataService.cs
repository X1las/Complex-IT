using Microsoft.EntityFrameworkCore;

namespace DataServiceLayer;
public class DataService
{

    public List<Category> GetCategories()
    {
        using var db = new NorthwindContext();
        return db.Categories.ToList();
    }

    public Category? GetCategory(int id)
    {
        using var db = new NorthwindContext();
        return db.Categories.FirstOrDefault(c => c.Id == id);
    }

public Category CreateCategory(string name, string description)
{
    using var db = new NorthwindContext();

    // Get the next ID by finding the max and adding 1
    var maxId = db.Categories.Any() ? db.Categories.Max(c => c.Id) : 0;
    var nextId = maxId + 1;

    var category = new Category
    {
        Id = nextId,
        Name = name,
        Description = description
    };

    db.Categories.Add(category);

        // Verify it was saved by reloading from database
    db.Entry(category).Reload();
    db.SaveChanges();
    return category;

}

    public bool DeleteCategory(int id)
    {
        using var db = new NorthwindContext();
        var category = db.Categories.FirstOrDefault(c => c.Id == id);
        if (category == null) return false;
        db.Categories.Remove(category);
        db.SaveChanges();
        return true;
    }

    public bool UpdateCategory(int id, string name, string description)
    {
        using var db = new NorthwindContext();
        var category = db.Categories.FirstOrDefault(c => c.Id == id);
        if (category == null) return false;
        category.Name = name;
        category.Description = description;
        db.SaveChanges();
        return true;
    }

    // Product methods
    public Product? GetProduct(int id)
    {
        using var db = new NorthwindContext();
        return db.Products.Include(p => p.Category).FirstOrDefault(p => p.Id == id);
    }

    public List<Product> GetProductByCategory(int categoryId)
    {
        using var db = new NorthwindContext();
        return db.Products
            .Where(p => p.CategoryId == categoryId)
            .Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                CategoryName = p.Category!.Name,
                UnitPrice = p.UnitPrice,
                UnitsInStock = p.UnitsInStock,
                QuantityPerUnit = p.QuantityPerUnit,
                CategoryId = p.CategoryId
            })
            .ToList();
    }

    public List<Product> GetProductByName(string name)
    {
        using var db = new NorthwindContext();
        return db.Products
            .Where(p => p.Name!.Contains(name))
            .OrderBy(p => p.Id)
            .Select(p => new Product
            {
                Id = p.Id,
                Name = p.Name,
                ProductName = p.Name,
                UnitPrice = p.UnitPrice,
                UnitsInStock = p.UnitsInStock,
                QuantityPerUnit = p.QuantityPerUnit,
                CategoryId = p.CategoryId
            })
            .ToList();
    }

    // Order methods
    public Order? GetOrder(int id)
    {
        using var db = new NorthwindContext();
        return db.Orders
            .Include(o => o.OrderDetails!)
                .ThenInclude(od => od.Product)
                    .ThenInclude(p => p!.Category)
            .FirstOrDefault(o => o.Id == id);
    }

    public List<Order> GetOrders()
    {
        using var db = new NorthwindContext();
        return db.Orders.ToList();
    }

    // OrderDetails methods
    public List<OrderDetails> GetOrderDetailsByOrderId(int orderId)
    {
        using var db = new NorthwindContext();
        return db.OrderDetails
            .Include(od => od.Product)
            .Where(od => od.OrderId == orderId)
            .ToList();
    }

    public List<OrderDetails> GetOrderDetailsByProductId(int productId)
    {
        using var db = new NorthwindContext();
        return db.OrderDetails
            .Include(od => od.Order)
            .Where(od => od.ProductId == productId)
            .ToList();
    }
    public int GetProductCount()
    {
        var db = new NorthwindContext();
        return db.Products.Count();
    }
    public IList<Product> GetProducts(int page, int pageSize)
    {
        var db = new NorthwindContext();
        return db.Products
            .Include(x => x.Category)
            .OrderBy(x => x.Id)
            .Skip(page* pageSize)
            .Take(pageSize)
            .ToList();
    }
}