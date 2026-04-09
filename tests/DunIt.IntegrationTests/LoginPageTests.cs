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
    public async Task ShouldShowGoogleSignInButton_WhenNotAuthenticated()
    {
        await Page.GotoAsync(BaseUrl);

        await Expect(Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Sign in with Google" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowHomePageChores_WhenSignedIn()
    {
        await FirebaseAuthEmulator.SignIn(Page, BaseUrl);

        await Expect(Page.GetByText("Alice")).ToBeVisibleAsync();
        await Expect(Page.GetByText("Bob")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowLoginPage_WhenSignedOutFromHomePage()
    {
        await FirebaseAuthEmulator.SignIn(Page, BaseUrl);
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Sign out" }).ClickAsync();

        await Expect(Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Sign in with Google" })).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowLoginPage_WhenSignedOutFromAdminPage()
    {
        var adminUrl = BaseUrl + "/admin";
        await FirebaseAuthEmulator.SignIn(Page, BaseUrl);
        await Page.GotoAsync(adminUrl);
        await Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Sign out" }).ClickAsync();

        await Expect(Page.GetByRole(Microsoft.Playwright.AriaRole.Button, new() { Name = "Sign in with Google" })).ToBeVisibleAsync();
    }
}
