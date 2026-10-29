using System.Net.Http.Json;
using VehicleDataSimulator.Application.Interfaces;

namespace VehicleDataSimulator.Infrastructure.ExternalServices;

public class VehicleApiClient(HttpClient httpClient, ILogger<VehicleApiClient> logger) : IVehicleApiClient
{

    public async Task<bool> SendDataAsync(object data, string plate, CancellationToken cancellationToken)
    {
        try
        {
            var response = await httpClient.PostAsJsonAsync("api/vehicle-data/" + plate, data, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            logger.LogWarning("API returned non-success status: {StatusCode}", response.StatusCode);

        }
        catch (HttpRequestException ex)
        {
            logger.LogWarning(ex, "Vehicle Gateway servisine ulaşılamadı.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception occurred while calling vehicle data API.");
        }
        return false;
    }
}