using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;
using VehicleGateway.Application.Interfaces;

namespace VehicleGateway.Application.Commands.StoreVehicleData;

public class StoreVehicleDataHandler(IMessageProducer messageProducer)
{
    public async Task Handle(StoreVehicleDataCommand command, CancellationToken cancellationToken)
    {
        var (isValid, payload, error) = VehicleDataValidator.Validate(command.Data);

        if (!isValid)
        {
            throw new ValidationException(error!);
        }

        var obj = JsonNode.Parse(command.Data.GetRawText())!.AsObject();

        obj["ReceivedAt"] = DateTimeOffset.UtcNow;

        await messageProducer.PublishAsync(obj, cancellationToken);
    }
}