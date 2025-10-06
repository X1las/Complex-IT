using System.Net.Sockets;
using System.Net;
using System.Text;

var client = new TcpClient();

client.Connect(IPAddress.Loopback, 5000);

var msg = client.Read();

Console.WriteLine(msg);
