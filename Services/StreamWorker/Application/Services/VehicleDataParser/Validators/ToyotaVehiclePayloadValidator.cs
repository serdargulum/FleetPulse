using System.Text.Json;
using FluentValidation;

namespace StreamWorker.Application.Services.VehicleDataParser.Validators;

public class ToyotaVehiclePayloadValidator : AbstractValidator<JsonElement>, IVehicleDataParserValidator
{
    public string VehicleType => "toyota";

    public ToyotaVehiclePayloadValidator()
    {
        RuleFor(x => x)
            .Must(x => x.ValueKind == JsonValueKind.Object)
            .WithMessage("Payload bir JSON nesnesi olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("plate", out var p) &&
                       p.ValueKind == JsonValueKind.String &&
                       !string.IsNullOrWhiteSpace(p.GetString()))
            .WithMessage("'plate' boş olmayan bir metin olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("latitude", out var p) &&
                       p.ValueKind == JsonValueKind.Number &&
                       p.TryGetDecimal(out _))
            .WithMessage("'latitude' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("longitude", out var p) &&
                       p.ValueKind == JsonValueKind.Number &&
                       p.TryGetDecimal(out _))
            .WithMessage("'longitude' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("speed", out var p) &&
                       p.ValueKind == JsonValueKind.Number &&
                       p.TryGetDecimal(out _))
            .WithMessage("'speed' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("coolantTemp", out var p) &&
                       p.ValueKind == JsonValueKind.Number &&
                       p.TryGetDecimal(out _))
            .WithMessage("'coolantTemp' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("fuelLevelPercentage", out var p) &&
                       p.ValueKind == JsonValueKind.Number &&
                       p.TryGetDouble(out _))
            .WithMessage("'fuelLevelPercentage' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("timestamp", out var p) &&
                       p.ValueKind == JsonValueKind.String &&
                       p.TryGetDateTime(out _))
            .WithMessage("'timestamp' geçerli bir tarih olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("dtcCodes", out var p) &&
                       p.ValueKind == JsonValueKind.Object)
            .WithMessage("'dtcCodes' bir JSON nesnesi olmalıdır.");
    }

    public void ValidatePayload(JsonElement json)
    {
        var result = Validate(json);

        if (!result.IsValid)
        {
            throw new ValidationException(result.Errors);
        }
    }
}