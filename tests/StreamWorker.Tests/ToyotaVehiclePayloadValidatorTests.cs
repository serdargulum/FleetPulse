using System.Text.Json;
using FluentValidation;
using StreamWorker.Application.Services.VehicleDataParser.Validators;
using Xunit;

namespace StreamWorker.Tests.Application.Services.VehicleDataParser.Validators;

public class ToyotaVehiclePayloadValidatorTests
{
    private readonly ToyotaVehiclePayloadValidator _validator = new();

    [Fact]
    public void ValidatePayload_ValidPayload_DoesNotThrow()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "latitude": 41.0082,
            "longitude": 28.9784,
            "speed": 80.5,
            "coolantTemp": 90.5,
            "fuelLevelPercentage": 65.2,
            "timestamp": "2026-10-17T08:00:00Z",
            "dtcCodes": {
                "P0420": "Catalyst System Efficiency"
            }
        }
        """);

        var exception = Record.Exception(
            () => _validator.ValidatePayload(document.RootElement));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidatePayload_MissingPlate_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("""
        {
            "latitude": 41.0,
            "longitude": 29.0,
            "speed": 80.0,
            "coolantTemp": 90.0,
            "fuelLevelPercentage": 65.0,
            "timestamp": "2026-10-17T08:00:00Z",
            "dtcCodes": {}
        }
        """);

        var exception = Assert.Throws<ValidationException>(
            () => _validator.ValidatePayload(document.RootElement));

        Assert.Contains(exception.Errors,
            e => e.ErrorMessage == "'plate' boş olmayan bir metin olmalıdır.");
    }

    [Fact]
    public void ValidatePayload_InvalidLatitudeType_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "latitude": "invalid",
            "longitude": 29.0,
            "speed": 80.0,
            "coolantTemp": 90.0,
            "fuelLevelPercentage": 65.0,
            "timestamp": "2026-10-17T08:00:00Z",
            "dtcCodes": {}
        }
        """);

        var exception = Assert.Throws<ValidationException>(
            () => _validator.ValidatePayload(document.RootElement));

        Assert.Contains(exception.Errors,
            e => e.ErrorMessage == "'latitude' sayısal olmalıdır.");
    }

    [Fact]
    public void ValidatePayload_InvalidTimestamp_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "latitude": 41.0,
            "longitude": 29.0,
            "speed": 80.0,
            "coolantTemp": 90.0,
            "fuelLevelPercentage": 65.0,
            "timestamp": "not-a-date",
            "dtcCodes": {}
        }
        """);

        var exception = Assert.Throws<ValidationException>(
            () => _validator.ValidatePayload(document.RootElement));

        Assert.Contains(exception.Errors,
            e => e.ErrorMessage == "'timestamp' geçerli bir tarih olmalıdır.");
    }

    [Fact]
    public void ValidatePayload_InvalidDtcCodes_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "latitude": 41.0,
            "longitude": 29.0,
            "speed": 80.0,
            "coolantTemp": 90.0,
            "fuelLevelPercentage": 65.0,
            "timestamp": "2026-10-17T08:00:00Z",
            "dtcCodes": []
        }
        """);

        var exception = Assert.Throws<ValidationException>(
            () => _validator.ValidatePayload(document.RootElement));

        Assert.Contains(exception.Errors,
            e => e.ErrorMessage == "'dtcCodes' bir JSON nesnesi olmalıdır.");
    }
}