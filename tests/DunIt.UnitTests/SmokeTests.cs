using NUnit.Framework;
using Shouldly;

namespace DunIt.UnitTests;

public class SmokeTests
{
    [Test]
    public void ShouldPass_WhenSolutionBuildsAndTestsRun()
    {
        true.ShouldBeTrue();
    }
}
