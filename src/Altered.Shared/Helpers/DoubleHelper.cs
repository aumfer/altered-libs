using System;
using System.Collections.Generic;
using System.Text;

namespace Altered.Shared.Helpers
{
    public static class DoubleHelper
    {
        public static double? TryParse(string s) => double.TryParse(s, out double v) ? v : (double?)null;
    }
}
