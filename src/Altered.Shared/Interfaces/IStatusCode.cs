using Newtonsoft.Json;
using System.Net;

namespace Altered.Shared.Interfaces
{
    public interface IStatusCode
    {
        [JsonProperty()]
        StatusCode StatusCode { get; set; }
    }
}
