using System.ComponentModel.DataAnnotations;

namespace TopGear.Application.DTOs.PartSaleDTO;

public class CustomerPurchaseDTO
{
    [Required]
    [MinLength(1, ErrorMessage = "At least one item is required.")]
    public List<CreatePartSaleItemDTO> Items { get; set; } = new();
}
