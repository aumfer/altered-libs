using Amazon.ECS;
using Amazon.ECS.Model;
using Altered.Aws;
using System;
using System.Collections.Generic;
using System.Text;
using Altered.Pipeline;

namespace Altered.Aws.Ecs
{
    public class DescribeTasks : AlteredPipeline<DescribeTasksRequest, DescribeTasksResponse>
    {
        public DescribeTasks(IAmazonECS ecs) : base(ecs.DescribeTasksAsync)
        {

        }
    }

    public static class DescribeTasksExtensions
    {
        public static DescribeTasks DescribeTasks(this IAmazonECS ecs) => new DescribeTasks(ecs);
    }
}
