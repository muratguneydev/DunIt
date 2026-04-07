namespace DunIt.IntegrationTests.Mobile;

using DunIt.IntegrationTests.Firebase;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

public class MobileAdminPageTests : PageTest
{
    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL") ?? "http://localhost:5000";

    private static readonly string AdminUrl = BaseUrl + "/admin";

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
    public async Task ShouldShowChildrenAndChores_WhenAdminPageLoads()
    {
        await Page.GotoAsync(AdminUrl);

        await Expect(Page.Locator(".admin-list").First.GetByText("Alice")).ToBeVisibleAsync();
        await Expect(Page.Locator(".admin-list").First.GetByText("Bob")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Make bed").First).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldAddChild_WhenFormSubmitted()
    {
        await Page.GotoAsync(AdminUrl);
        await Expect(Page.Locator(".admin-list").First.GetByText("Alice")).ToBeVisibleAsync();

        await Page.GetByPlaceholder("Child's name").FillAsync("Charlie");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Add child" }).ClickAsync();

        await Expect(Page.Locator(".admin-list").First.GetByText("Charlie"))
            .ToBeVisibleAsync(new() { Timeout = 30000 });
    }

    [Test]
    public async Task ShouldAddChore_WhenFormSubmitted()
    {
        await Page.GotoAsync(AdminUrl);

        await Page.GetByPlaceholder("Chore title").First.FillAsync("Do laundry");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Add chore" }).First.ClickAsync();

        await Expect(Page.GetByText("Do laundry")).ToBeVisibleAsync();
    }
}
