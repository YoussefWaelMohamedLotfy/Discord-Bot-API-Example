using Discord;
using Discord.WebSocket;

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
}
