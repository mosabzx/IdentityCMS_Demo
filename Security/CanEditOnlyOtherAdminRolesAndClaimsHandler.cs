using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityCMS_Demo.Security
{

    /*This Class to create a Custom Authorization Handler*/
    /*This handler must be register in startup class in ConfiguerServices method*/
    public class CanEditOnlyOtherAdminRolesAndClaimsHandler: AuthorizationHandler<ManageAdminRolesAndClaimsRequierment>
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public CanEditOnlyOtherAdminRolesAndClaimsHandler(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ManageAdminRolesAndClaimsRequierment requirement)
        {
            //var authFilterContext = context.Resource as AuthorizationFilterContext; // this work in .Net core 2 till 3.0 .
            /*For .Net COre 3.0+ must used the HttpContextAccessor.*/
            var authFilterContext = httpContextAccessor.HttpContext;
            if (authFilterContext == null)
            {
                return Task.CompletedTask;
            }

            string loggedInAdmin = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;
            string adminBeingEdited = authFilterContext.Request.Query["userId"];

            if (context.User.IsInRole("Admin") &&
                context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") &&
                adminBeingEdited.ToLower() != loggedInAdmin.ToLower())
            {
                context.Succeed(requirement);
            }
            //else
            //{
            //    /*Use it when the policy not fulfilled and need to return explicit failure.*/
            //    context.Fail();
            //}

            return Task.CompletedTask; // Task.CompledtedTask meaning the access not authorized.


        }




    }
}
