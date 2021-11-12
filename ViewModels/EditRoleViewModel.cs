using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityCMS_Demo.ViewModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            //initialize the users property list to avoid throwing exception error while executing in UI.
            Users = new List<string>();
        }
        public string Id { get; set; }

        [Required(ErrorMessage ="Role name is required")]
        public string RoleName { get; set; }

        public IList<string> Users { get; set; }

    }
}
