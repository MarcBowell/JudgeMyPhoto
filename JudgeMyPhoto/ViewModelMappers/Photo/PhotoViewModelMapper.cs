using Marcware.JudgeMyPhoto.Constants;
using Marcware.JudgeMyPhoto.Entities.Models;
using Marcware.JudgeMyPhoto.ViewModels.Photo;

namespace Marcware.JudgeMyPhoto.ViewModelMappers.Photo
{
    public class PhotoViewModelMapper
    {
        public PhotoViewModel BuildViewModel(Photograph repositoryModel)
        {
            PhotoViewModel viewModel = new PhotoViewModel();
            viewModel.PhotoId = repositoryModel.PhotoId;
            viewModel.Orientation = repositoryModel.Orientation;
            if (repositoryModel.Category.StatusCode == CategoryStatusCodes.Completed)
                viewModel.Title = $"'{repositoryModel.AnonymousPhotoName}' by {repositoryModel.Photographer.Nickname}";
            else
                viewModel.Title = $"'{repositoryModel.AnonymousPhotoName}'";
            return viewModel;
        }
    }
}
