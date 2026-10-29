namespace StreamWorker.Application.Interfaces;

public interface IVehicleTelemetryProcessor
{
    Task ProcessAsync(string message, IServiceScope scope, CancellationToken cancellationToken);
}