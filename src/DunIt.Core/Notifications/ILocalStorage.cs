namespace DunIt.Core.Notifications;

public interface ILocalStorage
{
    ValueTask SetItemAsync(string key, string value);
    ValueTask<string?> GetItemAsync(string key);
}
