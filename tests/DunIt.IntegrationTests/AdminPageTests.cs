namespace DunIt.IntegrationTests;

using DunIt.IntegrationTests.Firebase;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

public class AdminPageTests : PageTest
{
    private static readonly string AdminUrl =
        (Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL") ?? "http://localhost:5000") + "/admin";

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
    public async Task ShouldShowChildrenAndTheirChores_WhenAdminPageLoads()
    {
        await Page.GotoAsync(AdminUrl);

        // Children list is the first .admin-list on the page
        await Expect(Page.Locator(".admin-list").First.GetByText("Alice")).ToBeVisibleAsync();
        await Expect(Page.Locator(".admin-list").First.GetByText("Bob")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Make bed").First).ToBeVisibleAsync();
        await Expect(Page.GetByText("Brush teeth").First).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowNewChild_WhenChildAdded()
    {
        await Page.GotoAsync(AdminUrl);
        await Expect(Page.Locator(".admin-list").First.GetByText("Alice")).ToBeVisibleAsync();

        await Page.GetByPlaceholder("Child's name").FillAsync("Charlie");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Add child" }).ClickAsync();

        await Expect(Page.Locator(".admin-list").First.GetByText("Charlie"))
            .ToBeVisibleAsync(new() { Timeout = 30000 });
    }

    [Test]
    public async Task ShouldRemoveChild_WhenChildDeleted()
    {
        await Page.GotoAsync(AdminUrl);

        await Page.Locator(".admin-list").First.Locator(".admin-item")
            .Filter(new() { HasText = "Alice" })
            .GetByRole(AriaRole.Button).ClickAsync();

        await Expect(Page.Locator(".admin-list").First.GetByText("Alice")).Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowNewChore_WhenChoreAdded()
    {
        await Page.GotoAsync(AdminUrl);

        await Page.GetByPlaceholder("Chore title").First.FillAsync("Do laundry");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Add chore" }).First.ClickAsync();

        await Expect(Page.GetByText("Do laundry")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldRemoveChore_WhenChoreDeleted()
    {
        await Page.GotoAsync(AdminUrl);

        // "Make bed" exists for both Alice and Bob — assert count drops from 2 to 1
        await Expect(Page.Locator(".admin-item").Filter(new() { HasText = "Make bed" }))
            .ToHaveCountAsync(2);

        await Page.Locator(".admin-item").Filter(new() { HasText = "Make bed" })
            .First.GetByRole(AriaRole.Button).ClickAsync();

        await Expect(Page.Locator(".admin-item").Filter(new() { HasText = "Make bed" }))
            .ToHaveCountAsync(1);
    }

    [Test]
    public async Task ShouldShowSignOutButton_WhenAuthenticated()
    {
        await Page.GotoAsync(AdminUrl);

        await Expect(Page.GetByRole(AriaRole.Button, new() { Name = "Sign out" })).ToBeVisibleAsync();
    }
}
