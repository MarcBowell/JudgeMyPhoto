using Marcware.JudgeMyPhoto.Entities.Models;
using Marcware.JudgeMyPhoto.ViewModels.Account;

namespace Marcware.JudgeMyPhoto.ViewModelMappers.Account
{
    public class UserListItemViewModelMapper
    {
        public UserListItemViewModel BuildViewModel(ApplicationUser repositoryModel)
        {
            UserListItemViewModel viewModel = new UserListItemViewModel();
            viewModel.UserName = repositoryModel.UserName;
            viewModel.Nickname = repositoryModel.Nickname;
            viewModel.Email = repositoryModel.Email;
            return viewModel;
        }
    }
}
