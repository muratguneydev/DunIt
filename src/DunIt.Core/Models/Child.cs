namespace DunIt.Core.Models;

public record Child(ChildId Id, string Name, string Avatar = "🧒", FirebaseUid FirebaseUid = default)
{
    public static readonly Child Empty = new(new ChildId(string.Empty), string.Empty);
}
