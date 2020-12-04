using System.Linq;
using Marcware.JudgeMyPhoto.Classes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Marcware.JudgeMyPhoto.ExtensionMethods
{
    internal static class ModelStateExtenstionMethods
    {
        public static void AddIdentityResultErrors(this ModelStateDictionary modelState, IdentityResult result)
        { 
            if (!result.Succeeded)
            {
                modelState.AddModelError(string.Empty, result.Errors.FirstOrDefault().Description);
            }
        }

        public static void AddProcessResultErrors<T>(this ModelStateDictionary modelState, ProcessResult<T> result)
        {
            if (!result.Success)
            {
                if (!string.IsNullOrEmpty(result.ErrorMessage))
                    modelState.AddModelError(string.Empty, result.ErrorMessage);
                foreach (var fieldError in result.FieldErrors)
                {
                    modelState.AddModelError(fieldError.FieldName, fieldError.ErrorMessage);
                }
            }
        }
    }
}
