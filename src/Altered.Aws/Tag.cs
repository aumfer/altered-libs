using System.Collections.Generic;
using System.Linq;

namespace Altered.Aws
{
    public static class TagExtensions
    {
        public static string GetValue(this IEnumerable<Amazon.AutoScaling.Model.Tag> tags, string key) => tags
            .FirstOrDefault(t => t.Key == key)?.Value;

        public static string GetValue(this IEnumerable<Amazon.ECS.Model.Tag> tags, string key) => tags
            .FirstOrDefault(t => t.Key == key)?.Value;

        public static string GetValue(this IEnumerable<Amazon.ElasticLoadBalancing.Model.Tag> tags, string key) => tags
            .FirstOrDefault(t => t.Key == key)?.Value;

        public static string GetValue(this IEnumerable<Amazon.CodeBuild.Model.Tag> tags, string key) => tags
            .FirstOrDefault(t => t.Key == key)?.Value;

        public static string GetValue(this IEnumerable<Amazon.AutoScaling.Model.TagDescription> tags, string key) => tags
            .FirstOrDefault(t => t.Key == key)?.Value;

        public static string GetValue(this IEnumerable<Amazon.ElasticLoadBalancing.Model.TagDescription> tagDescriptions, string key) =>
            (from description in tagDescriptions
             from tag in description.Tags
             where tag.Key == key
             select tag.Value).FirstOrDefault();

        public static string GetValue(this IEnumerable<Amazon.ElasticLoadBalancingV2.Model.TagDescription> tagDescriptions, string key) =>
            (from description in tagDescriptions
             from tag in description.Tags
             where tag.Key == key
             select tag.Value).FirstOrDefault();
    }
}
