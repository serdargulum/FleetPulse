using Microsoft.AspNetCore.SignalR;

namespace StreamWorker.Infrastructure.Realtime;

public class TelemetryHub(ILogger<TelemetryHub> logger) : Hub
{
    public override async Task OnConnectedAsync()
    {
        logger.LogInformation("SignalR connected: {ConnectionId}", Context.ConnectionId);

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        logger.LogInformation("SignalR disconnected: {ConnectionId}", Context.ConnectionId);

        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinVehicle(string plate)
    {
        var groupName = plate.Trim().ToUpperInvariant();

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        logger.LogInformation("Connection {ConnectionId} joined vehicle {Plate}", Context.ConnectionId, groupName);
    }

    public async Task LeaveVehicle(Guid vehicleId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, vehicleId.ToString());

        logger.LogInformation(
            "Connection {ConnectionId} left vehicle {VehicleId}",
            Context.ConnectionId,
            vehicleId);
    }
}