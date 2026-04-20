namespace DunIt.Testing;

using System.Reflection;
using global::AutoFixture;
using global::AutoFixture.AutoMoq;
using global::AutoFixture.Kernel;
using Moq;

public class MockCustomization : ICustomization
{
    private readonly ParameterInfo _parameterInfo;

    public MockCustomization(ParameterInfo parameterInfo) => _parameterInfo = parameterInfo;

    public void Customize(IFixture fixture)
    {
        var mockedType = typeof(IMock<object>).IsAssignableFrom(_parameterInfo.ParameterType)
            ? _parameterInfo.ParameterType.GetGenericArguments().First()
            : _parameterInfo.ParameterType;
        fixture.Customizations.Add(new MockRelay(new ExactTypeSpecification(mockedType)));
    }
}
