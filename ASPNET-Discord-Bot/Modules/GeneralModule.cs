using ASPNET_Discord_Bot.Services;
using Discord.Commands;
using Discord.WebSocket;
using System.Data;

namespace ASPNET_Discord_Bot.Modules;

public class GeneralModule : ModuleBase<SocketCommandContext>
{
    private readonly ImageService _imageService;

    public GeneralModule(ImageService imageService)
    {
        _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
    }

    [Command("math")]
    public async Task MathAsync([Remainder] string math)
    {
        var dt = new DataTable();
        var result = dt.Compute(math, null);

        await ReplyAsync($"Result: {result}");
    }

    [Command("image")]
    public async Task Image(SocketGuildUser user)
    {
        var path = await _imageService.CreateImageAsync(user);
        await Context.Channel.SendFileAsync(path);
        File.Delete(path);
    }
}
