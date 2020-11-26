using System.Threading.Tasks;
using Marcware.JudgeMyPhoto.Entities.Models;
using Microsoft.AspNetCore.Identity;

namespace Marcware.JudgeMyPhoto.ExtensionMethods
{
    internal static class UserAccountExtensionMethods
    {
        /// <summary>
        /// Creates a Judge My Photo application user
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="userName"></param>
        /// <param name="nickname"></param>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public async static Task<IdentityResult> CreateApplicationUserAsync(this UserManager<ApplicationUser> userManager, string userName, string nickname, string email, string password, params string[] roles)
        {
            IdentityResult result = new IdentityResult();

            ApplicationUser user = new ApplicationUser()
            {
                UserName = userName,
                Email = email,
                Nickname = nickname
            };

            result = await userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                foreach (string role in roles)
                {
                    if (result.Succeeded)
                    {
                        result = await userManager.AddToRoleAsync(user, role);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Add or remove a role to/from a user
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="userName"></param>
        /// <param name="roleName"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public async static Task<IdentityResult> SetUserRoleState(this UserManager<ApplicationUser> userManager, string userName, string roleName, bool status)
        {
            IdentityResult result = IdentityResult.Success;
            ApplicationUser user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                bool userIsAlreadyInRole = await userManager.IsInRoleAsync(user, roleName);

                if (userIsAlreadyInRole && !status)
                {
                    result = await userManager.RemoveFromRoleAsync(user, roleName);
                }
                if (!userIsAlreadyInRole && status)
                {
                    result = await userManager.AddToRoleAsync(user, roleName);
                }

            }
            else
            {
                result = IdentityResult.Failed(new IdentityError()
                {
                    Description = "Cannot find a user with this username"
                }); 
            }
            return result;
        }

        /// <summary>
        /// Delete a user from this application
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async static Task<IdentityResult> DeleteApplicationUserAsync(this UserManager<ApplicationUser> userManager, string userName)
        {
            IdentityResult result = IdentityResult.Success;
            ApplicationUser user = await userManager.FindByNameAsync(userName);
            if (user != null)
            {
                result = await userManager.DeleteAsync(user);
            }
            return result;
        }
    }
}
