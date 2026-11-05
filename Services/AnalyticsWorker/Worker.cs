namespace AnalyticsWorker;

/// <summary>
/// Analyzes vehicle telemetry using AI-driven processes to detect inconsistencies
/// in fuel consumption and vehicle movement patterns.
/// The commercial analytics implementation is intentionally kept private.
/// </summary>
public class Worker(ILogger<Worker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(10000, stoppingToken);
        }
    }
}