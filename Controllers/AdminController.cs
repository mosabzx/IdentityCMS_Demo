using IdentityCMS_Demo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityCMS_Demo.Controllers
{
    public class AdminController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        
        public AdminController(UserManager<ApplicationUser> UsrMgr)
        {
            userManager = UsrMgr;
        }
        public IActionResult Index()
        {
            //var users = userManager.Users;
            return View(userManager.Users);
        }

        //HttpGet for Create Action.
        public IActionResult Create() => View();

        //[HttpPost]
        //public async Task <IActionResult> Create(User user)
        //{
        //    if (ModelState.IsValid) 
        //    {
        //        ApplicationUser applicationUser = new ApplicationUser
        //        {
        //            UserName = user.Name,
        //            Email = user.Email,
                    
        //        };
        //        IdentityResult result = await userManager.CreateAsync(applicationUser, user.Password);
        //        if (result.Succeeded)
        //        {
        //            return RedirectToAction("Index");
        //        }
        //        else
        //        {
        //            foreach (IdentityError error in result.Errors)
        //            {
        //                ModelState.AddModelError("", error.Description);
        //            }
        //        }
                
        //    }

        //    return View(user);
        //}










    }
}
