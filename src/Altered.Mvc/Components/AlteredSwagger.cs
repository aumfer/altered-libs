using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using System.Collections.Generic;

namespace Altered.Mvc.Components
{
    public sealed class AlteredSwagger
    {
        public string Name { get; set; } = "Altered";

        public string Version { get; set; } = "v1";

        public string Description { get; set; } = "Altered API";

        public string SwaggerUrl => $"/swagger/{Version}/swagger.json";

        public Dictionary<string, SecurityScheme> SecuritySchemes { get; set; } = new Dictionary<string, SecurityScheme>
        { 
            { "Bearer", new ApiKeyScheme
                    {
                        Description = "JWT",
                        Name = "Authorization",
                        In = "header",
                    }
            },
            { "OAuth", new OAuth2Scheme
                    {
                        Type = "oauth2",
                        Flow = "implicit",
                        AuthorizationUrl = "https://uat.auth.coxautoinc.com/authorize",
                        Scopes = new Dictionary<string, string>
                        {
                            { "org.ns.carvim.read", "Access read operations" },
                            { "org.ns.carvim.write", "Access write operations" }
                        }
                    }
            }
        };

        public Dictionary<string, IEnumerable<string>> SecurityRequirements { get; set; } = new Dictionary<string, IEnumerable<string>>
        {
            { "Bearer", new string[]{ } }
        };

        public string OAuth2RedirectUrl { get; set; }

        public bool OAuthUseBasicAuthenticationWithAccessCodeGrant { get; set; }

        public string OAuthClientId { get; set; }
    }

    public static class AlteredSwaggerExtensions
    {
        public static IWebHostBuilder UseAlteredSwagger(this IWebHostBuilder builder, AlteredSwagger alteredSwagger) => builder
            .ConfigureServices((context, services) => services
                .AddSwaggerGen(swaggerGen =>
                {
                    foreach (var kvp in alteredSwagger.SecuritySchemes)
                    {
                        swaggerGen.AddSecurityDefinition(kvp.Key, kvp.Value);
                    }

                    swaggerGen.AddSecurityRequirement(alteredSwagger.SecurityRequirements);

                    var info = new Info
                    {
                        Title = alteredSwagger.Name,
                        Version = alteredSwagger.Version,
                        Description = alteredSwagger.Description,
                    };
                    swaggerGen.SwaggerDoc(alteredSwagger.Version, info);
                })
            )
            .AlteredConfigure(app => app
                .UseSwagger()
                .UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint(alteredSwagger.SwaggerUrl, alteredSwagger.Name);
                    c.RoutePrefix = string.Empty;

                    c.DisplayOperationId();
                    c.DisplayRequestDuration();
                    c.EnableDeepLinking();
                    c.EnableValidator();

                    if (alteredSwagger.OAuth2RedirectUrl != null)
                    {
                        c.OAuth2RedirectUrl(alteredSwagger.OAuth2RedirectUrl);
                    }
                    if (alteredSwagger.OAuthUseBasicAuthenticationWithAccessCodeGrant)
                    {
                        c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
                    }
                    if (alteredSwagger.OAuthClientId != null)
                    {
                        c.OAuthClientId(alteredSwagger.OAuthClientId);
                    }
                }));
    }
}
