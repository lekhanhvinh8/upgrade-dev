using System.Security.Claims;
using Duende.IdentityServer.Validation;

namespace IAMService
{
    public class TransactionScopeTokenRequestValidator : ICustomTokenRequestValidator
    {
        public async Task ValidateAsync(CustomTokenRequestValidationContext context)
        {
            
                // emit transaction id as a claim
            context?.Result?.ValidatedRequest.ClientClaims.Add(
                new Claim("myCustomClaim", "myValue"));

        }
    }

}