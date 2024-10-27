using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ChatterShell.UI;

/// <summary>
/// A hosted service that runs the chat application.
/// </summary>
internal sealed class ChatterShellService(ILogger<ChatterShellService> logger, IHostApplicationLifetime appLifetime, ChatApplication chatApplication) : IHostedService
{
    // The logger, application lifetime, and chat application are injected by the DI container
    private readonly ILogger<ChatterShellService> _logger = logger;
    private readonly IHostApplicationLifetime _appLifetime = appLifetime;
    private readonly ChatApplication _chatApplication = chatApplication;

    // The exit code to use when the application stops
    private int? _exitCode;

    /// <summary>
    /// Starts the chat application when the host starts.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // Register a task to run when the application starts
        _appLifetime.ApplicationStarted.Register(() => Task.Run(async () =>
        {
            try
            {
                _logger.LogDebug("Starting service...");
                await _chatApplication.RunAsync(cancellationToken);
                _exitCode = 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred during service startup");
                _exitCode = 1;
                throw;
            }
            finally
            {
                _appLifetime.StopApplication();
            }
        }));

        return Task.CompletedTask;
    }

    /// <summary>
    /// Stops the chat application when the host stops.
    /// </summary>
    /// <param name="cancellationToken">A token that can be used to cancel the operation.</param>
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogDebug("Stopping service...");
        // Exit code may be null if the user cancelled via Ctrl+C/SIGTERM
        Environment.ExitCode = _exitCode.GetValueOrDefault(-1);
        return Task.CompletedTask;
    }
}