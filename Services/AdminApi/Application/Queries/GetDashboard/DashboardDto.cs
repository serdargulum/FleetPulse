namespace AdminApi.Application.Queries.GetDashboard;

public class DashboardDto
{
    public DashboardSummaryDto Summary { get; set; } = new();
    public List<DashboardAlertDto> Alerts { get; set; } = [];
    public List<DashboardVehicleDto> Vehicles { get; set; } = [];
    public List<VehiclePositionDto> Positions { get; init; } = [];
}

public class DashboardSummaryDto
{
    public int TotalVehicles { get; set; }
    public int ActiveVehicles { get; set; }
    public int ActiveAlerts { get; set; }
}

public class DailyStatDto
{
    public string Day { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class DashboardAlertDto
{
    public Guid VehicleId { get; set; }
    public string Plate { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string AlertType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class DashboardVehicleDto
{
    public Guid Id { get; set; }
    public string Plate { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public bool IsActive { get; set; } = false;
    public decimal Speed { get; set; }
    public decimal EngineTemperature { get; set; }
    public decimal FuelLevel { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}
