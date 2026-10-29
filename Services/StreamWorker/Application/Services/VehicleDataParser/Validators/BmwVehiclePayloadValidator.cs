using System.Text.Json;
using FluentValidation;

namespace StreamWorker.Application.Services.VehicleDataParser.Validators;

public class BmwVehiclePayloadValidator : AbstractValidator<JsonElement>, IVehicleDataParserValidator
{
    public string VehicleType => "bmw";

    public BmwVehiclePayloadValidator()
    {
        RuleFor(x => x)
            .Must(x => x.ValueKind == JsonValueKind.Object)
            .WithMessage("Payload bir JSON nesnesi olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("plate", out var p) && p.ValueKind == JsonValueKind.String && !string.IsNullOrWhiteSpace(p.GetString()))
            .WithMessage("'plate' boş olmayan bir metin olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("timestamp", out var p) && p.ValueKind == JsonValueKind.String && p.TryGetDateTime(out _))
            .WithMessage("'timestamp' geçerli bir tarih olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("location", out var p) && p.ValueKind == JsonValueKind.Object)
            .WithMessage("'location' bir JSON nesnesi olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("engine", out var p) && p.ValueKind == JsonValueKind.Object)
            .WithMessage("'engine' bir JSON nesnesi olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("diagnostics", out var p) && p.ValueKind == JsonValueKind.Object && p.TryGetProperty("dtcCodes", out var d) && d.ValueKind == JsonValueKind.Object)
            .WithMessage("'diagnostics.dtcCodes' bir JSON nesnesi olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("location", out var p) && p.TryGetProperty("latitude", out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetDecimal(out _))
            .WithMessage("'location.latitude' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("location", out var p) && p.TryGetProperty("longitude", out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetDecimal(out _))
            .WithMessage("'location.longitude' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("location", out var p) && p.TryGetProperty("speed", out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out _))
            .WithMessage("'location.speed' tam sayı olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("engine", out var p) && p.TryGetProperty("coolantTemp", out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out _))
            .WithMessage("'engine.coolantTemp' tam sayı olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("engine", out var p) && p.TryGetProperty("fuelLevelPercentage", out var v) && v.ValueKind == JsonValueKind.Number && v.TryGetInt32(out _))
            .WithMessage("'engine.fuelLevelPercentage' tam sayı olmalıdır.");
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