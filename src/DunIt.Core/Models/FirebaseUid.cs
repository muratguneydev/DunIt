namespace DunIt.Core.Models;

public readonly record struct FirebaseUid(string Value)
{
    public static implicit operator string(FirebaseUid uid) => uid.Value;
    public override string ToString() => Value;
}
