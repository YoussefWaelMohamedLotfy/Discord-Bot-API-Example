using ASPNET_Discord_Bot.HostedServices;
using ASPNET_Discord_Bot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MediatR;
using Serilog;
using System.Reflection;
using System.Text;

Console.OutputEncoding = Encoding.UTF8;
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
builder.Services.AddSingleton(s => new DiscordSocketClient(new()
{
    LogLevel = LogSeverity.Verbose,
    AlwaysDownloadUsers = true,
    MessageCacheSize = 200,
}));
builder.Services.AddSingleton(s => new CommandService(new() 
{
    LogLevel = LogSeverity.Verbose,
    CaseSensitiveCommands = false,
    DefaultRunMode = RunMode.Async,
}));
builder.Services.AddSingleton<DiscordBotEventListener>();
builder.Services.AddSingleton<IImageService, ImageService>();

builder.Services.AddHostedService<DiscordBotInitializer>();

builder.Services.AddMediatR(Assembly.GetEntryAssembly()!);

var app = builder.Build();

app.MapGet("/", () => "Welcome to ASP.NET Discord Bot!");

app.Run();
