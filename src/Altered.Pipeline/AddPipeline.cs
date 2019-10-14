using Altered.Shared;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Altered.Pipeline
{
    public static class ServiceCollectionExtensions
    {
        /*public static IServiceCollection AddPipeline<TInterface, TRequest, TResponse>(this IServiceCollection services,
            Func<TInterface, Func<TRequest, Task<TResponse>>> operationFactory,
            Func<Func<TRequest, Task<TResponse>>, Func<TRequest, Task<TResponse>>> pipelineFactory = null) =>
            services.AddPipeline<TInterface, Func<TRequest, Task<TResponse>>>(operationFactory, pipelineFactory);*/

        public static IServiceCollection AddPipeline<TInterface, TOperation>(this IServiceCollection services,
            Func<TInterface, TOperation> operationFactory,
            Func<TOperation, TOperation> pipelineFactory = null) where TOperation : class => services
            .AddSingleton(sp =>
            {
                var svc = sp.GetService<TInterface>();
                var op = operationFactory(svc);
                if (pipelineFactory != null)
                {
                    op = pipelineFactory(op);
                }
                return op;
            });

        public static IServiceCollection AddPipeline<TInterface, TOperation, TService1>(this IServiceCollection services,
            Func<TInterface, TOperation> operationFactory,
            Func<TOperation, TService1, TOperation> pipelineFactory = null) where TOperation : class => services
            .AddSingleton(sp =>
            {
                var svc = sp.GetService<TInterface>();
                var op = operationFactory(svc);
                if (pipelineFactory != null)
                {
                    var svc1 = sp.GetService<TService1>();
                    op = pipelineFactory(op, svc1);
                }
                return op;
            });

        public static IServiceCollection AddPipeline<TInterface, TOperation, TService1, TService2>(this IServiceCollection services,
            Func<TInterface, TOperation> operationFactory,
            Func<TOperation, TService1, TService2, TOperation> pipelineFactory = null) where TOperation : class => services
            .AddSingleton(sp =>
            {
                var svc = sp.GetService<TInterface>();
                var op = operationFactory(svc);
                if (pipelineFactory != null)
                {
                    var svc1 = sp.GetService<TService1>();
                    var svc2 = sp.GetService<TService2>();
                    op = pipelineFactory(op, svc1, svc2);
                }
                return op;
            });
    }
}
