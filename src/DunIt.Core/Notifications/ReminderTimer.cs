namespace DunIt.Core.Notifications;

public class ReminderTimer
{
    public virtual ValueTask Schedule(TimeSpan delay, Func<Task> callback) => ValueTask.CompletedTask;
    public virtual void Cancel() { }
}
