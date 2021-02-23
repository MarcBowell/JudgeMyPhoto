using System.ComponentModel.DataAnnotations;

namespace Marcware.JudgeMyPhoto.Entities.Models
{
    public class PhotoVote
    {
        [Key]
        public int PhotoVoteId { get; set; }

        /// <summary>
        /// Position where the photo was voted
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Photo
        /// </summary>
        public Photograph Photo { get; set; }

        /// <summary>
        /// Voter of the photo
        /// </summary>
        public ApplicationUser Voter { get; set; }
    }
}
