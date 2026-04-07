namespace DunIt.IntegrationTests.Mobile;

using DunIt.IntegrationTests.Firebase;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

public class MobileChoresPageTests : PageTest
{
    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL") ?? "http://localhost:5000";

    public override BrowserNewContextOptions ContextOptions() =>
        Playwright.Devices["iPhone 14"];

    [SetUp]
    public async Task SetUp()
    {
        await FirestoreEmulator.SeedDefaultData();
        await FirebaseAuthEmulator.SeedTestUser();
        await PlaywrightTracing.Start(Context);
        Page.SetDefaultTimeout(15000);
        await FirebaseAuthEmulator.SignIn(Page, BaseUrl);
    }

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
    public async Task ShouldShowChoresForChild_WhenChildTapped()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.GetByText("Alice").ClickAsync();

        await Expect(Page.GetByText("Make bed")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Brush teeth")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldMarkChoreAsDone_WhenTapped()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.GetByText("Alice").ClickAsync();

        await Page.Locator(".chore-item:not(.done)").GetByText("Make bed").ClickAsync();

        await Expect(Page.Locator(".done").GetByText("Make bed")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldUpdateProgress_WhenChoreTapped()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.GetByText("Alice").ClickAsync();

        await Expect(Page.GetByText("0 /")).ToBeVisibleAsync();

        await Page.Locator(".chore-item:not(.done)").GetByText("Make bed").ClickAsync();

        await Expect(Page.GetByText("1 /")).ToBeVisibleAsync();
    }
}
