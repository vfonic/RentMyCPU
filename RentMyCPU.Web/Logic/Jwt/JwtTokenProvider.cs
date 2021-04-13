using System;
using System.Collections.Generic;

using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt; 

namespace RentMyCPU.Backend.Logic.Jwt
{
    public class JwtTokenProvider : IJwtTokenProvider
    {
        private readonly JwtTokenProviderOptions _configuration;
        public JwtTokenProvider(IOptions<JwtTokenProviderOptions> options)
        {
            _configuration = options.Value;
        }
        public JwtTokenCreationResult CreateAccessToken(ClaimsIdentity identity, TimeSpan? expiration = null)
        {
            var claims = CreateJwtClaims(identity);
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? _configuration.Expiration),
                signingCredentials: _configuration.SigningCredentials
            );

            return new JwtTokenCreationResult()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken),
                Expire = _configuration.Expiration
            };
        }

        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            });

            return claims;
        }
    }
}
