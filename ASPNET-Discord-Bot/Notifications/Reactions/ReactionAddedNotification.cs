using Discord;
using Discord.WebSocket;
using MediatR;

namespace ASPNET_Discord_Bot.Notifications.Reactions;

public class ReactionAddedNotification : INotification
{
    public Cacheable<IUserMessage, ulong> UserMessage { get; }
    public Cacheable<IMessageChannel, ulong> MessageChannel { get; }
    public SocketReaction SocketReaction { get; }

    public ReactionAddedNotification(Cacheable<IUserMessage, ulong> userMessage, Cacheable<IMessageChannel, ulong> messageChannel, SocketReaction socketReaction)
    {
        UserMessage = userMessage;
        MessageChannel = messageChannel;
        SocketReaction = socketReaction ?? throw new ArgumentNullException(nameof(socketReaction));
    }
}