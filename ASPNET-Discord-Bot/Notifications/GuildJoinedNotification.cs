using Discord.WebSocket;
using MediatR;

namespace ASPNET_Discord_Bot.Notifications;

public class GuildJoinedNotification : INotification
{
    public SocketGuild SocketGuild { get; }

    public GuildJoinedNotification(SocketGuild socketGuild)
        => SocketGuild = socketGuild ?? throw new ArgumentNullException(nameof(socketGuild));
}
