namespace DunIt.IntegrationTests;

using DunIt.IntegrationTests.Firebase;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

public class ReportPageTests : PageTest
{
    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL") ?? "http://localhost:5000";

    private static readonly string ReportUrl = BaseUrl + "/report";

    [SetUp]
    public async Task SetUp()
    {
        await FirestoreEmulator.SeedDefaultData();
        var parentUid = await FirebaseAuthEmulator.SeedTestUser();
        await FirestoreEmulator.AddParent(parentUid);
        await PlaywrightTracing.Start(Context);
        Page.SetDefaultTimeout(15000);
    }

    [TearDown]
    public Task StopTracing() => PlaywrightTracing.Stop(Context, Page,
        TestContext.CurrentContext.Test.Name,
        TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed);

    [Test]
    public async Task ShouldShowWeeklyReport_WhenParentNavigatesToReport()
    {
        await FirebaseAuthEmulator.SignIn(Page, BaseUrl);
        await Page.GotoAsync(ReportUrl);

        await Expect(Page.Locator(".child-selector")).ToBeVisibleAsync();
        await Expect(Page.Locator(".report-days")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowAlicesReport_WhenAliceSelected()
    {
        await FirebaseAuthEmulator.SignIn(Page, BaseUrl);
        await Page.GotoAsync(ReportUrl);
        await Page.Locator(".child-btn", new() { HasText = "Alice" }).ClickAsync();

        await Expect(Page.GetByRole(Microsoft.Playwright.AriaRole.Heading, new() { Name = "Alice's Week" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldReflectCompletion_WhenChoreCompletedToday()
    {
        await FirestoreEmulator.AddCompletion("comp-1", "chore-1", "child-1", DateTimeOffset.UtcNow);
        await FirebaseAuthEmulator.SignIn(Page, BaseUrl);
        await Page.GotoAsync(ReportUrl);
        // Alice is auto-selected as first child

        await Expect(Page.Locator(".day-count", new() { HasText = "1 /" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowOwnReport_WhenSignedInAsChild()
    {
        var childUid = await FirebaseAuthEmulator.SeedChildUser();
        await FirestoreEmulator.AddChild("child-linked", "Alice", "👧", childUid);
        await FirestoreEmulator.AddChore("chore-linked-1", "Make bed", "child-linked", "daily");

        await FirebaseAuthEmulator.SignInAsChild(Page, BaseUrl);
        await Page.GotoAsync(ReportUrl);

        await Expect(Page.GetByText("Alice's Week")).ToBeVisibleAsync();
        await Expect(Page.Locator(".child-selector")).Not.ToBeVisibleAsync();
    }
}
