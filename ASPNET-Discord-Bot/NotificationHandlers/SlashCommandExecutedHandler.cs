using ASPNET_Discord_Bot.Notifications;
using MediatR;

namespace ASPNET_Discord_Bot.NotificationHandlers;

public class SlashCommandExecutedHandler : INotificationHandler<SlashCommandExecutedNotification>
{
    private readonly ILogger<SlashCommandExecutedHandler> _logger;

    public SlashCommandExecutedHandler(ILogger<SlashCommandExecutedHandler> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(SlashCommandExecutedNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("{Username} executed the Slash Command [{SlashCommand}]", notification.SlashCommand.User.Username, notification.SlashCommand.Data.Name);

        await notification.SlashCommand.RespondAsync($"You executed {notification.SlashCommand.Data.Name}");

        await Task.CompletedTask;
    }
}
