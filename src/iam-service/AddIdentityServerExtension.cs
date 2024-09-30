using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Duende.IdentityServer.Models;
using identityserver4_ef_template;
using identityserver4_ef_template.Pages.Admin.ApiScopes;
using identityserver4_ef_template.Pages.Admin.Clients;
using identityserver4_ef_template.Pages.Admin.IdentityScopes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace IAMService
{
    public static class AddIdentityServerExtension
    {
        public static IServiceCollection AddAppIdentityServer(this IServiceCollection services)
        {
            services.AddRazorPages();

            // Generate RSA Key
            var rsa = RSA.Create();
            var key = new RsaSecurityKey(rsa)
            {
                KeyId = "my-rsa-key" // This should match 'kid' in your token
            };
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
            
            // string privateKeyPem = ExportPrivateKey(rsa);
            // string publicKeyPem = ExportPublicKey(rsa);

            var keyJson = GetJsonRs256Key(rsa, "my-rsa-key");

            var connectionString = "Server=DESKTOP-GMI2BNJ; Database=IAM; Integrated Security=True; MultipleActiveResultSets=true;TrustServerCertificate=True;";

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();


            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

            services.AddIdentityServer(options => {
                options.Events.RaiseErrorEvents = true;
                options.Events.RaiseInformationEvents = true;
                options.Events.RaiseFailureEvents = true;
                options.Events.RaiseSuccessEvents = true;

                // options.EmitStaticAudienceClaim = true;
                // options.EmitIssuerIdentificationResponseParameter = true;

                // options.Endpoints.EnableAuthorizeEndpoint = true;

                options.UserInteraction.LoginUrl = "/auth/login";

            })
                .AddSigningCredential(signingCredentials)
                .AddAspNetIdentity<IdentityUser>() 
                //.AddDeveloperSigningCredential()
                .AddInMemoryClients(GetClients())
                .AddInMemoryIdentityResources(GetIdentityResources())
                .AddInMemoryApiResources(GetApiResources())
                .AddInMemoryApiScopes(GetApiScopes())
                .AddProfileService<CustomProfileService>()
                //.AddTestUsers(TestUsers.Users)
                .AddCustomTokenRequestValidator<TransactionScopeTokenRequestValidator>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                        builder.UseSqlServer(connectionString,
                            sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                //this is something you will want in production to reduce load on and requests to the DB
                //.AddConfigurationStoreCache()
                //this adds the operational data from DB (codes, tokens, consents)
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = builder =>
                    builder.UseSqlServer(connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly));

                    // Automatically cleanup expired tokens
                    options.EnableTokenCleanup = true;
                });


            // services.ConfigureApplicationCookie(options =>
            // {
            //     // Custom login page path
            //     options.LoginPath = "/auth/login";  // Set this to your desired login path
            //     //options.AccessDeniedPath = "/CustomAccessDenied"; // You can also configure a custom access denied path
            // });

            services.AddAuthentication();
                // .AddGoogle(options =>
                // {
                //     options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;

                //     // register your IdentityServer with Google at https://console.developers.google.com
                //     // enable the Google+ API
                //     // set the redirect URI to https://localhost:5001/signin-google
                //     options.ClientId = "copy client ID from Google here";
                //     options.ClientSecret = "copy client secret from Google here";
                // });

            services.AddTransient<identityserver4_ef_template.Pages.Portal.ClientRepository>();
            services.AddTransient<ClientRepository>();
            services.AddTransient<IdentityScopeRepository>();
            services.AddTransient<ApiScopeRepository>();

            return services;
        }
        
        static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("api1", "My API")
            };
        }

        // Define the clients
        static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientId = "client",
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
                    RedirectUris = { "http://localhost:5144/signin-oidc" },
                    AllowedScopes = { "openid", "profile", "api" },
                    Claims =
                    {
                        new ClientClaim("customer_id", "124")
                    },
                    AlwaysIncludeUserClaimsInIdToken = true,
                    RequirePkce = false,  // Required for code flow
                }
            };
        }

        // Define the identity resources
        static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        static IEnumerable<ApiScope> GetApiScopes()
        {
            return new List<ApiScope>
            {
                new ApiScope(
                    "api", 
                    displayName: "Write your data.",
                    userClaims: new[] { "user_level" }
                )
            };
        }

        static string ExportPrivateKey(RSA rsa)
        {
            var privateKeyBytes = rsa.ExportRSAPrivateKey();
            var base64PrivateKey = Convert.ToBase64String(privateKeyBytes);
            return $"-----BEGIN PRIVATE KEY-----\n{FormatKey(base64PrivateKey)}\n-----END PRIVATE KEY-----";
        }

        // Method to export the public key in PEM format
        static string ExportPublicKey(RSA rsa)
        {
            var publicKeyBytes = rsa.ExportRSAPublicKey();
            var base64PublicKey = Convert.ToBase64String(publicKeyBytes);
            return $"-----BEGIN PUBLIC KEY-----\n{FormatKey(base64PublicKey)}\n-----END PUBLIC KEY-----";
        }

        // Helper method to format keys by splitting them into lines of 64 characters
        static string FormatKey(string key)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < key.Length; i += 64)
            {
                sb.AppendLine(key.Substring(i, Math.Min(64, key.Length - i)));
            }
            return sb.ToString();
        }

        private static string GetJsonRs256Key(RSA rsa, string kid)
        {
            RSAParameters rsaParams = rsa.ExportParameters(true);

            // Create the JWK object
            var publicKey = new
            {
                kty = "RSA",
                use = "sig",
                alg = "RS256",
                kid = kid, // Key ID
                n = Base64UrlEncode(rsaParams.Modulus), // Public part: modulus
                e = Base64UrlEncode(rsaParams.Exponent), // Public part: exponent
              
            };

            string publicKeyStr = JsonConvert.SerializeObject(publicKey, Formatting.Indented);

            var privateKey = new 
            {
                kty = "RSA",
                use = "sig",
                alg = "RS256",
                kid = kid, // Key ID
                n = Base64UrlEncode(rsaParams.Modulus), // Public part: modulus
                e = Base64UrlEncode(rsaParams.Exponent), // Public part: exponent
                d = Base64UrlEncode(rsaParams.D), // Private part: private exponent
                p = Base64UrlEncode(rsaParams.P), // Private part: first prime factor
                q = Base64UrlEncode(rsaParams.Q), // Private part: second prime factor
                dp = Base64UrlEncode(rsaParams.DP), // Private part: exponent1
                dq = Base64UrlEncode(rsaParams.DQ), // Private part: exponent2
                qi = Base64UrlEncode(rsaParams.InverseQ) // Private part: coefficient
            };

            // Serialize the JWK to JSON
            string privateKeyStr = JsonConvert.SerializeObject(privateKey, Formatting.Indented);


            return publicKeyStr + " " + privateKey;
        }

        private static string Base64UrlEncode(byte[]? input)
        {
            if(input == null)
            {
                return "";
            }
            
            string base64 = Convert.ToBase64String(input);
            return base64
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }
    
    }


}