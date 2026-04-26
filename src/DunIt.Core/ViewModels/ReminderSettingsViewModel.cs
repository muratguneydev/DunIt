namespace DunIt.Core.ViewModels;

using DunIt.Core.Models;
using DunIt.Core.Notifications;

public class ReminderSettingsViewModel
{
    private readonly ReminderSettingsService _settingsService;
    private readonly ReminderScheduler _scheduler;

    public event Action StateChanged = delegate { };

    public bool Enabled { get; set; }
    public TimeOnly Time { get; set; }
    public bool IsSaved { get; private set; }

    public ReminderSettingsViewModel(ReminderSettingsService settingsService, ReminderScheduler scheduler)
    {
        _settingsService = settingsService;
        _scheduler = scheduler;
    }

    public async Task LoadAsync()
    {
        var settings = await _settingsService.GetSettingsAsync();
        Enabled = settings.Enabled;
        Time = settings.Time;
        StateChanged();
    }

    public async Task SaveAsync()
    {
        var settings = new ReminderSettings(Enabled, Time);
        await _settingsService.SaveSettingsAsync(settings);
        await _scheduler.ScheduleDaily(settings);
        IsSaved = true;
        StateChanged();
    }
}
