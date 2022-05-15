using Discord.WebSocket;
using MediatR;

namespace ASPNET_Discord_Bot.Notifications;

public class SlashCommandExecutedNotification : INotification
{
    public SocketSlashCommand SlashCommand { get; }

    public SlashCommandExecutedNotification(SocketSlashCommand slashCommand)
        => SlashCommand = slashCommand ?? throw new ArgumentNullException(nameof(slashCommand));
}
