using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopGear.Application.Interfaces;

namespace TopGear.Controllers;

[ApiController]
[Route("api/report")]
[Authorize(Roles = "Admin")]
public class ReportController : ControllerBase
{
    private readonly IReportService _reportService;

    public ReportController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Get purchase invoice report with optional date range filter
    /// </summary>
    [HttpGet("purchase-invoice")]
    public async Task<IActionResult> GetPurchaseInvoiceReport(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var report = await _reportService.GetPurchaseInvoiceReportAsync(from, to);
        return Ok(report);
    }
}
