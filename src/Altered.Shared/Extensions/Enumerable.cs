using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Shared.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Use<T>(this T t) where T : IDisposable
        {
            try
            {
                yield return t;
            }
            finally
            {
                t?.Dispose();
            }
        }
    }
}
