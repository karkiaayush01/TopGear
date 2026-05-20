using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TopGear.Application.DTOs.PartSaleDTO;
using TopGear.Application.Interfaces;

namespace TopGear.Controllers;

[ApiController]
[Route("api/part-sale")]
public class PartSaleController(IPartSaleService partSaleService) : ControllerBase
{
    /// <summary>
    /// Customer self-purchase — cash only, no credit. Customer ID is taken from the JWT token.
    /// </summary>
    [Authorize(Roles = "Customer")]
    [HttpPost("purchase")]
    public async Task<IActionResult> SelfPurchase(CustomerPurchaseDTO dto)
    {
        var customerId = GetCurrentUserId();

        var createDto = new CreatePartSaleDTO
        {
            CustomerId = customerId,
            IsCredit = false,
            Items = dto.Items
        };

        var sale = await partSaleService.CreateSaleAsync(createDto, customerId);
        return CreatedAtAction(nameof(GetSaleById), new { saleId = sale.SaleId }, sale);
    }

    /// <summary>
    /// Create a new parts sale. Automatically applies 10% loyalty discount if total exceeds NPR 5,000.
    /// Credit sales are marked as unpaid; cash sales are marked paid immediately.
    /// </summary>
    [Authorize(Roles = "Admin,Staff")]
    [HttpPost]
    public async Task<IActionResult> CreateSale(CreatePartSaleDTO dto)
    {
        var staffId = GetCurrentUserId();
        var sale = await partSaleService.CreateSaleAsync(dto, staffId);
        return CreatedAtAction(nameof(GetSaleById), new { saleId = sale.SaleId }, sale);
    }

    /// <summary>
    /// Get all sales.
    /// </summary>
    [Authorize(Roles = "Admin,Staff")]
    [HttpGet]
    public async Task<IActionResult> GetAllSales()
    {
        var sales = await partSaleService.GetAllSalesAsync();
        return Ok(sales);
    }

    /// <summary>
    /// Get a sale by ID.
    /// </summary>
    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("{saleId:guid}")]
    public async Task<IActionResult> GetSaleById(Guid saleId)
    {
        var sale = await partSaleService.GetSaleByIdAsync(saleId);
        return Ok(sale);
    }

    /// <summary>
    /// Get all sales for a specific customer.
    /// </summary>
    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("customer/{customerId:guid}")]
    public async Task<IActionResult> GetSalesByCustomer(Guid customerId)
    {
        var sales = await partSaleService.GetSalesByCustomerAsync(customerId);
        return Ok(sales);
    }

    /// <summary>
    /// Get the authenticated customer's sales history.
    /// </summary>
    [Authorize(Roles = "Customer")]
    [HttpGet("mine")]
    public async Task<IActionResult> GetMySales()
    {
        var customerId = GetCurrentUserId();
        var sales = await partSaleService.GetSalesByCustomerAsync(customerId);
        return Ok(sales);
    }

    /// <summary>
    /// Mark a credit sale as paid when payment is received.
    /// </summary>
    [Authorize(Roles = "Admin,Staff")]
    [HttpPatch("{saleId:guid}/mark-paid")]
    public async Task<IActionResult> MarkAsPaid(Guid saleId)
    {
        var sale = await partSaleService.MarkAsPaidAsync(saleId);
        return Ok(sale);
    }

    /// <summary>
    /// Send the sales invoice to the customer's email address.
    /// </summary>
    [Authorize(Roles = "Admin,Staff")]
    [HttpPost("{saleId:guid}/send-invoice")]
    public async Task<IActionResult> SendInvoice(Guid saleId)
    {
        await partSaleService.SendInvoiceEmailAsync(saleId);
        return Ok(new { message = "Invoice sent successfully." });
    }

    private Guid GetCurrentUserId()
    {
        var userId = User.FindFirstValue("sub")
            ?? throw new UnauthorizedAccessException("User identity not found.");
        return Guid.Parse(userId);
    }
}
