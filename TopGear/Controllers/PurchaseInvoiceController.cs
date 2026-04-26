using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopGear.Application.DTOs.PurchaseInvoiceDTO;
using TopGear.Application.Interfaces;

namespace TopGear.Controllers;

[ApiController]
[Route("api/purchase-invoice")]
public class PurchaseInvoiceController : ControllerBase
{
    private readonly IPurchaseInvoiceService _invoiceService;

    public PurchaseInvoiceController(IPurchaseInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }


    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAllInvoices()
    {
        var invoices = await _invoiceService.GetPurchaseInvoiceAsync();
        return Ok(invoices);
    }


    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetInvoiceById(Guid id)
    {
        var invoice = await _invoiceService.GetPurchaseInvoiceByIdAsync(id);

        if (invoice == null)
        {
            return NotFound();
        }

        return Ok(invoice);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateInvoice([FromBody] CreatePurchaseInvoiceDTO dto)
    {
        var createdInvoice = await _invoiceService.CreatePurchaseInvoiceAsync(dto);

        return CreatedAtAction(nameof(GetInvoiceById),
            new { id = createdInvoice.PurchaseInvoiceId },
            createdInvoice);
    }


    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteInvoice(Guid id)
    {
        var success = await _invoiceService.DeletePurchaseInvoiceAsync(id);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}