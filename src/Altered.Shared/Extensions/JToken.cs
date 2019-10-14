using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altered.Shared.Extensions
{
    public static class JTokenExtensions
    {
        public static JToken ReplacePath(this JToken root, string pointer, JToken replacement)
        {
            var pointers = pointer.Split('.');

            JToken t = root;
            for (int i = 0; i < pointers.Length; ++i)
            {
                var p = pointers[i];

                JToken c = t.SelectToken(p);
                if (c == null)
                {
                    c = new JObject();

                    JContainer o = t as JContainer;
                    o.Add(new JProperty(p, c));
                }
                t = c;
            }
            t.Replace(replacement);
            return t;
        }

        public static T WithProperty<T>(this T token, string name, JToken value)
            where T : JToken
        {
            token[name] = value;
            return token;
        }
    }
}
