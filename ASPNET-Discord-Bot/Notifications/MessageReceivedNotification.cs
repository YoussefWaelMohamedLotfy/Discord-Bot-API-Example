using Discord.WebSocket;
using MediatR;

namespace ASPNET_Discord_Bot.Notifications;

public class MessageReceivedNotification : INotification
{
    public SocketMessage Message { get; }

    public MessageReceivedNotification(SocketMessage message)
        => Message = message ?? throw new ArgumentNullException(nameof(message));
}
