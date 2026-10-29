using System.Text.Json;
using StreamWorker.Application.Services.VehicleDataParser.Validators;
using Xunit;

public class BmwVehiclePayloadValidatorTests
{
    [Fact]
    public void ValidatePayload_ValidJson_DoesNotThrow()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "location": {
                "latitude": 41.0082,
                "longitude": 28.9784,
                "speed": 80
            },
            "engine": {
                "coolantTemp": 90,
                "fuelLevelPercentage": 60
            },
            "timestamp": "2026-10-16T10:00:00",
            "diagnostics": {
                "dtcCodes": {
                    "P0300": "Engine Misfire"
                }
            }
        }
        """);

        var validator = new BmwVehiclePayloadValidator();

        validator.ValidatePayload(document.RootElement);
    }

    [Fact]
    public void ValidatePayload_MissingPlate_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("""
        {
            "location": {
                "latitude": 41.0082,
                "longitude": 28.9784,
                "speed": 80
            },
            "engine": {
                "coolantTemp": 90,
                "fuelLevelPercentage": 60
            },
            "timestamp": "2026-10-16T10:00:00",
            "diagnostics": {
                "dtcCodes": {}
            }
        }
        """);

        var validator = new BmwVehiclePayloadValidator();

        Assert.Throws<FluentValidation.ValidationException>(() => validator.ValidatePayload(document.RootElement));
    }

    [Fact]
    public void ValidatePayload_InvalidLatitude_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "location": {
                "latitude": "invalid",
                "longitude": 28.9784,
                "speed": 80
            },
            "engine": {
                "coolantTemp": 90,
                "fuelLevelPercentage": 60
            },
            "timestamp": "2026-10-16T10:00:00",
            "diagnostics": {
                "dtcCodes": {}
            }
        }
        """);

        var validator = new BmwVehiclePayloadValidator();

        Assert.Throws<FluentValidation.ValidationException>(() => validator.ValidatePayload(document.RootElement));
    }
}