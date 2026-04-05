namespace DunIt.IntegrationTests.Mobile;

using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

public class MobileAdminPageTests : PageTest
{
    private static readonly string AdminUrl =
        (Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL") ?? "http://localhost:5000") + "/admin";

    public override BrowserNewContextOptions ContextOptions() =>
        Playwright.Devices["iPhone 14"];

    [SetUp]
    public Task StartTracing() => PlaywrightTracing.Start(Context);

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

        await Page.GetByPlaceholder("Child's name").FillAsync("Charlie");
        await Page.GetByRole(AriaRole.Button, new() { Name = "Add child" }).ClickAsync();

        await Expect(Page.Locator(".admin-list").First.GetByText("Charlie")).ToBeVisibleAsync();
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
