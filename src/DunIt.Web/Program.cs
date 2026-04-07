using DunIt.Core.Auth;
using DunIt.Core.Firebase;
using DunIt.Core.Repositories;
using DunIt.Core.ViewModels;
using DunIt.Web;
using DunIt.Web.Firebase;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var firebaseConfig = FirebaseConfig.From(builder.Configuration.GetSection("Firebase"));

builder.Services.AddSingleton<IFirebaseAppSettings>(firebaseConfig);
builder.Services.AddSingleton<IFirebaseEmulatorSettings>(firebaseConfig);
builder.Services.AddSingleton<IFirebaseInterop, JsFirebaseInterop>();
builder.Services.AddSingleton<IAuthService, FirebaseAuthService>();
builder.Services.AddSingleton<IChoreRepository, FirebaseChoreRepository>();
builder.Services.AddSingleton<IChildRepository, FirebaseChildRepository>();
builder.Services.AddTransient<DailyChoreViewModel>();
builder.Services.AddTransient<AdminViewModel>();
builder.Logging.SetMinimumLevel(LogLevel.Warning);

await builder.Build().RunAsync();
