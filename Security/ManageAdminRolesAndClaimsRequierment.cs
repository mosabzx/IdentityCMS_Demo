using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityCMS_Demo.Security
{
    /*This Calss to create a Custom Authorization Requierment*/
    /*This custom authorization requierment must be register as policy in startup calss Configerservices(services)=> services.AddAuthorization(option) => option.AddPolicy(policy) => policy.AddRequierment()*/
    public class ManageAdminRolesAndClaimsRequierment : IAuthorizationRequirement
    {

    }



}
