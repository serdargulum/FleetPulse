using System.Text.Json;
using StreamWorker.Application.Services.VehicleDataParser;
using StreamWorker.Application.Services.VehicleDataParser.Parsers;
using StreamWorker.Application.Services.VehicleDataParser.Validators;
using Xunit;
using StreamWorker.Domain.Models;

namespace StreamWorker.Tests;

public class VehicleDataParserTests
{
    [Theory]
    [InlineData("bmw")]
    [InlineData("BMW")]
    [InlineData("Honda")]
    [InlineData("mazda")]
    [InlineData("TOYOTA")]
    public void Generate_RegisteredVehicleType_ResolvesParser(string vehicleType)
    {
        var parser = new FakeParser(vehicleType);
        var validator = new FakeValidator(vehicleType);
        var sut = new VehicleDataParser([parser], [validator]);
        using var doc = JsonDocument.Parse("{}");

        sut.Generate(vehicleType, doc.RootElement);

        Assert.Equal(1, parser.CallCount);
    }

    [Fact]
    public void Generate_UnknownVehicleType_ThrowsInvalidOperationException()
    {
        var validator = new FakeValidator("bmw");
        var sut = new VehicleDataParser([], [validator]);
        using var doc = JsonDocument.Parse("{}");

        var exception = Assert.Throws<InvalidOperationException>(() => sut.Generate("unknown", doc.RootElement));

        Assert.Contains("unknown", exception.Message);
    }

    private sealed class FakeParser(string vehicleType) : IVehicleDataParser
    {
        public string VehicleType => vehicleType;
        public int CallCount { get; private set; }

        public UnifiedTelemetryDto Generate(JsonElement data)
        {
            CallCount++;
            return new UnifiedTelemetryDto();
        }
    }

    public class FakeValidator(string vehicleType) : IVehicleDataParserValidator
    {
        public string VehicleType => vehicleType;
        public void ValidatePayload(JsonElement json) { }
    }
}