using ASPNET_Discord_Bot.Notifications;
using MediatR;

namespace ASPNET_Discord_Bot.NotificationHandlers;

public class CommandExecutedHandler : INotificationHandler<CommandExecutedNotification>
{
    private readonly ILogger<CommandExecutedHandler> _logger;

    public CommandExecutedHandler(ILogger<CommandExecutedHandler> logger)
        => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Handle(CommandExecutedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Command executed by [{Username}] - [{CommandName}]", notification.Context.User.Username, notification.Command.Value.Name);

        if (!notification.Command.IsSpecified || !notification.Result.IsSuccess)
        {
            _logger.LogError("Command error by [{Username}]: {CommandError} - {CommandErrorReason}", notification.Context.User.Username,
                notification.Result.Error, notification.Result.ErrorReason);
            await notification.Context.Channel.SendMessageAsync($"Error: {notification.Result.Error}\nReason: {notification.Result.ErrorReason}");
        }

        await Task.CompletedTask;
    }
}
