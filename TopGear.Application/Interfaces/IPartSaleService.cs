using TopGear.Application.DTOs.PartSaleDTO;

namespace TopGear.Application.Interfaces;

public interface IPartSaleService
{
    Task<PartSaleDTO> CreateSaleAsync(CreatePartSaleDTO dto, Guid staffId);
    Task<PartSaleDTO> GetSaleByIdAsync(Guid saleId);
    Task<List<PartSaleDTO>> GetSalesByCustomerAsync(Guid customerId);
    Task<List<PartSaleDTO>> GetAllSalesAsync();
    Task<PartSaleDTO> MarkAsPaidAsync(Guid saleId);
    Task SendInvoiceEmailAsync(Guid saleId);
}
