using System.ComponentModel.DataAnnotations;

namespace TopGear.Application.DTOs.ReviewDTO;

public class EditReviewDTO
{
    [Range(1, 5)]
    public int? Rating { get; set; }

    public string? Comment { get; set; }
}
