using ASPNET_Discord_Bot.HostedServices;
using ASPNET_Discord_Bot.Services;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MediatR;
using Serilog;
using System.Reflection;

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
builder.Services.AddSingleton<DiscordSocketClient>();
builder.Services.AddSingleton<CommandService>();
builder.Services.AddSingleton<DiscordBotEventListener>();
builder.Services.AddSingleton<ImageService>();

builder.Services.AddHostedService<DiscordBotInitializer>();

builder.Services.Configure<DiscordSocketConfig>(options =>
{
    options.LogLevel = LogSeverity.Verbose;
    options.AlwaysDownloadUsers = true;
    options.MessageCacheSize = 200;
});

builder.Services.Configure<CommandServiceConfig>(options =>
{
    options.LogLevel = LogSeverity.Verbose;
    options.CaseSensitiveCommands = true;
    options.DefaultRunMode = RunMode.Async;
});

builder.Services.AddMediatR(Assembly.GetEntryAssembly()!);

var app = builder.Build();

app.MapGet("/", () => "Welcome to ASP.NET Discord Bot!");

app.Run();
