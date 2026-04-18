namespace DunIt.Core.Notifications;

public class NotificationService
{
    private readonly INotificationInterop _interop;
    private bool _isPermissionGranted;

    public NotificationService(INotificationInterop interop)
    {
        _interop = interop;
    }

    public bool IsPermissionGranted => _isPermissionGranted;

    public async Task<NotificationPermission> RequestPermission()
    {
        var permission = await _interop.RequestPermission();

        if (permission == NotificationPermission.Granted)
        {
            _isPermissionGranted = true;
        }
        else
        {
            _isPermissionGranted = false;
        }

        return permission;
    }
}
