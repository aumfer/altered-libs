using System;

namespace Altered.Shared.Interfaces
{
    public interface IRequestDuration
    {
        // switched from TimeSpan to double (milliseconds) because...serilog
        double RequestDuration { get; set; }
    }
}
