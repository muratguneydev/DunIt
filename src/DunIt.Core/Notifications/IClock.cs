namespace DunIt.Core.Notifications;

public interface IClock
{
    DateTimeOffset Now { get; }
}
