using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Identity;

namespace identityserver4_ef_template;

public static class Config
{
    public static IEnumerable<IdentityResource> IdentityResources =>
        new IdentityResource[]
        {
            new IdentityResources.OpenId(),
            new IdentityResources.Profile(),
        };

    public static IEnumerable<ApiScope> ApiScopes =>
        new ApiScope[]
        {
            new ApiScope("scope1"),
            new ApiScope("scope2"),
        };

    public static IEnumerable<Client> Clients =>
        new Client[]
        {
            // m2m client credentials flow client
            new Client
            {
                ClientId = "m2m.client",
                ClientName = "Client Credentials Client",

                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedScopes = { "scope1" }
            },

            // interactive client using code flow + pkce
            new Client
            {
                ClientId = "interactive",
                ClientSecrets = { new Secret("secret".Sha256()) },

                AllowedGrantTypes = GrantTypes.Code,

                RedirectUris = { "https://localhost:44300/signin-oidc" },
                FrontChannelLogoutUri = "https://localhost:44300/signout-oidc",
                PostLogoutRedirectUris = { "https://localhost:44300/signout-callback-oidc" },

                AllowOfflineAccess = true,
                AllowedScopes = { "openid", "profile", "scope2" },
            },
            new Client
            {
                ClientId = "myClient",
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                ClientSecrets = { new Secret("secret".Sha256()) },
                AllowedScopes = { "openid", "profile", "api" },
                Claims =
                {
                    new ClientClaim("customer_id", "123")
                },
                AlwaysIncludeUserClaimsInIdToken = true,
            },
            new Client
            {
                ClientId = "clientweb",
                AllowedGrantTypes = GrantTypes.Code,
                ClientSecrets = { new Secret("secret".Sha256()) },
                RedirectUris = { "http://localhost:5000/api/auth/exchange" },
                AllowedScopes = { "openid", "profile", "api" },
                Claims =
                {
                    new ClientClaim("customer_id", "124")
                },
                AlwaysIncludeUserClaimsInIdToken = true,
                RequirePkce = false,  // Required for code flow
            }
        };


    public static IEnumerable<IdentityUser> Users =>
        new IdentityUser[]
        {
            new IdentityUser() 
            {
                UserName = "admin",
                Email = "admin@gmail.com",
            }
     
        };
}
