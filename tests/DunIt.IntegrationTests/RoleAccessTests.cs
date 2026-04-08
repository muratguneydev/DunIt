namespace DunIt.IntegrationTests;

using DunIt.IntegrationTests.Firebase;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

public class RoleAccessTests : PageTest
{
    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("PLAYWRIGHT_BASE_URL") ?? "http://localhost:5000";

    private static readonly string AdminUrl = BaseUrl + "/admin";

    [SetUp]
    public async Task SetUp()
    {
        await FirestoreEmulator.SeedDefaultData();
        var parentUid = await FirebaseAuthEmulator.SeedTestUser();
        await FirestoreEmulator.AddParent(parentUid);
        await FirebaseAuthEmulator.SeedChildUser();
        await PlaywrightTracing.Start(Context);
        Page.SetDefaultTimeout(15000);
    }

    [TearDown]
    public Task StopTracing() => PlaywrightTracing.Stop(Context, Page,
        TestContext.CurrentContext.Test.Name,
        TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed);

    [Test]
    public async Task ShouldAllowAdminAccess_WhenSignedInAsParent()
    {
        await FirebaseAuthEmulator.SignIn(Page, BaseUrl);

        await Page.GotoAsync(AdminUrl);

        await Expect(Page).ToHaveURLAsync(AdminUrl);
        await Expect(Page.GetByText("Children")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldRedirectToHome_WhenChildUserNavigatesToAdmin()
    {
        await FirebaseAuthEmulator.SignInAsChild(Page, BaseUrl);

        await Page.GotoAsync(AdminUrl);

        await Expect(Page).ToHaveURLAsync(BaseUrl + "/");
    }

    [Test]
    public async Task ShouldNotShowAdminLink_WhenSignedInAsChild()
    {
        await FirebaseAuthEmulator.SignInAsChild(Page, BaseUrl);

        await Expect(Page.GetByText("Manage chores")).Not.ToBeVisibleAsync();
    }

    [Test]
    public async Task ShouldShowAdminLink_WhenSignedInAsParent()
    {
        await FirebaseAuthEmulator.SignIn(Page, BaseUrl);

        await Expect(Page.GetByText("Manage chores")).ToBeVisibleAsync();
    }
}
