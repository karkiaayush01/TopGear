using System;
using System.Collections.Generic;
using System.Text;
using TopGear.Application.DTOs.PurchaseInvoiceDTO;
using TopGear.Application.DTOs.VendorDTO;
using TopGear.Domain.Entities;

namespace TopGear.Application.Interfaces;

public interface IPurchaseInvoiceService
{
    Task<IEnumerable<PurchaseInvoiceDTO>> GetPurchaseInvoiceAsync();
    Task<PurchaseInvoiceDTO> GetPurchaseInvoiceByIdAsync(Guid id);
    Task<PurchaseInvoiceDTO> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceDTO purchaseInvoiceCreateDTO);
    Task<bool> DeletePurchaseInvoiceAsync(Guid id);
}
