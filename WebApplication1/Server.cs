using TcpListener = System.Net.Sockets.TcpListener;
using TcpClient = System.Net.Sockets.TcpClient;
using System.Text;
using System.Net;
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
        var request = JsonSerializer.Deserialize<Request>(json);

        Response response;

        var validator = new RequestValidator();
        try
        {
            response = validator.ValidateRequest(request);
        }
        catch (Exception ex)
        {
            response = new Response
            {
                Status = "5",
                Body = ex.Message
            };
        }


        stream.Write(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response)));
    }
}