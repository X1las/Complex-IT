using TcpListener = System.Net.Sockets.TcpListener;
using TcpClient = System.Net.Sockets.TcpClient;
using System.Text;
using System.Text.Json;

namespace Assignment3;

public class Server
{
    // Variables
    TcpListener? tcp_service;
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
            
            // Handle each client on a separate thread
            var clientThread = new Thread(() => HandleClient(client));
            clientThread.IsBackground = true; // Allows main thread to exit
            clientThread.Start();
        }
    }

    // Alternative async version for better performance

    private void HandleClient(TcpClient client)
{
    using (client) // Add proper disposal
    {
        var stream = client.GetStream();
        Console.WriteLine("Handling client request...");
        var buffer = new byte[1024];
        Console.WriteLine("Reading data from client...");
        int count = stream.Read(buffer, 0, buffer.Length);
        Console.WriteLine($"Read {count} bytes from client.");

        if (count == 0)
        {
            Console.WriteLine("No data received from client.");
            return;
        }

        var json = Encoding.UTF8.GetString(buffer, 0, count);
        Console.WriteLine($"Decoded JSON: {json}");
        var response = new Response();
        Request? request = null;
        
        try
        {
            request = JsonSerializer.Deserialize<Request>(json);
            Console.WriteLine("Deserialized request object. " + request?.Method + " " + request?.Path);
                if (request == null)
                {
                    response.Status = "4 Bad Request";
                    response.Body = "Invalid Request";
                    Console.WriteLine("Request object is null.");
                }
                else
                {
                    var validator = new RequestValidator();
                    Console.WriteLine("Validating request...");
                    response = validator.ValidateRequest(request);
                    
                    if (response.Status != "1 Ok")
                    {
                        Console.WriteLine("Request validation failed.");
                    }
                    else
                    {
                        UrlParser parser = new UrlParser();
                
               
                        var parsed = parser.ParseUrl(request.Path?.Trim('/') ?? "");

                        if (!parsed && request.Method?.ToLower() != "echo")
                        {
                            response.Status = "5 Not Found";
                        }
                        else if (!int.TryParse(parser.Id, out int isAnInt))
                        {
                            response.Status = "4 Bad Request";
                        }
                        else if (parser.HasId && request.Method?.ToLower() == "create")
                        {
                            response.Status = "4 Bad Request";
                            Console.WriteLine("Create method should not have an ID in the URL.");
                        }
                    }
                    Console.WriteLine($"Validation result: {response.Status}");
                    Console.WriteLine($"Response method: {request.Method}");
                }

            }
            catch (JsonException)
            {
                response.Status = "4 Bad Request";
                response.Body = "Invalid JSON";
            }
        if (response.Status == "1 Ok")
        {  
        if (request?.Method?.ToLower() == "echo")
        {
            response.Status = "";
            response.Body = request.Body;
        }

        if (request?.Method?.ToLower() == "read" && request.Path == "/api/categories")
        {
            CategoryService categories = new();
            response.Body = JsonSerializer.Serialize(categories.GetCategories());
        }

        Console.WriteLine($"Response Status: {response.Status}");
        
        // Send response back to client
        var responseJson = JsonSerializer.Serialize(response);
        var responseBytes = Encoding.UTF8.GetBytes(responseJson);
        stream.Write(responseBytes, 0, responseBytes.Length);
        stream.Flush();
    }
}
}
}