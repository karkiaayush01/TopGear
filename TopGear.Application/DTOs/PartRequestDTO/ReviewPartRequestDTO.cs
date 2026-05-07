using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.PartRequestDTO;

public class ReviewPartRequestDTO
{
    public PartRequestStatus Status { get; set; }
    public string? AdminNotes { get; set; }
}
