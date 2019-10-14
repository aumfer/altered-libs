using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Aws
{
    public static class AlteredEnvironment
    {
        public static readonly string App = Environment.GetEnvironmentVariable("app");
        public static readonly string Env = Environment.GetEnvironmentVariable("env");
        public static readonly string Sha = Environment.GetEnvironmentVariable("sha");
        public static readonly string Name = $"{App}-{Env}";
    }
}
