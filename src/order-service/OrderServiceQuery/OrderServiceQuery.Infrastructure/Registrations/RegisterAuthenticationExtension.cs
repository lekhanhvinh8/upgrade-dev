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
                    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler1>("ApiKey1", option => {
                        option.Events = new 
                        {
                            OnChallenge = 123
                        };
                    });

                services.AddAuthentication("ApiKey2")
                    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler2>("ApiKey2", null);


                services.AddAuthentication(o => {
                    o.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                    .AddJwtBearer("Bearer", option => 
                    {
                        option.Authority = "https://localhost:7128";
                        option.TokenValidationParameters = new TokenValidationParameters 
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateLifetime = false,
                            ValidateIssuerSigningKey = true,
                        };

                        option.Events = new JwtBearerEvents
                        {
                            OnChallenge = async context =>
                            {
                                await Task.CompletedTask;
                            }
                        };

                    });


                services.AddAuthentication(o => {
                    o.DefaultScheme = "Bearer1";
                })
                    .AddJwtBearer("Bearer1", option => 
                    {
                        option.Events = new JwtBearerEvents
                        {
                            OnChallenge = async context =>
                            {
                                await Task.CompletedTask;
                            },
                            OnTokenValidated = async context => 
                            {
                                await Task.CompletedTask;
                            }
                        };

                        option.Authority = "https://localhost:7128";
                        option.TokenValidationParameters = new TokenValidationParameters 
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateLifetime = false,
                            ValidateIssuerSigningKey = false,
                        };

                    });

                services.AddAuthentication(o => {
                    o.DefaultScheme = "Bearer2";
                })
                    .AddJwtBearer("Bearer2", option => 
                    {
                        option.Events = new JwtBearerEvents
                        {
                            OnChallenge = async context =>
                            {
                                await Task.CompletedTask;
                            },
                            OnTokenValidated = async context => 
                            {
                                await Task.CompletedTask;
                            }
                        };

                        option.Authority = "https://iam-stag.fpt.net";
                        option.TokenValidationParameters = new TokenValidationParameters 
                        {
                            ValidateAudience = false,
                            ValidateIssuer = false,
                            ValidateLifetime = false,
                            ValidateIssuerSigningKey = false,
                            IssuerSigningKeyResolver = (token, securityToken, kid, parameters) => 
                            {
                                HttpClient httpClient = new HttpClient();
                                var httpResponse = httpClient.GetAsync("https://iam-stag.fpt.net/auth/realms/fpt/protocol/openid-connect/certs").Result;
                                string responseBody = httpResponse.Content.ReadAsStringAsync().Result;

                                var newCachedValue = responseBody;

                                var jwts = new JsonWebKeySet(newCachedValue);
                                return jwts.Keys;
                            }
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