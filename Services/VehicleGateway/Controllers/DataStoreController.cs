using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VehicleGateway.Application.Commands.StoreVehicleData;

namespace VehicleGateway.Controllers;

public class DataStoreController
{
    [EnableRateLimiting("VehiclePlatePolicy")]
    [HttpPost("{plate}")]
    public static async Task<IResult> Store(string plate, JsonElement data, StoreVehicleDataHandler handler, CancellationToken cancellationToken)
    {
        Console.WriteLine(9999999999);
        await handler.Handle(new StoreVehicleDataCommand(data), cancellationToken);

        return Results.Ok(new
        {
            success = true,
            message = "Veri başarıyla kuyruğa gönderildi."
        });
    }
}