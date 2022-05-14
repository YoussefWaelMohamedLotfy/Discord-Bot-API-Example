using Discord.WebSocket;

namespace ASPNET_Discord_Bot.Services;

public interface IImageService
{
    Task<string> CreateImageAsync(SocketGuildUser user);
}