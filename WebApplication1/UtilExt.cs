using System.Net.Sockets;
using System.Text;

namespace Assignment3;
/*
public static class UtilExt
{
    public static string Read(this TcpClient client )
    {
        var stream = client.GetStream();

        byte[] buffer = new byte[1024];

        var readCount = stream.Read(buffer);

        return Encoding.UTF8.GetString(buffer, 0, readCount);
    }
}

*/