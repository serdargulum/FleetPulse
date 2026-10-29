using System.Text.Json;
using FluentValidation;
using StreamWorker.Application.Services.VehicleDataParser.Validators;
using Xunit;

namespace StreamWorker.Tests.Application.Services.VehicleDataParser.Validators;

public class HondaVehiclePayloadValidatorTests
{
    private readonly HondaVehiclePayloadValidator _validator = new();

    [Fact]
    public void ValidatePayload_ValidPayload_DoesNotThrow()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "event_time": 1792200000,
            "gps": {
                "lat": 41.0082,
                "lon": 28.9784,
                "velocity": 80.5
            },
            "powertrain": {
                "temperature": 90.5,
                "fuel": 65.2
            },
            "faults": {
                "codes": {
                    "P0420": "Catalyst System Efficiency"
                }
            }
        }
        """);

        var exception = Record.Exception(() => _validator.ValidatePayload(document.RootElement));

        Assert.Null(exception);
    }

    [Fact]
    public void ValidatePayload_MissingPlate_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("""
        {
            "event_time": 1792200000,
            "gps": { "lat": 41.0, "lon": 29.0, "velocity": 80.0 },
            "powertrain": { "temperature": 90.0, "fuel": 65.0 },
            "faults": { "codes": {} }
        }
        """);

        var exception = Assert.Throws<ValidationException>(() => _validator.ValidatePayload(document.RootElement));

        Assert.Contains(exception.Errors, e => e.ErrorMessage == "'plate' boş olmayan bir metin olmalıdır.");
    }

    [Fact]
    public void ValidatePayload_InvalidLatitudeType_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "event_time": 1792200000,
            "gps": { "lat": "invalid", "lon": 29.0, "velocity": 80.0 },
            "powertrain": { "temperature": 90.0, "fuel": 65.0 },
            "faults": { "codes": {} }
        }
        """);

        var exception = Assert.Throws<ValidationException>(() => _validator.ValidatePayload(document.RootElement));

        Assert.Contains(exception.Errors, e => e.ErrorMessage == "'gps.lat' sayısal olmalıdır.");
    }

    [Fact]
    public void ValidatePayload_MissingGps_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "event_time": 1792200000,
            "powertrain": { "temperature": 90.0, "fuel": 65.0 },
            "faults": { "codes": {} }
        }
        """);

        var exception = Assert.Throws<ValidationException>(() => _validator.ValidatePayload(document.RootElement));

        Assert.Contains(exception.Errors, e => e.ErrorMessage == "'gps' bir JSON nesnesi olmalıdır.");
    }

    [Fact]
    public void ValidatePayload_InvalidEventTime_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "event_time": "not-a-number",
            "gps": { "lat": 41.0, "lon": 29.0, "velocity": 80.0 },
            "powertrain": { "temperature": 90.0, "fuel": 65.0 },
            "faults": { "codes": {} }
        }
        """);

        var exception = Assert.Throws<ValidationException>(() => _validator.ValidatePayload(document.RootElement));

        Assert.Contains(exception.Errors,
            e => e.ErrorMessage == "'event_time' tam sayı olmalıdır.");
    }

    [Fact]
    public void ValidatePayload_InvalidFaultCodes_ThrowsValidationException()
    {
        using var document = JsonDocument.Parse("""
        {
            "plate": "34ABC123",
            "event_time": 1792200000,
            "gps": { "lat": 41.0, "lon": 29.0, "velocity": 80.0 },
            "powertrain": { "temperature": 90.0, "fuel": 65.0 },
            "faults": { "codes": [] }
        }
        """);

        var exception = Assert.Throws<ValidationException>(
            () => _validator.ValidatePayload(document.RootElement));

        Assert.Contains(exception.Errors, e => e.ErrorMessage == "'faults.codes' bir JSON nesnesi olmalıdır.");
    }
}