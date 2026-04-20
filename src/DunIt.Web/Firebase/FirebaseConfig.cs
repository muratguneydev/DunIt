namespace DunIt.Web.Firebase;

using Microsoft.Extensions.Configuration;

public class FirebaseConfig : IFirebaseAppSettings, IFirebaseEmulatorSettings
{
    private readonly bool _isUsingEmulator;

    public FirebaseConfig(
        string apiKey, string authDomain, string projectId,
        string storageBucket, string messagingSenderId, string appId,
        string vapidKey, string fcmServerKey)
    {
        ApiKey = apiKey;
        AuthDomain = authDomain;
        ProjectId = projectId;
        StorageBucket = storageBucket;
        MessagingSenderId = messagingSenderId;
        AppId = appId;
        VapidKey = vapidKey;
        FcmServerKey = fcmServerKey;
        EmulatorHost = string.Empty;
        _isUsingEmulator = false;
    }

    public FirebaseConfig(
        string apiKey, string authDomain, string projectId,
        string storageBucket, string messagingSenderId, string appId,
        string vapidKey, string fcmServerKey,
        string emulatorHost)
        : this(apiKey, authDomain, projectId, storageBucket, messagingSenderId, appId, vapidKey, fcmServerKey)
    {
        EmulatorHost = emulatorHost;
        _isUsingEmulator = true;
    }

    public FirebaseConfig(
        string apiKey, string authDomain, string projectId,
        string storageBucket, string messagingSenderId, string appId,
        string vapidKey, string fcmServerKey,
        string emulatorHost, string authEmulatorHost)
        : this(apiKey, authDomain, projectId, storageBucket, messagingSenderId, appId, vapidKey, fcmServerKey, emulatorHost)
    {
        AuthEmulatorHost = authEmulatorHost;
    }

    public string ApiKey { get; }
    public string AuthDomain { get; }
    public string ProjectId { get; }
    public string StorageBucket { get; }
    public string MessagingSenderId { get; }
    public string AppId { get; }
    public string VapidKey { get; }
    public string FcmServerKey { get; }
    public string EmulatorHost { get; }
    public string AuthEmulatorHost { get; } = string.Empty;
    public bool IsUsingEmulator => _isUsingEmulator;

    public static FirebaseConfig From(IConfigurationSection section)
    {
        var apiKey = section["ApiKey"]!;
        var authDomain = section["AuthDomain"]!;
        var projectId = section["ProjectId"]!;
        var storageBucket = section["StorageBucket"]!;
        var messagingSenderId = section["MessagingSenderId"]!;
        var appId = section["AppId"]!;
        var vapidKey = section["VapidKey"] ?? string.Empty;
        var fcmServerKey = section["FcmServerKey"] ?? string.Empty;
        var emulatorHost = section["EmulatorHost"];
        var authEmulatorHost = section["AuthEmulatorHost"];
        if (!string.IsNullOrEmpty(emulatorHost) && !string.IsNullOrEmpty(authEmulatorHost))
            return new FirebaseConfig(apiKey, authDomain, projectId, storageBucket, messagingSenderId, appId,
                vapidKey, fcmServerKey, emulatorHost, authEmulatorHost);
        if (!string.IsNullOrEmpty(emulatorHost))
            return new FirebaseConfig(apiKey, authDomain, projectId, storageBucket, messagingSenderId, appId,
                vapidKey, fcmServerKey, emulatorHost);
        return new FirebaseConfig(apiKey, authDomain, projectId, storageBucket, messagingSenderId, appId,
            vapidKey, fcmServerKey);
    }
}
