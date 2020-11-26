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
            if (ModelState.IsValid)
            {
                ProcessResult<PhotoCategory> categoryProcessResult = await GetPhotoCategory(viewModel.CategoryId);
                ModelState.AddProcessResultErrors(categoryProcessResult);

                if (categoryProcessResult.Success)
                {
                    CategoryViewModelMapper mapper = new CategoryViewModelMapper();
                    PhotoCategory repositoryModel = mapper.BuildRepositoryModel(viewModel, categoryProcessResult.Result);
                    ProcessResult<bool> saveResult = _db.SaveUpdates(repositoryModel);
                    ModelState.AddProcessResultErrors(saveResult);
                }
            }

            if (ModelState.IsValid)
                return RedirectToAction("Index");
            else
                return View(viewModel);
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
        public IActionResult Add(CategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CategoryViewModelMapper mapper = new CategoryViewModelMapper();
                PhotoCategory repositoryModel = mapper.BuildRepositoryModel(viewModel, null);
                ProcessResult<bool> saveResult = _db.SaveAdditions(repositoryModel);
                ModelState.AddProcessResultErrors(saveResult);
            }

            if (ModelState.IsValid)
                return RedirectToAction("Index");
            else
                return View(viewModel);
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
