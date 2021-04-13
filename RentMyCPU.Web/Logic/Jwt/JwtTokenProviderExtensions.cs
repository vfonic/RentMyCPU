using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentMyCPU.Backend.Logic.Jwt
{
    public static class JwtTokenProviderExtensions
    {
        public static IServiceCollection AddJwtTokenProvider(this IServiceCollection services, Action<JwtTokenProviderOptions> options = null)
        {
            services.Configure(options);
            services.AddTransient<IJwtTokenProvider, JwtTokenProvider>();
            return services;
        }
    }
}
