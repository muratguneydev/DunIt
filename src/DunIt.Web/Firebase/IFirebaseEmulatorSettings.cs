namespace DunIt.Web.Firebase;

public interface IFirebaseEmulatorSettings
{
    string EmulatorHost { get; }
    string AuthEmulatorHost { get; }
    bool IsUsingEmulator { get; }
}
