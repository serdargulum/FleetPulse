using FluentValidation;
using System.Text.Json;

namespace StreamWorker.Application.Services.VehicleDataParser;

public class VehicleDataParserValidator : AbstractValidator<VehicleDataParserRequest>
{
    public VehicleDataParserValidator()
    {
        RuleFor(x => x.VehicleType).NotEmpty();
        RuleFor(x => x.Json).Must(x => x.ValueKind == JsonValueKind.Object);
    }
}