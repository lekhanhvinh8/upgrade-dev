using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace MonitorService.Configurations
{
    public class AppConfigAuthorizationRequirement : IAuthorizationRequirement
    {
        public AppConfigAuthorizationRequirement()
        {
        }
    }

    public class AppConfigAuthorizationHandler : AuthorizationHandler<AppConfigAuthorizationRequirement>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IHttpContextAccessor contextAccessor;

        public AppConfigAuthorizationHandler(IServiceProvider serviceProvider, IHttpContextAccessor contextAccessor)
        {
            this._serviceProvider = serviceProvider;
            this.contextAccessor = contextAccessor;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AppConfigAuthorizationRequirement requirement)
        {
            using (var scope = this._serviceProvider.CreateScope())
            {
                try
                {
                    var email = context.User.Claims.FirstOrDefault(x => x.Type == "username")?.Value;
                    if(email == null)
                    {
                        context.Fail();
                        return;
                    }

                    if(!new List<string>() {"vinhlk4"}.Contains(email.ToLower()))
                    {
                        context.Fail();
                        return;
                    }
                
                    context.Succeed(requirement);
                }
                catch (Exception)
                {
                }
            }

            await Task.CompletedTask;
        }
    }
}