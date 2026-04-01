namespace DunIt.UnitTests;

using NUnit.Framework;
using Shouldly;

public class SmokeTests
{
    [Test]
    public void ShouldPass_WhenSolutionBuildsAndTestsRun()
    {
        true.ShouldBeTrue();
    }
}
