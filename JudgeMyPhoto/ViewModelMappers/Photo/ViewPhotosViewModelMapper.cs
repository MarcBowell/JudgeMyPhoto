using Marcware.JudgeMyPhoto.ViewModels.Photo;

namespace Marcware.JudgeMyPhoto.ViewModelMappers.Photo
{
    public class ViewPhotosViewModelMapper
    {
        public ViewPhotosViewModel BuildViewModel(int categoryId)
        {
            ViewPhotosViewModel viewModel = new ViewPhotosViewModel();
            viewModel.CategoryId = categoryId;
            return viewModel;
        }
    }
}
