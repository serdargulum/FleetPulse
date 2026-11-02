using VehicleDataSimulator.Application.Services;

namespace VehicleDataSimulator.Worker;

public class VehicleDataWorker(ILogger<VehicleDataWorker> logger, IServiceProvider serviceProvider) : BackgroundService
{

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("VehicleDataWorker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = serviceProvider.CreateScope();
                logger.LogInformation("Kuyruktan iş alınıyor ve işleniyor...");

                var vehicleService = scope.ServiceProvider.GetRequiredService<VehicleDataGeneratorService>();
                await vehicleService.ProcessVehicleTickAsync(stoppingToken);

            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Kapanma sinyali alındı, mevcut iş güvenle bitiriliyor...");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "VehicleDataWorker çalışırken bir hata oluştu.");
                try
                {
                    // Bekleme süresi local ortam gözetilerek fazla tutuldu
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(0.05), stoppingToken);
        }

        logger.LogInformation("Servis güvenli bir şekilde durduruldu.");
    }
}