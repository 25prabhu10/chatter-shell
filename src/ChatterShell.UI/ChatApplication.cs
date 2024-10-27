using Microsoft.Extensions.Logging;

namespace ChatterShell.UI;

/// <summary>
/// Represents the chat application.
/// </summary>
public class ChatApplication(ILogger<ChatApplication> logger)
{
    private readonly ILogger<ChatApplication> _logger = logger;

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting chat application...");
        await Console.Out.WriteLineAsync("Welcome to ChatterShell!");
    }
}