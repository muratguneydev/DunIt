using DunIt.Core.Auth;
using DunIt.Core.Firebase;
using DunIt.Core.Notifications;
using DunIt.Core.Repositories;
using DunIt.Core.ViewModels;
using DunIt.Web;
using DunIt.Web.Firebase; // JsFirebaseInterop, FirebaseConfig
using DunIt.Web.Notifications;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using Sentry;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var firebaseConfig = FirebaseConfig.From(builder.Configuration.GetSection("Firebase"));

builder.Services.AddSingleton<IFirebaseAppSettings>(firebaseConfig);
builder.Services.AddSingleton<IFirebaseEmulatorSettings>(firebaseConfig);
builder.Services.AddSingleton<IFirebaseInterop, JsFirebaseInterop>();
builder.Services.AddSingleton<IUserContext, UserContext>();
builder.Services.AddSingleton<IChoreRepository, FirebaseChoreRepository>();
builder.Services.AddSingleton<IChildRepository, FirebaseChildRepository>();
builder.Services.AddSingleton<ILocalStorage, JsLocalStorage>();
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<ReminderTimer>();
builder.Services.AddSingleton<ReminderSettingsService>();
builder.Services.AddSingleton<ReminderScheduler>();
builder.Services.AddTransient<DailyChoreViewModel>();
builder.Services.AddTransient<WeeklyReportViewModel>();
builder.Services.AddTransient<AdminViewModel>();
builder.Services.AddTransient<ReminderSettingsViewModel>();
builder.Logging.SetMinimumLevel(LogLevel.Warning);

builder.UseSentry(options =>
{
    options.Dsn = "https://c5433c7540fcca8d8477b31f51a41c71@o4511320192909312.ingest.de.sentry.io/4511320204312656";
    options.Environment = builder.HostEnvironment.Environment;
});

await builder.Build().RunAsync();
