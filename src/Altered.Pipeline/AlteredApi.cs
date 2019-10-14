using Altered.Pipeline.Pipelines;
using Altered.Shared;
using Altered.Shared.Interfaces;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace Altered.Pipeline
{
    public class AlteredApiRequest : ApiRequest, IAlteredRequest
    {
        readonly Lazy<JwtSecurityToken> bearerToken;

        public AlteredApiRequest()
        {
            bearerToken = new Lazy<JwtSecurityToken>(() =>
            {
                if (Headers.TryGetValue(HeaderNames.Authorization, out StringValues authorization))
                {
                    var encodedToken = new string($"{authorization}".Skip(8).ToArray());
                    if (!string.IsNullOrEmpty(encodedToken))
                    {
                        var decodedToken = AlteredApi.ReadJwtToken(encodedToken);
                        return decodedToken;
                    }
                }
                return null;
            });
        }

        public string RequestId
        {
            get
            {
                Headers.TryGetValue(AlteredHeaderNames.CorrelationId, out StringValues correlationId);
                if (string.IsNullOrEmpty(correlationId))
                {
                    correlationId = $"{Guid.NewGuid()}";
                    RequestId = correlationId;
                }
                return correlationId;
            }
            set
            {
                Headers[AlteredHeaderNames.CorrelationId] = value;
            }
        }

        public JwtSecurityToken BearerToken
        {
            get => bearerToken.Value;
            set => throw new InvalidOperationException($"Cannot set {nameof(BearerToken)} of {nameof(AlteredApiRequest)}");
        }
    }

    public sealed class AlteredApiResponse : ApiResponse, IAlteredResponse
    {           
        public string RequestId
        {
            get
            {
                Headers.TryGetValue(AlteredHeaderNames.CorrelationId, out StringValues correlationId);
                return correlationId;
            }
            set
            {
                Headers[AlteredHeaderNames.CorrelationId] = value;
            }
        }

        public double RequestDuration
        {
            get
            {
                if (Headers.TryGetValue(AlteredHeaderNames.Duration, out StringValues duration))
                {
                    double.TryParse(duration, out double requestDuration);
                    return requestDuration;
                }
                return 0.0;
            }
            set
            {
                Headers[AlteredHeaderNames.Duration] = $"{value}";
            }
        }

        StatusCode IStatusCode.StatusCode
        {
            get => base.StatusCode;
            set => base.StatusCode = value;
        }
    }

    public class AlteredApi : AlteredPipeline<AlteredApiRequest, AlteredApiResponse>
    {
        static readonly JwtSecurityTokenHandler jwt = new JwtSecurityTokenHandler();
        
        public static JwtSecurityToken ReadJwtToken(string value)
        {
            lock (jwt)
            {
                var token = jwt.ReadJwtToken(value);
                return token;
            }
        }

        public AlteredApi(Func<AlteredApiRequest, Task<AlteredApiResponse>> api) : base(api
            .WithCopyRequestId()
            .WithMeasureRequestDuration())
        { }
    }
}
