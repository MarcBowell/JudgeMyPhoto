using Marcware.JudgeMyPhoto.Entities.Models;
using Marcware.JudgeMyPhoto.ViewModels.Photo;

namespace Marcware.JudgeMyPhoto.ViewModelMappers.Photo
{
    public class ViewPhotosViewModelMapper
    {
        public ViewPhotosViewModel BuildViewModel(PhotoCategory category)
        {
            ViewPhotosViewModel viewModel = new ViewPhotosViewModel();
            viewModel.CategoryId = category.CategoryId;
            viewModel.StatusCode = category.StatusCode;
            return viewModel;
        }
    }
}
