using ASPNET_Discord_Bot.Notifications.Reactions;
using MediatR;

namespace ASPNET_Discord_Bot.NotificationHandlers.Reactions;

public class ReactionRemovedHandler : INotificationHandler<ReactionRemovedNotification>
{
    private readonly ILogger<ReactionRemovedHandler> _logger;

    public ReactionRemovedHandler(ILogger<ReactionRemovedHandler> logger)
        => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Handle(ReactionRemovedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{{Username}} removed reaction on message with ID {notification.UserMessage.Id} - {{Reaction}}",
            notification.SocketReaction.User.Value.Username, notification.SocketReaction.Emote);

        await Task.CompletedTask;
    }
}
