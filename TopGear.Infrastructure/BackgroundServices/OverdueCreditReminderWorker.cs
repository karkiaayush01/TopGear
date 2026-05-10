using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TopGear.Application.Interfaces;
using TopGear.Infrastructure.Config;
using TopGear.Infrastructure.Data;

namespace TopGear.Infrastructure.BackgroundServices;

public class OverdueCreditReminderWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<NotificationSettings> settings,
    ILogger<OverdueCreditReminderWorker> logger) : BackgroundService
{
    private readonly NotificationSettings _settings = settings.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Overdue credit reminder worker started. Interval: {Hours}h", _settings.OverdueCreditReminderIntervalHours);

        using var timer = new PeriodicTimer(TimeSpan.FromHours(_settings.OverdueCreditReminderIntervalHours));

        do
        {
            await CheckAndNotifyAsync(stoppingToken);
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }

    private async Task CheckAndNotifyAsync(CancellationToken ct)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var overdueThreshold = DateTime.UtcNow.AddMonths(-1);

            var overdueSales = await db.PartSales
                .AsNoTracking()
                .Include(s => s.Customer)
                .Where(s => s.IsCredit && !s.IsPaid && s.SaleDate < overdueThreshold)
                .ToListAsync(ct);

            if (overdueSales.Count == 0)
            {
                logger.LogInformation("Overdue credit check: no overdue sales found.");
                return;
            }

            // Group by customer so each customer gets one consolidated email.
            var byCustomer = overdueSales
                .GroupBy(s => s.CustomerId)
                .ToList();

            logger.LogWarning("Overdue credit check: {CustomerCount} customer(s) with overdue balances.", byCustomer.Count);

            foreach (var group in byCustomer)
            {
                var customer = group.First().Customer;
                if (customer.Email is null) continue;

                var customerName = $"{customer.FirstName} {customer.LastName}";
                var invoices = group.Select(s => (s.SaleId, s.SaleDate, s.FinalAmount));

                try
                {
                    await emailService.SendOverdueCreditReminderAsync(customer.Email, customerName, invoices);
                    logger.LogInformation("Overdue reminder sent to {Email}.", customer.Email);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to send overdue reminder to {Email}.", customer.Email);
                }
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error during overdue credit reminder check.");
        }
    }
}
