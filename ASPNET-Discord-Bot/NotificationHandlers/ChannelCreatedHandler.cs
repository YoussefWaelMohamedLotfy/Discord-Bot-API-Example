using ASPNET_Discord_Bot.Notifications;
using Discord;
using MediatR;

namespace ASPNET_Discord_Bot.NotificationHandlers;

public class ChannelCreatedHandler : INotificationHandler<ChannelCreatedNotification>
{
    private readonly ILogger<ChannelCreatedHandler> _logger;

    public ChannelCreatedHandler(ILogger<ChannelCreatedHandler> logger)
        => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Handle(ChannelCreatedNotification notification, CancellationToken cancellationToken)
    {
        if (notification.SocketChannel as ITextChannel is null)
            return;

        var channel = notification.SocketChannel as ITextChannel;
        _logger.LogInformation("New Channel Created [{ChannelName}]", channel!.Name);

        await channel!.SendMessageAsync($"Welcome to the \"{channel.Name}\" Channel");

        await Task.CompletedTask;
    }
}
