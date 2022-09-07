using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ExampleService : IHostedService
{
    private readonly ILogger<ExampleService> logger;

    public ExampleService(ILogger<ExampleService> logger)
    {
        this.logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting example worker service...");
        logger.LogCritical("THIS IS A CRITICAL PROBLEM");
        logger.LogError("This went wrong but somebody probably tried again and it worked!");
        logger.LogWarning("Something went wrong but nobody noticed");
        logger.LogInformation("Started the pricing client!");
        logger.LogDebug("This is really detailed debugging information");
        logger.LogTrace("This is even more detailed debugging information");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Stopping example worker service...");
        return Task.CompletedTask;
    }
}