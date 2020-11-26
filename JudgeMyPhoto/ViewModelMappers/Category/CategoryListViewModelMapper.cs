using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marcware.JudgeMyPhoto.Entities.Models;
using Marcware.JudgeMyPhoto.ViewModels.Category;
using Microsoft.EntityFrameworkCore;

namespace Marcware.JudgeMyPhoto.ViewModelMappers.Category
{
    public class CategoryListViewModelMapper
    {
        public async Task<CategoryListViewModel> BuildViewModelAsync(IEnumerable<PhotoCategory> photoCategories)
        {
            CategoryListViewModel viewModel = new CategoryListViewModel();
            CategoryListItemViewModelMapper mapper = new CategoryListItemViewModelMapper();
            viewModel.Items = await photoCategories
                .AsQueryable()
                .OrderBy(p => p.CategoryId)
                .Select(p => mapper.BuildViewModel(p))
                .ToListAsync();
            return viewModel;
        }
    }
}
