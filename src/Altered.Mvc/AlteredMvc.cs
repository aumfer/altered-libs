using Microsoft.AspNetCore.Hosting;
using Altered.Mvc.Components;
using System.Reflection;
using Altered.Shared.Extensions;
using Serilog;
using System;
using Altered.Pipeline;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Altered.Mvc
{
    public sealed class AlteredMvc
    {
        // just use ASPNETCORE_URLS
        //public string[] Urls { get; set; } = { "http://localhost:5000", "https://localhost:5001" };

        public string AwsRegion { get; set; }

        public string CloudwatchLogGroup { get; set; }

        public AlteredSwagger Swagger { get; set; } = new AlteredSwagger();

        public Func<LoggerConfiguration, LoggerConfiguration> LoggerConfiguration { get; set; }

        public Action<IServiceCollection, MvcOptions> MvcOptions { get; set; }

        public Action<IServiceProvider, IRouteBuilder> RouteBuilder { get; set; }
    }

    public static class AlteredMvcExtensions
    {
        public static IWebHostBuilder UseAlteredMvc(this IWebHostBuilder builder, params Assembly[] assemblies) => builder
            .UseAlteredMvc(new AlteredMvc(), assemblies);
        public static IWebHostBuilder UseAlteredMvc(this IWebHostBuilder builder, AlteredMvc mvc, params Assembly[] assemblies) => builder
            .UseAlteredAppConfiguration()
            .EnsureAwsRegion(mvc.AwsRegion)
            .UseAlteredLog(mvc.CloudwatchLogGroup, mvc.LoggerConfiguration)
            .UseAlteredConfigure()
            .UseAlteredSwagger(mvc.Swagger)
            //.UseUrls(mvc.Urls)
            //.UseAlteredHttps()
            .UseAlteredMvcCore(mvc.MvcOptions, mvc.RouteBuilder, assemblies);
    }
}
