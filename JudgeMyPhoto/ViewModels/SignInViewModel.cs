using System.ComponentModel.DataAnnotations;

namespace Marcware.JudgeMyPhoto.ViewModels
{
    public class SignInViewModel
    {
        [Display(Name = "User name")]
        [Required(ErrorMessage = "User name or email is required")]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
