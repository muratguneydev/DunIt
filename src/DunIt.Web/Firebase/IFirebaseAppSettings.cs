namespace DunIt.Web.Firebase;

public interface IFirebaseAppSettings
{
    string ApiKey { get; }
    string AuthDomain { get; }
    string ProjectId { get; }
    string StorageBucket { get; }
    string MessagingSenderId { get; }
    string AppId { get; }
}
