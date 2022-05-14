using Discord;
using Discord.WebSocket;
using MediatR;

namespace ASPNET_Discord_Bot.Notifications.Reactions;

public class ReactionRemovedNotification : INotification
{
    public Cacheable<IUserMessage, ulong> UserMessage { get; }
    public Cacheable<IMessageChannel, ulong> MessageChannel { get; }
    public SocketReaction SocketReaction { get; }

    public ReactionRemovedNotification(Cacheable<IUserMessage, ulong> userMessage, Cacheable<IMessageChannel, ulong> messageChannel, SocketReaction socketReaction)
    {
        UserMessage = userMessage;
        MessageChannel = messageChannel;
        SocketReaction = socketReaction ?? throw new ArgumentNullException(nameof(socketReaction));
    }
}
