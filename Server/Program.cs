using Microsoft.AspNetCore.Connections;
using Server;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenLocalhost(5001, l =>
    {
        l.UseConnectionHandler<RabbitHandler>();
    });
});

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.Run();
