using System.Text.Json;
using Microsoft.AspNetCore.SignalR;
using StreamWorker.Application.Interfaces;
using StreamWorker.Domain.Models;
using StreamWorker.Infrastructure.Realtime;

namespace StreamWorker.Application.Services;

public class VehicleTelemetryProcessor(
    IHubContext<TelemetryHub> hubContext,
    ILogger<VehicleTelemetryProcessor> logger)
    : IVehicleTelemetryProcessor
{
    public async Task ProcessAsync(string message, IServiceScope scope, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        logger.LogInformation("Kuyruktan mesaj alındı: {Message}", message);

        var json = JsonSerializer.Deserialize<JsonElement>(message);

        var vehicleData = await GenerateData(json, scope, cancellationToken);

        var telemetryRepository = scope.ServiceProvider.GetRequiredService<ITelemetryRepository>();
        await telemetryRepository.SaveTelemetryAndAlarmAsync(vehicleData, "active", cancellationToken);

        await SendSignal(vehicleData, cancellationToken);
    }

    private async Task<UnifiedTelemetryDto> GenerateData(
        JsonElement json,
        IServiceScope scope,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var plate = json.GetProperty("plate").GetString()
            ?? throw new InvalidOperationException("Payload içinde plate bulunamadı.");

        var vehicleRepository = scope.ServiceProvider.GetRequiredService<IVehicleRepository>();
        var vehicle = await vehicleRepository.GetByPlateAsync(plate, cancellationToken)
            ?? throw new InvalidOperationException($"Araç bulunamadı: {plate}");

        var parser = scope.ServiceProvider.GetRequiredService<StreamWorker.Application.Services.VehicleDataParser.VehicleDataParser>();

        return parser.Generate(vehicle.Brand, json);
    }

    private async Task SendSignal(UnifiedTelemetryDto vehicleData, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(vehicleData.Plate))
        {
            return;
        }

        var groupName = vehicleData.Plate.Trim().ToUpperInvariant();

        await hubContext.Clients.Group(groupName).SendAsync("TelemetryUpdated", vehicleData, cancellationToken);

        logger.LogInformation("SignalR TelemetryUpdated gönderildi. Plate: {Plate}", groupName);
    }
}