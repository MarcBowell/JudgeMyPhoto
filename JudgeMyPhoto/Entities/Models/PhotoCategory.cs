using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marcware.JudgeMyPhoto.Entities.Models
{
    [Table("Category")]
    public class PhotoCategory
    {
        /// <summary>
        /// Category id
        /// </summary>
        [Key]
        public int CategoryId { get; set; }

        /// <summary>
        /// Category text
        /// </summary>
        [StringLength(50)]
        [MaxLength(50)]
        public string CategoryName { get; set; }

        /// <summary>
        /// Status code as held in the database
        /// </summary>
        [StringLength(2)]
        [MaxLength(2)]
        public string StatusCode { get; set; }

        /// <summary>
        /// Long status text
        /// </summary>
        [StringLength(200)]
        [MaxLength(200)]
        public string StatusText { get; set; }

        /// <summary>
        /// Theme used in the category when giving anonymous names to the photos
        /// </summary>
        [StringLength(3)]
        [MaxLength(3)]
        public string PhotoNamingThemeCode { get; set; }

        /// <summary>
        /// Photos for this category
        /// </summary>
        public IEnumerable<Photograph> Photographs { get; set; }
    }
}
