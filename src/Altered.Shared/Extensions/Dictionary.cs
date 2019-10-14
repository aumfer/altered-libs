using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Altered.Shared.Extensions
{
    public static class DictionaryExtensions
    {
        /*public static T WithAdd<T, TKey, TValue>(this T dict, TKey key, TValue value)
            where T : IDictionary<TKey, TValue>
        {
            dict.Add(key, value);
            return dict;
        }
        public static T WithRemove<T, TKey, TValue>(this T dict, TKey key)
            where T : IDictionary<TKey, TValue>
        {
            dict.Remove(key);
            return dict;
        }*/
        public static Dictionary<TKey, TValue> WithAdd<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            dict.Add(key, value);
            return dict;
        }
        public static Dictionary<TKey, TValue> WithRemove<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
        {
            dict.Remove(key);
            return dict;
        }

        public static Dictionary<TKey, TValue> WithSet<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue value)
        {
            dict[key] = value;
            return dict;
        }

        /// <summary>
		/// Converts a collection of key-value pairs into a query string parameter string.
		/// </summary>
		/// <param name="dictionary">A collection of key value pairs</param>
		/// <returns>A query string parameter string</returns>
		public static string ToQueryString(this IEnumerable<KeyValuePair<string, string>> dictionary)
        {
            List<KeyValuePair<string, string>> kvps = dictionary.ToList() ?? throw new ArgumentNullException(nameof(dictionary));

            if (!kvps.Any())
            {
                return string.Empty;
            }

            var queryStringBuilder = kvps.Aggregate(new StringBuilder().Append('?'), (sb, kvp) =>
            {
                var key = WebUtility.UrlEncode(kvp.Key);
                var value = WebUtility.UrlEncode(kvp.Value);
                return sb.Append(key).Append('=').Append(value).Append('&');
            });
            var queryString = queryStringBuilder.ToString(0, queryStringBuilder.Length - 1);
            return queryString;
        }

        public static TValue GetValueOrNull<TKey, TValue>(this IDictionary<TKey, TValue> d, TKey key)
            where TValue : class
        {
            d.TryGetValue(key, out TValue v);
            return v;
        }

        public static IDictionary<TKey, TValue> Upsert<TKey, TValue>(this IDictionary<TKey, TValue> into, IDictionary<TKey, TValue> from)
        {
            foreach (var kvp in from)
            {
                into[kvp.Key] = kvp.Value;
            }
            return into;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDictionary<TKey, TValue> dict) => dict.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        public static IEnumerable<(TKey, TValue)> ToTuple<TKey, TValue>(this IDictionary<TKey, TValue> dict) => dict.Select(kvp => (kvp.Key, kvp.Value));
    }
}
