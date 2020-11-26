using System.Threading.Tasks;
using Marcware.JudgeMyPhoto.Classes;
using Marcware.JudgeMyPhoto.Constants;
using Marcware.JudgeMyPhoto.Entities.Models;
using Marcware.JudgeMyPhoto.ExtensionMethods;
using Marcware.JudgeMyPhoto.ViewModelMappers.Account;
using Marcware.JudgeMyPhoto.ViewModels.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Marcware.JudgeMyPhoto.Controllers
{
    public class AccountController : Controller
    {
        #region Constructor
        private readonly UserManager<ApplicationUser> _userManager;
        public readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        } 
        #endregion

        #region Index
        [Authorize(Roles = JudgeMyPhotoRoles.Admin)]
        public IActionResult Index()
        {
            UserListViewModelMapper mapper = new UserListViewModelMapper();
            UserListViewModel viewModel = mapper.BuildViewModel(_userManager.Users);
            return View(viewModel);
        } 
        #endregion

        #region My profile
        [Authorize(Roles = JudgeMyPhotoRoles.AllRoles)]
        public async Task<IActionResult> MyProfile()
        {
            string userName = User.Identity.Name;
            ProcessResult<UserViewModel> viewModelProcess = await GetUserViewModel(userName);
            ModelState.AddProcessResultErrors(viewModelProcess);
            return View(viewModelProcess.Result);
        } 

        [HttpPost]
        [Authorize(Roles = JudgeMyPhotoRoles.AllRoles)]
        public async Task<IActionResult> MyProfile(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                ProcessResult<bool> result = await SaveUserViewModel(viewModel, FormMode.Edit);
                ModelState.AddProcessResultErrors(result);
            }

            if (ModelState.IsValid)
                return RedirectToAction("Index", "Category");
            else
                return View(viewModel);
        }
        #endregion

        #region Add
        [Authorize(Roles = JudgeMyPhotoRoles.Admin)]
        public IActionResult Add()
        {
            UserViewModel viewModel = new UserViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = JudgeMyPhotoRoles.Admin)]
        public async Task<IActionResult> Add(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                ProcessResult<bool> result = await SaveUserViewModel(viewModel, FormMode.Add);
                ModelState.AddProcessResultErrors(result);
            }

            if (ModelState.IsValid)
                return RedirectToAction("Index");
            else
                return View(viewModel);
        }
        #endregion

        #region Edit
        public async Task<IActionResult> Edit(string id)
        {
            ProcessResult<UserViewModel> viewModelProcess = await GetUserViewModel(id);
            ModelState.AddProcessResultErrors(viewModelProcess);
            return View(viewModelProcess.Result);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                ProcessResult<bool> result = await SaveUserViewModel(viewModel, FormMode.Edit);
                ModelState.AddProcessResultErrors(result);
            }

            if (ModelState.IsValid)
                return RedirectToAction("Index");
            else
                return View(viewModel);
        }
        #endregion

        #region Delete
        [Authorize(Roles = JudgeMyPhotoRoles.Admin)]
        public async Task<IActionResult> Delete(string id)
        {
            ProcessResult<UserViewModel> viewModelProcess = await GetUserViewModel(id);
            ModelState.AddProcessResultErrors(viewModelProcess);
            return View(viewModelProcess.Result);
        }

        [HttpPost]
        [Authorize(Roles = JudgeMyPhotoRoles.Admin)]
        public async Task<IActionResult> Delete(UserViewModel viewModel)
        {
            IdentityResult result = await _userManager.DeleteApplicationUserAsync(viewModel.UserName);
            ModelState.AddIdentityResultErrors(result);
            if (result.Succeeded)
                return RedirectToAction("Index");
            else
                return View(viewModel);
        }
        #endregion

        #region Timeout
        public IActionResult Timeout()
        {
            return View();
        }
        #endregion

        #region Sign out
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        } 
        #endregion

        #region Helper methods
        private async Task<ProcessResult<UserViewModel>> GetUserViewModel(string userName)
        {
            ProcessResult<UserViewModel> result = new ProcessResult<UserViewModel>();

            ApplicationUser user = await _userManager.FindByNameAsync(userName);
            if (user != null)
            {
                UserViewModelMapper mapper = new UserViewModelMapper();
                UserViewModel viewModel = mapper.BuildViewModel(user);
                result.SetResult(viewModel);
            }
            else
            {
                result.AddError("Unable to find this user");
                result.SetResult(new UserViewModel());
            }

            return result;
        }

        private async Task<ProcessResult<bool>> SaveUserViewModel(UserViewModel viewModel, FormMode mode)
        {
            ProcessResult<bool> processResult = new ProcessResult<bool>();
            if (mode == FormMode.Edit)
            {
                ApplicationUser user = await _userManager.FindByNameAsync(viewModel.UserName);
                if (user != null)
                {
                    UserViewModelMapper mapper = new UserViewModelMapper();
                    user = mapper.BuildRepositoryModel(viewModel, user);
                    IdentityResult identityResult = await _userManager.UpdateAsync(user);
                    processResult.SetResult(identityResult);

                    if (identityResult.Succeeded && !string.IsNullOrEmpty(viewModel.Password))
                    {
                        string token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        identityResult = await _userManager.ResetPasswordAsync(user, token, viewModel.Password);
                        processResult.SetResult(identityResult);
                    }
                }
                else
                {
                    processResult.AddError("Unable to find this user");
                }
            }
            else
            {
                IdentityResult identityResult = await _userManager.CreateApplicationUserAsync(viewModel.UserName, viewModel.Nickname, viewModel.Email, viewModel.Password, JudgeMyPhotoRoles.Photographer);
                processResult.SetResult(identityResult);
            }
            return processResult;
        } 
        #endregion
    }
}
