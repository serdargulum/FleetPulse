using System.Text.Json;
using StreamWorker.Application.Interfaces;
using StreamWorker.Application.Services.VehicleDataParser;
using StreamWorker.Domain.Models;
using Microsoft.AspNetCore.SignalR;
using StreamWorker.Infrastructure.Realtime;
using RabbitMQ.Client.Exceptions;


namespace StreamWorker.Worker;

public class VehicleQueueConsumerWorker(
    IServiceScopeFactory scopeFactory,
    ILogger<VehicleQueueConsumerWorker> logger,
    IMessageConsumer messageConsumer) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("VehicleQueueConsumerWorker başlatıldı.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await messageConsumer.ConsumeAsync(async message =>
                {
                    using var scope = scopeFactory.CreateScope();

                    var processor = scope.ServiceProvider.GetRequiredService<IVehicleTelemetryProcessor>();
                    await processor.ProcessAsync(message, scope, stoppingToken);
                }
                , stoppingToken);

                break;
            }
            catch (BrokerUnreachableException ex)
            {
                logger.LogCritical(ex, "RabbitMQ'ya bağlanılamadı. 5 saniye sonra yeniden denenecek...");

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
            catch (OperationCanceledException)
            {
                logger.LogInformation("Kapanma sinyali alındı, mevcut iş güvenle bitiriliyor...");
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Kuyruk dinlenirken beklenmeyen bir hata oluştu. Yeniden başlatılıyor...");
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        logger.LogInformation("VehicleQueueConsumerWorker güvenli bir şekilde durduruldu.");
    }

}