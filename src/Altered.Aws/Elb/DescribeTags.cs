using Altered.Shared;
using Amazon.ElasticLoadBalancing;
using Amazon.ElasticLoadBalancing.Model;
using Altered.Aws;
using System;
using System.Collections.Generic;
using System.Text;
using Altered.Pipeline;

namespace Altered.Aws.Elb
{
    public class DescribeTags : AlteredPipeline<DescribeTagsRequest, DescribeTagsResponse>
    {
        public DescribeTags(IAmazonElasticLoadBalancing alb) : base(alb.DescribeTagsAsync) { }

        public DescribeTags(IAlteredPipeline<DescribeTagsRequest, DescribeTagsResponse> describeTags) : base(describeTags
            // default cache of 5 mins for tags
            .WithAlteredCache(getKey: request => AlteredJson.SerializeObject(request.LoadBalancerNames)))
        { }
    }

    public static class DescribeTagsExtensions
    {
        public static DescribeTags DescribeTags(this IAmazonElasticLoadBalancing alb) => new DescribeTags(alb);

        public static DescribeTags ToDescribeTags(this IAlteredPipeline<DescribeTagsRequest, DescribeTagsResponse> describeTags) =>
            new DescribeTags(describeTags);
    }
}
