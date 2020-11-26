using Microsoft.AspNetCore.Identity;

namespace Marcware.JudgeMyPhoto.Entities.Models
{
    public class ApplicationUser : IdentityUser
    {
        /// <summary>
        /// Nickname of the user
        /// </summary>
        public string Nickname { get; set; }
    }
}
