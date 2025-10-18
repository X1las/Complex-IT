
using Npgsql;

namespace DataServiceLayer;

class Program
{
     public static DataService DataService{ get; } = new DataService();
    static void Main(string[] args)
    {
        var connectionString = "host=newtlike.com;db=northwind;uid=rucdb;pwd=testdb";
        var connection = new NpgsqlConnection(connectionString);

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
        {
            Console.WriteLine($"{reader.GetInt32(0)} {reader.GetString(1)}");
        }

        // Move to next result set (products)
        reader.NextResult();
        Console.WriteLine("\n=== PRODUCTS ===");
        while (reader.Read())
        {
            Console.WriteLine($"{reader.GetInt32(0)} {reader.GetString(1)}");
        }

        // Move to next result set (orders)
        reader.NextResult();
        Console.WriteLine("\n=== ORDERS ===");
        while (reader.Read())
        {
            Console.WriteLine($"{reader.GetInt32(0)}");
        }

        // Move to next result set (order details)
        reader.NextResult();
        Console.WriteLine("\n=== ORDER DETAILS ===");
        while (reader.Read())
        {
            Console.WriteLine($"{reader.GetInt32(0)}");
        }

        reader.Close();
        connection.Close();
    }
}