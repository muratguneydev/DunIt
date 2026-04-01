namespace DunIt.Testing;

using AutoFixture.NUnit3;

public class DomainAutoDataAttribute : AutoDataAttribute
{
    public DomainAutoDataAttribute() : base(() => new DunItFixture()) { }
}
