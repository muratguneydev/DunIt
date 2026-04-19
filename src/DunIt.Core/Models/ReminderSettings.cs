namespace DunIt.Core.Models;

public record ReminderSettings(bool Enabled, TimeOnly Time)
{
    public static readonly ReminderSettings Default = new(false, new TimeOnly(7, 0));
}