using Discord;
using Discord.Commands;
using MediatR;
using IResult = Discord.Commands.IResult;

namespace ASPNET_Discord_Bot.Notifications;

public class CommandExecutedNotification : INotification
{
    public Optional<CommandInfo> Command { get; }
    public ICommandContext Context { get; }
    public IResult Result { get; }

    public CommandExecutedNotification(Optional<CommandInfo> command, ICommandContext context, IResult result)
    {
        Command = command;
        Context = context ?? throw new ArgumentNullException(nameof(context));
        Result = result ?? throw new ArgumentNullException(nameof(result));
    }
}
