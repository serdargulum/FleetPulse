using System.Text.Json;
using System.Text.Json.Nodes;

namespace VehicleGateway.Application.Commands.StoreVehicleData;

public static class VehicleDataValidator
{
    private const int MaxPayloadBytes = 64 * 1024;

    public static (bool IsValid, JsonObject? Payload, string? Error) Validate(JsonElement data)
    {
        if (data.ValueKind != JsonValueKind.Object)
        {
            return (false, null, "Veri bir JSON nesnesi olmalıdır.");
        }

        var raw = data.GetRawText();
        // UTF-8 byte olarak boyutu ölç
        if (System.Text.Encoding.UTF8.GetByteCount(raw) > MaxPayloadBytes)
        {
            return (false, null, "Veri çok büyük.");
        }

        var obj = JsonNode.Parse(raw)!.AsObject();

        if (obj.Count == 0)
        {
            return (false, null, "Veri boş olamaz.");
        }

        foreach (var key in obj.Select(p => p.Key))
        {
            if (key.Equals("MessageId", StringComparison.OrdinalIgnoreCase) ||
                key.Equals("ReceivedAt", StringComparison.OrdinalIgnoreCase))
            {
                return (false, null, $"'{key}' alanı rezervedir.");
            }
        }

        return (true, obj, null);
    }
}