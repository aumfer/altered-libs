using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace Altered.Mvc.Components
{
    public static class AlteredAppConfigurationExtensions
    {
        public static IWebHostBuilder UseAlteredAppConfiguration(this IWebHostBuilder builder) => builder
            .ConfigureAppConfiguration((context, configurationBuilder) => configurationBuilder
                .AddJsonFile(AppSettingsJson, optional: true)
                .AddEnvironmentVariables()
            );

        static readonly string AppSettingsJson = "appsettings.json";
    }
}
