using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TopGear.Application.Interfaces;
using TopGear.Infrastructure.Config;
using TopGear.Infrastructure.Data;

namespace TopGear.Infrastructure.BackgroundServices;

public class LowStockNotificationWorker(
    IServiceScopeFactory scopeFactory,
    IOptions<NotificationSettings> settings,
    ILogger<LowStockNotificationWorker> logger) : BackgroundService
{
    private readonly NotificationSettings _settings = settings.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Low-stock notification worker started. Interval: {Hours}h", _settings.LowStockCheckIntervalHours);

        using var timer = new PeriodicTimer(TimeSpan.FromHours(_settings.LowStockCheckIntervalHours));

        // Run immediately on startup, then on each tick.
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

            var lowStockParts = await db.Parts
                .AsNoTracking()
                .Where(p => p.IsActive && p.Quantity < _settings.LowStockThreshold)
                .Select(p => new { p.PartName, p.Quantity })
                .ToListAsync(ct);

            if (lowStockParts.Count == 0)
            {
                logger.LogInformation("Low-stock check: all parts above threshold.");
                return;
            }

            logger.LogWarning("Low-stock check: {Count} part(s) below threshold. Notifying admin.", lowStockParts.Count);

            await emailService.SendLowStockAlertAsync(
                _settings.AdminEmail,
                lowStockParts.Select(p => (p.PartName, p.Quantity)));
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogError(ex, "Error during low-stock notification check.");
        }
    }
}
