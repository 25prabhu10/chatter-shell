using ChatterShell.UI;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

// Create a temporary logger to log any exceptions that occur during startup
// This is necessary because the logger is not yet configured
var logger = Log.Logger = new LoggerConfiguration().WriteTo.Console()
                                                           .CreateBootstrapLogger();

try
{
    logger.Debug("Starting application...");

    // Create a host builder
    var builder = Host.CreateApplicationBuilder(args);

    // Add Serilog to the host builder
    builder.Services.AddSerilog((_, lc) => lc.ReadFrom.Configuration(builder.Configuration));

    // Add services to the host builder
    builder.Services.AddSingleton<ChatApplication>();
    builder.Services.AddHostedService<ChatterShellService>();

    // Build the host
    using var app = builder.Build();

    // Run the host
    await app.RunAsync();

    logger.Debug("Application stopped!");
}
catch (Exception ex)
{
    logger.Fatal(ex, "An unhandled exception occurred during application startup");
    throw;
}
finally
{
    await Log.CloseAndFlushAsync();
}