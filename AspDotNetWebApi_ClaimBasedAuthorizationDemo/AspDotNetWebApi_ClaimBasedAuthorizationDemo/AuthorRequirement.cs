using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace AspDotNetWebApi_ClaimBasedAuthorizationDemo
{
    public class AuthorRequirement : IAuthorizationRequirement { }

    public class AuthorRequirementHandler : AuthorizationHandler<AuthorRequirement, Report>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            AuthorRequirement requirement,
            Report resource)
        {
            if (context.User.Identity.Name == resource.Author)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}