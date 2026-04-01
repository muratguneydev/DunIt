namespace DunIt.Core.Models;

public record ChoreCompletion(string Id, string ChoreId, string ChildId, DateTimeOffset CompletedAt);
