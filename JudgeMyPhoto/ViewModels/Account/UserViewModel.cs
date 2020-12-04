using System.ComponentModel.DataAnnotations;

namespace Marcware.JudgeMyPhoto.ViewModels.Account
{
    public class UserViewModel
    {        
        [Display(Name = "User name")]
        [MaxLength(20, ErrorMessage = "Fool! Maximum length is 20")]
        [Required(ErrorMessage = "You need a user name you silly!")]
        [RegularExpression("^[a-zA-Z0-9]*$", ErrorMessage = "Oh you are silly. Only alpha-numeric characters are allowed for the user name")]
        public string UserName { get; set; }
                
        [Display(Name = "Nickname")]
        [MaxLength(50, ErrorMessage = "Fool! Maximum length is 50")]
        [Required(ErrorMessage = "You cad. Trying to remove the nickname isn't allowed.")]
        [RegularExpression("^[a-zA-Z0-9' ]*$", ErrorMessage = "Oh you are silly. Only alpha-numeric, ' and space characters are allowed for nickname")]
        public string Nickname { get; set; }

        [Display(Name = "Email")]
        [MaxLength(255, ErrorMessage = "Fool! Maximum length is 255")]
        [Required(ErrorMessage = "Now how is communication going to happen without an email")]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@([a-zA-Z0-9_\-\.]+)\.([a-zA-Z]{2,5})$", ErrorMessage = "You are crazy. That's not an email address.")]        
        public string Email { get; set; }

        [Display(Name = "Change password")]
        [MaxLength(255, ErrorMessage = "Fool! Maximum length is 255")]
        public string Password { get; set; }

        [Display(Name = "Confirm password")]
        [MaxLength(255, ErrorMessage = "Fool! Maximum length is 255")]
        [Compare("Password", ErrorMessage = "Tut! Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}
