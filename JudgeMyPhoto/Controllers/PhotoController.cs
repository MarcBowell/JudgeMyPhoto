using System;
using System.Linq;
using System.Threading.Tasks;
using Marcware.JudgeMyPhoto.Classes;
using Marcware.JudgeMyPhoto.Constants;
using Marcware.JudgeMyPhoto.Entities.Context;
using Marcware.JudgeMyPhoto.Entities.Models;
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

        public IActionResult Index(int id)
        {
            return View();
        }

        private string GetImageString(byte[] imageContents)
        {
            string imreBase64Data = Convert.ToBase64String(imageContents);
            string result = string.Format("data:image/jpg;base64,{0}", imreBase64Data);
            return result;
        }
    }
}
