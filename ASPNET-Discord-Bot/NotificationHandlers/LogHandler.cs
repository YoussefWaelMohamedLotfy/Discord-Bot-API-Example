using ASPNET_Discord_Bot.Notifications;
using Discord;
using MediatR;

namespace ASPNET_Discord_Bot.NotificationHandlers;

public class LogHandler : INotificationHandler<LogNotification>
{
    private readonly ILogger<LogHandler> _logger;

    public LogHandler(ILogger<LogHandler> logger)
        => _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task Handle(LogNotification notification, CancellationToken cancellationToken)
    {
        var severity = notification.LogMessage.Severity switch
        {
            LogSeverity.Critical => LogLevel.Critical,
            LogSeverity.Error => LogLevel.Error,
            LogSeverity.Warning => LogLevel.Warning,
            LogSeverity.Info => LogLevel.Information,
            LogSeverity.Verbose => LogLevel.Trace,
            LogSeverity.Debug => LogLevel.Debug,
            _ => LogLevel.Information
        };

        _logger.Log(severity, notification.LogMessage.Exception, "[{Source}] {Message}", notification.LogMessage.Source, notification.LogMessage.Message);

        await Task.CompletedTask;
    }
}
