using ASPNET_Discord_Bot.Notifications;
using MediatR;

namespace ASPNET_Discord_Bot.NotificationHandlers;

public class GuildJoinedHandler : INotificationHandler<GuildJoinedNotification>
{
    private readonly ILogger<GuildJoinedHandler> _logger;

    public GuildJoinedHandler(ILogger<GuildJoinedHandler> logger)
        => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Handle(GuildJoinedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("New Guild joined [{Guild}]", notification.SocketGuild.CurrentUser.Username);
        await notification.SocketGuild.DefaultChannel.SendMessageAsync("Thank you for using my Discord Bot <3");

        await Task.CompletedTask;
    }
}
