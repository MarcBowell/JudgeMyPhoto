using System.Linq;
using System.Threading.Tasks;
using Marcware.JudgeMyPhoto.Classes;
using Marcware.JudgeMyPhoto.Constants;
using Marcware.JudgeMyPhoto.Entities.Context;
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
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JudgeMyPhotoDbContext _db;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JudgeMyPhotoDbContext db)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
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
            if (viewModel != null)
                viewModel.UserName = User.Identity.Name;
            else
                ModelState.AddModelError(string.Empty, "View model is empty");

            if (ModelState.IsValid)
            {
                ProcessResult<bool> uniqueUserResult = await UserIdentityDetailsAreUniqueAsync(viewModel, User.Identity.Name);
                ModelState.AddProcessResultErrors(uniqueUserResult);
            }

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
            if (viewModel == null)
                ModelState.AddModelError(string.Empty, "View model is empty");

            if (ModelState.IsValid)
            {
                ProcessResult<bool> uniqueUserResult = await UserIdentityDetailsAreUniqueAsync(viewModel);
                ModelState.AddProcessResultErrors(uniqueUserResult);

                if (string.IsNullOrEmpty(viewModel.Password))
                    ModelState.AddModelError("Password", "A password must be entered");
                if (string.IsNullOrEmpty(viewModel.ConfirmPassword))
                    ModelState.AddModelError("ConfirmPassword", "A password must be entered");
            }

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
        [Authorize(Roles = JudgeMyPhotoRoles.Admin)]
        public async Task<IActionResult> Edit(string id)
        {
            ProcessResult<UserViewModel> viewModelProcess = await GetUserViewModel(id);
            ModelState.AddProcessResultErrors(viewModelProcess);
            return View(viewModelProcess.Result);
        }

        [HttpPost]
        [Authorize(Roles = JudgeMyPhotoRoles.Admin)]
        public async Task<IActionResult> Edit(UserViewModel viewModel)
        {
            if (viewModel == null)
                ModelState.AddModelError(string.Empty, "View model is empty");

            if (ModelState.IsValid)
            {
                ProcessResult<bool> uniqueUserResult = await UserIdentityDetailsAreUniqueAsync(viewModel, viewModel.UserName);
                ModelState.AddProcessResultErrors(uniqueUserResult);
            }

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

        #region Validation helper methods
        private async Task<ProcessResult<bool>> UserIdentityDetailsAreUniqueAsync(UserViewModel viewModel, string existingUserName = "")
        {
            ProcessResult<bool> result = new ProcessResult<bool>();

            // User name is unique            
            ApplicationUser userByUserName = await _userManager.FindByNameAsync(viewModel.UserName);
            // Attempting to add an existing user name
            if (userByUserName != null && string.IsNullOrEmpty(existingUserName))
                result.AddFieldError("UserName", $"Copy cat. User name '{viewModel.UserName}' already exists");
            if (userByUserName == null && !string.IsNullOrEmpty(existingUserName))
                result.AddFieldError("UserName", $"Copy cat. User with user name '{viewModel.UserName}' cannot be found");

            ApplicationUser userByEmail = await _userManager.FindByEmailAsync(viewModel.Email);
            // Attempting to add an email of the same address
            if (userByEmail != null && string.IsNullOrEmpty(existingUserName))
                result.AddFieldError("Email", $"Copy cat. Email address '{viewModel.Email}' already exists for another user");
            if (userByEmail != null && !string.IsNullOrEmpty(existingUserName) && viewModel.UserName != userByEmail.UserName)
                result.AddFieldError("Email", $"Copy cat. Email address '{viewModel.Email}' already exists for another user");

            //TODO: Inject this into the controller
            ApplicationUser userByNickname = _db.Users.FirstOrDefault(u => u.Nickname.ToUpper() == viewModel.Nickname.ToUpper());
            if (userByNickname != null && string.IsNullOrEmpty(existingUserName))
                result.AddFieldError("Nickname", "Copy cat. Nickname already exists for another user");
            if (userByNickname != null && !string.IsNullOrEmpty(existingUserName) && userByNickname.UserName != viewModel.UserName)
                result.AddFieldError("Nickname", "Copy cat. Nickname already exists for another user");

            return result;
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
