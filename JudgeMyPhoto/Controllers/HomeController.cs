using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using JudgeMyPhoto.Models;
using Marcware.JudgeMyPhoto.ViewModels;
using Microsoft.AspNetCore.Identity;
using Marcware.JudgeMyPhoto.Entities.Models;

namespace JudgeMyPhoto.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public readonly SignInManager<ApplicationUser> _signInManager;

        public readonly UserManager<ApplicationUser> _userManager;

        public HomeController(ILogger<HomeController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            SignInViewModel viewModel = new SignInViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(SignInViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Assume it is an email address
                if (viewModel.UserName.Contains("@"))
                {
                    ApplicationUser user = await _userManager.FindByEmailAsync(viewModel.UserName);
                    if (user != null)
                        viewModel.UserName = user.UserName;
                }

                // Attempt to sign in
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(viewModel.UserName, viewModel.Password, false, false);
                if (!result.Succeeded)
                    ModelState.AddModelError(string.Empty, "User or password details are incorrect");
            }

            if (ModelState.IsValid)
            {
                return RedirectToAction("Index", "Category");
            }
            else
            {
                viewModel.Password = string.Empty;
                return View(viewModel);
            }
        }        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
