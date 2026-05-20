using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopGear.Application.Interfaces;

namespace TopGear.Controllers;

[ApiController]
[Route("api/report")]
[Authorize]
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
    [Authorize(Roles = "Admin")]
    [HttpGet("purchase-invoice")]
    public async Task<IActionResult> GetPurchaseInvoiceReport(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to)
    {
        var report = await _reportService.GetPurchaseInvoiceReportAsync(from, to);
        return Ok(report);
    }

    /// <summary>
    /// Get sales financial report grouped by period (daily, monthly, yearly)
    /// </summary>
    [Authorize(Roles = "Admin")]
    [HttpGet("financial")]
    public async Task<IActionResult> GetFinancialReport(
        [FromQuery] string period = "daily",
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var validPeriods = new[] { "daily", "monthly", "yearly" };
        if (!validPeriods.Contains(period.ToLower()))
            return BadRequest("Period must be one of: daily, monthly, yearly");

        var report = await _reportService.GetFinancialReportAsync(period, from, to);
        return Ok(report);
    }

    /// <summary>
    /// Get customers with repeat purchases (regulars)
    /// </summary>
    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("customers/regulars")]
    public async Task<IActionResult> GetRegularCustomers(
        [FromQuery] int minPurchases = 2)
    {
        var report = await _reportService.GetRegularCustomersAsync(minPurchases);
        return Ok(report);
    }

    /// <summary>
    /// Get top customers by total spend
    /// </summary>
    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("customers/high-spenders")]
    public async Task<IActionResult> GetHighSpenders(
        [FromQuery] int top = 10)
    {
        var report = await _reportService.GetHighSpendersAsync(top);
        return Ok(report);
    }

    /// <summary>
    /// Get customers with unpaid credit sales
    /// </summary>
    [Authorize(Roles = "Admin,Staff")]
    [HttpGet("customers/pending-credits")]
    public async Task<IActionResult> GetPendingCredits()
    {
        var report = await _reportService.GetPendingCreditsAsync();
        return Ok(report);
    }
}
