using Microsoft.AspNetCore.Http;

namespace Marcware.JudgeMyPhoto.ViewModels.Photo
{
    public class SubmitSinglePhotoViewModel
    {
        public int CategoryId { get; set; }

        public short PhotoNumber { get; set; }

        public string FileType { get; set; }

        public IFormFile FileContents { get; set; }
    }
}
