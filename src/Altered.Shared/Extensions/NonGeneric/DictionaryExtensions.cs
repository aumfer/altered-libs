using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Altered.Shared.Extensions.NonGeneric
{
    public static class DictionaryExtensions
    {
        public static Dictionary<object, object> ToDictionary(this IDictionary dict)
        {
            var result = new Dictionary<object, object>(dict.Count);
            foreach (var k in dict.Keys)
            {
                var v = dict[k];
                result.Add(k, v);
            }
            return result;
        }
    }
}
