using System.ComponentModel.DataAnnotations;

namespace Marcware.JudgeMyPhoto.ViewModels.Account
{
    public class UserListItemViewModel
    {
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Display(Name = "Nickname")]
        public string Nickname { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
