using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marcware.JudgeMyPhoto.Entities.Models;
using Marcware.JudgeMyPhoto.ViewModels.Account;

namespace Marcware.JudgeMyPhoto.ViewModelMappers.Account
{
    public class UserListViewModelMapper
    {
        public UserListViewModel BuildViewModel(IQueryable<ApplicationUser> users)
        {
            UserListViewModel result = new UserListViewModel();
            UserListItemViewModelMapper itemMapper = new UserListItemViewModelMapper();
            result.Items = users.OrderBy(u => u.UserName).Select(u => itemMapper.BuildViewModel(u)).ToList();
            return result;
        }
    }
}
