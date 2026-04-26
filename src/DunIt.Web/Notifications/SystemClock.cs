namespace DunIt.Web.Notifications;

using DunIt.Core.Notifications;

public sealed class SystemClock : IClock
{
    public DateTimeOffset Now => DateTimeOffset.Now;
}
