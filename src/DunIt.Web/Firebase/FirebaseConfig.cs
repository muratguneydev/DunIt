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

    public FirebaseConfig(
        string apiKey, string authDomain, string projectId,
        string storageBucket, string messagingSenderId, string appId,
        string emulatorHost, string authEmulatorHost)
        : this(apiKey, authDomain, projectId, storageBucket, messagingSenderId, appId, emulatorHost)
    {
        AuthEmulatorHost = authEmulatorHost;
    }

    public string ApiKey { get; }
    public string AuthDomain { get; }
    public string ProjectId { get; }
    public string StorageBucket { get; }
    public string MessagingSenderId { get; }
    public string AppId { get; }
    public string EmulatorHost { get; }
    public string AuthEmulatorHost { get; } = string.Empty;
    public bool IsUsingEmulator => _isUsingEmulator;

    public static FirebaseConfig From(IConfigurationSection section)
    {
        var emulatorHost = section["EmulatorHost"];
        var authEmulatorHost = section["AuthEmulatorHost"];
        if (!string.IsNullOrEmpty(emulatorHost) && !string.IsNullOrEmpty(authEmulatorHost))
            return new FirebaseConfig(
                section["ApiKey"]!, section["AuthDomain"]!, section["ProjectId"]!,
                section["StorageBucket"]!, section["MessagingSenderId"]!, section["AppId"]!,
                emulatorHost, authEmulatorHost);
        if (!string.IsNullOrEmpty(emulatorHost))
            return new FirebaseConfig(
                section["ApiKey"]!, section["AuthDomain"]!, section["ProjectId"]!,
                section["StorageBucket"]!, section["MessagingSenderId"]!, section["AppId"]!,
                emulatorHost);
        return new FirebaseConfig(
            section["ApiKey"]!, section["AuthDomain"]!, section["ProjectId"]!,
            section["StorageBucket"]!, section["MessagingSenderId"]!, section["AppId"]!);
    }
}
