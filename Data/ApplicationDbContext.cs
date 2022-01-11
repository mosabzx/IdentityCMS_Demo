using IdentityCMS_Demo.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Text;
using IdentityCMS_Demo.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Linq;

namespace IdentityCMS_Demo.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ////To prevent deleteing a Role unless to remove a Users if belongs to this Role.

            //foreach (var foreignKey in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            //{
            //    foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            //}



            //Seeding data for Admin Role.
            //string roleId = Guid.NewGuid().ToString();

            //modelBuilder.Entity<IdentityRole>().HasData(
            //    new IdentityRole
            //    {​
            //        Id = roleId,
            //        Name = "Admin",
            //        NormalizedName = "ADMIN",
            //        ConcurrencyStamp = roleId
            //    }​);






        }



    }

   


}










