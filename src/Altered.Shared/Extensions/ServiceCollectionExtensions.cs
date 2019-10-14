using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Shared.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /*public static IServiceCollection AddAlteredSingleton<TResult, TDep1>(this IServiceCollection services, Func<TDep1, TResult> factory) => services
            .AddSingleton(sp =>
            {
                var d1 = sp.GetService<TDep1>();
                var svc = factory(d1);
                return svc;
            });*/
    }
}
