namespace DunIt.IntegrationTests;

using Microsoft.Playwright;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

public static class PlaywrightTracing
{
    public static Task Start(IBrowserContext context) =>
        context.Tracing.StartAsync(new() { Screenshots = true, Snapshots = true });

    public static async Task Stop(IBrowserContext context, IPage page, string testName, bool failed)
    {
        await context.Tracing.StopAsync(new() { Path = $"/results/traces/{testName}.zip" });

        if (failed)
            await page.ScreenshotAsync(new() { Path = $"/results/screenshots/{testName}.png", FullPage = true });
    }
}
