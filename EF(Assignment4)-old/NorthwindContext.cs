using Microsoft.EntityFrameworkCore;


namespace EfExample;
public class NorthwindContext : DbContext
{
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderDetails> OrderDetails { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.UseNpgsql("host=localhost;db=northwind;uid=;pwd=");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>().ToTable("categories");
        modelBuilder.Entity<Category>().Property(x => x.categoryId).HasColumnName("categoryid");
        modelBuilder.Entity<Category>().Property(x => x.categoryName).HasColumnName("categoryname");
        modelBuilder.Entity<Category>().Property(x => x.categoryDescription).HasColumnName("description");


        modelBuilder.Entity<Product>().ToTable("products");
        modelBuilder.Entity<Product>().Property(x => x.productId).HasColumnName("productid");
        modelBuilder.Entity<Product>().Property(x => x.productName).HasColumnName("productname");
        modelBuilder.Entity<Product>().Property(x => x.SupplierId).HasColumnName("supplierid");
        modelBuilder.Entity<Product>().Property(x => x.quantityperunit).HasColumnName("quantityperunit");
        modelBuilder.Entity<Product>().Property(x => x.UnitsInStock).HasColumnName("unitsinstock");
        modelBuilder.Entity<Product>().Property(x => x.UnitPrice).HasColumnName("unitprice");
        modelBuilder.Entity<Product>().Property(x => x.CategoryId).HasColumnName("categoryid");

        modelBuilder.Entity<Order>().ToTable("orders");
        modelBuilder.Entity<Order>().Property(x => x.orderid).HasColumnName("orderid");
        modelBuilder.Entity<Order>().Property(x => x.customerid).HasColumnName("customerid");
        modelBuilder.Entity<Order>().Property(x => x.employeeid).HasColumnName("employeeid");
        modelBuilder.Entity<Order>().Property(x => x.orderdate).HasColumnName("orderdate");
        modelBuilder.Entity<Order>().Property(x => x.requiredate).HasColumnName("requireddate");
        modelBuilder.Entity<Order>().Property(x => x.shippeddate).HasColumnName("shippeddate");
        modelBuilder.Entity<Order>().Property(x => x.freight).HasColumnName("freight");
        modelBuilder.Entity<Order>().Property(x => x.shipname).HasColumnName("shipname");
        modelBuilder.Entity<Order>().Property(x => x.shipaddress).HasColumnName("shipaddress");
        modelBuilder.Entity<Order>().Property(x => x.shipcity).HasColumnName("shipcity");


        modelBuilder.Entity<OrderDetails>().ToTable("orderdetails");
        modelBuilder.Entity<OrderDetails>().Property(x => x.orderId).HasColumnName("orderid");
        modelBuilder.Entity<OrderDetails>().Property(x => x.productId).HasColumnName("productid");
        modelBuilder.Entity<OrderDetails>().Property(x => x.unitPrice).HasColumnName("unitprice");
        modelBuilder.Entity<OrderDetails>().Property(x => x.quantity).HasColumnName("quantity");
        modelBuilder.Entity<OrderDetails>().Property(x => x.discount).HasColumnName("discount");
    }
}
