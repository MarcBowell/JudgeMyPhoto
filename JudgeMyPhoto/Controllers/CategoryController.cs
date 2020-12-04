using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marcware.JudgeMyPhoto.Classes;
using Marcware.JudgeMyPhoto.Constants;
using Marcware.JudgeMyPhoto.Entities.Context;
using Marcware.JudgeMyPhoto.Entities.Models;
using Marcware.JudgeMyPhoto.ExtensionMethods;
using Marcware.JudgeMyPhoto.ViewModelMappers.Category;
using Marcware.JudgeMyPhoto.ViewModels.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Marcware.JudgeMyPhoto.Controllers
{
    public class CategoryController : Controller
    {
        #region Constructor
        private readonly JudgeMyPhotoDbContext _db;

        public CategoryController(JudgeMyPhotoDbContext db)
        {
            _db = db;
        } 
        #endregion

        #region Index
        [Authorize(Roles = JudgeMyPhotoRoles.AllRoles)]
        public async Task<IActionResult> Index()
        {
            CategoryListViewModelMapper mapper = new CategoryListViewModelMapper();
            CategoryListViewModel viewModel = await mapper.BuildViewModelAsync(_db.PhotoCategories);
            return View(viewModel);
        }
        #endregion

        #region Edit
        [Authorize(Roles = JudgeMyPhotoRoles.Admin)]
        public async Task<IActionResult> Edit(int id)
        {
            ProcessResult<PhotoCategory> categoryProcessResult = await GetPhotoCategory(id);
            ModelState.AddProcessResultErrors(categoryProcessResult);
            CategoryViewModelMapper mapper = new CategoryViewModelMapper();
            CategoryViewModel viewModel = mapper.BuildViewModel(categoryProcessResult.Result);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = JudgeMyPhotoRoles.Admin)]
        public async Task<ActionResult> Edit(CategoryViewModel viewModel)
        {
            if (viewModel == null)
                ModelState.AddModelError(string.Empty, "View model is null");

            if (ModelState.IsValid)
            {
                ProcessResult<bool> categoryUniqueResult = await CategoryViewModelIsValid(viewModel, viewModel.CategoryId);
                ModelState.AddProcessResultErrors(categoryUniqueResult);
            }

            if (ModelState.IsValid)
            {
                ProcessResult<PhotoCategory> categoryProcessResult = await GetPhotoCategory(viewModel.CategoryId);
                ModelState.AddProcessResultErrors(categoryProcessResult);

                if (categoryProcessResult.Success)
                {                    
                    CategoryViewModelMapper mapper = new CategoryViewModelMapper();

                    ProcessResult<List<Photograph>> photosToUpdateResult = new ProcessResult<List<Photograph>>();                    
                    if ((viewModel.StatusCode == CategoryStatusCodes.Judging || viewModel.StatusCode == CategoryStatusCodes.Completed) && 
                        categoryProcessResult.Result.StatusCode != viewModel.StatusCode)
                    {
                        photosToUpdateResult = await GetPhotoNameUpdatesAsync(viewModel.CategoryId, viewModel.PhotoNamingThemeCode);
                    }

                    // Save photo naming changes
                    ProcessResult<bool> saveResult = new ProcessResult<bool>();
                    if (photosToUpdateResult.Success && (photosToUpdateResult.Result?.Count ?? 0) > 0)
                    {
                        saveResult = _db.SaveUpdates(photosToUpdateResult.Result.ToArray());
                    }

                    // Save category changes
                    if (photosToUpdateResult.Success && saveResult.Success)
                    {
                        PhotoCategory repositoryModel = mapper.BuildRepositoryModel(viewModel, categoryProcessResult.Result);
                        saveResult = _db.SaveUpdates(repositoryModel);
                    }

                    ModelState.AddProcessResultErrors(photosToUpdateResult);
                    ModelState.AddProcessResultErrors(saveResult);
                }
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            else
            {
                CategoryViewModelMapper mapper = new CategoryViewModelMapper();
                viewModel = mapper.BuildViewModelDropdownCriteria(viewModel);
                return View(viewModel);
            }
        }

        private async Task<ProcessResult<List<Photograph>>> GetPhotoNameUpdatesAsync(int categoryId, string namingThemeCode)
        {
            string[] existingNames = await _db.Photographs
                .Where(p => p.Category.CategoryId == categoryId && !string.IsNullOrEmpty(p.AnonymousPhotoName))
                .Select(p => p.AnonymousPhotoName)
                .ToArrayAsync();

            List<string> potentialNames = PhotoNamingThemes
                .GetThemeItems(namingThemeCode)
                .Minus(existingNames)
                .InRandomSequence();

            List<Photograph> photosToUpdate = await _db.Photographs
                .Where(p => p.Category.CategoryId == categoryId && string.IsNullOrEmpty(p.AnonymousPhotoName))
                .ToListAsync();
            photosToUpdate = photosToUpdate.InRandomSequence();

            ProcessResult<List<Photograph>> result = new ProcessResult<List<Photograph>>();
            if (photosToUpdate.Count > potentialNames.Count)
            {
                result.AddError("Not enough anonymous photo names to assign for the number of photos in this category");
            }
            else
            {
                int itemNo = 0;
                foreach (Photograph photo in photosToUpdate)
                {
                    photo.AnonymousPhotoName = potentialNames[itemNo];
                    itemNo++;
                }
                result.SetResult(photosToUpdate);
            }

            return result;
        }
        #endregion

        #region Add
        [Authorize(Roles = JudgeMyPhotoRoles.Admin)]
        public IActionResult Add()
        {
            CategoryViewModelMapper mapper = new CategoryViewModelMapper();
            CategoryViewModel viewModel = mapper.BuildViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = JudgeMyPhotoRoles.Admin)]
        public async Task<IActionResult> Add(CategoryViewModel viewModel)
        {
            if (viewModel == null)
                ModelState.AddModelError(string.Empty, "View model is null");

            if (ModelState.IsValid)
            {
                ProcessResult<bool> categoryUniqueResult = await CategoryViewModelIsValid(viewModel);
                ModelState.AddProcessResultErrors(categoryUniqueResult);                
            }

            if (ModelState.IsValid)
            {
                CategoryViewModelMapper mapper = new CategoryViewModelMapper();
                PhotoCategory repositoryModel = mapper.BuildRepositoryModel(viewModel, null);
                ProcessResult<bool> saveResult = _db.SaveAdditions(repositoryModel);
                ModelState.AddProcessResultErrors(saveResult);
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Index");
            }
            else
            {
                CategoryViewModelMapper mapper = new CategoryViewModelMapper();
                viewModel = mapper.BuildViewModelDropdownCriteria(viewModel);
                return View(viewModel);
            }
        }
        #endregion

        #region Delete
        [Authorize(Roles = JudgeMyPhotoRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            ProcessResult<PhotoCategory> categoryProcessResult = await GetPhotoCategory(id);
            ModelState.AddProcessResultErrors(categoryProcessResult);
            DeleteCategoryViewModelMapper mapper = new DeleteCategoryViewModelMapper();
            DeleteCategoryViewModel viewModel = mapper.BuildViewModel(categoryProcessResult.Result);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = JudgeMyPhotoRoles.Admin)]
        public async Task<IActionResult> Delete(DeleteCategoryViewModel viewModel)
        {
            ProcessResult<PhotoCategory> categoryProcessResult = await GetPhotoCategory(viewModel.CategoryId);
            ModelState.AddProcessResultErrors(categoryProcessResult);
            if (ModelState.IsValid)
            {
                ProcessResult<bool> saveResult = _db.SaveRemoves(categoryProcessResult.Result);
                ModelState.AddProcessResultErrors(saveResult);
            }

            if (ModelState.IsValid)
                return RedirectToAction("Index");
            else
                return View(viewModel);
        }
        #endregion

        #region Helper validation methods
        private async Task<ProcessResult<bool>> CategoryViewModelIsValid(CategoryViewModel viewModel, int existingCategoryId = 0)
        {
            ProcessResult<bool> result = new ProcessResult<bool>();

            PhotoCategory category = await _db
                .PhotoCategories
                .FirstOrDefaultAsync(p => p.CategoryName.ToUpper() == viewModel.CategoryName.ToUpper()
                    && p.CategoryId != existingCategoryId);
            if (category != null)
                result.AddFieldError("CategoryName", "You dummy. A category with name has already been created");

            if (!CategoryStatusCodes.GetAll().Contains(viewModel.StatusCode))
                result.AddFieldError("StatusCode", "Invalid status code");

            if (!PhotoNamingThemes.ThemeCodes.GetAll().Contains(viewModel.PhotoNamingThemeCode))
                result.AddFieldError("PhotoNamingThemeCode", "Invalid theme code");

            if (existingCategoryId != 0)
            {
                category = await _db
                    .PhotoCategories
                    .FirstOrDefaultAsync(p => p.CategoryId == existingCategoryId);
                if (category == null)
                    result.AddError("This category cannot be found"); 
            }

            return result;
        }
        #endregion

        #region Helper methods
        private async Task<ProcessResult<PhotoCategory>> GetPhotoCategory(int categoryId)
        {
            ProcessResult<PhotoCategory> result = new ProcessResult<PhotoCategory>();
            PhotoCategory category = await _db
                .PhotoCategories
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
            if (category != null)
            {
                result.SetResult(category);
            }
            else
            {
                result.SetResult(new PhotoCategory());
                result.AddError("Unable to find this category");             
            }
            return result;
        } 
        #endregion
    }
}
