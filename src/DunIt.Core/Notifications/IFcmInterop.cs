namespace DunIt.Core.Notifications;

public interface IFcmInterop
{
    ValueTask<string> GetToken();
    ValueTask SendTestMessage(string token);
}
