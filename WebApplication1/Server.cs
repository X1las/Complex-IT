using TcpListener = System.Net.Sockets.TcpListener;
using TcpClient = System.Net.Sockets.TcpClient;
using System.Text;

namespace Assignment3;

public class Server
{
    public int Port { get; set; }
    public Server(int port)
    {
        Port = port;
    }

    TcpListener tcp_service;
    public void Start()
    {
        tcp_service = new TcpListener(System.Net.IPAddress.Loopback, Port);
        tcp_service.Start();
        Console.WriteLine($"Server started on port {Port}");
        // Simulate server running
        while (true)
        {
            TcpClient client = tcp_service.AcceptTcpClient();
            Console.WriteLine("Client connected");
            HandleClient(client);
        }
    }

    private void HandleClient(TcpClient client)
    {
        var stream = client.GetStream();
        var msg = "Hello form server";
        stream.Write(Encoding.UTF8.GetBytes(msg));

    }
}

/*
public class EchoServer
{
    public static void Main(string[] args)
    {
        Console.WriteLine(" //////////////////////////////////////////////////////////\n///\n/// Testing UrlParser class \n///\n//////////////////////////////////////////////////////////");
        Console.WriteLine();
        var parser = new UrlParser(); // create a parser instance
        if (parser.ParseUrl("/api/categories/1", 2)) // parse a sample URL with minSegments=2
        {
            Console.WriteLine($"Path: {parser.Path}"); // show parsed path
            Console.WriteLine($"HasId: {parser.HasId}"); // show whether id was found
            Console.WriteLine($"Id: {parser.Id}\n"); // show extracted id
        }
        else
        {
            Console.WriteLine("Invalid URL"); // fallback
        }

        Console.WriteLine(" //////////////////////////////////////////////////////////\n///\n/// Testing RequestValidator class \n///\n//////////////////////////////////////////////////////////");
        Console.WriteLine();

        // Testing RequestValidator class
        var validator = new RequestValidator(); // create request validator

        // 1. Missing Method
        Console.WriteLine("Missing Method:");
        Console.WriteLine(validator.ValidateRequest(new Request
        {
            Path = "/api/xxx",
            Date = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() // current unix seconds
        }).Status); // print validator status for the crafted request
        Console.WriteLine();

        // 2. Illegal Method
        Console.WriteLine("Illegal Method:");
        Console.WriteLine(validator.ValidateRequest(new Request
        {
            Method = "fetch", // invalid method
            Path = "/api/categories/1",
            Date = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
        }).Status);
        Console.WriteLine();

        // 3. Missing Path
        Console.WriteLine("Missing Path:");
        Console.WriteLine(validator.ValidateRequest(new Request
        {
            Method = "read",
            Date = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
        }).Status);
        Console.WriteLine();

        // 4. Valid Body (JSON)
        Console.WriteLine("Valid Body:");
        Console.WriteLine(validator.ValidateRequest(new Request
        {
            Method = "create",
            Path = "/api/xxx",
            Date = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            Body = "{\"id\":1,\"name\":\"xxx\"}" // valid JSON body
        }).Status);
        Console.WriteLine();

        // 5. Illegal Body (invalid JSON)
        Console.WriteLine("Illegal Body:");
        Console.WriteLine(validator.ValidateRequest(new Request
        {
            Method = "create",
            Path = "/api/xxx",
            Date = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
            Body = "not-json" // deliberately invalid JSON
        }).Status);
        Console.WriteLine();

        Console.WriteLine(" //////////////////////////////////////////////////////////\n///\n/// Testing CategoryService class \n///\n//////////////////////////////////////////////////////////");
        Console.WriteLine();

        // Testing CategoryService class
        var service = new CategoryService(); // instantiate in-memory service

        // 1. Get all categories
        Console.WriteLine("Initial categories:");
        foreach (var c in service.GetCategories()) // iterate seeded categories
            Console.WriteLine($"{c.Id}: {c.Name}");
        Console.WriteLine();

        // 2. Get category by ID
        var cat = service.GetCategory(2); // should return the 'Condiments' category
        Console.WriteLine($"GetCategory(2): {cat?.Name}\n");

        // 3. Try to get non-existent category
        var none = service.GetCategory(-1); // no category with negative id
        Console.WriteLine($"GetCategory(-1): {(none == null ? "null" : "exists")}\n");

        Console.WriteLine($"The Last Instance Of This Project ran at: {DateTime.Now:dd-MM MMMM -yyyy HH:mm}");
        Console.WriteLine(validator.GetType());
        Console.WriteLine(service.GetType());
        Console.WriteLine(parser.GetType());
    }
        
    }

    private void HandleClient(TcpClient client)
    {
        var stream = client.GetStream();

        var msg = "Hello form server";

        stream.Write(Encoding.UTF8.GetBytes(msg));

    }
}
*/