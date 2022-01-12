using IdentityCMS_Demo.Data;
using IdentityCMS_Demo.Models;
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


            //Add Policies.
            services.AddAuthorization(options =>
            {
                //Add a Calim policy for delete.
                options.AddPolicy("DeleteRolePolicy", policy => policy.RequireClaim("Delete Role", "true"));

                //Add a Calim policy for delete and create.
                options.AddPolicy("DeleteCreateRolePolicy", policy => policy.RequireClaim("Delete Role").RequireClaim("Create Role"));

                //Add a Claim for Edit policy.
                options.AddPolicy("EditRolePolicy", policy => policy.RequireClaim("Edit Role" , "true"));

                //Add a Claim for Create policy.
                options.AddPolicy("CreateRolePolicy", policy => policy.RequireClaim("Create Role", "true"));


                //Add Role policy for Admin Role./In ASP.Net Core The Role is a Claim with type Role so it can added as policy.
                options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("Admin"));

                //Add Custom authoriztion policy using func.


            });


            //Change the AccessDenied from defult Path(Account/AccessDenied) to Path(Administration/AccessDenied) or any desired path.
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = new PathString("/Administration/AccessDenied");
            });





            // OR the password could be configured by this way:

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
