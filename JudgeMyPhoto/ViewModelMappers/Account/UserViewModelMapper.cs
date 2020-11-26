using Marcware.JudgeMyPhoto.Entities.Models;
using Marcware.JudgeMyPhoto.ViewModels.Account;

namespace Marcware.JudgeMyPhoto.ViewModelMappers.Account
{
    internal class UserViewModelMapper
    {
        public UserViewModel BuildViewModel(ApplicationUser repositoryModel)
        {
            UserViewModel viewModel = new UserViewModel();
            viewModel.UserName = repositoryModel.UserName;
            viewModel.Nickname = repositoryModel.Nickname;
            viewModel.Email = repositoryModel.Email;
            return viewModel;
        }

        public ApplicationUser BuildRepositoryModel(UserViewModel viewModel, ApplicationUser existingUser = null)
        {
            if (existingUser == null)
            {
                existingUser = new ApplicationUser();
                existingUser.UserName = viewModel.UserName;
            }
            existingUser.Nickname = viewModel.Nickname;
            existingUser.Email = viewModel.Email;
            return existingUser;
        }
    }
}
