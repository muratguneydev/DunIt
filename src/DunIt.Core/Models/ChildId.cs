namespace DunIt.Core.Models;

public readonly record struct ChildId(string Value)
{
    public static implicit operator string(ChildId id) => id.Value;
    public override string ToString() => Value;
}
