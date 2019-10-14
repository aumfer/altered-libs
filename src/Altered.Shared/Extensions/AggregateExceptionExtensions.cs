using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Altered.Shared.Extensions
{
    public static class AggregateExceptionExtensions
    {
        public static bool Has<T>(this Exception e) =>
            e is T || e.InnerException?.Has<T>() == true;

        public static bool Has<T>(this AggregateException e) =>
            e.InnerExceptions?.Any(ie =>
                ie is T || ie.InnerException?.Has<T>() == true) == true;

        public static T Get<T>(this Exception e)
            where T : Exception
        {
            var t = e as T;
            return t ?? e.InnerException?.Get<T>();
        }
        public static T Get<T>(this AggregateException e)
            where T : Exception =>
            e.InnerExceptions?.Select(ie => ie.Get<T>()).FirstOrDefault();
    }
}
