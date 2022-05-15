using ASPNET_Discord_Bot.Notifications;
using ASPNET_Discord_Bot.Notifications.Reactions;
using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using MediatR;
using Microsoft.VisualBasic;
using System.Reflection;

namespace ASPNET_Discord_Bot.HostedServices;

public class DiscordBotEventListener
{
    private readonly CancellationToken _cancellationToken;

    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly InteractionService _interactionService;
    private readonly IServiceProvider _provider;
    private readonly IConfiguration _configuration;
    private readonly IMediator _mediator;

    public DiscordBotEventListener(DiscordSocketClient client,
                                   CommandService commands,
                                   IServiceProvider provider,
                                   IMediator mediator,
                                   InteractionService interactionService,
                                   IConfiguration configuration)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _commands = commands ?? throw new ArgumentNullException(nameof(commands));
        _interactionService = interactionService ?? throw new ArgumentNullException(nameof(interactionService));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        _cancellationToken = new CancellationTokenSource().Token;
    }

    public async Task StartAsync()
    {
        _client.Log += OnLogReceivedAsync;
        _commands.Log += OnLogReceivedAsync;

        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: _provider);
        await _interactionService.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: _provider);
        _client.Ready += OnClientReadyAsync;
        _client.MessageReceived += OnMessageReceivedAsync;
        _client.ChannelCreated += OnChannelCreatedAsync;
        _client.JoinedGuild += OnJoinedGuildAsync;
        _client.ReactionAdded += OnReactionAddedAsync;
        _client.ReactionRemoved += OnReactionRemovedAsync;
        _client.SlashCommandExecuted += OnSlashCommandExecutedAsync;

        _commands.CommandExecuted += OnCommandExecutedAsync;

        await Task.CompletedTask;
    }

    private async Task _client_InteractionCreated(SocketInteraction arg)
{
        try
        {
            // create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules
            var ctx = new SocketInteractionContext(_client, arg);
            await _interactionService.ExecuteCommandAsync(ctx, _provider);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            // if a Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
            // response, or at least let the user know that something went wrong during the command execution.
            if (arg.Type == InteractionType.ApplicationCommand)
            {
                await arg.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
            }
        }
    }

    private async Task OnSlashCommandExecutedAsync(SocketSlashCommand slashCommand)
        => await _mediator.Publish(new SlashCommandExecutedNotification(slashCommand), _cancellationToken);

    private async Task OnClientReadyAsync()
    {
        //var guild = _client.GetGuild(_configuration.GetValue<ulong>("DiscordBot:GuildId"));

        //var guildCommand = new SlashCommandBuilder()
        //    .WithName("first-command")
        //    .WithDescription("This is my first guild slash command!")
        //    .Build();

        //await guild.CreateApplicationCommandAsync(guildCommand);

        await _interactionService.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        await _interactionService.RegisterCommandsToGuildAsync(_configuration.GetValue<ulong>("DiscordBot:GuildId"));

        _client.InteractionCreated += _client_InteractionCreated;

        await Task.CompletedTask;
    }

    private Task OnReactionRemovedAsync(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        => _mediator.Publish(new ReactionRemovedNotification(arg1, arg2, arg3), _cancellationToken);

    private Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        => _mediator.Publish(new ReactionAddedNotification(arg1, arg2, arg3), _cancellationToken);

    private async Task OnJoinedGuildAsync(SocketGuild arg)
        => await _mediator.Publish(new GuildJoinedNotification(arg), _cancellationToken);

    private async Task OnCommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, Discord.Commands.IResult result)
        => await _mediator.Publish(new CommandExecutedNotification(command, context, result), _cancellationToken);

    private async Task OnChannelCreatedAsync(SocketChannel socketChannel)
        => await _mediator.Publish(new ChannelCreatedNotification(socketChannel), _cancellationToken);

    private async Task OnLogReceivedAsync(LogMessage logMessage)
        => await _mediator.Publish(new LogNotification(logMessage), _cancellationToken);

    private async Task OnMessageReceivedAsync(SocketMessage socketMessage)
        => await _mediator.Publish(new MessageReceivedNotification(socketMessage), _cancellationToken);
}
