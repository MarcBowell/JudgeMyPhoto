namespace Marcware.JudgeMyPhoto.ViewModels.Photo
{
    public class PhotoViewModel
    {
        public int PhotoId { get; set; }

        public string Orientation { get; set; }

        public string Title { get; set; }

        /// <summary>
        /// Position that it has been voted in for this user
        /// 0 = not voted for, 1 = 1st place, 2 = 2nd place etc.
        /// </summary>
        public int VotePosition { get; set; }
    }
}
