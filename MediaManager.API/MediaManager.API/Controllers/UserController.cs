using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.API.Helpers;
using MediaManager.API.Models;
using MediaManager.API.Repository;
using MediaManager.API.Services;
using MediaManager.DAL.DBEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MediaManager.API.Controllers
{
    [Authorize]
    [EnableCors("AnyGET")]
    //[Route("api/Users")]
    public class UserController : Controller
    {
        private UserRepository userRepository;
        private ITypeHelperService typeHelperService;
        private IPropertyMappingService propertyMappingService;
        private IUrlHelper urlHelper;
        private UserManager<IdentityUser> userManager;

        public UserController(UserRepository userRepository, ITypeHelperService typeHelperService, IUrlHelper urlHelper, IPropertyMappingService propertyMappingService, UserManager<IdentityUser> userManager)
        {
            this.userRepository = userRepository;
            this.typeHelperService = typeHelperService;
            this.propertyMappingService = propertyMappingService;
            this.urlHelper = urlHelper;
            this.userManager = userManager;
        }

        [EnableCors("AnyGET")]
        [Authorize(Policy = "Reader")]
        [HttpGet("api/Users/{username}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest();
           // var userIdentity = await userManager.FindByNameAsync(this.User.Identity.Name);
            var result = userRepository.GetEntity(username);
            if (result == null)
                return NotFound();
            return Ok(result);

        }

        [Authorize(Policy = "Reader")]
        [EnableCors("AnyGET")]
        [HttpGet(Name = "GetUsers")]
        public IActionResult GetUsers([FromQuery] UserResourceParameter parameter)
        {
            if (!typeHelperService.TypeHasProperties<UserForDisplayDTO>(parameter.Fields))
                return BadRequest();

            if (!propertyMappingService.ValidMappingExistsFor<UserForDisplayDTO, User>(parameter.OrderBy))
                return BadRequest();

            var userList = userRepository.GetEntities(parameter);

            var mappedUserList = AutoMapper.Mapper.Map<IEnumerable<UserForDisplayDTO>>(userList);
            var videosForDisplay = mappedUserList.ShapeData<UserForDisplayDTO>(parameter.Fields);

            var result = videosForDisplay.Select(user =>
              {
                  var userAsDictionary = user as IDictionary<string, object>;
                  var userLinks = CreateLinksForUser((string)userAsDictionary["Username"]);

                  userAsDictionary.Add("links", userLinks);
                  return userAsDictionary;
              });
            var linksForUsersCollection = CreateLinksForUsers(parameter, userList.HasNext, userList.HasPrevious);
            var resultWithLinks = new { value = result, links = linksForUsersCollection};
            return Ok(resultWithLinks);
        }

        [Authorize(Policy = "SuperUser")]
        [EnableCors("default")]
        [HttpDelete("{username}", Name = "DeleteUser")]
        public IActionResult DeleteUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return BadRequest();
            if (!userRepository.DeleteEntity(username))
                return new StatusCodeResult(500);
            return NoContent();

        }

        [Authorize(Policy = "Writer")]
        [EnableCors("default")]
        [HttpPost("api/Users", Name = "CreateUser")]
        public async Task<IActionResult> PostUser(string username, [FromBody] UserForCreationDTO userForCreationDto)
        {
            if (userForCreationDto == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new UnAcceptableEntity(ModelState);

            var user = AutoMapper.Mapper.Map<User>(userForCreationDto);

            user.RegistrationDate = DateTime.Now;

            var result = await userRepository.PostEntity(user);
            if (result == null)
                return new StatusCodeResult(500);
            var mappedResult = AutoMapper.Mapper.Map<UserForDisplayDTO>(result);
            return CreatedAtRoute("GetUser", new { username = userForCreationDto.Username }, mappedResult);


        }

        [Authorize(Policy = "SuperUser")]
        [EnableCors("default")]
        [HttpPut("api/Users/{username}", Name = "UpdateUser")]
        public async Task<IActionResult> UpdateUser(string username, [FromBody] UserForUpdateDTO userForUpdateDto)
        {
            if (userForUpdateDto == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return new UnAcceptableEntity(ModelState);



            var entity = userRepository.GetEntity(username);
            if (entity == null)
                return NotFound();
            entity.Email = userForUpdateDto.Email;
           

            var result = await userRepository.UpdateEntity(entity);
            if (result == null)
                return new StatusCodeResult(500);
            var mappedResult = AutoMapper.Mapper.Map<UserForDisplayDTO>(result);
            return CreatedAtRoute("GetUser", new { username = entity.Username }, mappedResult);


        }

        [Authorize(Policy = "SuperUser")]
        [EnableCors("default")]
        [HttpPatch("api/Users/{user}", Name = "PartiallyUpdateUser")]
        public IActionResult PatchUser(string user, [FromBody] JsonPatchDocument<UserForUpdateDTO> patchDocument)
        {
            if (patchDocument == null)
                return BadRequest();
            var entity = userRepository.GetEntity(user);
            if (entity == null)
                return NotFound();
            var userToPatch = AutoMapper.Mapper.Map<UserForUpdateDTO>(entity);
            patchDocument.ApplyTo(userToPatch);

            // add validation

            AutoMapper.Mapper.Map(userToPatch, entity);
           var updatedUser =  userRepository.UpdateEntity(entity);
            if (updatedUser == null)
                throw new Exception($"patching user with username = {user} failed");
            return CreatedAtRoute("GetUser", new { username = user }, updatedUser);
        }

        private object CreateLinksForUsers(UserResourceParameter parameter, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDTO>();
            links.Add(new LinkDTO(GenerateUsersResourceUri(parameter, ResourceUriType.Current), "self", "GET"));
            if (hasNext)
                links.Add(new LinkDTO(GenerateUsersResourceUri(parameter, ResourceUriType.nextPage), "next", "GET"));
            if (hasPrevious)
                links.Add(new LinkDTO(GenerateUsersResourceUri(parameter, ResourceUriType.previousPage), "prev", "GET"));
            return links;
        }

        private string GenerateUsersResourceUri(UserResourceParameter parameter, ResourceUriType uriType)
        {
            switch (uriType)
            {


                case ResourceUriType.nextPage:
                    return urlHelper.Link("GetUsers", new
                    {
                        parameter = new UserResourceParameter
                        {
                            Username = parameter.Username,
                            searchQuery = parameter.searchQuery,
                            Fields = parameter.Fields,
                            PageNumber = parameter.PageNumber + 1,
                            PageSize = parameter.PageSize,
                            OrderBy = parameter.OrderBy,
                        }
                    });

                case ResourceUriType.previousPage:
                    return urlHelper.Link("GetUsers", new
                    {
                        parameter = new UserResourceParameter
                        {
                            Username = parameter.Username,
                            searchQuery = parameter.searchQuery,
                            Fields = parameter.Fields,
                            PageNumber = parameter.PageNumber - 1,
                            PageSize = parameter.PageSize,
                            OrderBy = parameter.OrderBy,
                        }
                    });
                default:
                case ResourceUriType.Current:
                    return urlHelper.Link("GetUsers", new
                    {
                        parameter = new UserResourceParameter
                        {
                            Username = parameter.Username,
                            searchQuery = parameter.searchQuery,
                            Fields = parameter.Fields,
                            PageNumber = parameter.PageNumber,
                            PageSize = parameter.PageSize,
                            OrderBy = parameter.OrderBy,
                        }
                    });

            }


        }

        private object CreateLinksForUser(string user)
        {
            var links = new List<LinkDTO>();
            links.Add(new LinkDTO(urlHelper.Link("GetUser", new { username = user }), "self", "GET"));
            return links;
        }

    }
}
