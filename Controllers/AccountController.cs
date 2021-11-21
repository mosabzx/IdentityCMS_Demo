using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using IdentityCMS_Demo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityCMS_Demo.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace IdentityCMS_Demo.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
       
        public AccountController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }
        
        

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Name, Email = model.Email, City = model.City, Country = model.Country };
                var result = await userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (signInManager.IsSignedIn(User) && User.IsInRole("Admin"))
                    {
                        return RedirectToAction("UsersList","Administration");
                    }
                    await signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("index", "home");
                }
                else
                {
                    foreach (var erorr in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, erorr.Description);
                    }
                }


            }


            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl)
        {
            try
            {
                
                if (ModelState.IsValid)
                {
                   
                    var result = await signInManager.PasswordSignInAsync(model.Name, model.Password, model.RememberMe, false);

                    if (result.Succeeded)
                    {
                        // Or adding && Url.IsLocalUrl(returnUrl) in If statment.
                        if (!string.IsNullOrEmpty(returnUrl))
                        {
                            return LocalRedirect(returnUrl);
                        }
                        else
                        {
                            return RedirectToAction("index", "home");
                        }

                    }
                    else
                    {

                        ModelState.AddModelError(string.Empty, "Invaild Login Attempt!!");

                    }


                }

                return View(model);
            }
            catch (Exception e)
            {

                return View(e.Message);
            }
            
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {

            return View();
        }


    }
}
