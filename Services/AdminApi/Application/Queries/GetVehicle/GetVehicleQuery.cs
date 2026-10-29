using Mediator;

namespace AdminApi.Application.Queries.GetVehicle;

public sealed record GetVehicleQuery(Guid Id) : IRequest<VehicleDto?>;