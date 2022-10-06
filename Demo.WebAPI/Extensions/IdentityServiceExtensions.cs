
using Demo.WebAPI.Infrastructure.JwtAuth;
using Demo.WebAPI.Infrastructure.Policy;
using Demo.WebAPI.Infrastructure.Security;
using Demo.WebAPI.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

namespace Demo.WebAPI.Extensions
{
    public static class IdentityServiceExtensions
    {
       public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthorization(config =>
            {
                config.AddPolicy("User", options =>
                {
                    options.RequireAuthenticatedUser();
                    options.AuthenticationSchemes.Add(
                            JwtBearerDefaults.AuthenticationScheme);
                    options.Requirements.Add(new UserRequirement());
                });

                // Add a new Policy with requirement to check for Admin
                config.AddPolicy("Admin", options =>
                {
                    options.RequireAuthenticatedUser();
                    options.AuthenticationSchemes.Add(
                            JwtBearerDefaults.AuthenticationScheme);
                    options.Requirements.Add(new AdminRequirement());
                });
            });

            //Load appsettings
            var jwtTokenConfig = configuration.GetSection("jwtTokenConfig").Get<JwtTokenConfig>();
            services.AddSingleton(jwtTokenConfig);
            var SecurityConfig = configuration.GetSection("securityConfig").Get<SecurityConfig>();
            services.AddSingleton(SecurityConfig);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtTokenConfig.Issuer,
                    ValidAudience = jwtTokenConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),
                    TokenDecryptionKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtTokenConfig.Secret)),

                    ClockSkew = TimeSpan.FromMinutes(1)
                };
            });

            return services;
        }
    }
}
