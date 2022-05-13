using Discord;
using Discord.WebSocket;
using Serilog.Events;
using Serilog;
using System.Reflection;
using Discord.Commands;

namespace ASPNET_Discord_Bot.HostedServices;

public class DiscordBotInitializer : IHostedService
{
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _provider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<DiscordBotInitializer> _logger;

    public DiscordBotInitializer(DiscordSocketClient client,
                                 CommandService commands,
                                 IServiceProvider provider,
                                 IConfiguration configuration,
                                 ILogger<DiscordBotInitializer> logger)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _commands = commands ?? throw new ArgumentNullException(nameof(commands));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _client.Log += LogAsync;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _client.LoginAsync(TokenType.Bot, _configuration.GetValue<string>("DiscordBot:BotToken"));
        await _client.StartAsync();
        //await _client.SetStatusAsync(UserStatus.Idle);
        await _client.SetGameAsync("Visual Studio 2022");

        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: _provider);

        _client.MessageReceived += HandleCommandAsync;

        await Task.CompletedTask;
    }

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

    private async Task HandleCommandAsync(SocketMessage messageParam)
    {
        // Don't process the command if it was a system message
        if (messageParam is not SocketUserMessage message)
            return;

        if (message.Source != MessageSource.User)
            return;

        // Create a number to track where the prefix ends and the command begins
        int argPos = 0;

        // Create a WebSocket-based command context based on the message
        var context = new SocketCommandContext(_client, message);

        // Determine if the message is a command based on the prefix and make sure no bots trigger commands
        if (!(message.HasCharPrefix('!', ref argPos) ||
            message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
            message.Author.IsBot)
        {
            //await message.Channel.SendMessageAsync("You forgot the !");
            return;
        }


        // Execute the command with the command context we just
        // created, along with the service provider for precondition checks.
        var result = await _commands.ExecuteAsync(context: context, argPos: argPos, services: _provider);

        if (!result.IsSuccess)
            _logger.LogError(result.ErrorReason);

        if (result.Error.Equals(CommandError.UnmetPrecondition))
            await message.Channel.SendMessageAsync(result.ErrorReason);
    }
}
