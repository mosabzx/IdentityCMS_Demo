using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityCMS_Demo.Security
{
    /*Note: Any handler must be registered in ConfiguerService method at Startup Class*/
    public class SuperAdminHandler : AuthorizationHandler<ManageAdminRolesAndClaimsRequierment>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public SuperAdminHandler(IHttpContextAccessor httpContextAccerssor)
        {
            this.httpContextAccessor = httpContextAccerssor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequierment requirement)
        {
            if (context.User.IsInRole("Super Admin"))
            {
                context.Succeed(requirement);

            }
            return Task.CompletedTask;
        }



    }
}
