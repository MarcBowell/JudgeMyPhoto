using System.Collections.Generic;
using System.Linq;
using Marcware.JudgeMyPhoto.Constants;
using Marcware.JudgeMyPhoto.Entities.Models;
using Marcware.JudgeMyPhoto.ViewModels.Category;

namespace Marcware.JudgeMyPhoto.ViewModelMappers.Category
{
    internal class CategoryViewModelMapper
    {
        public CategoryViewModel BuildViewModel(PhotoCategory repositoryModel = null)
        {
            CategoryViewModel viewModel = new CategoryViewModel();
            if (repositoryModel == null)
            {
                viewModel.StatusText = "Initialising category";
                viewModel.StatusCode = CategoryStatusCodes.SettingUp;
            }
            else
            {
                viewModel.CategoryId = repositoryModel.CategoryId;
                viewModel.CategoryName = repositoryModel.CategoryName;
                if (CategoryStatusCodes.GetAll().Contains(repositoryModel.StatusCode))
                {
                    viewModel.StatusCode = repositoryModel.StatusCode;
                    viewModel.StatusText = repositoryModel.StatusText;
                }
                else
                {
                    viewModel.StatusText = "Initialising category";
                    viewModel.StatusCode = CategoryStatusCodes.SettingUp;
                }
            }
            viewModel.StatusCodeTypes = CategoryStatusCodes
                .GetAll()
                .Select(c => new KeyValuePair<string, string>(c, CategoryStatusCodes.GetStatusText(c)));
            return viewModel;
        }

        public PhotoCategory BuildRepositoryModel(CategoryViewModel viewModel, PhotoCategory existingRepositoryModel)
        {
            if (existingRepositoryModel == null)
                existingRepositoryModel = new PhotoCategory();
            existingRepositoryModel.CategoryName = viewModel.CategoryName;
            existingRepositoryModel.StatusCode = viewModel.StatusCode;
            existingRepositoryModel.StatusText = viewModel.StatusText;
            return existingRepositoryModel;
        }
    }
}
