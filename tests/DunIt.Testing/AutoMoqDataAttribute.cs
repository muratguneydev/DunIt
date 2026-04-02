namespace DunIt.Testing;

using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.NUnit3;

public class AutoMoqDataAttribute : AutoDataAttribute
{
    public AutoMoqDataAttribute() : base(() =>
    {
        var fixture = new DunItFixture();
        fixture.Customize(new AutoMoqCustomization { ConfigureMembers = true });
        return fixture;
    }) { }
}
