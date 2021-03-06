using IdentityCMS_Demo.Data;
using IdentityCMS_Demo.Models;
using IdentityCMS_Demo.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityCMS_Demo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Add connection string for SQL Server.
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("SqlCon")));


            //Add Identity to the project.
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                //options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequiredLength = 4;
                //false no need Such as[ @ ! # $ ] , etc...
                options.Password.RequireNonAlphanumeric = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>();

           

            //Adding Session (To use Cockies).
            //services.AddSession();
            //option =>
            //option.IdleTimeout = TimeSpan.FromMinutes(2));

            



            //Add Policies.
            services.AddAuthorization(options =>
            {
                /* Claim value could be many as long as the Claim parameter accept array*/

                //Add a Calim policy for delete with (Claim type and Claim Value).
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role", "true"));


                //Add a Calim policy with two claims types with values for create and delete claims.
                options.AddPolicy("DeleteCreateRolePolicy", policy => policy.RequireClaim("Delete Role", "true").RequireClaim("Create Role", "true"));


                ///*Add a Claim for Edit policy with (Claim type and Claim Value).*/
                //options.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role" , "true"));


                //Add a Claim for Create policy with (Claim type and Claim Value).
                options.AddPolicy("CreateRolePolicy", policy => policy.RequireClaim("Create Role", "true"));


                ///*Add Role policy for Admin Role./In ASP.Net Core The Role is a Claim with type Role so it can added as policy.*/
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));


                ///* Add Custom policy with requirements of Claim and Role together.*/
                //options.AddPolicy("EditRolePolicy", policy => policy.RequireRole("Admin")
                //.RequireClaim("Edit Role", "true")
                //.RequireRole("Super Admin"));


                /*Add Custom policy with either requirements of Claim with value and Role together or by one Role by using Func and RequierAssertion method. As an assertion method give the result as boolean*/
                //options.AddPolicy("EditRolePolicy",
                //    policy => policy.RequireAssertion(context => 
                //    context.User.IsInRole("Admin") && 
                //    context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true")||
                //    context.User.IsInRole("Super Admin")));



                /*Add Custom policy with Custom Authorization Requierment and Custom Authorization Handler*/
                /* This custom authorization requierment prevent the logged in user has Admin role and edit role claim with true value to edit the user role But can edit other user IF not logged in.
                 
                 * An Admin user can manage other Admin user roles and claims but not thier own roles cand claims.*/
                options.AddPolicy("EditRolePolicy",
                   policy => policy.AddRequirements(new ManageAdminRolesAndClaimsRequierment()));




            });


            /*Registering the Custom Authorization Handler To active the custom authorization requierment*/
            services.AddSingleton<IAuthorizationHandler, CanEditOnlyOtherAdminRolesAndClaimsHandler>();

            /*Registering The other Authoraization Handler*/
            services.AddSingleton<IAuthorizationHandler, SuperAdminHandler>();


            //Change the AccessDenied from defult Path(Account/AccessDenied) to Path(Administration/AccessDenied) or any desired path.
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });





            // OR the Password option could be configured by this way:

            //services.Configure<IdentityOptions>(option => {
            //    option.Password.RequiredLength = 4;
            //    option.Password.RequireNonAlphanumeric = false;
            //});



            /*//In StartUp Class/ConfigureServices Metohd
            Add global policy for all application( All contreollers and Actions).*/

            //services.AddMvc(option =>
            //{
            //    var policy = new AuthorizationPolicyBuilder()
            //    .RequireAuthenticatedUser().Build();
            //    option.Filters.Add(new AuthorizeFilter(policy));
            //});


            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddHttpContextAccessor();

            




        }




        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            //app.UseSession();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });



        }


    }



}
