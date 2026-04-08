namespace DunIt.IntegrationTests;

using DunIt.IntegrationTests.Firebase;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

public class LoginPageTests : PageTest
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
    }

    [TearDown]
    public Task StopTracing() => PlaywrightTracing.Stop(Context, Page,
        TestContext.CurrentContext.Test.Name,
        TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed);

    [Test]
    public async Task ShouldShowLoginForm_WhenNotAuthenticated()
    {
        await Page.GotoAsync(BaseUrl);

        await Expect(Page.Locator("input[type=email]")).ToBeVisibleAsync();
        await Expect(Page.Locator("input[type=password]")).ToBeVisibleAsync();
        await Expect(Page.Locator("button[type=submit]")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowHomePageChores_WhenSignedIn()
    {
        await FirebaseAuthEmulator.SignIn(Page, BaseUrl);

        await Expect(Page.GetByText("Alice")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Bob")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowError_WhenInvalidCredentials()
    {
        await Page.GotoAsync(BaseUrl);
        await Page.Locator("input[type=email]").FillAsync(FirebaseAuthEmulator.TestEmail);
        await Page.Locator("input[type=password]").FillAsync("wrongpassword");
        await Page.Locator("button[type=submit]").ClickAsync();

        await Expect(Page.GetByText("Invalid email or password.")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowLoginForm_WhenSignedOutFromHomePage()
    {
        await FirebaseAuthEmulator.SignIn(Page, BaseUrl);
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Sign out" }).ClickAsync();

        await Expect(Page.Locator("input[type=email]")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowLoginForm_WhenSignedOutFromAdminPage()
    {
        var adminUrl = BaseUrl + "/admin";
        await FirebaseAuthEmulator.SignIn(Page, BaseUrl);
        await Page.GotoAsync(adminUrl);
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Sign out" }).ClickAsync();

        await Expect(Page.Locator("input[type=email]")).ToBeVisibleAsync();
    }
}
