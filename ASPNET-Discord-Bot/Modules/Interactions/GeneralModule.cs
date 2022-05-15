using Discord.Interactions;

namespace ASPNET_Discord_Bot.Modules.Interactions;

public class GeneralModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "Send a ping message to the bot")]
    public async Task Ping()
    {
        await ReplyAsync("Pong!");
    }

}
