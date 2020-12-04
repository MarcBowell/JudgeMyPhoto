using System.Collections.Generic;
using System.Linq;
using Marcware.JudgeMyPhoto.Constants;
using Marcware.JudgeMyPhoto.Entities.Models;
using Marcware.JudgeMyPhoto.ExtensionMethods;
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
                viewModel.PhotoNamingThemeCode = PhotoNamingThemes.ThemeCodes.Fruit;
            }
            else
            {
                viewModel.CategoryId = repositoryModel.CategoryId;
                viewModel.CategoryName = repositoryModel.CategoryName;
                viewModel.PhotoNamingThemeCode = repositoryModel.PhotoNamingThemeCode;
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
            viewModel = BuildViewModelDropdownCriteria(viewModel);
            return viewModel;
        }

        public CategoryViewModel BuildViewModelDropdownCriteria(CategoryViewModel viewModel)
        {
            viewModel.StatusCodeTypes = CategoryStatusCodes
                .GetAll()
                .Select(c => new KeyValuePair<string, string>(c, CategoryStatusCodes.GetStatusText(c)));
            viewModel.PhotoNamingThemeTypes = PhotoNamingThemes
                .ThemeCodes
                .GetAll()
                .Select(p => new KeyValuePair<string, string>(p, PhotoNamingThemes.GetThemeDescription(p)));
            return viewModel;
        }

        public PhotoCategory BuildRepositoryModel(CategoryViewModel viewModel, PhotoCategory existingRepositoryModel)
        {
            if (existingRepositoryModel == null)
                existingRepositoryModel = new PhotoCategory();
            existingRepositoryModel.CategoryName = viewModel.CategoryName;
            existingRepositoryModel.StatusCode = viewModel.StatusCode;
            existingRepositoryModel.StatusText = viewModel.StatusText;
            existingRepositoryModel.PhotoNamingThemeCode = viewModel.PhotoNamingThemeCode;
            return existingRepositoryModel;
        }
    }
}
