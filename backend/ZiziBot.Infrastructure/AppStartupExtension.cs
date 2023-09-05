using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ZiziBot.Infrastructure;

public static class AppStartupExtension
{
    public static IApplicationBuilder PrintAbout(this IApplicationBuilder app)
    {
        var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(AppStartupExtension));

        logger.LogInformation("ZiziBot 5");
        logger.LogInformation("Version: {Version}", VersionUtil.GetVersionNumber());

        return app;
    }
}