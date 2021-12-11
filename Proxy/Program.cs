using Microsoft.AspNetCore.Connections;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenLocalhost(5000, l =>
    {
        l.UseConnectionHandler<ProxyHandler>();
    });
});

var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

app.Run();
