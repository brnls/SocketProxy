using Microsoft.AspNetCore.Connections;
using System.Text;

namespace Server;

public class RabbitHandler : ConnectionHandler
{
    private readonly ILogger<RabbitHandler> _logger;

    public RabbitHandler(ILogger<RabbitHandler> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync(ConnectionContext connection)
    {
        _logger.LogInformation("Accepting connection");
        var readStream = connection.Transport.Input.AsStream();

        var buf = new byte[512];
        var bytesRead = await readStream.ReadAsync(buf, connection.ConnectionClosed);

        var message = Encoding.UTF8.GetString(buf[0..bytesRead]);

        var writeStream = connection.Transport.Output.AsStream();
        await writeStream.WriteAsync(Encoding.UTF8.GetBytes($"Hello {message}"));
        await writeStream.FlushAsync();
        _logger.LogInformation("Finishing connection");
    }
}
