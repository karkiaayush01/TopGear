namespace TopGear.Infrastructure.Config;

public class NotificationSettings
{
    public const string SectionName = "NotificationSettings";

    public string AdminEmail { get; set; } = string.Empty;

    /// <summary>How many units below triggers a low-stock alert.</summary>
    public int LowStockThreshold { get; set; } = 10;

    /// <summary>How often (in hours) the low-stock check runs.</summary>
    public double LowStockCheckIntervalHours { get; set; } = 24;

    /// <summary>How often (in hours) the overdue credit reminder check runs.</summary>
    public double OverdueCreditReminderIntervalHours { get; set; } = 24;
}
