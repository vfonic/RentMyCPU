using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace RentMyCPU.Backend.Logic.Jwt
{
    public interface IJwtTokenProvider
    {
        JwtTokenCreationResult CreateAccessToken(ClaimsIdentity identity, TimeSpan? expiration = null);
    }
}
