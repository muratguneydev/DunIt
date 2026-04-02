namespace DunIt.Testing;

using AutoFixture.NUnit4;

public class DomainAutoDataAttribute : AutoDataAttribute
{
    public DomainAutoDataAttribute() : base(() => new DunItFixture()) { }
}
