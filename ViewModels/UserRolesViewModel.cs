using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityCMS_Demo.ViewModels
{
    public class UserRolesViewModel
    {
        //Get it by url query and save it in VewBag.
        //public string UserId { get; set; }
        public string RoleId { get; set; }
        public string RoleName { get; set; }

        public bool IsSelected { get; set; }
    }
}
