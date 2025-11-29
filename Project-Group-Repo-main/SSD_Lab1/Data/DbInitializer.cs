using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SSD_Lab1.Models;
using System;
using System.Threading.Tasks;

namespace SSD_Lab1.Data
{
    public static class DbInitializer
    {
        public static AppSecrets appSecrets { get; set; }

        public static async Task<int> SeedUsersAndRoles(IServiceProvider serviceProvider)
        {
            // create the database if it doesn't exist
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            context.Database.Migrate();

            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Check if roles already exist and exit if there are
            if (roleManager.Roles.Count() > 0)
                return 1; // should log an error message here

            // Seed roles
            int result = await SeedRoles(roleManager);
            if (result != 0)
                return 2; // should log an error message here

            // Check if users already exist and exit if there are
            if (userManager.Users.Count() > 0)
                return 3; // should log an error message here

            // Seed users
            result = await SeedUsers(userManager);
            if (result != 0)
                return 4; // should log an error message here

            return 0;
        }

        private static async Task<int> SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            // Create Supervisor Role
            var result = await roleManager.CreateAsync(new IdentityRole("Supervisor"));
            if (!result.Succeeded)
                return 1; // should log an error message here

            // Create Employee Role
            result = await roleManager.CreateAsync(new IdentityRole("Employee"));
            if (!result.Succeeded)
                return 2; // should log an error message here

            return 0;
        }

        private static async Task<int> SeedUsers(UserManager<ApplicationUser> userManager)
        {
            // Create Supervisor User
            var supervisorUser = new ApplicationUser
            {
                UserName = "the.supervisor@mohawkcollege.ca",
                Email = "the.supervisor@mohawkcollege.ca",
                FirstName = "The",
                LastName = "Supervisor",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(supervisorUser, appSecrets.SupervisorPassword);
            if (!result.Succeeded)
                return 1; // should log an error message here

            // Assign user to Supervisor role
            result = await userManager.AddToRoleAsync(supervisorUser, "Supervisor");
            if (!result.Succeeded)
                return 2; // should log an error message here

            // Create Employee User
            var employeeUser = new ApplicationUser
            {
                UserName = "the.employee@mohawkcollege.ca",
                Email = "the.employee@mohawkcollege.ca",
                FirstName = "The",
                LastName = "Employee",
                EmailConfirmed = true
            };

            result = await userManager.CreateAsync(employeeUser, appSecrets.EmployeePassword);
            if (!result.Succeeded)
                return 3; // should log an error message here

            // Assign user to Employee role
            result = await userManager.AddToRoleAsync(employeeUser, "Employee");
            if (!result.Succeeded)
                return 4; // should log an error message here

            return 0;
        }
    }
}