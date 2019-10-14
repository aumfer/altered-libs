using Altered.Shared.Extensions;
using Destructurama;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Pipeline
{
    public static class AddAlteredLogExtensions
    {
        public static IServiceCollection AddAlteredLog(this IServiceCollection services, string logGroupName = null, Func<LoggerConfiguration, LoggerConfiguration> configure = null)
        {
            var loggerConfiguration = new LoggerConfiguration()
                .WithAlteredDefault()
                .Destructure.JsonNetTypes();

            if (configure != null)
            {
                loggerConfiguration = configure(loggerConfiguration);
            }

            Log.Logger = loggerConfiguration
                .CreateLogger();
            return services;
        }
    }
}
