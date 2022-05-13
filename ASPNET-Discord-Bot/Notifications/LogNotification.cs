using Discord;
using MediatR;

namespace ASPNET_Discord_Bot.Notifications;

public class LogNotification : INotification
{
    public LogMessage LogMessage { get; }

    public LogNotification(LogMessage logMessage) 
        => LogMessage = logMessage;
}
