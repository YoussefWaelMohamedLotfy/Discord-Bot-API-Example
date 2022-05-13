﻿using ASPNET_Discord_Bot.Notifications;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MediatR;
using System.Reflection;

namespace ASPNET_Discord_Bot.HostedServices;

public class DiscordBotEventListener
{
    private readonly CancellationToken _cancellationToken;

    private readonly DiscordSocketClient _client;
    private readonly CommandService _commands;
    private readonly IServiceProvider _provider;
    private readonly IMediator _mediator;

    public DiscordBotEventListener(DiscordSocketClient client, CommandService commands, IServiceProvider provider, IMediator mediator)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _commands = commands ?? throw new ArgumentNullException(nameof(commands));
        _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        
        _cancellationToken = new CancellationTokenSource().Token;
    }


    public async Task StartAsync()
    {
        _client.Log += OnLogReceivedAsync;

        await _commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(), services: _provider);
        _client.MessageReceived += OnMessageReceivedAsync;
        _client.ChannelCreated += OnChannelCreatedAsync;

        await Task.CompletedTask;
    }

    private async Task OnChannelCreatedAsync(SocketChannel socketChannel)
        => await _mediator.Publish(new ChannelCreatedNotification(socketChannel), _cancellationToken);

    private async Task OnLogReceivedAsync(LogMessage arg)
        => await _mediator.Publish(new LogNotification(arg), _cancellationToken);

    private async Task OnMessageReceivedAsync(SocketMessage socketMessage)
        => await _mediator.Publish(new MessageReceivedNotification(socketMessage), _cancellationToken);
}