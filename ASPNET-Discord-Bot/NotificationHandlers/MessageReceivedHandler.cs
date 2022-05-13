using ASPNET_Discord_Bot.Notifications;
using Discord.Commands;
using Discord.WebSocket;
using Discord;
using MediatR;

namespace ASPNET_Discord_Bot.NotificationHandlers;

public class MessageReceivedHandler : INotificationHandler<MessageReceivedNotification>
{
    private readonly ILogger<MessageReceivedHandler> _logger;
    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _provider;

    public MessageReceivedHandler(ILogger<MessageReceivedHandler> logger, DiscordSocketClient client, CommandService commands, IServiceProvider provider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _commands = commands ?? throw new ArgumentNullException(nameof(commands));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
    }

    public async Task Handle(MessageReceivedNotification notification, CancellationToken cancellationToken)
    {
        // Don't process the command if it was a system message
        if (notification.Message is not SocketUserMessage message)
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
            return;
        }

        // Execute the command with the command context we just
        // created, along with the service provider for precondition checks.
        await _commands.ExecuteAsync(context: context, argPos: argPos, services: _provider);

        await Task.CompletedTask;
    }
}