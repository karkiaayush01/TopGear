using System.ComponentModel.DataAnnotations;
using TopGear.Domain.Enums;

namespace TopGear.Application.DTOs.ReviewDTO;

public class CreateReviewDTO
{
    [Required]
    public ReviewType ReviewType { get; set; }

    public Guid? PartId { get; set; }

    public Guid? AppointmentId { get; set; }

    [Required]
    [Range(1, 5)]
    public int Rating { get; set; }

    public string? Comment { get; set; }
}
