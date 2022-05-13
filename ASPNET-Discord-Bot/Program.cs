using ASPNET_Discord_Bot.HostedServices;
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

builder.Services.AddHostedService<DiscordBotInitializer>();

builder.Services.AddMediatR(Assembly.GetEntryAssembly()!);

var app = builder.Build();

app.MapGet("/", () => "Welcome to ASP.NET Discord Bot!");

app.Run();
