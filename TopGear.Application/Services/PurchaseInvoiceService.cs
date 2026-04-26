using Microsoft.Extensions.Logging;
using TopGear.Application.DTOs.PurchaseInvoiceDTO;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;

namespace TopGear.Application.Services;

public class PurchaseInvoiceService : IPurchaseInvoiceService
{
    private readonly IPurchaseInvoiceRepository _invoiceRepository;
    private readonly IPartRepository _partRepository;
    private readonly ILogger<PurchaseInvoiceService> _logger;

    public PurchaseInvoiceService(
        IPurchaseInvoiceRepository invoiceRepository,
        IPartRepository partRepository,
        ILogger<PurchaseInvoiceService> logger)
    {
        _invoiceRepository = invoiceRepository;
        _partRepository = partRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<PurchaseInvoiceDTO>> GetPurchaseInvoiceAsync()
    {
        _logger.LogInformation("Fetching all purchase invoices");

        var invoices = await _invoiceRepository.FindAllWithDetailsAsync();

        return invoices.Select(MapToDTO);
    }

    public async Task<PurchaseInvoiceDTO> GetPurchaseInvoiceByIdAsync(Guid id)
    {
        _logger.LogInformation("Fetching purchase invoice with id {InvoiceId}", id);

        var invoice = await _invoiceRepository.GetByIdWithDetailsAsync(id);

        if (invoice == null)
        {
            _logger.LogWarning("Purchase invoice not found with id {InvoiceId}", id);
            return null!;
        }

        return MapToDTO(invoice);
    }

    public async Task<PurchaseInvoiceDTO> CreatePurchaseInvoiceAsync(CreatePurchaseInvoiceDTO dto)
    {
        _logger.LogInformation("Creating purchase invoice for vendor {VendorId}", dto.VendorId);

        var invoice = new PurchaseInvoice
        {
            VendorId = dto.VendorId,
            InvoiceDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var itemDto in dto.Items)
        {
            var part = await _partRepository.GetByIdAsync(itemDto.PartId);

            if (part == null)
            {
                _logger.LogWarning("Part not found with id {PartId}", itemDto.PartId);
                throw new KeyNotFoundException($"Part not found with id {itemDto.PartId}");
            }

            var item = new PurchaseInvoiceItem
            {
                PartId = itemDto.PartId,
                Quantity = itemDto.Quantity,
                UnitPrice = itemDto.UnitPrice
            };

            invoice.Items.Add(item);

            part.Quantity += itemDto.Quantity;
            part.PurchasePrice = itemDto.UnitPrice;
            part.UpdatedAt = DateTime.UtcNow;

            _partRepository.Update(part);
        }

        _invoiceRepository.Create(invoice);
        await _invoiceRepository.SaveChangesAsync();

        _logger.LogInformation("Created purchase invoice {InvoiceId}", invoice.PurchaseInvoiceId);

        var savedInvoice = await _invoiceRepository.GetByIdWithDetailsAsync(invoice.PurchaseInvoiceId);
        return MapToDTO(savedInvoice!);
    }

    public async Task<bool> DeletePurchaseInvoiceAsync(Guid id)
    {
        _logger.LogInformation("Deleting purchase invoice {InvoiceId}", id);

        var invoice = await _invoiceRepository.GetByIdAsync(id);

        if (invoice == null)
        {
            _logger.LogWarning("Purchase invoice {InvoiceId} not found", id);
            return false;
        }

        _invoiceRepository.Delete(invoice);
        await _invoiceRepository.SaveChangesAsync();

        return true;
    }

    private static PurchaseInvoiceDTO MapToDTO(PurchaseInvoice invoice)
    {
        return new PurchaseInvoiceDTO
        {
            PurchaseInvoiceId = invoice.PurchaseInvoiceId,
            VendorId = invoice.VendorId,
            VendorName = invoice.Vendor?.VendorName ?? "",
            InvoiceDate = invoice.InvoiceDate,


            Items = invoice.Items.Select(i => new PurchaseInvoiceItemDTO
            {
                PartId = i.PartId,
                PartName = i.Part?.PartName ?? "",
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
            }).ToList()
        };
    }
}