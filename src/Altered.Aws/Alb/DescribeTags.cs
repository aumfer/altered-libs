using Altered.Shared;
using Amazon.ElasticLoadBalancingV2;
using Amazon.ElasticLoadBalancingV2.Model;
using Altered.Aws;
using System;
using System.Collections.Generic;
using System.Text;
using Altered.Pipeline;

namespace Altered.Aws.Alb
{
    public class DescribeTags : AlteredPipeline<DescribeTagsRequest, DescribeTagsResponse>
    {
        public DescribeTags(IAmazonElasticLoadBalancingV2 alb) :
            base(alb.DescribeTagsAsync) { }

        public DescribeTags(IAlteredPipeline<DescribeTagsRequest, DescribeTagsResponse> describeTags) :
            base(describeTags
                // default cache of 5 mins for tags
                .WithAlteredCache(getKey: request => AlteredJson.SerializeObject(request.ResourceArns))) { }
    }

    public static class DescribeTagsExtensions
    {
        public static DescribeTags DescribeTags(this IAmazonElasticLoadBalancingV2 alb) => new DescribeTags(alb);

        public static DescribeTags AsDescribeTags(this IAlteredPipeline<DescribeTagsRequest, DescribeTagsResponse> describeTags) =>
            new DescribeTags(describeTags);
    }

}
