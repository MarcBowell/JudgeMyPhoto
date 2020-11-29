using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Marcware.JudgeMyPhoto.Classes;
using Marcware.JudgeMyPhoto.Constants;
using Marcware.JudgeMyPhoto.Entities.Context;
using Marcware.JudgeMyPhoto.Entities.Models;
using Marcware.JudgeMyPhoto.ExtensionMethods;
using Marcware.JudgeMyPhoto.ViewModelMappers.Photo;
using Marcware.JudgeMyPhoto.ViewModels.Photo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Marcware.JudgeMyPhoto.Controllers
{
    [Authorize(Roles = JudgeMyPhotoRoles.Photographer)]
    public class PhotoController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JudgeMyPhotoDbContext _db;

        public PhotoController(UserManager<ApplicationUser> userManager, JudgeMyPhotoDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public IActionResult Submit(int id)
        {
            SubmitPhotosViewModel viewModel = new SubmitPhotosViewModel();
            viewModel.CategoryId = id;
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = JudgeMyPhotoRoles.Photographer)]
        public async Task<ProcessResult<string>> SubmitPhoto(SubmitSinglePhotoViewModel viewModel)
        {
            ProcessResult<string> result = new ProcessResult<string>();

            Photograph photo = await _db.Photographs
                .FirstOrDefaultAsync(p => 
                    p.Category.CategoryId == viewModel.CategoryId && 
                    p.UserCategoryPhotoNumber == viewModel.PhotoNumber &&
                    p.Photographer.UserName == User.Identity.Name);
            SubmitSinglePhotoViewModelMapper mapper = new SubmitSinglePhotoViewModelMapper();
            ProcessResult<bool> saveResult = null;

            if (photo == null)
            {
                ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                    result.AddError("Unable to find this user");

                PhotoCategory category = await _db.PhotoCategories.FirstOrDefaultAsync(c => c.CategoryId == viewModel.CategoryId);
                if (category == null)
                    result.AddError("Unable to find this category");
                
                if (result.Success)
                    photo = mapper.BuildRepositoryModel(viewModel, user, category);

                saveResult = _db.SaveAdditions(photo);
            }
            else
            {
                photo = mapper.BuildRepositoryModel(viewModel, photo);
                saveResult = _db.SaveUpdates(photo);
            }

            if (result.Success)
            {                
                if (saveResult.Success)
                    result.SetResult(GetImageString(photo.SmallImage));
                else
                    result.AddError(saveResult.ErrorMessage);
            }
                        
            return result;
        }

        public async Task<ProcessResult<string>> GetPreviewPhoto(int cId, short pId)
        {
            Photograph photo = await _db.Photographs
                .FirstOrDefaultAsync(p =>
                    p.Category.CategoryId == cId &&
                    p.UserCategoryPhotoNumber == pId &&
                    p.Photographer.UserName == User.Identity.Name);

            ProcessResult<string> result = new ProcessResult<string>();
            if (photo == null)
                result.SetResult(string.Empty);
            else
                result.SetResult(GetImageString(photo.SmallImage));

            return result;                
        }

        public IActionResult Index(int id)
        {
            ViewPhotosViewModelMapper mapper = new ViewPhotosViewModelMapper();
            ViewPhotosViewModel viewModel = mapper.BuildViewModel(id);
            return View(viewModel);
        }

        public async Task<ActionResult> GetFullPhoto(int pId, int cId)
        {
            Photograph photo = await _db.Photographs
                .FirstOrDefaultAsync(p => p.PhotoId == pId && p.Category.CategoryId == cId);

            if (photo == null)
                return new JsonResult(string.Empty);
            else
                return File(photo.LargeImage, "image/jpg");
        }

        public async Task<JsonResult> GetPhotosForCategory(int id)
        {
            List<Photograph> photos = await _db.Photographs
                .Where(p => p.Category.CategoryId == id)
                .ToListAsync();
            PhotoViewModelMapper mapper = new PhotoViewModelMapper();
            List<PhotoViewModel> result = photos
                .InRandomSequence()
                .Select(p => mapper.BuildViewModel(p))
                .ToList();
            return new JsonResult(result);
        }

        private string GetImageString(byte[] imageContents)
        {
            string imreBase64Data = Convert.ToBase64String(imageContents);
            string result = string.Format("data:image/jpg;base64,{0}", imreBase64Data);
            return result;
        }
    }
}
