using System.Security.Claims;
using Duende.IdentityServer;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Test;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Identity;

namespace IAMService
{
    public class CustomProfileService : IProfileService
    {
        private readonly UserManager<IdentityUser> _userManager;


        public CustomProfileService(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        // GetProfileDataAsync is what controls what claims are issued in the response
        // the sample code below shows *many* different approaches, and you can adjust 
        // these based on your needs and requirements.
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await _userManager.GetUserAsync(context.Subject);
         
            if (context.RequestedClaimTypes.Any())
            {
               
                context.AddRequestedClaims(context.Subject.Claims);
              
                context.AddRequestedClaims(new List<Claim>(){new Claim("a", "b")});
            }

          
            if (!context.IssuedClaims.Any(x => x.Type == "picture"))
            {
                var picture = context.Subject.FindFirst("picture");
                if (picture != null)
                {
                    context.IssuedClaims.Add(picture);
                }
            }

            // OPTION 3: always emit claims based on client (regardless of the requested claims)
            // context.Client holds the client making the request
            if (context.Client.ClientId == "client1")
            {
                // sample adding a tenant claim based on the client obtaining the tokens
                context.IssuedClaims.Add(new Claim("tenant", "tenant1"));
            }

            // OPTION 4: always emit claims based on the token (regardless of the requested claims)
            // context.Caller describes why the claims are needed (access token, id token, userinfo endpoint)
            if (context.Caller == IdentityServerConstants.ProfileDataCallers.ClaimsProviderAccessToken)
            {
                // sample adding a tenant claim based on the type of token
                context.IssuedClaims.Add(new Claim("foo", "bar"));

                context.IssuedClaims.Add(new Claim("username_access", user.UserName!));
            }

            context.IssuedClaims.Add(new Claim("username", user.UserName!));

            await Task.CompletedTask;
        }

        // IsActiveAsync is called to ask your custom logic if the user is still "active".
        // If the user is not "active" then no new tokens will be created for them, even 
        // if the user has an active session with IdentityServer.
        public Task IsActiveAsync(IsActiveContext context)
        {
            // as above, context.Subject is the user for whom the result is being made
            // setting context.IsActive to false allows your logic to indicate that the token should not be created
            // context.IsActive defaults to true

            return Task.CompletedTask;
        }
    }
  
}