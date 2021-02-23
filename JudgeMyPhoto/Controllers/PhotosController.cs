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
    public class PhotosController : Controller
    {
        #region Constructor
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JudgeMyPhotoDbContext _db;

        public PhotosController(UserManager<ApplicationUser> userManager, JudgeMyPhotoDbContext db)
        {
            _userManager = userManager;
            _db = db;
        }
        #endregion

        #region Submit photos
        public async Task<IActionResult> Submit(int id)
        {
            ProcessResult<bool> categoryStatus = await CategoryIsInRequiredStatus(id, CategoryStatusCodes.SubmittingPhotos);

            if (categoryStatus.Success)
            {
                SubmitPhotosViewModel viewModel = new SubmitPhotosViewModel();
                viewModel.CategoryId = id;
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Category");
            }
        }

        [HttpPost]
        public async Task<ProcessResult<string>> SubmitPhoto(SubmitSinglePhotoViewModel viewModel)
        {
            ProcessResult<string> result = new ProcessResult<string>();

            ProcessResult<bool> categoryStatus = await CategoryIsInRequiredStatus(viewModel.CategoryId, CategoryStatusCodes.SubmittingPhotos);            
            if (!categoryStatus.Success)
                result.AddError(categoryStatus.ErrorMessage);

            if (!viewModel.FileType.ToLower().StartsWith("image"))
                result.AddError("Silly thing. Only an image file type is allowed");

            if (result.Success)
            {
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
                    {
                        photo = mapper.BuildRepositoryModel(viewModel, user, category);
                        saveResult = _db.SaveAdditions(photo);
                    }
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

        [HttpPost]
        public async Task<ProcessResult<bool>> RemovePhoto(RemovePhotoViewModel viewModel)
        {
            ProcessResult<bool> result = new ProcessResult<bool>();
            
            ProcessResult<bool> categoryStatus = await CategoryIsInRequiredStatus(viewModel.CategoryId, CategoryStatusCodes.SubmittingPhotos);
            if (!categoryStatus.Success)
                result.AddError(categoryStatus.ErrorMessage);

            if (result.Success)
            {
                Photograph photo = await _db.Photographs
                    .FirstOrDefaultAsync(p =>
                        p.Category.CategoryId == viewModel.CategoryId &&
                        p.UserCategoryPhotoNumber == viewModel.PhotoNumber &&
                        p.Photographer.UserName == User.Identity.Name);

                if (photo != null)
                {
                    result = _db.SaveRemoves(photo);
                }
                else
                {
                    result.AddError("Unable to find a photo with these details");
                } 
            }

            return result;
        }
        #endregion

        #region Vote photos
        public async Task<ProcessResult<string[]>> SubmitVotes([FromBody]VotePhotosViewModel viewModel)
        {
            ProcessResult<string[]> result = new ProcessResult<string[]>();

            if (viewModel.PhotoIds.Length != 3)
                result.AddError("Invalid number of photos voted for");

            ApplicationUser user = null;
            if (result.Success)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                    result.AddError("Unable to find this user");
            }

            PhotoCategory category = null;
            if (result.Success)
            {
                category = _db.PhotoCategories.FirstOrDefault(c => c.CategoryId == viewModel.CategoryId);
                if (category == null)
                    result.AddError("Unable to find this category");
            }

            PhotoVote[] votes = new PhotoVote[3];
            if (result.Success)
            {                
                int position = 0;
                foreach (int photoId in viewModel.PhotoIds)
                {
                    
                    Photograph photo = _db.Photographs
                        .FirstOrDefault(p => p.PhotoId == photoId);
                    if (photo == null)
                    {
                        result.AddError($"Unable to find photo id {photoId}");
                    }
                    else
                    {
                        PhotoVote vote = new PhotoVote()
                        {
                            Position = position + 1,
                            Voter = user,
                            Photo = photo
                        };
                        votes[position] = vote;
                    }
                    position++;
                }
            }

            if (result.Success)
            {                
                _db.PhotoVotes.RemoveRange(_db.PhotoVotes.Where(p => p.Photo.Category.CategoryId == viewModel.CategoryId && p.Voter.UserName == user.UserName));
                ProcessResult<bool> saveResult = _db.SaveAdditions(votes);
                if (!saveResult.Success)
                    result.AddError(saveResult.ErrorMessage);
            }

            if (result.Success)
            {
                result = await GetExistingVotes(viewModel.CategoryId);
            }

            return result;
        }

        public async Task<ProcessResult<string[]>> GetExistingVotes(int id)
        {
            ProcessResult<string[]> result = new ProcessResult<string[]>();

            ApplicationUser user = null;
            if (result.Success)
            {
                user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                    result.AddError("Unable to find this user");
            }

            if (result.Success)
            {
                string[] votes = _db.PhotoVotes
                        .Where(v => v.Photo.Category.CategoryId == id && v.Voter.UserName == user.UserName)
                        .OrderBy(v => v.Position)
                        .Select(v => v.Photo.AnonymousPhotoName)
                        .ToArray();
                result.SetResult(votes);
            }

            return result;
        }
        #endregion

        #region Photo viewing and judging
        public async Task<IActionResult> Index(int id)
        {
            ProcessResult<bool> categoryStatus = await CategoryIsInRequiredStatus(id, CategoryStatusCodes.Judging, CategoryStatusCodes.Completed);

            if (categoryStatus.Success)
            {
                ViewPhotosViewModelMapper mapper = new ViewPhotosViewModelMapper();
                ViewPhotosViewModel viewModel = mapper.BuildViewModel(id);
                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Index", "Category");
            }
        }

        public async Task<ActionResult> GetFullPhoto(int pId, int cId)
        {
            Photograph photo = await _db.Photographs
                .FirstOrDefaultAsync(p => p.PhotoId == pId && p.Category.CategoryId == cId);

            if (photo == null)
                return new JsonResult(string.Empty);
            else
                return File(photo.LargeImage, photo.FileType);
        }

        public async Task<JsonResult> GetPhotosForCategory(int id)
        {
            List<Photograph> photos = await _db.Photographs
                .Include(p => p.Category)
                .Include(p => p.Photographer)
                .Where(p => p.Category.CategoryId == id)
                .ToListAsync();
            PhotoViewModelMapper mapper = new PhotoViewModelMapper();
            List<PhotoViewModel> result = photos
                .InRandomSequence()
                .Select(p => mapper.BuildViewModel(p))
                .ToList();
            return new JsonResult(result);
        }
        #endregion

        #region Helper methods
        private string GetImageString(byte[] imageContents)
        {
            string imreBase64Data = Convert.ToBase64String(imageContents);
            string result = string.Format("data:image/jpg;base64,{0}", imreBase64Data);
            return result;
        } 

        private async Task<ProcessResult<bool>> CategoryIsInRequiredStatus(int categoryId, params string[] statuses)
        {
            ProcessResult<bool> result = new ProcessResult<bool>();

            PhotoCategory category = await _db.
                PhotoCategories
                .FirstOrDefaultAsync(p => p.CategoryId == categoryId);

            if (category == null)
                result.AddError("Cannot find category");

            if (result.Success && !statuses.Contains(category.StatusCode))
                result.AddError("Category does not have the correct status to perform this action");

            return result;
        }
        #endregion
    }
}
