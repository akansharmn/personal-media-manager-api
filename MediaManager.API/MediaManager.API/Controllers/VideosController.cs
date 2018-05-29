using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediaManager.API.Helpers;
using MediaManager.API.Models;
using MediaManager.API.Repository;
using MediaManager.API.Services;
using MediaManager.DAL.DBEntities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace MediaManager.API.Controllers
{

    /// <summary>
    /// Controller class to manage API call related to videos
    /// </summary>
    public class VideosController : Controller
    {
        private VideoRepository videoRepository;

        private ParticipantRepository participantRepository;
        private IPropertyMappingService propertyMappingService;
        private IUrlHelper urlHelper;
        private ITypeHelperService typeHelperService;

        /// <summary>
        /// Constructor of VideosConroller
        /// </summary>
        /// <param name="videoRepository">videoRepository</param>
        /// <param name="participantRepository">participantRepository</param>
        /// <param name="urlHelper">UrlHelper</param>
        /// <param name="propertyMappingService">propertyMappingService which helps to map the sort order of fields of model to the database field</param>
        /// <param name="typeHelperService">typehelper service which helps to know whether a property exists ina type</param>
        public VideosController(VideoRepository videoRepository, ParticipantRepository participantRepository, IUrlHelper urlHelper, IPropertyMappingService propertyMappingService, ITypeHelperService typeHelperService)
        {
            this.videoRepository = videoRepository;
            this.participantRepository = participantRepository;
            this.propertyMappingService = propertyMappingService;
            this.typeHelperService = typeHelperService;
            this.urlHelper = urlHelper;
        }

        private IEnumerable<LinkDTO> CreateLinksForVideos(string user, VideoResourceParameters parameters, bool hasNext, bool hasPrevious)
        {
            var links = new List<LinkDTO>();
            links.Add(new LinkDTO(CreateVideosResourceUri(parameters, ResourceUriType.Current, user), "self", "GET"));
            if (hasNext)
            links.Add(new LinkDTO(CreateVideosResourceUri( parameters, ResourceUriType.nextPage, user), "nextPage", "GET"));
            if(hasPrevious)
                links.Add(new LinkDTO(CreateVideosResourceUri(parameters, ResourceUriType.previousPage, user), "nextPage", "GET"));
            return links;
        }


        private VideoForDisplayDTO CreateLinksForVideo(VideoForDisplayDTO video)
        {
            video.Links.Add(new LinkDTO(urlHelper.Link("GetVideo", new { id = video.VideoId }), "self", "GET"));
            return video;
        }
      
        private IEnumerable<LinkDTO> CreateLinksForVideoByUser(string user, string field)
        {
            var links = new List<LinkDTO>();

            if(string.IsNullOrWhiteSpace(field))
            {
                links.Add(new LinkDTO(urlHelper.Link("GetVideoByUser", new { username = user}), "self", "GET"));

            }
            else
            {
                links.Add(new LinkDTO(urlHelper.Link("GetVideoByUser", new { username = user,  fields = field }), "self", "GET"));
            }

            return links;
        }

        private IEnumerable<LinkDTO> CreateLinksForVideoById( int id, string field)
        {
            var links = new List<LinkDTO>();

            if (string.IsNullOrWhiteSpace(field))
            {
                links.Add(new LinkDTO(urlHelper.Link("GetVideoById", new {  videoId = id }), "self", "GET"));

            }
            else
            {
                links.Add(new LinkDTO(urlHelper.Link("GetVideoById", new {  videoId = id, fields = field }), "self", "GET"));
            }

            return links;
        }

        private string CreateVideosResourceUri(VideoResourceParameters parameters, ResourceUriType type, string user)
        {
            switch (type)
            {
                case ResourceUriType.previousPage:
                    return urlHelper.Link("GetVideos", new
                    {
                        username = user,
                        parameters = new VideoResourceParameters
                        {
                            Fields = parameters.Fields,
                            OrderBy = parameters.OrderBy,
                            searchQuery = parameters.searchQuery,
                            Title = parameters.Title,
                            PageNumber = parameters.pageNumber - 1,
                            PageSize = parameters.PageSize
                        }
                    });

                case ResourceUriType.nextPage:
                    return urlHelper.Link("GetVideos", new
                    {

                        username = user,
                        parameters = new VideoResourceParameters
                        {
                            Fields = parameters.Fields,
                            OrderBy = parameters.OrderBy,
                            searchQuery = parameters.searchQuery,
                            Title = parameters.Title,
                            PageNumber = parameters.pageNumber + 1,
                            PageSize = parameters.PageSize
                        }
                    });

                default:

                    return urlHelper.Link("GetVideos", new
                    {
                        searchQuery = parameters.searchQuery,
                        Title = parameters.Title,
                        username = user,
                        parameters = new VideoResourceParameters
                        {
                            Fields = parameters.Fields,
                            OrderBy = parameters.OrderBy,
                            searchQuery = parameters.searchQuery,
                            Title = parameters.Title,
                            PageNumber = parameters.pageNumber,
                            PageSize = parameters.PageSize
                        }
                    });


            }
        }


        /// <summary>
        /// Retrieves a video based on username and videoId and exposes the desired fields.
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="fields">fields of video to be exposed</param>
        /// <returns>Video</returns>
        /// <response code="200">Video entry retrieved</response>
        /// <response code="400">Video search parameter has missing/invalid values</response>
        /// <response code="500">Oops! Can't retrieve your video entry right now</response>
        [ProducesResponseType(typeof(Video), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        [EnableCors("AnyGET")]
        [Authorize(Policy = "Reader")]
        [HttpGet("api/user/{username}/videos", Name = "GetVideoByUser")]
        public IActionResult GetVideo(string username, [FromQuery] string fields)
        {
            if (!typeHelperService.TypeHasProperties<VideoForDisplayDTO>(fields))
                return BadRequest();
            if ( string.IsNullOrEmpty(username))
                return BadRequest();
            var video = videoRepository.GetEntity(username);
            if (video == null)
                return NotFound();
            var displayVideo = Mapper.Map<VideoForDisplayDTO>(video);
            var links = CreateLinksForVideoByUser(username, fields);
            var linkedResourceToReturn = displayVideo.ShapeData(fields) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);
            return Ok(linkedResourceToReturn);
        }


        /// <summary>
        /// Retrieves a video based on videoId and exposes the desired fields.
        /// </summary>
        /// <param name="videoId">Id of video</param>
        /// <param name="fields">fields of video to be exposed</param>
        /// <returns>Video</returns>
        /// <response code="200">Video entry retrieved</response>
        /// <response code="400">Video search parameter has missing/invalid values</response>
        /// <response code="500">Oops! Can't retrieve your video entry right now</response>
        [ProducesResponseType(typeof(Video), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        [EnableCors("AnyGET")]
        [Authorize(Policy = "Reader")]
        [HttpGet("api/videos/{videoId}", Name = "GetVideoById")]
        public IActionResult GetVideoById( int videoId, [FromQuery] string fields)
        {
            if (!typeHelperService.TypeHasProperties<VideoForDisplayDTO>(fields))
                return BadRequest();
            if (videoId == 0 )
                return BadRequest();
            var video = videoRepository.GetEntity(videoId);
            if (video == null)
                return NotFound();
            var displayVideo = Mapper.Map<VideoForDisplayDTO>(video);
            var links = CreateLinksForVideoById(videoId, fields);
            var linkedResourceToReturn = displayVideo.ShapeData(fields) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);
            return Ok(linkedResourceToReturn);
        }

        /// <summary>
        ///  Retrieves a video based on username and exposes the desired fields.
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="parameters">parameters containing search, sort, pagedResult parameters</param>
        /// <param name="mediaType">the media type value derived from the header value of Accept</param>
        /// <response code="200">Video entry retrieved</response>
        /// <response code="400">Video search parameter has missing/invalid values</response>
        /// <response code="500">Oops! Can't retrieve your video entry right now</response>
        [ProducesResponseType(typeof(Video), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        [EnableCors("AnyGET")]
        [Authorize(Policy = "Reader")]
        [HttpGet("api/user/{username}/videosv2", Name = "GetVideosWithCustomeMedia")]
        public IActionResult GetVideosWithCustomeMedia( string username, VideoResourceParameters parameters, [FromHeader(Name = "Accept")]string mediaType)
        {

            if (!typeHelperService.TypeHasProperties<VideoForDisplayDTO>(parameters.Fields))
            {
                return BadRequest();
            }
            if (!propertyMappingService.ValidMappingExistsFor<VideoForDisplayDTO, Video>(parameters.OrderBy))
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(username))
                return BadRequest();


            var videos = videoRepository.GetEntities(username, parameters);
            if (videos == null)
                return NotFound();

           

            var paginatioMetadata = new
            {
                totalCount = videos.TotalCount,
                pageSize = videos.PageSize,
                currentPage = videos.CurrentPage,
                totalPages = videos.TotalPages
               
            };

            Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginatioMetadata));
            var videosToReturn = Mapper.Map<IEnumerable<VideoForDisplayDTO>>(videos);
            if (mediaType == "application/vnd.ak.hateoas+json")
            {
                var linksForVideos = CreateLinksForVideos(username, parameters, videos.HasNext, videos.HasPrevious);


                var result = videosToReturn.ShapeData(parameters.Fields);
                result.Select(video =>
                {
                    var videoAsDictionary = video as IDictionary<string, object>;
                    var videoLinks = CreateLinksForVideoById( (int)videoAsDictionary["VideoId"], parameters.Fields);
                    videoAsDictionary.Add("links", videoLinks);
                    return videoAsDictionary;
                });
                var linkedCollectionResource = new
                {
                    value = result,
                    links = linksForVideos
                };
                return Ok(linkedCollectionResource);
            }
            else
            {
                var previousLink = videos.HasPrevious ? CreateVideosResourceUri(parameters, ResourceUriType.previousPage, username) : null;
                var nextLink = videos.HasNext ? CreateVideosResourceUri(parameters, ResourceUriType.nextPage, username) : null;

                 var newpaginatioMetadata = new
                {
                    totalCount = videos.TotalCount,
                    pageSize = videos.PageSize,
                    currentPage = videos.CurrentPage,
                    totalPages = videos.TotalPages,
                    previousPageLink = previousLink,
                    nextPageLink = nextLink
                };

                Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(newpaginatioMetadata));
                 videosToReturn = Mapper.Map<IEnumerable<VideoForDisplayDTO>>(videos);
                var links = CreateLinksForVideos(username, parameters, videos.HasNext, videos.HasPrevious);

                videosToReturn = videosToReturn.Select(v =>
                {
                    var videoWithLinks = CreateLinksForVideo(v);
                    return videoWithLinks;
                });
                var result = videosToReturn.ShapeData(parameters.Fields);

                return Ok(result);
            }
        }

        [EnableCors("AnyGET")]
        [Authorize(Policy = "Reader")]
        [HttpGet("api/user/{username}/videosObs", Name = "GetVideos")]
        private IActionResult GetVideos(string username,[FromQuery] VideoResourceParameters parameters)
        {
            if(!typeHelperService.TypeHasProperties<VideoForDisplayDTO>(parameters.Fields))
            {
                return BadRequest();
            }
            if (!propertyMappingService.ValidMappingExistsFor<VideoForDisplayDTO, Video>(parameters.OrderBy))
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(username))
                return BadRequest();


            var videos = videoRepository.GetEntities(username, parameters);
            if (videos == null)
                return NotFound();

            var previousLink = videos.HasPrevious ? CreateVideosResourceUri(parameters, ResourceUriType.previousPage, username) : null;
            var nextLink = videos.HasNext ? CreateVideosResourceUri(parameters, ResourceUriType.nextPage, username) : null;

            var paginatioMetadata = new
            {
                totalCount = videos.TotalCount,
                pageSize = videos.PageSize,
                currentPage = videos.CurrentPage,
                totalPages = videos.TotalPages,
                previousPageLink = previousLink,
                nextPageLink = nextLink
            };
             
            Response.Headers.Add("X-Pagination", Newtonsoft.Json.JsonConvert.SerializeObject(paginatioMetadata));
            var videosToReturn = Mapper.Map<IEnumerable<VideoForDisplayDTO>>(videos);
            var links = CreateLinksForVideos(username, parameters, videos.HasNext, videos.HasPrevious);

            videosToReturn = videosToReturn.Select(v =>
            {
                var videoWithLinks = CreateLinksForVideo(v);
                return videoWithLinks;
            });
            var result = videosToReturn.ShapeData(parameters.Fields);
            
            return Ok(result);
        }

        /// <summary>
        /// Creates a video entry
        /// </summary>
        /// <param name="videoForCreationDTO"></param>
        /// <response code="200">Video entry created</response>
        /// <response code="400">Video entry parameter has missing/invalid values</response>
        /// <response code="500">Oops! Can't create your video entry right now</response>
        [ProducesResponseType(typeof(Video), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        [EnableCors("default")]
        [Authorize(Policy = "Writer")]
        [HttpPost("api/user/videos", Name = "CreateVideo")]
        public async Task<IActionResult> PostVideo([FromBody] VideoForCreationDTO videoForCreationDTO)
        {
            if (videoForCreationDTO == null)
                return BadRequest();

            if(!ModelState.IsValid)
            {
                return new UnAcceptableEntity(ModelState);
            }
            var video = Mapper.Map<Video>(videoForCreationDTO);          
            var videoCreated =await videoRepository.PostEntity(video, videoForCreationDTO.Participants, videoForCreationDTO.Domain, videoForCreationDTO.AuthorName);
            if (videoCreated == null)
                return new StatusCodeResult(500);
            var videoForDisplay = Mapper.Map<VideoForDisplayDTO>(videoCreated);
            //  videoForDisplay.AuthorName = participantRepository.GetEntity(videoCreated.Author.ParticipantName, videoCreated.AuthorId)?.ParticipantName;
           // videoForDisplay.VideoParticipants = videoForCreationDTO.Participants;
            var route = new { username = videoForDisplay.Username, videoId = videoCreated.VideoId };
            videoForDisplay.Links.Add(new LinkDTO(urlHelper.Link("GetVideo", route), "self", "GET"));
            return CreatedAtRoute("GetVideo",route, videoForDisplay);
        }

        /// <summary>
        /// Updates a video. 
        /// </summary>
        /// <param name="videoForUpdate">Full body of the video to be updated. If any parameter is null or missing , it will be set to null</param>
        /// <response code="200">Video entry updated</response>
        /// <response code="400">Video update parameter has missing/invalid values</response>
        /// <response code="500">Oops! Can't update your video entry right now</response>
        [ProducesResponseType(typeof(Video), 200)]
        [ProducesResponseType(typeof(IDictionary<string, string>), 400)]
        [ProducesResponseType(500)]
        [EnableCors("AnyGET")]
        [Authorize(Policy = "Reader")]
        [HttpPut("api/user/videos", Name = "UpdateVideo")]
        public async Task<IActionResult> PutVideo( [FromBody] VideoForUpdateDTO videoForUpdate)
        {
            if ( videoForUpdate == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return new UnAcceptableEntity(ModelState);
            }
            var video = Mapper.Map<Video>(videoForUpdate);
            var videoCreated = await videoRepository.PutEntity(video, videoForUpdate.Domain, videoForUpdate.AuthorName );
            if (videoCreated == null)
            {
                return new StatusCodeResult(500);
            }
            var videoForDisplay = Mapper.Map<VideoForDisplayDTO>(videoCreated);
            return CreatedAtRoute("GetVideo", new { username = videoForUpdate.Username, videoId = videoCreated.VideoId }, videoForDisplay);
        }
    }
}
