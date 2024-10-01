
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrderServiceQuery.Core.Domain;
using OrderServiceQuery.Core.Event;
using OrderServiceQuery.Core.EventHandler;
using OrderServiceQuery.Core.Repositories;
using OrderServiceQuery.Infrastructure.UnitOfWork;

namespace OrderServiceQuery.Infrastructure.Authentication
{
    public class ApiKeyAuthenticationHandler1 : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public ApiKeyAuthenticationHandler1(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder
            ) 
            : base(options, logger, encoder) { }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            Console.WriteLine("AuthenSchema1");
            if (!Request.Headers.TryGetValue("X-Api-Key1", out var apiKey))
            {
                return AuthenticateResult.NoResult();
            }

            // Validate the API key (replace with your logic)
            if (apiKey != "your-api-key") // Replace with your actual API key validation logic
            {
                return AuthenticateResult.Fail("Invalid API Key");
            }

            // Create claims and identity
            var claims = new[] { new Claim(ClaimTypes.Name, "ApiUser") };
            var identity = new ClaimsIdentity(claims, nameof(ApiKeyAuthenticationHandler1));
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "ApiKey");

            return AuthenticateResult.Success(ticket);
        }
    }
}