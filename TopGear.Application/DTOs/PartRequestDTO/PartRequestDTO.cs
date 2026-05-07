using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.PartRequestDTO;

public class PartRequestDTO
{
    public Guid PartRequestId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string PartName { get; set; } = string.Empty;
    public string? VehicleDetails { get; set; }
    public int Quantity { get; set; }
    public string? Notes { get; set; }
    public PartRequestStatus Status { get; set; }
    public string? AdminNotes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
