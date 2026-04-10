namespace DunIt.IntegrationTests;

using DunIt.IntegrationTests.Firebase;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

public class ChildChorePageTests : PageTest
{
    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL") ?? "http://localhost:5000";

    [SetUp]
    public async Task SetUp()
    {
        await FirestoreEmulator.ClearAll();
        var parentUid = await FirebaseAuthEmulator.SeedTestUser();
        await FirestoreEmulator.AddParent(parentUid);
        var childUid = await FirebaseAuthEmulator.SeedChildUser();

        await FirestoreEmulator.AddChild("child-1", "Alice", "👧", childUid);
        await FirestoreEmulator.AddChild("child-2", "Bob", "👦");
        await FirestoreEmulator.AddChore("chore-1", "Make bed", "child-1", "daily");
        await FirestoreEmulator.AddChore("chore-2", "Brush teeth", "child-1", "daily");
        await FirestoreEmulator.AddChore("chore-3", "Make bed", "child-2", "daily");

        await PlaywrightTracing.Start(Context);
        Page.SetDefaultTimeout(15000);
    }

    [TearDown]
    public Task StopTracing() => PlaywrightTracing.Stop(Context, Page,
        TestContext.CurrentContext.Test.Name,
        TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed);

    [Test]
    public async Task ShouldShowOnlyOwnChores_WhenSignedInAsChild()
    {
        await FirebaseAuthEmulator.SignInAsChild(Page, BaseUrl);

        await Expect(Page.GetByText("Make bed")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Brush teeth")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Bob")).Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldNotShowChildSelector_WhenSignedInAsChild()
    {
        await FirebaseAuthEmulator.SignInAsChild(Page, BaseUrl);

        await Expect(Page.Locator(".child-selector")).Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldNotShowAdminLink_WhenSignedInAsChild()
    {
        await FirebaseAuthEmulator.SignInAsChild(Page, BaseUrl);

        await Expect(Page.GetByText("Manage chores")).Not.ToBeVisibleAsync();
    }
}
