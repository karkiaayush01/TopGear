using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.ReviewDTO;

public class ReviewDTO
{
    public Guid ReviewId { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = null!;
    public ReviewType ReviewType { get; set; }
    public Guid? PartId { get; set; }
    public Guid? AppointmentId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
