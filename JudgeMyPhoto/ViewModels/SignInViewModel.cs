using System.ComponentModel.DataAnnotations;

namespace Marcware.JudgeMyPhoto.ViewModels
{
    public class SignInViewModel
    {
        [Display(Name = "Username")]
        [Required]
        public string UserName { get; set; }

        [Display(Name = "Password")]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
