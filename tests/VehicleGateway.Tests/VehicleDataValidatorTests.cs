using System.Text.Json;
using VehicleGateway.Application.Commands.StoreVehicleData;
using Xunit;

namespace VehicleGateway.Tests;

public class VehicleDataValidatorTests
{
    [Fact]
    public void Validate_ValidObject_ReturnsValid()
    {
        using var document = JsonDocument.Parse("""{"Plate":"34ABC123"}""");

        var result = VehicleDataValidator.Validate(document.RootElement);

        Assert.True(result.IsValid);
        Assert.NotNull(result.Payload);
        Assert.Null(result.Error);
    }

    [Fact]
    public void Validate_EmptyObject_ReturnsInvalid()
    {
        using var document = JsonDocument.Parse("{}");

        var result = VehicleDataValidator.Validate(document.RootElement);

        Assert.False(result.IsValid);
        Assert.Equal("Veri boş olamaz.", result.Error);
    }

    [Theory]
    [InlineData("null")]
    [InlineData("123")]
    [InlineData("\"text\"")]
    [InlineData("[1,2,3]")]
    public void Validate_NonObjectJson_ReturnsInvalid(string json)
    {
        using var document = JsonDocument.Parse(json);

        var result = VehicleDataValidator.Validate(document.RootElement);

        Assert.False(result.IsValid);
        Assert.Equal("Veri bir JSON nesnesi olmalıdır.", result.Error);
    }

    [Theory]
    [InlineData("MessageId")]
    [InlineData("ReceivedAt")]
    [InlineData("messageid")]
    [InlineData("receivedat")]
    public void Validate_ReservedProperty_ReturnsInvalid(string property)
    {
        using var document = JsonDocument.Parse(
            JsonSerializer.Serialize(new Dictionary<string, string>
            {
                [property] = "test"
            }));

        var result = VehicleDataValidator.Validate(document.RootElement);

        Assert.False(result.IsValid);
        Assert.Contains("rezervedir", result.Error);
    }

    [Fact]
    public void Validate_OversizedPayload_ReturnsInvalid()
    {
        var json = JsonSerializer.Serialize(new
        {
            Data = new string('a', 65 * 1024)
        });

        using var document = JsonDocument.Parse(json);

        var result = VehicleDataValidator.Validate(document.RootElement);

        Assert.False(result.IsValid);
        Assert.Equal("Veri çok büyük.", result.Error);
    }

    [Fact]
    public void Validate_PayloadAtMaxSize_ReturnsValid()
    {
        var json = "{\"data\":\"" + new string('a', 65525) + "\"}";
        using var document = JsonDocument.Parse(json);

        Assert.Equal(65536, System.Text.Encoding.UTF8.GetByteCount(json));

        var result = VehicleDataValidator.Validate(document.RootElement);

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_PayloadExceedingMaxSize_ReturnsInvalid()
    {
        var json = "{\"data\":\"" + new string('a', 65526) + "\"}";
        using var document = JsonDocument.Parse(json);

        Assert.Equal(65537, System.Text.Encoding.UTF8.GetByteCount(json));

        var result = VehicleDataValidator.Validate(document.RootElement);

        Assert.False(result.IsValid);
        Assert.Equal("Veri çok büyük.", result.Error);
    }
}