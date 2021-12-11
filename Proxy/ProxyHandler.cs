using Microsoft.AspNetCore.Connections;
using System.Net;
using System.Net.Sockets;
using System.Text;

internal class ProxyHandler : ConnectionHandler
{
    private readonly ILogger<ProxyHandler> _logger;

    public ProxyHandler(ILogger<ProxyHandler> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        _logger.LogInformation("Accepting connection");
        using var client = new TcpClient();
        await client.ConnectAsync(IPAddress.Loopback, 5001);

        var stream = client.GetStream();
        var readingRequest = Task.Run(async () =>
        {
            while (true)
            {
                var readResult = await connection.Transport.Input.ReadAsync();

                foreach (var segment in readResult.Buffer)
                {
                    if (segment.Length == 0) continue;
                    await stream.WriteAsync(segment, connection.ConnectionClosed);
                }

                if (readResult.IsCompleted)
                {
                    break;
                }

                connection.Transport.Input.AdvanceTo(readResult.Buffer.End);
            }
        });

        var writingResponse = Task.Run(async () =>
        {
            while (true)
            {
                var mem = connection.Transport.Output.GetMemory();
                int bytesRead = await stream.ReadAsync(mem, connection.ConnectionClosed);
                if (bytesRead == 0)
                {
                    break;
                }
                connection.Transport.Output.Advance(bytesRead);
                await connection.Transport.Output.FlushAsync(connection.ConnectionClosed);
            }
        });


        await Task.WhenAll(readingRequest, writingResponse);
        _logger.LogInformation("Finishing connection");
    }
}