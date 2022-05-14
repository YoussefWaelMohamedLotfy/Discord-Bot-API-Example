using Discord;
using Discord.Commands;
using Newtonsoft.Json.Linq;

namespace ASPNET_Discord_Bot.Modules;

public class MemeModule : ModuleBase<SocketCommandContext>
{
    private readonly ILogger<MemeModule> _logger;

    public MemeModule(ILogger<MemeModule> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [Command("meme")]
    [Alias("reddit")]
    public async Task Meme(string subreddit = default!)
    {
        var client = new HttpClient();
        var result = await client.GetStringAsync($"https://reddit.com/r/{subreddit ?? "memes"}/random.json?limit=1");

        if (!result.StartsWith("["))
        {
            await ReplyAsync("This subbreddit does not exist.");
            return;
        }

        JArray arr = JArray.Parse(result);
        JObject post = JObject.Parse(arr[0]!["data"]!["children"]![0]!["data"]!.ToString());

        var builder = new EmbedBuilder()
                .WithImageUrl(post["url"]!.ToString())
                .WithTitle($"{post["title"]}")
                .WithUrl($"https://reddit.com{post["permalink"]}")
                .WithColor(new Color(60, 196, 33))
                .WithFooter($"🗨️ {post["num_comments"]} ⬆️ {post["ups"]}");

        var embed = builder.Build();
        await ReplyAsync(embed: embed);
    }
}
