using Discord.WebSocket;
using MediatR;

namespace ASPNET_Discord_Bot.Notifications;

public class ChannelCreatedNotification : INotification
{
    public SocketChannel SocketChannel { get; }

    public ChannelCreatedNotification(SocketChannel socketChannel)
        => SocketChannel = socketChannel ?? throw new ArgumentNullException(nameof(socketChannel));
}
