using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Altered.Pipeline
{
    public interface IBearerToken
    {
        JwtSecurityToken BearerToken { get; set; }
    }
}
