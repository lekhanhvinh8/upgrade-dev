
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace MonitorService.Configurations
{
    public static class AddAppAuthenticationExtension
    {
        public static IServiceCollection AddAppAuthentication(this IServiceCollection services)
        {
                services.AddAuthentication(options =>
                {
                    // Default authentication scheme is Cookies
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    
                    // Default challenge scheme for OIDC
                    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme) // Store auth state in cookies
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, options =>
                {
                    // OIDC config
                    options.Authority = "https://localhost:5144"; // Identity provider URL
                    options.ClientId = "monitor-service"; // Client ID
                    options.ClientSecret = "secret"; // Client Secret
                    options.UsePkce = false;
                    
                    // Response type for Authorization Code flow
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    
                    // Scopes to request
                    //options.Scope.Clear();
                    //options.Scope.Add("apiscope");

                    // Save tokens in the authentication cookie
                    options.SaveTokens = true;

                    // Redirect after login
                    //options.CallbackPath = "/";

                    //https://localhost:7240/signin-oidc

                    // Configure claim mapping (e.g., map 'sub' to ClaimTypes.NameIdentifier)
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true
                    };

                    // Optional: Handle token expiration, logging, etc.
                    options.Events = new OpenIdConnectEvents
                    {
                        OnTokenValidated = context =>
                        {
                            // Handle the token after validation if needed
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            context.HandleResponse();
                            context.Response.Redirect("/Error?message=" + context.Exception.Message);
                            return Task.FromResult(0);
                        },
                    };
                });

            return services;
        }
    }
}