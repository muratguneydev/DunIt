namespace DunIt.IntegrationTests;

using DunIt.IntegrationTests.Firebase;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

public class DailyChoresPageTests : PageTest
{
    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL") ?? "http://localhost:5000";

    [SetUp]
    public async Task SetUp()
    {
        await FirestoreEmulator.SeedDefaultData();
        var parentUid = await FirebaseAuthEmulator.SeedTestUser();
        await FirestoreEmulator.AddParent(parentUid);
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
    public async Task ShouldShowNewChild_WhenAddedExternallyWhilePageIsOpen()
    {
        await Page.GotoAsync(BaseUrl);
        await Expect(Page.GetByText("Alice")).ToBeVisibleAsync();

        // Simulate another device adding a child
        await FirestoreEmulator.AddChild("child-99", "Charlie", "🧒");

        // Charlie should appear in the child selector without a page reload
        await Expect(Page.GetByText("Charlie")).ToBeVisibleAsync();
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

    [Test]
    public async Task ShouldShowGetStartedLink_WhenNoChildrenExist()
    {
        await FirestoreEmulator.ClearAll();
        await Page.GotoAsync(BaseUrl);

        await Expect(Page.GetByText("No children added yet.")).ToBeVisibleAsync();
        await Expect(Page.GetByRole(Microsoft.Playwright.AriaRole.Link, new() { Name = "Go to Admin to get started →" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowSignOutButton_WhenAuthenticated()
    {
        await Page.GotoAsync(BaseUrl);

        await Expect(Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Sign out" })).ToBeVisibleAsync();
    }
}
