using System;

namespace RentMyCPU.Backend.Logic.Jwt
{
    public class JwtTokenCreationResult
    {
        public string Token { get; set; }
        public TimeSpan Expire { get; set; }
    }
}
