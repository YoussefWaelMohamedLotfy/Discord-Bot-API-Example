using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace ASPNET_Discord_Bot.Modules;

public class ModerationModule : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<ModerationModule> _logger;

    public ModerationModule(ILogger<ModerationModule> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Command("purge")]
    [RequireUserPermission(GuildPermission.ManageMessages)]
    public async Task Purge(int amount = 100)
    {
        var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
        await (Context.Channel as SocketTextChannel)!.DeleteMessagesAsync(messages);

        var message = await ReplyAsync($"{messages.Count()} messages deleted Successfully!");
        await Task.Delay(2500);
        await message.DeleteAsync();
    }
}
