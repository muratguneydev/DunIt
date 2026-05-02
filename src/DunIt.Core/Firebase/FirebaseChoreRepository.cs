namespace DunIt.Core.Firebase;

using DunIt.Core.Models;
using DunIt.Core.Repositories;
using DunIt.Core.Schedules;
using Microsoft.Extensions.Logging;

public class FirebaseChoreRepository(IFirebaseInterop interop, ILogger<FirebaseChoreRepository> logger) : IChoreRepository
{
    public async Task<Chore> AddChore(Chore chore)
    {
        var dto = new ChoreDto(chore.Id, chore.Title, chore.AssignedTo, ToScheduleType(chore.Schedule), chore.DueBy.ToString("HH:mm"));
        var saved = await interop.AddChore(dto);
        return ToChore(saved);
    }

    public Task DeleteChore(ChoreId choreId) => interop.DeleteChore(choreId);

    public async Task<IReadOnlyList<Chore>> GetChoresForChild(ChildId childId)
    {
        var dtos = await interop.GetChoresForChild(childId);
        return dtos.Select(ToChore).ToList();
    }

    public async Task<ChoreCompletion> CompleteChore(ChoreId choreId, ChildId childId, DateTimeOffset completedAt)
    {
        var dto = new ChoreCompletionDto(
            Guid.NewGuid().ToString(),
            choreId,
            childId,
            completedAt.ToString("O"));
        var saved = await interop.CompleteChore(dto);
        return ToCompletion(saved);
    }

    public Task UndoChore(ChoreCompletionId completionId) => interop.UndoChore(completionId);

    public async Task<IReadOnlyList<ChoreCompletion>> GetCompletionsFor(ChildId childId, DateTimeOffset date)
    {
        var dateStr = date.ToString("yyyy-MM-dd");
        var dtos = await interop.GetCompletionsFor(childId, dateStr);
        return dtos.Select(ToCompletion).ToList();
    }

    private Chore ToChore(ChoreDto dto)
    {
        try
        {
            return new Chore(new ChoreId(dto.Id), dto.Title, new ChildId(dto.AssignedTo), ToSchedule(dto.ScheduleType))
            {
                DueBy = TimeOnly.ParseExact(dto.DueBy, "HH:mm")
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to convert ChoreDto to Chore: {Dto}", dto);
            throw;
        }
    }

    private ChoreCompletion ToCompletion(ChoreCompletionDto dto)
    {
        try
        {
            return new ChoreCompletion(
                new ChoreCompletionId(dto.Id),
                new ChoreId(dto.ChoreId),
                new ChildId(dto.ChildId),
                DateTimeOffset.Parse(dto.CompletedAt, null, System.Globalization.DateTimeStyles.RoundtripKind));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to convert ChoreCompletionDto to ChoreCompletion: {Dto}", dto);
            throw;
        }
    }

    private static ChoreSchedule ToSchedule(string scheduleType) => scheduleType switch
    {
        "weekdays" => new WeekdaysSchedule(),
        "weekends" => new WeekendsSchedule(),
        _ => new DailySchedule()
    };

    public async Task<ISubscription> SubscribeToChores(ChildId childId, Action<IReadOnlyList<Chore>> onUpdate)
    {
        var id = await interop.SubscribeToChores(childId, dtos =>
        {
            onUpdate(dtos.Select(ToChore).ToList());
            return Task.CompletedTask;
        });
        return new FirebaseSubscription(id, interop);
    }

    public async Task<ISubscription> SubscribeToCompletions(ChildId childId, DateTimeOffset date, Action<IReadOnlyList<ChoreCompletion>> onUpdate)
    {
        var dateStr = date.ToString("yyyy-MM-dd");
        var id = await interop.SubscribeToCompletions(childId, dateStr, dtos =>
        {
            onUpdate(dtos.Select(ToCompletion).ToList());
            return Task.CompletedTask;
        });
        return new FirebaseSubscription(id, interop);
    }

    private static string ToScheduleType(ChoreSchedule schedule) => schedule switch
    {
        WeekdaysSchedule => "weekdays",
        WeekendsSchedule => "weekends",
        _ => "daily"
    };
}
