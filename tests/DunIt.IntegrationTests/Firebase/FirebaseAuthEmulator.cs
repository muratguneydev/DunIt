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
    public const string ChildEmail = "child@dunit.app";
    public const string ChildPassword = "child1234";
    private static readonly HttpClient Http = new();

    public static async Task<string> SeedTestUser()
    {
        await ClearAll();
        return await CreateUser(TestEmail, TestPassword);
    }

    public static async Task<string> SeedChildUser()
    {
        return await CreateUser(ChildEmail, ChildPassword);
    }

    public static async Task SignIn(IPage page, string baseUrl)
    {
        await page.GotoAsync(baseUrl);
        await page.Locator("input[type=email]").FillAsync(TestEmail);
        await page.Locator("input[type=password]").FillAsync(TestPassword);
        await page.Locator("button[type=submit]").ClickAsync();
        await page.WaitForSelectorAsync(".child-selector, .empty-state");
    }

    public static async Task SignInAsChild(IPage page, string baseUrl)
    {
        await page.GotoAsync(baseUrl);
        await page.Locator("input[type=email]").FillAsync(ChildEmail);
        await page.Locator("input[type=password]").FillAsync(ChildPassword);
        await page.Locator("button[type=submit]").ClickAsync();
        await page.WaitForSelectorAsync(".child-selector, .empty-state");
    }

    private static async Task ClearAll() =>
        await Http.DeleteAsync($"{BaseUrl}/emulator/v1/projects/{ProjectId}/accounts");

    private static async Task<string> CreateUser(string email, string password)
    {
        var body = JsonSerializer.Serialize(new { email, password, returnSecureToken = false });
        var url = $"{BaseUrl}/identitytoolkit.googleapis.com/v1/accounts:signUp?key=demo-api-key";
        var response = await Http.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
        var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("localId").GetString()!;
    }
}
