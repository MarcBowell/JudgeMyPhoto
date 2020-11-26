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
        public string CategoryName { get; set; }

        /// <summary>
        /// Status code as held in the database
        /// </summary>
        [Display(Name = "Status")]
        [StringLength(2)]
        [MaxLength(2)]
        public string StatusCode { get; set; }

        /// <summary>
        /// Long status text
        /// </summary>
        [Display(Name = "Status text")]
        [StringLength(200)]
        [MaxLength(200)]
        public string StatusText { get; set; }

        /// <summary>
        /// Collection of all status types available
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> StatusCodeTypes { get; set; }
    }
}
