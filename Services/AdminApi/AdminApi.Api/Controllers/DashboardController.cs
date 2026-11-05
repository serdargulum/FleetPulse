
using AdminApi.Application.Queries.GetDashboard;
using Mediator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AdminApi.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
[EnableRateLimiting("fixed")]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardDto>> Get(
        CancellationToken cancellationToken)
    {
        var data = await mediator.Send(new GetDashboardQuery(), cancellationToken);

        return Ok(data);
    }
}
