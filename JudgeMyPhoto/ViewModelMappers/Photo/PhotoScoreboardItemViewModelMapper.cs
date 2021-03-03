using Marcware.JudgeMyPhoto.Entities.Models;
using Marcware.JudgeMyPhoto.ViewModels.Photo;

namespace Marcware.JudgeMyPhoto.ViewModelMappers.Photo
{
    public class PhotoScoreboardItemViewModelMapper
    {
        public PhotoScoreboardItemViewModel BuildModel(int position, int points, Photograph photograph)
        {
            PhotoScoreboardItemViewModel result = new PhotoScoreboardItemViewModel();
            result.Position = position;
            result.Points = points;
            result.PhotoName = photograph.AnonymousPhotoName;
            return result;
        }
    }
}
