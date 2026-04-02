namespace DunIt.Web;

using DunIt.Core.Models;
using DunIt.Core.Repositories;
using DunIt.Core.Schedules;
using DunIt.Core.ViewModels;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<IChoreRepository, InMemoryChoreRepository>();
builder.Services.AddSingleton<IChildRepository, InMemoryChildRepository>();
builder.Services.AddTransient<DailyChoreViewModel>();
builder.Logging.SetMinimumLevel(LogLevel.Warning);

var host = builder.Build();

await SeedData(host.Services);

await host.RunAsync();

static async Task SeedData(IServiceProvider services)
{
    var choreRepo = services.GetRequiredService<IChoreRepository>();
    var childRepo = services.GetRequiredService<IChildRepository>();

    var alice = new Child("child-1", "Alice", "👧");
    var bob = new Child("child-2", "Bob", "👦");

    await childRepo.AddChild(alice);
    await childRepo.AddChild(bob);

    await choreRepo.AddChore(new Chore("chore-1", "Make bed", alice.Id, new DailySchedule()));
    await choreRepo.AddChore(new Chore("chore-2", "Brush teeth", alice.Id, new DailySchedule()));
    await choreRepo.AddChore(new Chore("chore-3", "Tidy room", alice.Id, new WeekdaysSchedule()));
    await choreRepo.AddChore(new Chore("chore-4", "Make bed", bob.Id, new DailySchedule()));
    await choreRepo.AddChore(new Chore("chore-5", "Brush teeth", bob.Id, new DailySchedule()));
}
