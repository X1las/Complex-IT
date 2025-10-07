using TcpListener = System.Net.Sockets.TcpListener;
using TcpClient = System.Net.Sockets.TcpClient;
using System.Text;
using System.Text.Json;

namespace Assignment3;

public class Server
{
    // Variables
    TcpListener tcp_service;
    public int Port { get; set; }

    // Constructor to initialize the server with a specific port
    public Server(int port)
    {
        Port = port;
    }

    // Method to start the server and listen for incoming connections
    public void Start()
    {
        tcp_service = new TcpListener(System.Net.IPAddress.Loopback, Port);
        tcp_service.Start();
        Console.WriteLine($"Server started on port {Port}");

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
        var buffer = new byte[1024];
        int count = stream.Read(buffer, 0, buffer.Length);
        if (count == 0) return;

        var json = Encoding.UTF8.GetString(buffer, 0, count);
        
        var response = new Response();
        
        try
        {
            var request = JsonSerializer.Deserialize<Request>(json);
            var validator = new RequestValidator();
            response = validator.ValidateRequest(request);
        }
        catch (JsonException)
        {
            response.Status = "4 Bad Request";
            response.Body = "Invalid JSON";
        }
        catch (Exception ex)
        {
            response.Status = "5 Internal Error";
            response.Body = ex.Message;
        }

        // Send response back to client
        var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));
        stream.Write(responseBytes, 0, responseBytes.Length);
        stream.Flush(); // Ensure data is sent immediately
    }



}