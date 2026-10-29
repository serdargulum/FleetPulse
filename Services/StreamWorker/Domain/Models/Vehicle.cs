namespace StreamWorker.Domain.Models;

public class Vehicle
{
    public Guid Id { get; set; }
    public string Plate { get; set; } = null!;
    public string Brand { get; set; } = null!;
}