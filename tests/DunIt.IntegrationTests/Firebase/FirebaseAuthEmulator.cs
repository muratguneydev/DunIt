namespace DunIt.IntegrationTests.Firebase;

using System.Text;
using System.Text.Json;
using Microsoft.Playwright;

public static class FirebaseAuthEmulator
{
    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("AUTH_EMULATOR_URL") ?? "http://emulator:9099";

    private const string ProjectId = "demo-dunit";
    public const string TestEmail = "test@dunit.app";
    public const string TestPassword = "test1234";
    private static readonly HttpClient Http = new();

    public static async Task SeedTestUser()
    {
        await ClearAll();
        await CreateUser(TestEmail, TestPassword);
    }

    public static async Task SignIn(IPage page, string baseUrl)
    {
        await page.GotoAsync(baseUrl);
        await page.Locator("input[type=email]").FillAsync(TestEmail);
        await page.Locator("input[type=password]").FillAsync(TestPassword);
        await page.Locator("button[type=submit]").ClickAsync();
        await page.WaitForSelectorAsync(".child-selector, .empty-state");
    }

    private static async Task ClearAll() =>
        await Http.DeleteAsync($"{BaseUrl}/emulator/v1/projects/{ProjectId}/accounts");

    private static async Task CreateUser(string email, string password)
    {
        var body = JsonSerializer.Serialize(new { email, password, returnSecureToken = false });
        var url = $"{BaseUrl}/identitytoolkit.googleapis.com/v1/accounts:signUp?key=demo-api-key";
        await Http.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
    }
}
