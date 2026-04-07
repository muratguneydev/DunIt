namespace DunIt.Core.Models;

public readonly record struct ChoreCompletionId(string Value)
{
    public static implicit operator string(ChoreCompletionId id) => id.Value;
    public override string ToString() => Value;
}
