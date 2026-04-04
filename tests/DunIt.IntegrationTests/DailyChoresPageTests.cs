namespace DunIt.IntegrationTests;

using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

public class DailyChoresPageTests : PageTest
{
    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL") ?? "http://localhost:5000";

    [SetUp]
    public Task StartTracing() => PlaywrightTracing.Start(Context);

    [TearDown]
    public Task StopTracing() => PlaywrightTracing.Stop(Context, Page,
        TestContext.CurrentContext.Test.Name,
        TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed);

    [Test]
    public async Task ShouldShowBothChildren_WhenPageLoads()
    {
        await Page.GotoAsync(BaseUrl);

        await Expect(Page.GetByText("Alice")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Bob")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowAlicesChores_WhenAliceSelected()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.GetByText("Alice").ClickAsync();

        await Expect(Page.GetByText("Make bed")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Brush teeth")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowBobsChores_WhenChildSwitchedToBob()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.GetByText("Bob").ClickAsync();

        await Expect(Page.GetByText("Make bed")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Brush teeth")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowChoreAsCompleted_WhenTapped()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.GetByText("Alice").ClickAsync();

        await Page.Locator(".chore-item:not(.done)").GetByText("Make bed").ClickAsync();

        await Expect(Page.Locator(".done").GetByText("Make bed")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowChoreAsUncompleted_WhenUndone()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.GetByText("Alice").ClickAsync();

        await Page.Locator(".chore-item:not(.done)").GetByText("Make bed").ClickAsync();
        await Page.Locator(".done").GetByText("Make bed").ClickAsync();

        await Expect(Page.Locator(".chore-item:not(.done)").GetByText("Make bed")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldUpdateProgress_WhenChoreCompleted()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.GetByText("Alice").ClickAsync();

        await Expect(Page.GetByText("0 /")).ToBeVisibleAsync();

        await Page.Locator(".chore-item:not(.done)").GetByText("Make bed").ClickAsync();

        await Expect(Page.GetByText("1 /")).ToBeVisibleAsync();
    }
}
