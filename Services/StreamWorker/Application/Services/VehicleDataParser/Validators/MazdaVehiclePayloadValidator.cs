using System.Text.Json;
using FluentValidation;

namespace StreamWorker.Application.Services.VehicleDataParser.Validators;

public class MazdaVehiclePayloadValidator : AbstractValidator<JsonElement>, IVehicleDataParserValidator
{
    public string VehicleType => "mazda";

    public MazdaVehiclePayloadValidator()
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
            .Must(x => x.TryGetProperty("recordedAt", out var p) &&
                       p.ValueKind == JsonValueKind.String &&
                       DateTime.TryParse(p.GetString(), out _))
            .WithMessage("'recordedAt' geçerli bir tarih olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("position", out var p) &&
                       p.ValueKind == JsonValueKind.Object)
            .WithMessage("'position' bir JSON nesnesi olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("position", out var p) &&
                       p.ValueKind == JsonValueKind.Object &&
                       p.TryGetProperty("lat", out var v) &&
                       v.ValueKind == JsonValueKind.Number &&
                       v.TryGetDecimal(out _))
            .WithMessage("'position.lat' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("position", out var p) &&
                       p.ValueKind == JsonValueKind.Object &&
                       p.TryGetProperty("lng", out var v) &&
                       v.ValueKind == JsonValueKind.Number &&
                       v.TryGetDecimal(out _))
            .WithMessage("'position.lng' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("position", out var p) &&
                       p.ValueKind == JsonValueKind.Object &&
                       p.TryGetProperty("kmh", out var v) &&
                       v.ValueKind == JsonValueKind.Number &&
                       v.TryGetDecimal(out _))
            .WithMessage("'position.kmh' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("motor", out var p) &&
                       p.ValueKind == JsonValueKind.Object)
            .WithMessage("'motor' bir JSON nesnesi olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("motor", out var p) &&
                       p.ValueKind == JsonValueKind.Object &&
                       p.TryGetProperty("tempC", out var v) &&
                       v.ValueKind == JsonValueKind.Number &&
                       v.TryGetDecimal(out _))
            .WithMessage("'motor.tempC' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("motor", out var p) &&
                       p.ValueKind == JsonValueKind.Object &&
                       p.TryGetProperty("fuelPct", out var v) &&
                       v.ValueKind == JsonValueKind.Number &&
                       v.TryGetDouble(out _))
            .WithMessage("'motor.fuelPct' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("alerts", out var p) &&
                       p.ValueKind == JsonValueKind.Object &&
                       p.TryGetProperty("errorCodes", out var codes) &&
                       codes.ValueKind == JsonValueKind.Object)
            .WithMessage("'alerts.errorCodes' bir JSON nesnesi olmalıdır.");
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