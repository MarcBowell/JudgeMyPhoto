using System.ComponentModel.DataAnnotations;

namespace Marcware.JudgeMyPhoto.ViewModels.Category
{
    public class DeleteCategoryViewModel
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
    }
}
