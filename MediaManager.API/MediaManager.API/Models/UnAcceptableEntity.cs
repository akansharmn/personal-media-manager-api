using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MediaManager.API.Controllers
{
    /// <summary>
    /// Class containing at error result when model state is invalid
    /// </summary>
    public class UnAcceptableEntity : ObjectResult
    {
        /// <summary>
        /// Creates a list of errors from model state to be returned to user
        /// </summary>
        /// <param name="modelState">modelstate</param>
        public UnAcceptableEntity(ModelStateDictionary modelState):base(new SerializableError(modelState))
        {
            if (modelState == null)
            {
                throw new ArgumentNullException(nameof(modelState));
            }
            StatusCode = 422;
        }
    }
}