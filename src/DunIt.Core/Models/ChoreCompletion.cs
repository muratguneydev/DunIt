namespace DunIt.Core.Models;

public record ChoreCompletion(ChoreCompletionId Id, ChoreId ChoreId, ChildId ChildId, DateTimeOffset CompletedAt);
