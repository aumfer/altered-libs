using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altered.Shared.Extensions
{
    public static class JObjectExtensions
    {
        public static IEnumerable<(string, T)> AlteredProperties<T>(this JObject o) =>
            from p in o.Properties()
            select (p.Name, p.Value<T>());

        public static JObject WithRemove(this JObject @object, string name)
        {
            @object.Remove(name);
            return @object;
        }
    }
}
