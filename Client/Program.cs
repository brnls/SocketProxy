// See https://aka.ms/new-console-template for more information
using System.Net;
using System.Net.Sockets;
using System.Text;


foreach(var word in new [] { "a", "bc", "def", "ghij", "klmnop", "qrstuv"})
{
    using var tcpClient = new TcpClient();
    await tcpClient.ConnectAsync(IPAddress.Loopback, 5000);

    using var stream = tcpClient.GetStream();

    await stream.WriteAsync(Encoding.UTF8.GetBytes(word));
    var buf = new byte[512];
    var result = await stream.ReadAsync(buf);

    Console.WriteLine(result);
    Console.WriteLine(Encoding.UTF8.GetString(buf[0..result]));
}
