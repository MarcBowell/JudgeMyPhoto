using Marcware.JudgeMyPhoto.Constants;
using Marcware.JudgeMyPhoto.Entities.Models;
using Marcware.JudgeMyPhoto.ViewModels.Category;

namespace Marcware.JudgeMyPhoto.ViewModelMappers.Category
{
    internal class CategoryListItemViewModelMapper
    {
        public CategoryListItemViewModel BuildViewModel(PhotoCategory repositoryModel)
        {
            CategoryListItemViewModel viewModel = new CategoryListItemViewModel();
            viewModel.CategoryId = repositoryModel.CategoryId;
            viewModel.CategoryName = repositoryModel.CategoryName;
            viewModel.StatusCode = repositoryModel.StatusCode;
            viewModel.StatusCodeText = CategoryStatusCodes.GetStatusText(repositoryModel.StatusCode);
            viewModel.StatusText = repositoryModel.StatusText;
            return viewModel;
        }
    }
}
