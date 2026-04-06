namespace DunIt.IntegrationTests.Firebase;

using System.Text;
using System.Text.Json;

public static class FirestoreEmulator
{
    private static readonly string BaseUrl =
        Environment.GetEnvironmentVariable("FIRESTORE_EMULATOR_URL") ?? "http://emulator:8080";

    private const string ProjectId = "demo-dunit";
    private static readonly HttpClient Http = new();

    public static async Task SeedDefaultData()
    {
        await ClearAll();

        await AddChild("child-1", "Alice", "👧");
        await AddChild("child-2", "Bob", "👦");

        await AddChore("chore-1", "Make bed",    "child-1", "daily");
        await AddChore("chore-2", "Brush teeth", "child-1", "daily");
        await AddChore("chore-3", "Tidy room",   "child-1", "weekdays");
        await AddChore("chore-4", "Make bed",    "child-2", "daily");
        await AddChore("chore-5", "Brush teeth", "child-2", "daily");
    }

    public static async Task ClearAll()
    {
        await Http.DeleteAsync(
            $"{BaseUrl}/emulator/v1/projects/{ProjectId}/databases/(default)/documents");
    }

    private static Task AddChild(string id, string name, string avatar) =>
        SetDocument("children", id, new()
        {
            ["name"]   = StringField(name),
            ["avatar"] = StringField(avatar)
        });

    private static Task AddChore(string id, string title, string assignedTo, string scheduleType) =>
        SetDocument("chores", id, new()
        {
            ["title"]        = StringField(title),
            ["assignedTo"]   = StringField(assignedTo),
            ["scheduleType"] = StringField(scheduleType)
        });

    private static async Task SetDocument(string collection, string id, Dictionary<string, object> fields)
    {
        var body = JsonSerializer.Serialize(new { fields });
        var url = $"{BaseUrl}/v1/projects/{ProjectId}/databases/(default)/documents/{collection}?documentId={id}";
        await Http.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
    }

    private static object StringField(string value) => new { stringValue = value };
}
