using Altered.Pipeline;
using Amazon.AutoScaling;
using Amazon.CloudWatch;
using Amazon.CloudWatchLogs;
using Amazon.CodeBuild;
using Amazon.CodePipeline;
using Amazon.ECS;
using Amazon.ElasticLoadBalancing;
using Amazon.ElasticLoadBalancingV2;
using Amazon.S3;
using Amazon.SimpleSystemsManagement;
using Elasticsearch.Net;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Aws
{
    public static class AddAlteredAwsExtensions
    {
        public static IServiceCollection AddAlteredAws(this IServiceCollection services, Uri elasticSearchUri = null)
        {
            services = services
                .EnsureAwsRegion()
            .AddTransient<IAmazonS3, AmazonS3Client>()
            .AddTransient<IAmazonElasticLoadBalancingV2, AmazonElasticLoadBalancingV2Client>()
            // this is declaring an empty pipeline
            // inject in and call extension methods
            // to define service-specific pipelines
            .AddSingleton<Alb.DescribeTags>()
            .AddTransient<IAmazonAutoScaling, AmazonAutoScalingClient>()
            .AddTransient<IAmazonECS, AmazonECSClient>()
            .AddSingleton<Ecs.DescribeTasks>()
            .AddTransient<IAmazonElasticLoadBalancing, AmazonElasticLoadBalancingClient>()
            .AddSingleton<Elb.DescribeTags>()
            .AddTransient<IAmazonCloudWatch, AmazonCloudWatchClient>()
            .AddTransient<IAmazonCloudWatchLogs, AmazonCloudWatchLogsClient>()
            .AddTransient<IAmazonCodeBuild, AmazonCodeBuildClient>()
            .AddTransient<IAmazonCodePipeline, AmazonCodePipelineClient>()
            .AddTransient<IAmazonSimpleSystemsManagement, AmazonSimpleSystemsManagementClient>();

            if (elasticSearchUri != null)
            {
                services = services
                    .AddSingleton<IConnectionConfigurationValues>(new ConnectionConfiguration(elasticSearchUri))
                    .AddTransient<IElasticLowLevelClient, ElasticLowLevelClient>();
            }

            return services;
        }
    }
}
