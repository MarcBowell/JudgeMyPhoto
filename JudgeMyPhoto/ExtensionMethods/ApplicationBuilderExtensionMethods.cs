using Marcware.JudgeMyPhoto.Constants;
using Marcware.JudgeMyPhoto.Entities.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Marcware.JudgeMyPhoto.ExtensionMethods
{
    internal static class ApplicationBuilderExtensionMethods
    {
        public static IdentityResult SeedDatabase(this IApplicationBuilder app, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            IdentityResult result = SeedRoles(roleManager);
            if (result.Succeeded)
                result = SeedUsers(userManager, configuration);            
            return result;
        }

        private static IdentityResult SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            IdentityResult result = IdentityResult.Success;
            
            foreach (string roleName in JudgeMyPhotoRoles.GetAllRoles())
            {
                if (result.Succeeded && !roleManager.RoleExistsAsync(roleName).Result)
                {
                    IdentityRole role = new IdentityRole()
                    {
                        Name = roleName
                    };
                    
                    result = roleManager.CreateAsync(role).Result;
                }
            }
            return result;
        }

        private static IdentityResult SeedUsers(UserManager<ApplicationUser> userManager, IConfiguration configuration)
        {
            IdentityResult result = IdentityResult.Success;

            if (userManager.GetUsersInRoleAsync(JudgeMyPhotoRoles.Admin).Result.Count == 0)
            {
                string emailAddress = configuration.GetValue<string>(AppSettingKeys.DefaultAdmin.Email);
                string userName = configuration.GetValue<string>(AppSettingKeys.DefaultAdmin.UserName);

                if (string.IsNullOrEmpty(emailAddress) || string.IsNullOrEmpty(userName))
                {
                    result = IdentityResult.Failed(new IdentityError()
                    {
                        Description = "Email or user name cannot be blank"
                    });
                }

                if (result.Succeeded)
                {
                    string password = $"{userName}123!";
                    result = userManager.CreateApplicationUserAsync(userName, userName, emailAddress, password, JudgeMyPhotoRoles.Admin).Result;
                }
            }

            return result;
        }
    }
}
