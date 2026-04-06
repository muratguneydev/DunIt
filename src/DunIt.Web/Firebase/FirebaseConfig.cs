namespace DunIt.Web.Firebase;

using Microsoft.Extensions.Configuration;

public class FirebaseConfig
{
    private readonly bool _isUsingEmulator;

    public FirebaseConfig(
        string apiKey, string authDomain, string projectId,
        string storageBucket, string messagingSenderId, string appId)
    {
        ApiKey = apiKey;
        AuthDomain = authDomain;
        ProjectId = projectId;
        StorageBucket = storageBucket;
        MessagingSenderId = messagingSenderId;
        AppId = appId;
        EmulatorHost = string.Empty;
        _isUsingEmulator = false;
    }

    public FirebaseConfig(
        string apiKey, string authDomain, string projectId,
        string storageBucket, string messagingSenderId, string appId,
        string emulatorHost)
        : this(apiKey, authDomain, projectId, storageBucket, messagingSenderId, appId)
    {
        EmulatorHost = emulatorHost;
        _isUsingEmulator = true;
    }

    public string ApiKey { get; }
    public string AuthDomain { get; }
    public string ProjectId { get; }
    public string StorageBucket { get; }
    public string MessagingSenderId { get; }
    public string AppId { get; }
    public string EmulatorHost { get; }
    public bool IsUsingEmulator => _isUsingEmulator;

    public static FirebaseConfig From(IConfigurationSection section)
    {
        var emulatorHost = section["EmulatorHost"];
        return !string.IsNullOrEmpty(emulatorHost)
            ? new FirebaseConfig(
                section["ApiKey"]!, section["AuthDomain"]!, section["ProjectId"]!,
                section["StorageBucket"]!, section["MessagingSenderId"]!, section["AppId"]!,
                emulatorHost)
            : new FirebaseConfig(
                section["ApiKey"]!, section["AuthDomain"]!, section["ProjectId"]!,
                section["StorageBucket"]!, section["MessagingSenderId"]!, section["AppId"]!);
    }
}
