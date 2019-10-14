using Altered.Shared.Extensions;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Shared
{
    public static class AlteredLog
    {
        /// <remarks>
        /// Serializes an object with AlteredJson.DefaultJsonSerializer, and writes to ILogger
        /// </remarks>
        public static ILogger AlteredWrite(this ILogger log, object o, LogEventLevel logEventLevel)
        {
            dynamic msg = JObject.FromObject(o, AlteredJson.DefaultJsonSerializer)
                .ToObject<dynamic>();
            log.Write(logEventLevel, "{@log}", msg);
            return log;
        }

        /// <see cref="AlteredWrite"/>
        public static ILogger AlteredVerbose(this ILogger log, object o) => log.AlteredWrite(o, LogEventLevel.Verbose);
        /// <see cref="AlteredWrite"/>
        public static ILogger AlteredDebug(this ILogger log, object o) => log.AlteredWrite(o, LogEventLevel.Debug);
        /// <see cref="AlteredWrite"/>
        public static ILogger AlteredInformation(this ILogger log, object o) => log.AlteredWrite(o, LogEventLevel.Information);
        /// <see cref="AlteredWrite"/>
        public static ILogger AlteredWarning(this ILogger log, object o) => log.AlteredWrite(o, LogEventLevel.Warning);
        /// <see cref="AlteredWrite"/>
        public static ILogger AlteredError(this ILogger log, object o) => log.AlteredWrite(o, LogEventLevel.Error);
        /// <see cref="AlteredWrite"/>
        public static ILogger AlteredFatal(this ILogger log, object o) => log.AlteredWrite(o, LogEventLevel.Fatal);

        /// <see cref="AlteredWrite"/>
        public static ILogger Verbose(object o) => Log.Logger.AlteredVerbose(o);
        /// <see cref="AlteredWrite"/>
        public static ILogger Debug(object o) => Log.Logger.AlteredDebug(o);
        /// <see cref="AlteredWrite"/>
        public static ILogger Information(object o) => Log.Logger.AlteredInformation(o);
        /// <see cref="AlteredWrite"/>
        public static ILogger Warning(object o) => Log.Logger.AlteredWarning(o);
        /// <see cref="AlteredWrite"/>
        public static ILogger Error(object o) => Log.Logger.AlteredError(o);
        /// <see cref="AlteredWrite"/>
        public static ILogger Fatal(object o) => Log.Logger.AlteredFatal(o);
    }
}
