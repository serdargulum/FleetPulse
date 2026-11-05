using Mediator;

namespace AdminApi.Application.Queries.GetDashboard;

public sealed record GetDashboardQuery : IRequest<DashboardDto>;