namespace AdminApi.Application.Queries.GetDashboard;

public class VehiclePositionDto
{
    public string Plate { get; set; } = string.Empty;
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
}