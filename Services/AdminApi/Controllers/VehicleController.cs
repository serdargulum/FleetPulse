
using AdminApi.Application.Queries;
using AdminApi.Application.Queries.GetVehicle;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AdminApi.Controllers;

[ApiController]
[Route("api/vehicles")]
[Authorize]
[EnableRateLimiting("fixed")]
public class VehicleController(IMediator mediator) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<VehicleDto>> Get(
        Guid id,
        CancellationToken cancellationToken)
    {
        var vehicle = await mediator.Send(new GetVehicleQuery(id), cancellationToken);

        if (vehicle is null)
        {
            return NotFound();
        }

        return Ok(vehicle);
    }
}
