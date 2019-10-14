using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Shared
{
    public static class AlteredConsole
    {
        // need to return a value so we can use in linq expressions
        public static int WriteLine(string line)
        {
            Console.WriteLine(line);
            return line?.Length ?? -1;
        }
    }
}
