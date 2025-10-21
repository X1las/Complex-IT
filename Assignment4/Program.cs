using DataServiceLayer;
using Mapster;
using Microsoft.Extensions.DependencyInjection;

namespace WebServiceLayer;

public class Program
{
<<<<<<< HEAD
     public static DataService DataService{ get; } = new DataService();
    static void Main(string[] args)
=======
    public static DataService DataService { get; } = new DataService();
    public static void Main(string[] args)
>>>>>>> 1aa6250150feadf49d4305c12ae1141b60045214
    {
        var builder = WebApplication.CreateBuilder(args);

<<<<<<< HEAD
        connection.Open();

        var cmd = new NpgsqlCommand();
        cmd.Connection = connection;



        // Multiple queries separated by semicolons
        cmd.CommandText = @"
            select * from categories;
            select * from products;
            select * from orders;
            select * from orderdetails;
        ";

        var reader = cmd.ExecuteReader();

        // Read categories
        Console.WriteLine("=== CATEGORIES ===");
        while (reader.Read())
=======
        builder.WebHost.ConfigureKestrel(options =>
>>>>>>> 1aa6250150feadf49d4305c12ae1141b60045214
        {
            options.ListenLocalhost(5001);
        });

        // Add services to the container.
        builder.Services.AddSingleton<DataService>();
        builder.Services.AddMapster();
        builder.Services.AddControllers();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.MapControllers();

        app.Run();
    }
}