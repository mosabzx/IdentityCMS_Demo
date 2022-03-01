using IdentityCMS_Demo.Models;
using IdentityCMS_Demo.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace IdentityCMS_Demo.Controllers
{
    /*  Authorize the controller by using Policy parameter. */
    //[Authorize(Policy = "AdminRolePolicy")]

    //OR

    /* Authorize the controller by using many Roles parameters. */
    //[Authorize(Roles = "Admin,Manager")]

    //OR

    /* Authorize the controller by using one Role parameter. */
    //[Authorize(Roles = "Admin")]


    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly UserManager<ApplicationUser> userManager;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            this.roleManager = roleManager;
            this.userManager = userManager;
        }


        [HttpGet]
        public IActionResult UsersList()
        {
            var users = userManager.Users;
            var usersCount = users.Count();
            ViewBag.UsersCount = usersCount;
            return View(users);
        }


        [HttpGet]

        public async Task<IActionResult> EditUser(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            ViewBag.UserName = user.UserName;
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {Id} can not be found";
                return View("NotFound");
            }
            var userClaims = await userManager.GetClaimsAsync(user);
            var userRoles = await userManager.GetRolesAsync(user);

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Name = user.UserName,
                Email = user.Email,
                City = user.City,
                Country = user.Country,
                Claims = userClaims.Select(c => c.Type + " : " + c.Value).ToList(),
                Roles = userRoles.ToList()

            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} can not be found";
                return View("NotFound");
            }
            else
            {
                user.Id = model.Id;
                user.UserName = model.Name;
                user.Email = model.Email;
                user.City = model.City;
                user.Country = model.Country;

                var result = await userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("UsersList");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }


            return View(model);

        }

        [HttpGet]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            ViewBag.UserId = userId;
            var user = await userManager.FindByIdAsync(userId);
            ViewBag.UserName = user.UserName;
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} can not be found";
                return View("NotFound");
            }
            else
            {
                var model = new List<UserRolesViewModel>();
                foreach (var role in roleManager.Roles)
                {
                    var userRolesViewModel = new UserRolesViewModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name
                    };
                    if (await userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRolesViewModel.IsSelected = true;
                    }
                    else
                    {
                        userRolesViewModel.IsSelected = false;
                    }
                    model.Add(userRolesViewModel);
                }

                return View(model);

            }


        }

        [HttpPost]
        [Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> ManageUserRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} can not be found";
                return View("NotFound");
            }
            var roles = await userManager.GetRolesAsync(user);
            var result = await userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }

            result = await userManager.AddToRolesAsync(user, model.Where(x => x.IsSelected).Select(r => r.RoleName));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to this user");
                return View(model);
            }

            return RedirectToAction("EditUser", new { Id = userId });


        }





        [HttpGet]
        public async Task<IActionResult> ManageUserClaims(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            ViewBag.UserName = user.UserName;  /*"User Name";*/
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} can not be found";
                return View("NotFound");
            }
            var existingUserClaims = await userManager.GetClaimsAsync(user);
            var model = new UserClaimsViewModel
            {
                userId = userId,

            };

            foreach (Claim claim in ClaimsStore.AllClaims)
            {
                UserClaim userClaim = new UserClaim
                {
                    ClaimType = claim.Type,
                };
                /*If the user has the claim, set the IsSelected property to true, so the checkbox
                 next to claim is checked in the UI (User Interface)*/
                if (existingUserClaims.Any(c => c.Type == claim.Type && c.Value == "true"))
                {
                    userClaim.IsSelected = true;
                }
                model.Claims.Add(userClaim);

            }

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ManageUserClaims(UserClaimsViewModel model)
        {
            var user = await userManager.FindByIdAsync(model.userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.userId} can not be found";
                return View("NotFound");
            }

            var claims = await userManager.GetClaimsAsync(user);
            var result = await userManager.RemoveClaimsAsync(user, claims);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing claims");
                return View(model);
            }

            //Adding claim value as claim type.
            /*result = await userManager.AddClaimsAsync(user,
                model.Claims.Where(c => c.IsSelected).Select(c => new Claim(c.ClaimType, c.ClaimType)));*/

            //Adding Claim value as true or false based on IsSelected property.
            result = await userManager.AddClaimsAsync(user,
                model.Claims.Select(c => new Claim(c.ClaimType, c.IsSelected ? "true" : "false")));

            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add user selected claims");
                return View(model);
            }


            return RedirectToAction("EditUser", new { Id = model.userId });
        }







        [HttpPost]
        public async Task<IActionResult> DeleteUser(string Id)
        {
            var user = await userManager.FindByIdAsync(Id);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {Id} can not be found";
                return View("NotFound");
            }
            else
            {

                var result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("UsersList");
                }


                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("UsersList");

            }



        }





        [HttpGet]
        public IActionResult RolesList()
        {
            var role = roleManager.Roles;
            var rolesCount = roleManager.Roles.Count();
            ViewBag.RolesCount = rolesCount;

            return View(role);
        }







        [HttpGet]
        //[Authorize(Roles = "Super Admin")]
        [Authorize(Policy = "CreateRolePolicy")]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "CreateRolePolicy")]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole { Name = model.RoleName };
                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("RolesList", "Administration");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

            }

            return View(model);
        }




        [HttpGet]
        //[Authorize(Policy = "EditRolePolicy")]
        public async Task<IActionResult> EditRole(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id{Id} can not be found";
                return View("NotFound");
            }
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            return View(model);
        }
        [HttpPost]
        
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var role = await roleManager.FindByIdAsync(model.Id);
                if (role == null)
                {
                    ViewBag.ErrorMessage = $"Role with Id = {model.Id} can not be found";
                    return View("NotFound");
                }
                else
                {
                    role.Name = model.RoleName;
                    var result = await roleManager.UpdateAsync(role);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("RolesList");
                    }
                    else
                    {
                        foreach (var erorr in result.Errors)
                        {
                            ModelState.AddModelError("", erorr.Description);
                        }


                    }

                }


            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Policy = "DeleteRolePolicy")]
        public async Task<IActionResult> DeleteRole(string Id)
        {
            var role = await roleManager.FindByIdAsync(Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {Id} can not be found";
                return View("NotFound");
            }
            else
            {


                var result = await roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("RolesList");
                }


                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("RolesList");




            }



        }



        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;
            var role = await roleManager.FindByIdAsync(roleId);
            ViewBag.roleName = role.Name;

            //var roleName = roleManager.Roles.Where(r => r.Id == roleId).SingleOrDefault().Name;
            //ViewBag.roleName = roleName;

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} can not be found";
                return View("NotFound");
            }


            var model = new List<UserRoleViewModel>();

            foreach (var user in userManager.Users)
            {

                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName

                };
                if (await userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);


            }

            return View(model);


        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} can not be found";
                return View("NotFound");
            }

            for (int i = 0; i < model.Count; i++)
            {
                var user = await userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await userManager.AddToRoleAsync(user, role.Name);

                }
                else if (!(model[i].IsSelected) && await userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }
                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    else
                        return RedirectToAction("EditRole", new { Id = roleId });

                }


            }

            return RedirectToAction("EditRole", new { Id = roleId });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {

            return View();
        }









    }


}


