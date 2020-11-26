using Marcware.JudgeMyPhoto.Constants;
using Marcware.JudgeMyPhoto.ViewModels.Photo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Marcware.JudgeMyPhoto.Controllers
{
    [Authorize(Roles = JudgeMyPhotoRoles.Photographer)]
    public class PhotoController : Controller
    {
        public IActionResult Submit(int id)
        {
            SubmitPhotosViewModel viewModel = new SubmitPhotosViewModel();
            viewModel.CategoryId = id;
            return View(viewModel);
        }

        public IActionResult Index(int id)
        {
            return View();
        }
    }
}
