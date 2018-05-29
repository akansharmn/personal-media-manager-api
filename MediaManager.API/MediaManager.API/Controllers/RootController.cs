using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace MediaManager.API.Controllers
{
    /// <summary>
    /// A controller which 
    /// </summary>
    public class RootController : Controller
    {
        private IUrlHelper urlHelper;

        public RootController(IUrlHelper urlHelper)
        {
            this.urlHelper = urlHelper;
        }
        
        [HttpGet(Name = "GetRoot")]
        public IActionResult GetRoot([FromHeader(Name = "Accept")] string mediaType)
        {
            if(mediaType == "application/vnd.ak.hateoas+json")
            {
                var links = new List<LinkDTO>();
                return Ok(links);
            }
            return NoContent();
        }
    }
}
