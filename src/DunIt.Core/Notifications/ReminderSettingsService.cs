namespace DunIt.Core.Notifications;

using System.Text.Json;
using DunIt.Core.Models;

public class ReminderSettingsService
{
    private readonly ILocalStorage _storage;
    private const string Key = "reminderSettings";

    public ReminderSettingsService(ILocalStorage storage)
    {
        _storage = storage;
    }

    public async Task<ReminderSettings> GetSettingsAsync()
    {
        var json = await _storage.GetItemAsync(Key);
        if (json is null)
        {
            return ReminderSettings.Default;
        }
        return JsonSerializer.Deserialize<ReminderSettings>(json)!;
    }

    public async Task SaveSettingsAsync(ReminderSettings settings)
    {
        var json = JsonSerializer.Serialize(settings);
        await _storage.SetItemAsync(Key, json);
    }
}