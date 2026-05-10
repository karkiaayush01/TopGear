using System.ComponentModel.DataAnnotations;

namespace TopGear.Application.DTOs.PartSaleDTO;

public class CreatePartSaleDTO
{
    [Required]
    public Guid CustomerId { get; set; }

    public bool IsCredit { get; set; } = false;

    [Required]
    [MinLength(1, ErrorMessage = "At least one item is required.")]
    public List<CreatePartSaleItemDTO> Items { get; set; } = new();
}

public class CreatePartSaleItemDTO
{
    [Required]
    public Guid PartId { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }
}
