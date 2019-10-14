using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Altered.Shared.Extensions;
using System.Collections;
using Microsoft.Extensions.DependencyInjection;
using Altered.Shared;

namespace Altered.Mvc.Components
{
    public static class AlteredConfigureExtensions
    {
        public static IWebHostBuilder UseAlteredConfigure(this IWebHostBuilder builder) => builder
            .Configure(applicationBuilder =>
            {
                if (pendingConfigurations.TryRemove(builder, out List<Action<IApplicationBuilder>> configurations))
                {
                    foreach (var configuration in configurations)
                    {
                        configuration(applicationBuilder);
                    }

                    AlteredLog.Debug(new
                    {
                        Name = nameof(AlteredConfigureExtensions.UseAlteredConfigure),
                        Properties = applicationBuilder.Properties.Select(kvp => kvp.Key).ToList(),
                        Features = applicationBuilder.ServerFeatures.Select(kvp => kvp.Key.Name).ToList()
                    });
                }
            });

        public static IWebHostBuilder AlteredConfigure(this IWebHostBuilder builder, Action<IApplicationBuilder> configuration)
        {
            var configurations = pendingConfigurations.GetOrAdd(builder, _ => new List<Action<IApplicationBuilder>>());
            configurations.Add(configuration);
            return builder;
        }

        static readonly ConcurrentDictionary<IWebHostBuilder, List<Action<IApplicationBuilder>>> pendingConfigurations = new ConcurrentDictionary<IWebHostBuilder, List<Action<IApplicationBuilder>>>();
    }
}
