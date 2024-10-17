using Microsoft.EntityFrameworkCore;
using Serilog;
using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Duende.IdentityServer.Models;
using IAMService;
using Microsoft.AspNetCore.Identity;

namespace identityserver4_ef_template;

public class SeedData
{
    public static async void EnsureSeedData(WebApplication app)
    {
        using (var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
        {
            var configurationDbContext = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            var persistedGrantDbContext = scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>();
            var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            if (configurationDbContext.Database.GetPendingMigrations().Any())
            {
                configurationDbContext.Database.Migrate();
            }

            if(persistedGrantDbContext.Database.GetPendingMigrations().Any())
            {
                persistedGrantDbContext.Database.Migrate();
            }

            if(applicationDbContext.Database.GetPendingMigrations().Any())
            {
                applicationDbContext.Database.Migrate();
            }

            EnsureSeedData(configurationDbContext);

            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            await EnsureSeedData(applicationDbContext, userManager);
        }
    }

    private static void EnsureSeedData(ConfigurationDbContext context)
    {
        if (!context.Clients.Any())
        {
            Log.Debug("Clients being populated");
            foreach (var client in Config.Clients.ToList())
            {
                context.Clients.Add(client.ToEntity());
            }
            context.SaveChanges();
        }
        else
        {
            Log.Debug("Clients already populated");
        }

        if (!context.IdentityResources.Any())
        {
            Log.Debug("IdentityResources being populated");
            foreach (var resource in Config.IdentityResources.ToList())
            {
                context.IdentityResources.Add(resource.ToEntity());
            }
            context.SaveChanges();
        }
        else
        {
            Log.Debug("IdentityResources already populated");
        }

        if (!context.ApiScopes.Any())
        {
            Log.Debug("ApiScopes being populated");
            foreach (var resource in Config.ApiScopes.ToList())
            {
                context.ApiScopes.Add(resource.ToEntity());
            }
            context.SaveChanges();
        }
        else
        {
            Log.Debug("ApiScopes already populated");
        }

        if (!context.IdentityProviders.Any())
        {
            Log.Debug("OIDC IdentityProviders being populated");
            context.IdentityProviders.Add(new OidcProvider
            {
                Scheme = "demoidsrv",
                DisplayName = "IdentityServer",
                Authority = "https://demo.duendesoftware.com",
                ClientId = "login",
            }.ToEntity());
            context.SaveChanges();
        }
        else
        {
            Log.Debug("OIDC IdentityProviders already populated");
        }
    }

    private static async Task EnsureSeedData(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        if (!context.Users.Any())
        {
            Log.Debug("Users being populated");
            foreach (var user in Config.Users.ToList())
            {
                var result = await userManager.CreateAsync(user, "A123456a!");

                if (result.Succeeded)
                {
                    Log.Debug($"Users {user.UserName} populated successfully");
                }
            }
        }
        else
        {
            Log.Debug("Clients already populated");
        }
        
    }
}
