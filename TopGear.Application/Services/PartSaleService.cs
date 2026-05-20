using TopGear.Application.CustomExceptions;
using TopGear.Application.DTOs.PartSaleDTO;
using TopGear.Application.Interfaces;
using TopGear.Domain.Entities;

namespace TopGear.Application.Services;

public class PartSaleService(
    IPartSaleRepository saleRepository,
    IPartRepository partRepository,
    IEmailService emailService) : IPartSaleService
{
    private const decimal LoyaltyDiscountThreshold = 5000m;
    private const decimal LoyaltyDiscountRate = 0.10m;

    public async Task<PartSaleDTO> CreateSaleAsync(CreatePartSaleDTO dto, Guid staffId)
    {
        var itemDetails = new List<(Part Part, int Quantity)>();

        foreach (var item in dto.Items)
        {
            var part = await partRepository.GetByIdAsync(item.PartId)
                ?? throw new NotFoundException($"Part with ID {item.PartId} not found.");

            if (!part.IsActive)
                throw new BadRequestException($"Part '{part.PartName}' is not available.");

            if (part.Quantity < item.Quantity)
                throw new BadRequestException($"Insufficient stock for '{part.PartName}'. Available: {part.Quantity}, Requested: {item.Quantity}.");

            itemDetails.Add((part, item.Quantity));
        }

        var subTotal = itemDetails.Sum(x => x.Part.SellingPrice * x.Quantity);
        var discountAmount = subTotal > LoyaltyDiscountThreshold
            ? Math.Round(subTotal * LoyaltyDiscountRate, 2)
            : 0m;
        var finalAmount = subTotal - discountAmount;

        var sale = new PartSale
        {
            CustomerId = dto.CustomerId,
            CreatedById = staffId,
            SubTotal = subTotal,
            DiscountAmount = discountAmount,
            FinalAmount = finalAmount,
            IsCredit = dto.IsCredit,
            IsPaid = !dto.IsCredit,
            PaidAt = dto.IsCredit ? null : DateTime.UtcNow
        };

        foreach (var (part, quantity) in itemDetails)
        {
            sale.Items.Add(new PartSaleItem
            {
                PartId = part.PartId,
                Quantity = quantity,
                UnitPrice = part.SellingPrice,
                TotalPrice = part.SellingPrice * quantity
            });

            part.Quantity -= quantity;
            part.UpdatedAt = DateTime.UtcNow;
            partRepository.Update(part);
        }

        saleRepository.Create(sale);
        await saleRepository.SaveChangesAsync();

        var created = await saleRepository.GetByIdWithDetailsAsync(sale.SaleId)
            ?? throw new Exception("Failed to retrieve created sale.");

        return MapToDTO(created);
    }

    public async Task<PartSaleDTO> GetSaleByIdAsync(Guid saleId)
    {
        var sale = await saleRepository.GetByIdWithDetailsAsync(saleId)
            ?? throw new NotFoundException("Sale not found.");

        return MapToDTO(sale);
    }

    public async Task<List<PartSaleDTO>> GetSalesByCustomerAsync(Guid customerId)
    {
        var sales = await saleRepository.GetByCustomerIdAsync(customerId);
        return sales.Select(MapToDTO).ToList();
    }

    public async Task<List<PartSaleDTO>> GetAllSalesAsync()
    {
        var sales = await saleRepository.GetAllWithDetailsAsync();
        return sales.Select(MapToDTO).ToList();
    }

    public async Task<PartSaleDTO> MarkAsPaidAsync(Guid saleId)
    {
        var sale = await saleRepository.GetByIdWithDetailsAsync(saleId)
            ?? throw new NotFoundException("Sale not found.");

        if (!sale.IsCredit)
            throw new BadRequestException("This sale is not a credit sale.");

        if (sale.IsPaid)
            throw new BadRequestException("This sale is already marked as paid.");

        sale.IsPaid = true;
        sale.PaidAt = DateTime.UtcNow;

        saleRepository.Update(sale);
        await saleRepository.SaveChangesAsync();

        return MapToDTO(sale);
    }

    public async Task SendInvoiceEmailAsync(Guid saleId)
    {
        var sale = await saleRepository.GetByIdWithDetailsAsync(saleId)
            ?? throw new NotFoundException("Sale not found.");

        var customerEmail = sale.Customer.Email
            ?? throw new BadRequestException("Customer does not have an email address.");

        var customerName = $"{sale.Customer.FirstName} {sale.Customer.LastName}";

        await emailService.SendSalesInvoiceEmailAsync(customerEmail, customerName, MapToDTO(sale));
    }

    private static PartSaleDTO MapToDTO(PartSale sale) => new()
    {
        SaleId = sale.SaleId,
        CustomerId = sale.CustomerId,
        CustomerName = $"{sale.Customer.FirstName} {sale.Customer.LastName}",
        CustomerEmail = sale.Customer.Email ?? "",
        CreatedById = sale.CreatedById,
        CreatedByName = $"{sale.CreatedBy.FirstName} {sale.CreatedBy.LastName}",
        SaleDate = sale.SaleDate,
        SubTotal = sale.SubTotal,
        DiscountAmount = sale.DiscountAmount,
        FinalAmount = sale.FinalAmount,
        IsCredit = sale.IsCredit,
        IsPaid = sale.IsPaid,
        PaidAt = sale.PaidAt,
        Items = sale.Items.Select(i => new PartSaleItemDTO
        {
            SaleItemId = i.SaleItemId,
            PartId = i.PartId,
            PartName = i.Part.PartName,
            Quantity = i.Quantity,
            UnitPrice = i.UnitPrice,
            TotalPrice = i.TotalPrice
        }).ToList()
    };
}
