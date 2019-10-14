using Altered.Shared.Interfaces;

namespace Altered.Shared
{
    public interface IAlteredResponse :
        IRequestId,
        IStatusCode,
        IRequestDuration
    {
    }
}
