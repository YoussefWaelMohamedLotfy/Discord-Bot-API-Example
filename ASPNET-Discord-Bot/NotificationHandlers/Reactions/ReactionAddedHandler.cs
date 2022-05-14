using ASPNET_Discord_Bot.Notifications.Reactions;
using MediatR;

namespace ASPNET_Discord_Bot.NotificationHandlers.Reactions;

public class ReactionAddedHandler : INotificationHandler<ReactionAddedNotification>
{
    private readonly ILogger<ReactionAddedHandler> _logger;

    public ReactionAddedHandler(ILogger<ReactionAddedHandler> logger)
        => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Handle(ReactionAddedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{{Username}} reacted on message with ID {notification.UserMessage.Id} - {{Reaction}}",
            notification.SocketReaction.User.Value.Username, notification.SocketReaction.Emote);

        await Task.CompletedTask;
    }
}
