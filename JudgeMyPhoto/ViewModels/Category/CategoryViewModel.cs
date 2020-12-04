using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Marcware.JudgeMyPhoto.ViewModels.Category
{
    public class CategoryViewModel
    {
        /// <summary>
        /// Category id
        /// </summary>        
        [Display(Name = "Category id")]
        public int CategoryId { get; set; }

        /// <summary>
        /// Category text
        /// </summary>
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Fool. A category must have a description.")]
        public string CategoryName { get; set; }

        /// <summary>
        /// Status code as held in the database
        /// </summary>
        [Display(Name = "Status")]
        [StringLength(2)]
        [MaxLength(2)]
        [Required(ErrorMessage = "A category must have a status")]
        public string StatusCode { get; set; }

        /// <summary>
        /// Long status text
        /// </summary>
        [Display(Name = "Status text")]
        [StringLength(200)]
        [MaxLength(200)]
        [Required(ErrorMessage = "Now don't be silly. This can't be empty.")]
        public string StatusText { get; set; }

        /// <summary>
        /// Theme used in the category when giving anonymous names to the photos
        /// </summary>
        [Display(Name = "Photo naming theme")]
        [StringLength(3)]
        [MaxLength(3)]
        [Required(ErrorMessage = "A theme must be specified")]
        public string PhotoNamingThemeCode { get; set; }

        /// <summary>
        /// Collection of all status types available
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> StatusCodeTypes { get; set; }


        /// <summary>
        /// Collection of all photo naming themes available
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> PhotoNamingThemeTypes { get; set; }
    }
}
