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
    /// <summary>
    /// Controller class to handle web API request related to Users
    /// </summary>
    [Authorize]
    [EnableCors("AnyGET")]
    [Route("api/Users")]
    public class UserController : Controller
    {
        private UserRepository userRepository;
        private ITypeHelperService typeHelperService;
        private IPropertyMappingService propertyMappingService;
        private IUrlHelper urlHelper;

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

        /// <summary>
        /// Consructor of User Conroller
        /// </summary>
        /// <param name="userRepository">UserRepository</param>
        /// <param name="typeHelperService">typeHleper service to determine of a property exists on a type</param>
        /// <param name="urlHelper">urlhelper to help create links</param>
        /// <param name="propertyMappingService">propertymapping service to map the sort property to database property of the corresponding object</param>
        public UserController(UserRepository userRepository, ITypeHelperService typeHelperService, IUrlHelper urlHelper, IPropertyMappingService propertyMappingService)//, UserManager<IdentityUser> userManager)
        {
            this.userRepository = userRepository;
            this.typeHelperService = typeHelperService;
            this.propertyMappingService = propertyMappingService;
            this.urlHelper = urlHelper;
          //  this.userManager = userManager;
        }

        /// <summary>
        /// Gets a User using username
        /// </summary>
        /// <param name="username">username</param>
        /// <response code="200">User entry retrieved</response>
        /// <response code="400">user retrieval parameter has missing/invalid values</response>
        /// <response code="500">Oops! Can't retrieve the user entry right now</response>
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        [EnableCors("AnyGET")]
        [Authorize]
        [Authorize(Policy = "Reader")]
        [HttpGet("{username}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest();
            }
            var result = await userRepository.GetEntity(username);
            if (result == null)
            {
                return NotFound();
            }
            return Ok(result);

        }

        /// <summary>
        /// Gets Users and applies the search, sort, filter and pagedResult parameters.
        /// </summary>
        /// <param name="parameter">Parameter containing the details of the search, sort , filter and pagedResult</param>
        /// <response code="200">User entries retrieved</response>
        /// <response code="400">users retrieval parameter has missing/invalid values</response>
        /// <response code="500">Oops! Can't retrieve users entry right now</response>
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
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
            if (!parameter.Fields.Contains("username"))
            {
                parameter.Fields = parameter.Fields+", username";
            }
            var usersForDisplay = mappedUserList.ShapeData<UserForDisplayDTO>(parameter.Fields);


            var result = usersForDisplay.Select(user =>
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

        /// <summary>
        /// Deletes a user using username
        /// </summary>
        /// <param name="username">username to be deleted</param>
        /// <response code="204">User entry deleted</response>
        /// <response code="400">user retrieval parameter has missing/invalid values</response>
        /// <response code="500">Oops! Can't delet user entry right now</response>
        [ProducesResponseType(204)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        [Authorize(Policy = "SuperUser")]
        [EnableCors("default")]
        [HttpDelete("{username}", Name = "DeleteUser")]
        public IActionResult DeleteUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest();
            }
            if (!userRepository.DeleteEntity(username))
            {
                return new StatusCodeResult(500);
            }
            return NoContent();

        }

        /// <summary>
        ///  Creates a User
        /// </summary>
        /// <param name="userForCreationDto">user details to be created</param>
        /// <response code="204">User entry created</response>
        /// <response code="400">user creation parameter has missing/invalid values</response>
        /// <response code="500">Oops! Can't create user entry right now</response>
        [ProducesResponseType(typeof(User),201)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        [Authorize(Policy = "Writer")]
        [EnableCors("default")]
        [HttpPost( Name = "CreateUser")]
        public async Task<IActionResult> PostUser( [FromBody] UserForCreationDTO userForCreationDto)
        {
            if (userForCreationDto == null)
            {
                return BadRequest();
            }

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

        /// <summary>
        /// Updates a user
        /// </summary>
        /// <param name="username">username of the user to be updated</param>
        /// <param name="userForUpdateDto">the body of the user to be updated. This is a full update. So all the parameters to be updated shoudl be mentioned in addition to other parameters. If a parameter is null or missing, it will be updated to null</param>
        /// <response code="201">User entry updated</response>
        /// <response code="400">user creation parameter has missing/invalid values</response>
        /// <response code="500">Oops! Can't create user entry right now</response>
        [ProducesResponseType(typeof(User), 201)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        [Authorize(Policy = "SuperUser")]
        [EnableCors("default")]
        [HttpPut("{username}", Name = "UpdateUser")]
        public async Task<IActionResult> UpdateUser(string username, [FromBody] UserForUpdateDTO userForUpdateDto)
        {
            if (userForUpdateDto == null)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return new UnAcceptableEntity(ModelState);
            }
            var entity =await userRepository.GetEntity(username);
            if (entity == null)
            {
                return NotFound();
            }
            var result = await userRepository.UpdateEntity(entity);
            if (result == null)
            {
                return new StatusCodeResult(500);
            }
            var mappedResult = AutoMapper.Mapper.Map<UserForDisplayDTO>(result);
            return CreatedAtRoute("GetUser", new { username = entity.Username }, mappedResult);


        }

        /// <summary>
        /// Partially updates a user
        /// </summary>
        /// <param name="user">username of the user to be updated</param>
        /// <param name="patchDocument">the patch documents containing the list of updates to be applied to a user</param>
        /// <response code="201">User entry updated</response>
        /// <response code="400">user creation parameter has missing/invalid values</response>
        /// <response code="500">Oops! Can't create user entry right now</response>
        [ProducesResponseType(typeof(User), 201)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        [Authorize(Policy = "SuperUser")]
        [EnableCors("default")]
        [HttpPatch("{user}", Name = "PartiallyUpdateUser")]
        public async Task<IActionResult> PatchUser(string user, [FromBody] JsonPatchDocument<UserForUpdateDTO> patchDocument)
        {
            if (patchDocument == null)
                return BadRequest();
            var entity = await userRepository.GetEntity(user);
            if (entity == null)
                return NotFound();
            var userToPatch = AutoMapper.Mapper.Map<UserForUpdateDTO>(entity);
            patchDocument.ApplyTo(userToPatch);

            // add validation

            AutoMapper.Mapper.Map(userToPatch, entity);
            entity.Username = user;
           var updatedUser =  userRepository.UpdateEntity(entity);
            if (updatedUser == null)
                throw new Exception($"patching user with username = {user} failed");
            return CreatedAtRoute("GetUser", new { username = user }, updatedUser);
        } 
    }
}
