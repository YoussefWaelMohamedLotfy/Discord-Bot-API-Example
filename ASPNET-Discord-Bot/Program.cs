using ASPNET_Discord_Bot.HostedServices;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Host
builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .ReadFrom.Configuration(context.Configuration);
});

// Configure DI Services
builder.Services.AddHostedService<DiscordBotInitializer>();

var app = builder.Build();

app.MapGet("/", () => "Welcome to ASP.NET Discord Bot!");

app.Run();
