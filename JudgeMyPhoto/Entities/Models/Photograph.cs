using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Marcware.JudgeMyPhoto.Entities.Models
{
    public class Photograph
    {
        /// <summary>
        /// Photo id
        /// </summary>
        [Key]
        public int PhotoId { get; set; }

        /// <summary>
        /// Anonymous generated name for the photo
        /// </summary>
        [StringLength(50)]
        [MaxLength(50)]
        public string AnonymousPhotoName { get; set; }

        [StringLength(50)]
        [MaxLength(50)]
        public string FileType { get; set; }

        /// <summary>
        /// Orientation of the photo
        /// </summary>
        [StringLength(1)]
        [MaxLength(1)]
        public string Orientation { get; set; }

        /// <summary>
        /// Number of the photo submitted by the user in this category
        /// </summary>
        public short UserCategoryPhotoNumber { get; set; }

        /// <summary>
        /// Thumbnail image of the photo
        /// </summary>
        public byte[] SmallImage { get; set; }

        /// <summary>
        /// Large full size image for the photo
        /// </summary>
        public byte[] LargeImage { get; set; }

        /// <summary>
        /// Category this photo belongs to
        /// </summary>
        [Required]
        public PhotoCategory Category { get; set; }

        /// <summary>
        /// Photographer who took the photo
        /// </summary>
        [Required]
        public ApplicationUser Photographer { get; set; }

        /// <summary>
        /// Votes for this photo
        /// </summary>
        public IEnumerable<PhotoVote> Votes { get; set; }
    }
}
