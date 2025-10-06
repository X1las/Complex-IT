using System.Net.Sockets;
using System.Net;

namespace Assignment3;

class Client
{
    public static void Main()
    {
        var client = new TcpClient();

        client.Connect(IPAddress.Loopback, 5000);

        var msg = client.Read();

        Console.WriteLine(msg);
        client.Close();
    }
}