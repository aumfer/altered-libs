using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Altered.Shared.Extensions;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Altered.Shared;
using System;
using Microsoft.AspNetCore.Routing;

namespace Altered.Mvc.Components
{
    public static class AlteredMvcCoreExtensions
    {
        public static IWebHostBuilder UseAlteredMvcCore(this IWebHostBuilder builder, Action<IServiceCollection, MvcOptions> mvcOptions = null, Action<IServiceProvider, IRouteBuilder> routeBuilder = null, params Assembly[] assemblies) => builder
            .ConfigureServices(services => services
                .AddHttpContextAccessor()
                .AddCors()
                .AddMvcCore(options => mvcOptions?.Invoke(services, options))
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddApiExplorer() // todo AddVersionedApiExplorer
                .AddApplicationParts(assemblies)
                .AddControllersAsServices()
                .AddJsonFormatters()
                .AddJsonOptions(json =>
                    json.SerializerSettings.MakeDefault())
                // this breaks with jobject[] in request/response
                //.AddXmlSerializerFormatters()
            )
            .AlteredConfigure(app => app
                .UseCors(cors => cors
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowAnyOrigin()
                    .WithExposedHeaders(
                        HeaderNames.Location,
                        HeaderNames.ETag
                    )
                )
                .UseMvc(routes => routeBuilder?.Invoke(app.ApplicationServices, routes))
                .UseStaticFiles()
            );

        static IMvcCoreBuilder AddApplicationParts(this IMvcCoreBuilder mvc, IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                mvc = mvc.AddApplicationPart(assembly);
            }

            return mvc;
        }
    }
}
