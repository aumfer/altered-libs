using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Mvc.Components
{
    public static class AlteredHttpsExtensions
    {
        public static IWebHostBuilder UseAlteredHttps(this IWebHostBuilder builder) => builder
            .AlteredConfigure(app => app
                .UseHsts()
                .UseHttpsRedirection()
            );
    }
}
