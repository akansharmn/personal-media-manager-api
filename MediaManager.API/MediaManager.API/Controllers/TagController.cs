using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.API.Models;
using MediaManager.API.Repository;
using MediaManager.DAL.DBEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;

namespace MediaManager.API.Controllers
{
    /// <summary>
    /// The API controller to handle tags related calls
    /// </summary>
    [Route("api/Tags/{contentId}/{id}")]
    public class TagController : Controller
    {
        TagRepository tagRepository;
        IUrlHelper urlHelper;

        /// <summary>
        /// Constructor of TagController
        /// </summary>
        /// <param name="tagRepository">Tag repository</param>
        /// <param name="urlHelper">URL Helper class to help  create links</param>
        public TagController(TagRepository tagRepository, IUrlHelper urlHelper)
        {
            this.urlHelper = urlHelper;
            this.tagRepository = tagRepository;
        }

        /// <summary>
        /// Retrieves a list of tags for a particular item of a particular content type
        /// </summary>
        /// <param name="contentId">contentid denoting the media type </param>
        /// <param name="id">the id of the item for which tags are to be retrieved</param>
        /// <response code="200">Tags retrieved</response>
        /// <response code="400">Tags retrieval parameter has missing/invalid values</response>
        /// <response code="500">Oops! Can't retrieve tags entry right now</response>
        [ProducesResponseType(typeof(List<string>), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        [Authorize(Policy = "Reader")]
        [EnableCors("default")]
        [HttpGet(Name ="GetTags")]
        public async Task<IActionResult> GetTags(int contentId, int id)
        {
            if (contentId == 0 || id == 0)
            {
                return BadRequest();
            }
            var result =await tagRepository.GetTags(contentId, id);
            if (result == null)
            {
                return new StatusCodeResult(500);
            }
            return Ok(result);           
        }

        /// <summary>
        /// Creates tags for a particular item of a particular content type
        /// </summary>
        /// <param name="contentId">the id of content which dones the type of content</param>
        /// <param name="id">id</param>
        /// <param name="tags">List of tags to be added</param>
        /// <response code="201">Tags created</response>
        /// <response code="400">Tags creation parameter has missing/invalid values</response>
        /// <response code="500">Oops! Can't create tags entry right now</response>
        [ProducesResponseType(typeof(List<string>), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        [Authorize(Policy = "Writer")]
        [EnableCors("default")]
        [HttpPost]
        public async Task<IActionResult> AddTags(int contentId, int id,[FromBody] TagsInputDTO tags)
        {
            if (contentId == 0 || id == 0 || tags == null)
            {
                return BadRequest();
            }
            var result = await tagRepository.AddTags(contentId, id, tags.tags);
            if (result == false)
            {
                return new StatusCodeResult(500);
            }
            return CreatedAtRoute("GetTags", new { contentId = contentId, id = id });

        }

        /// <summary>
        /// Deletes tags for a particular item of a particular content type
        /// </summary>
        /// <param name="contentId">the id of content which dones the type of content</param>
        /// <param name="id">id</param>
        /// <param name="tags">List of tags to be deleted</param>
        /// <response code="201">Tags deleted</response>
        /// <response code="400">Tags deletion parameter has missing/invalid values</response>
        /// <response code="500">Oops! Can't delete tags entry right now</response>
        [ProducesResponseType(typeof(List<string>), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        [Authorize(Policy = "Reader")]
        [EnableCors("default")]
        [HttpDelete]
        public async Task<IActionResult> DeleteTags(int contentId, int id, [FromBody] TagsInputDTO tags)
        {
            if (contentId == 0 || id == 0 || tags == null)
            {
                return BadRequest();
            }
            var result = await tagRepository.DeleteTags(contentId, id, tags.tags);
            if (result == false)
            {
                return new StatusCodeResult(500);
            }
            return CreatedAtRoute("GetTags", new { contentId = contentId, id = id });

        }

        /// <summary>
        /// Creates tags for a particular item of a particulat content type
        /// </summary>
        /// <param name="contentId">the id of content which dones the type of content</param>
        /// <param name="id">id</param>
        /// <param name="patchDocument">list of json patch documents containing the update informations</param>
        /// <response code="201">Tags updated</response>
        /// <response code="400">Tags updation parameter has missing/invalid values</response>
        /// <response code="500">Oops! Can't update tags entry right now</response>
        [ProducesResponseType(typeof(List<string>), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        [Authorize(Policy = "Reader")]
        [EnableCors("default")]
        [HttpPatch]
        public async Task<IActionResult> UpdateTags(int contentId, int id, [FromBody] JsonPatchDocument<Tag> patchDocument)
        {
            try
            {
                if (contentId == 0 || id == 0 || patchDocument == null)
                {
                    return BadRequest();
                }
                var result = await tagRepository.GetEntity(contentId, id);
                if (result == null)
                {
                    return NotFound();
                }
                patchDocument.ApplyTo(result);
            }
            catch(Exception)
            {
                return new StatusCodeResult(500);
            }
            return CreatedAtRoute("GetTags", new { contentId = contentId, id =id });
        }
    }
}
