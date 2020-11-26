using System.ComponentModel.DataAnnotations;

namespace Marcware.JudgeMyPhoto.ViewModels.Category
{
    public class CategoryListItemViewModel
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
        /// Status code
        /// </summary>
        public string StatusCode { get; set; }

        /// <summary>
        /// Status code as held in the database
        /// </summary>
        [Display(Name = "Status")]
        public string StatusCodeText { get; set; }

        /// <summary>
        /// Long status text
        /// </summary>
        [Display(Name = "Status text")]        
        public string StatusText { get; set; }
    }
}
