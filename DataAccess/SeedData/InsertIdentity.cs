using Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.SeedData
{
    public class InsertIdentity 
    {
        public static async Task SeedEssentialAsync(UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            //Seed roles
            await roleManager.CreateAsync(new IdentityRole(Entities.DTOs.Authorization.Roles.Administrator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Entities.DTOs.Authorization.Roles.Moderator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Entities.DTOs.Authorization.Roles.User.ToString()));
            //Seed Default User
            var defaultUser = new User
            {
                UserName = Entities.DTOs.Authorization.default_username,
                Email = Entities.DTOs.Authorization.default_email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                await userManager.CreateAsync(defaultUser, Entities.DTOs.Authorization.default_password);
                await userManager.AddToRoleAsync(defaultUser, Entities.DTOs.Authorization.default_role.ToString());
            }
        }
    }
}
