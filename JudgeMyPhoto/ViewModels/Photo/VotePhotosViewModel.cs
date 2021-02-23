namespace Marcware.JudgeMyPhoto.ViewModels.Photo
{
    public class VotePhotosViewModel
    {
        public int CategoryId { get; set; }

        public int[] PhotoIds { get; set; } = new int[] { };
    }
}
