namespace DunIt.Core.Models;

public record Child(ChildId Id, string Name, string Avatar = "🧒", FirebaseUid FirebaseUid = default);
