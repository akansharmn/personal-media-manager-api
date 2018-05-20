using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MediaManager.API.Controllers
{
    internal class UnAcceptableEntity : ObjectResult
    {
        

        public UnAcceptableEntity(ModelStateDictionary modelState):base(new SerializableError(modelState))
        {
            if (modelState == null)
                throw new ArgumentNullException(nameof(modelState));
            StatusCode = 422;
        }
    }
}