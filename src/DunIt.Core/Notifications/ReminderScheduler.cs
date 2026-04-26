namespace DunIt.Core.Notifications;

using DunIt.Core.Models;

public class ReminderScheduler
{
    private readonly ReminderTimer _timer;
    private readonly IClock _clock;

    public ReminderScheduler(ReminderTimer timer, IClock clock)
    {
        _timer = timer;
        _clock = clock;
    }

    public virtual async Task ScheduleDaily(ReminderSettings settings)
    {
        if (!settings.Enabled)
        {
            _timer.Cancel();
            return;
        }

        var delay = CalculateDelay(settings.Time);
        await _timer.Schedule(delay, () => ScheduleDaily(settings));
    }

    private TimeSpan CalculateDelay(TimeOnly reminderTime)
    {
        var now = _clock.Now;
        var todayReminder = new DateTimeOffset(
            now.Year, now.Month, now.Day,
            reminderTime.Hour, reminderTime.Minute, reminderTime.Second,
            now.Offset);

        if (todayReminder > now)
            return todayReminder - now;

        return todayReminder.AddDays(1) - now;
    }
}
