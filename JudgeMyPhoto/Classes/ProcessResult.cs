using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Marcware.JudgeMyPhoto.Classes
{
    public class ProcessResult<T>
    {
        /// <summary>
        /// Log an error that has occurred in the process
        /// </summary>
        /// <param name="errorMessage"></param>
        public void AddError(string errorMessage)
        {
            ErrorMessage = errorMessage;
            Success = false;
        }

        /// <summary>
        /// Set the result of the process
        /// </summary>
        /// <param name="result"></param>
        public void SetResult(T result)
        {
            Result = result;
        }

        /// <summary>
        /// Set the result of the process
        /// </summary>
        /// <param name="result"></param>
        public void SetResult(IdentityResult result)
        {
            Success = result.Succeeded;
            if (!result.Succeeded)
                ErrorMessage = result.Errors.FirstOrDefault().Description;            
        }

        /// <summary>
        /// Was the process successful?
        /// </summary>
        public bool Success { get; private set; } = true;

        /// <summary>
        /// Error that occurred
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// Result of the process
        /// </summary>
        public T Result { get; private set; }
    }
}
