using System.Text.Json;
using FluentValidation;
using StreamWorker.Application.Services.VehicleDataParser.Validators;
using Xunit;

namespace StreamWorker.Tests.Application.Services.VehicleDataParser.Validators;

public class MazdaVehiclePayloadValidatorTests
{
    private readonly MazdaVehiclePayloadValidator _validator = new();

    [Fact]
    public void ValidatePayload_ValidPayload_DoesNotThrow()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "recordedAt": "2026-10-17T08:00:00Z",
            "position": {
                "lat": 41.0082,
                "lng": 28.9784,
                "kmh": 80.5
            },
            "motor": {
                "tempC": 90.5,
                "fuelPct": 65.2
            },
            "alerts": {
                "errorCodes": {
                    "P0420": "Catalyst System Efficiency"
                }
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
            "recordedAt": "2026-10-17T08:00:00Z",
            "position": { "lat": 41.0, "lng": 29.0, "kmh": 80.0 },
            "motor": { "tempC": 90.0, "fuelPct": 65.0 },
            "alerts": { "errorCodes": {} }
        }
        """);

        var exception = Assert.Throws<ValidationException>(
            () => _validator.ValidatePayload(document.RootElement));

        Assert.Contains(exception.Errors,
            e => e.ErrorMessage == "'plate' boş olmayan bir metin olmalıdır.");
    }

    [Fact]
    public void ValidatePayload_InvalidRecordedAt_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "recordedAt": "not-a-date",
            "position": { "lat": 41.0, "lng": 29.0, "kmh": 80.0 },
            "motor": { "tempC": 90.0, "fuelPct": 65.0 },
            "alerts": { "errorCodes": {} }
        }
        """);

        var exception = Assert.Throws<ValidationException>(
            () => _validator.ValidatePayload(document.RootElement));

        Assert.Contains(exception.Errors,
            e => e.ErrorMessage == "'recordedAt' geçerli bir tarih olmalıdır.");
    }

    [Fact]
    public void ValidatePayload_InvalidLatitudeType_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "recordedAt": "2026-10-17T08:00:00Z",
            "position": { "lat": "invalid", "lng": 29.0, "kmh": 80.0 },
            "motor": { "tempC": 90.0, "fuelPct": 65.0 },
            "alerts": { "errorCodes": {} }
        }
        """);

        var exception = Assert.Throws<ValidationException>(
            () => _validator.ValidatePayload(document.RootElement));

        Assert.Contains(exception.Errors,
            e => e.ErrorMessage == "'position.lat' sayısal olmalıdır.");
    }

    [Fact]
    public void ValidatePayload_MissingPosition_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "recordedAt": "2026-10-17T08:00:00Z",
            "motor": { "tempC": 90.0, "fuelPct": 65.0 },
            "alerts": { "errorCodes": {} }
        }
        """);

        var exception = Assert.Throws<ValidationException>(
            () => _validator.ValidatePayload(document.RootElement));

        Assert.Contains(exception.Errors,
            e => e.ErrorMessage == "'position' bir JSON nesnesi olmalıdır.");
    }

    [Fact]
    public void ValidatePayload_InvalidErrorCodes_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "recordedAt": "2026-10-17T08:00:00Z",
            "position": { "lat": 41.0, "lng": 29.0, "kmh": 80.0 },
            "motor": { "tempC": 90.0, "fuelPct": 65.0 },
            "alerts": { "errorCodes": [] }
        }
        """);

        var exception = Assert.Throws<ValidationException>(
            () => _validator.ValidatePayload(document.RootElement));

        Assert.Contains(exception.Errors,
            e => e.ErrorMessage == "'alerts.errorCodes' bir JSON nesnesi olmalıdır.");
    }
}