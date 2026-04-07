namespace DunIt.Core.Models;

public readonly record struct ChoreId(string Value)
{
    public static implicit operator string(ChoreId id) => id.Value;
    public override string ToString() => Value;
}
