using System.ComponentModel.DataAnnotations;

namespace Marcware.JudgeMyPhoto.ViewModels.Account
{
    public class UserViewModel
    {
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Display(Name = "Nickname")]
        public string Nickname { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Change password")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
