using Marcware.JudgeMyPhoto.Entities.Models;
using Marcware.JudgeMyPhoto.ViewModels.Category;

namespace Marcware.JudgeMyPhoto.ViewModelMappers.Category
{
    internal class DeleteCategoryViewModelMapper
    {
        public DeleteCategoryViewModel BuildViewModel(PhotoCategory repositoryModel)
        {
            DeleteCategoryViewModel viewModel = new DeleteCategoryViewModel();
            viewModel.CategoryId = repositoryModel.CategoryId;
            viewModel.CategoryName = repositoryModel.CategoryName;            
            return viewModel;
        }
    }
}
