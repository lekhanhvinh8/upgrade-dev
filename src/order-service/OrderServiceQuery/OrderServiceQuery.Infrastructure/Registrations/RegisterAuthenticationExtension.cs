using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using OrderServiceQuery.Core.Repositories;
using OrderServiceQuery.Infrastructure.Authentication;
using OrderServiceQuery.Infrastructure.DatabaseContext;

namespace OrderServiceQuery.Infrastructure.Registrations
{
    public static class RegisterAuthenticationExtension
    {
        public static IServiceCollection RegisterAuthentication(this IServiceCollection services)
        {
            try
            {
                services.AddAuthentication("ApiKey1")
                    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler1>("ApiKey1", null);

                services.AddAuthentication("ApiKey2")
                    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler2>("ApiKey2", null);

                services.AddAuthentication(o => {
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                    .AddJwtBearer("Bearer", option => 
                    {
                        option.Authority = "https://localhost:5144";
                        option.TokenValidationParameters = new TokenValidationParameters 
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateLifetime = false,


                            //By pass the signature validation
                            // SignatureValidator = delegate (string token, TokenValidationParameters parameters)
                            // {
                            //     var jwt = new JsonWebToken(token);
                            //     return jwt;
                            // },

                            // Do signature validation
                            ValidateIssuerSigningKey = true,
                        };
                    });

            }
            catch (Exception ex)
            {
                Console.WriteLine("ex: " + ex.Message + ", StackTrace: " + ex.StackTrace);
            }

            return services;
        }
    }
}