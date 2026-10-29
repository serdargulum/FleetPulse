using System.Text.Json;
using FluentValidation;

namespace StreamWorker.Application.Services.VehicleDataParser.Validators;

public class HondaVehiclePayloadValidator : AbstractValidator<JsonElement>, IVehicleDataParserValidator
{
    public string VehicleType => "honda";

    public HondaVehiclePayloadValidator()
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
            .Must(x => x.TryGetProperty("event_time", out var p) &&
                       p.ValueKind == JsonValueKind.Number &&
                       p.TryGetInt64(out _))
            .WithMessage("'event_time' tam sayı olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("gps", out var p) &&
                       p.ValueKind == JsonValueKind.Object)
            .WithMessage("'gps' bir JSON nesnesi olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("gps", out var p) &&
                       p.ValueKind == JsonValueKind.Object &&
                       p.TryGetProperty("lat", out var v) &&
                       v.ValueKind == JsonValueKind.Number &&
                       v.TryGetDecimal(out _))
            .WithMessage("'gps.lat' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("gps", out var p) &&
                       p.ValueKind == JsonValueKind.Object &&
                       p.TryGetProperty("lon", out var v) &&
                       v.ValueKind == JsonValueKind.Number &&
                       v.TryGetDecimal(out _))
            .WithMessage("'gps.lon' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("gps", out var p) &&
                       p.ValueKind == JsonValueKind.Object &&
                       p.TryGetProperty("velocity", out var v) &&
                       v.ValueKind == JsonValueKind.Number &&
                       v.TryGetDecimal(out _))
            .WithMessage("'gps.velocity' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("powertrain", out var p) &&
                       p.ValueKind == JsonValueKind.Object)
            .WithMessage("'powertrain' bir JSON nesnesi olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("powertrain", out var p) &&
                       p.ValueKind == JsonValueKind.Object &&
                       p.TryGetProperty("temperature", out var v) &&
                       v.ValueKind == JsonValueKind.Number &&
                       v.TryGetDecimal(out _))
            .WithMessage("'powertrain.temperature' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("powertrain", out var p) &&
                       p.ValueKind == JsonValueKind.Object &&
                       p.TryGetProperty("fuel", out var v) &&
                       v.ValueKind == JsonValueKind.Number &&
                       v.TryGetDouble(out _))
            .WithMessage("'powertrain.fuel' sayısal olmalıdır.");

        RuleFor(x => x)
            .Must(x => x.TryGetProperty("faults", out var p) &&
                       p.ValueKind == JsonValueKind.Object &&
                       p.TryGetProperty("codes", out var codes) &&
                       codes.ValueKind == JsonValueKind.Object)
            .WithMessage("'faults.codes' bir JSON nesnesi olmalıdır.");
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