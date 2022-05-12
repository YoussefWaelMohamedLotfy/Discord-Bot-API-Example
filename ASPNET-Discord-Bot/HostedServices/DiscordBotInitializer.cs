using Discord;
using Discord.WebSocket;
using Serilog.Events;
using Serilog;

namespace ASPNET_Discord_Bot.HostedServices;

public class DiscordBotInitializer : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;

    public DiscordBotInitializer(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        _client = new DiscordSocketClient();

        _client.Log += LogAsync;
    }

    private async Task LogAsync(LogMessage logMessage)
    {
        var severity = logMessage.Severity switch
        {
            LogSeverity.Critical => LogEventLevel.Fatal,
            LogSeverity.Error => LogEventLevel.Error,
            LogSeverity.Warning => LogEventLevel.Warning,
            LogSeverity.Info => LogEventLevel.Information,
            LogSeverity.Verbose => LogEventLevel.Verbose,
            LogSeverity.Debug => LogEventLevel.Debug,
            _ => LogEventLevel.Information
        };

        Log.Write(severity, logMessage.Exception, "[{Source}] {Message}", logMessage.Source, logMessage.Message);
        await Task.CompletedTask;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _client.LoginAsync(TokenType.Bot, _configuration.GetValue<string>("DiscordBot:BotToken"));
        await _client.StartAsync();

        await Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.LogoutAsync();
        await _client.StopAsync();
        await _client.DisposeAsync();

        await Task.CompletedTask;
    }
}
