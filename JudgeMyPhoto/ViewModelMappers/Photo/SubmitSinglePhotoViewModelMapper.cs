using System.IO;
using Marcware.JudgeMyPhoto.Constants;
using Marcware.JudgeMyPhoto.Entities.Models;
using Marcware.JudgeMyPhoto.ViewModels.Photo;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Marcware.JudgeMyPhoto.ViewModelMappers.Photo
{
    public class SubmitSinglePhotoViewModelMapper
    {
        public Photograph BuildRepositoryModel(SubmitSinglePhotoViewModel viewModel, Photograph existingPhoto)
        {
            existingPhoto.FileType = viewModel.FileType;            
            SetFileContentsProperties(viewModel.FileContents, existingPhoto);
            return existingPhoto;
        }

        public Photograph BuildRepositoryModel(SubmitSinglePhotoViewModel viewModel, ApplicationUser user, PhotoCategory category)
        {
            Photograph result = new Photograph();
            result.Photographer = user;
            result.Category = category;
            result.UserCategoryPhotoNumber = viewModel.PhotoNumber;
            return BuildRepositoryModel(viewModel, result);
        }

        private void SetFileContentsProperties(IFormFile formFile, Photograph photograph)
        {
            Stream fileStream = formFile.OpenReadStream();
            using (var image = Image.Load(fileStream))
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    formFile.CopyTo(memStream);
                    photograph.LargeImage = memStream.ToArray();
                }

                image.Mutate(x => x.AutoOrient());
                if (image.Height > image.Width)
                {
                    photograph.Orientation = PhotoOrientationCodes.Portrait;
                    var options = new ResizeOptions()
                    {
                        Mode = ResizeMode.Crop,
                        Size = new Size(150, 150)
                    };
                    image.Mutate(i => i.Resize(options));
                }
                else
                {
                    photograph.Orientation = PhotoOrientationCodes.Landscape;
                    var options = new ResizeOptions()
                    {
                        Mode = ResizeMode.Crop,
                        Size = new Size(300, 150)
                    };
                    image.Mutate(i => i.Resize(options));
                }                

                using (MemoryStream resizeMemStream = new MemoryStream())
                {
                    IImageEncoder imageEncoder = new JpegEncoder()
                    {
                        Quality = 90,
                        Subsample = JpegSubsample.Ratio444
                    };
                    image.Save(resizeMemStream, imageEncoder);
                    photograph.SmallImage = resizeMemStream.ToArray();
                }
            }
        }
    }
}
