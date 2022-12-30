﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ZiziBot.NgDashboard;

public static class DashboardLoaderExtension
{
    public static IServiceCollection AddNgDashboard(this IServiceCollection services)
    {
        services.AddSpaStaticFiles(configuration => { configuration.RootPath = Env.DASHBOARD_DIST_PATH; });

        return services;
    }

    public static WebApplication UseNgDashboard(this WebApplication app)
    {
        app.UseStaticFiles();

        if (!app.Environment.IsDevelopment())
        {
            app.UseSpaStaticFiles();
        }

        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = Env.DASHBOARD_PROJECT_PATH;
            if (app.Environment.IsDevelopment())
            {
                spa.UseAngularCliServer(npmScript: "start");
            }
        });


        return app;
    }
}