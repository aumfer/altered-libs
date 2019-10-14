using Altered.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Altered.Shared.Extensions;
using Altered.Mvc.Components;
using Altered.Pipeline.Serilog;
using Altered.Pipeline;
using Destructurama;

namespace Altered.Mvc.Components
{
    public static class AlteredLogExtensions
    {
        public static IWebHostBuilder UseAlteredLog(this IWebHostBuilder builder, string cloudwatchLogGroup = null, Func<LoggerConfiguration, LoggerConfiguration> configureLog = null) => builder
            .ConfigureServices((context, serviceCollection) =>
            {
                configureLog = configureLog ?? (lc => lc);

                serviceCollection.AddAlteredLog(cloudwatchLogGroup, loggerConfiguration => configureLog(loggerConfiguration
                    .Enrich.WithEnvironmentVariable("env", "ASPNETCORE_ENVIRONMENT")
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .ReadFrom.Configuration(context.Configuration)));
            })
            .UseSerilog()
            .AlteredConfigure(app => app
                //.Use(AlteredMiddlewareExtensions.AlteredMiddleware)
                .Use(AlteredLogMiddlewareExtensions.AlteredLogMiddleware)
            );
    }
}
