using Discord;
using Discord.WebSocket;
using Serilog.Events;
using Serilog;

namespace ASPNET_Discord_Bot.HostedServices;

public class DiscordBotInitializer : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly IServiceProvider _provider;
    private readonly IConfiguration _configuration;

    public DiscordBotInitializer(DiscordSocketClient client,
                                 IServiceProvider provider,
                                 IConfiguration configuration)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        _client.Log += LogAsync;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _client.LoginAsync(TokenType.Bot, _configuration.GetValue<string>("DiscordBot:BotToken"));
        await _client.StartAsync();
        //await _client.SetStatusAsync(UserStatus.Idle);
        await _client.SetGameAsync("Visual Studio 2022");

        var listener = _provider.GetRequiredService<DiscordBotEventListener>();
        await listener.StartAsync();

        await Task.CompletedTask;
    }

    //private async Task OnChannelCreated(SocketChannel arg)
    //{
    //    if (arg as ITextChannel is null)
    //        return;

    //    var channel = arg as ITextChannel;
    //    await channel!.SendMessageAsync("Welcome to the new Channel");
    //}

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _client.LogoutAsync();
        await _client.StopAsync();
        await _client.DisposeAsync();

        await Task.CompletedTask;
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
}
