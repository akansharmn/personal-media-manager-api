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
    [Route("api/user/{username}/videos")]
    public class VideosController : Controller
    {
        private VideoRepository videoRepository;

        private IRepository<Participant> participantRepository;
        private IPropertyMappingService propertyMappingService;
        private IUrlHelper urlHelper;
        private ITypeHelperService typeHelperService;

        public VideosController(VideoRepository videoRepository, IRepository<Participant> participantRepository, IUrlHelper urlHelper, IPropertyMappingService propertyMappingService, ITypeHelperService typeHelperService)
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
      
        private IEnumerable<LinkDTO> CreateLinksForVideo(string user, int id, string fields)
        {
            var links = new List<LinkDTO>();

            if(string.IsNullOrWhiteSpace(fields))
            {
                links.Add(new LinkDTO(urlHelper.Link("GetVideo", new { username = user, videoId = id }), "self", "GET"));

            }
            else
            {
                links.Add(new LinkDTO(urlHelper.Link("GetAuthor", new { username = user, videoId = id, fields = fields }), "self", "GET"));
            }

            return links;
        }

        [EnableCors("AnyGET")]
        [Authorize(Policy = "Reader")]
        [HttpGet("{videoId}", Name = "GetVideo")]
        public IActionResult GetVideo(string username, int videoId,[FromQuery] string fields)
        {
            if (!typeHelperService.TypeHasProperties<VideoForDisplayDTO>(fields))
                return BadRequest();
            if (videoId == 0 || string.IsNullOrEmpty(username))
                return BadRequest();
            var video = videoRepository.GetEntity(username, videoId);
            if (video == null)
                return NotFound();
            var displayVideo = Mapper.Map<VideoForDisplayDTO>(video);
            //if (video.VideoParticipants != null)
            //{
            //    displayVideo.Participant = new List<string>();
            //    foreach(var videoParticipant in video.VideoParticipants)
            //    {
            //        displayVideo.Participant.Add(videoParticipant.Participant.ParticipantName);
            //    }
            //}
            // return Ok(CreateLinksForVideo(displayVideo));
            var links = CreateLinksForVideo(username, videoId, fields);
            var linkedResourceToReturn = video.ShapeData(fields) as IDictionary<string, object>;
            linkedResourceToReturn.Add("links", links);
            return Ok(linkedResourceToReturn);
        }

        private string CreateVideosResourceUri(VideoResourceParameters parameters, ResourceUriType type, string user)
        {
            switch(type)
            {
                case ResourceUriType.previousPage: return urlHelper.Link("GetVideos", new
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
                            PageNumber = parameters.pageNumber ,
                            PageSize = parameters.PageSize
                        }
                    });


            }
        }

        [EnableCors("AnyGET")]
        [Authorize(Policy = "Reader")]
        [HttpGet(Name = "GetVideos")]
        public IActionResult GetVideos2(string username, VideoResourceParameters parameters, [FromHeader(Name = "Accept")]string mediaType)
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
                    var videoLinks = CreateLinksForVideo(username, (int)videoAsDictionary["VideoId"], parameters.Fields);
                    videoAsDictionary.Add("links", videoLinks);
                    return videoAsDictionary;
                });
                var linkedCollectionResource = new
                {
                    value = result,
                    links = linksForVideos
                };
                return Ok(result);
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
        [HttpGet(Name = "GetVideos")]
        public IActionResult GetVideos(string username, VideoResourceParameters parameters)
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

        [EnableCors("default")]
        [Authorize(Policy = "Writer")]
        [HttpPost]
        public IActionResult PostVideo(string username, [FromBody] VideoForCreationDTO videoForCreationDTO)
        {
            if (videoForCreationDTO == null)
                return BadRequest();

            if(!ModelState.IsValid)
            {
                return new UnAcceptableEntity(ModelState);
            }

            var video = Mapper.Map<Video>(videoForCreationDTO);
            // video.Domain = MetadataStore.Metadata.Domains.Where(x => x.DomainName == videoForCreationDTO.Domain).FirstOrDefault();
            if (!string.IsNullOrEmpty(videoForCreationDTO.Domain))
            {
                video.DomainId = MetadataStore.Metadata.Domains.Where(x => x.DomainName == videoForCreationDTO.Domain).Select(x => x.DomainId).FirstOrDefault();
            }
            video.Username = username;

            video.Author = participantRepository.GetEntity(videoForCreationDTO.AuthorName, 0);
            if(video.Author == null)
            {
               video.Author = participantRepository.PostEntity(new Participant { ParticipantName = videoForCreationDTO.AuthorName });
                if (video.Author == null)
                    return new StatusCodeResult(500);
            }
           
            video.AuthorId = video.Author.ParticipantId;

            var videoCreated = videoRepository.PostEntity(video, videoForCreationDTO.Participants);
            if (videoCreated == null)
                return new StatusCodeResult(500);
            var videoForDisplay = Mapper.Map<VideoForDisplayDTO>(videoCreated);
            videoForDisplay.AuthorName = participantRepository.GetEntity(videoCreated.Author.ParticipantName, videoCreated.AuthorId)?.ParticipantName;
            videoForDisplay.Participant = videoForCreationDTO.Participants;
            return CreatedAtRoute("GetVideo", new { username = username, videoId = videoCreated.VideoId }, videoForDisplay);


        }

        [EnableCors("AnyGET")]
        [Authorize(Policy = "Reader")]
        [HttpPut("{id}", Name = "ModifyVideo")]
        public IActionResult PutVideo(string username, int id, [FromBody] VideoForUpdateDTO videoForUpdate)
        {
            if (string.IsNullOrEmpty(username) || videoForUpdate == null)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                return new UnAcceptableEntity(ModelState);
            }
            if (!videoRepository.EntityExists(id))
                return new StatusCodeResult(409);

            var video = Mapper.Map<Video>(videoForUpdate);
            if (!string.IsNullOrEmpty(videoForUpdate.Domain))
            {
                video.DomainId = MetadataStore.Metadata.Domains.Where(x => x.DomainName == videoForUpdate.Domain).Select(x => x.DomainId).FirstOrDefault();
            }
            video.Username = username;
            video.Author = participantRepository.GetEntity(videoForUpdate.AuthorName, 0);
            if (video.Author == null)
            {
                video.Author = participantRepository.PostEntity(new Participant { ParticipantName = videoForUpdate.AuthorName });
                if (video.Author == null)
                    return new StatusCodeResult(500);
            }
            
                video.AuthorId = video.Author.ParticipantId;

            var videoCreated = videoRepository.PostEntity(video, videoForUpdate.Participants);
            if (videoCreated == null)
                return new StatusCodeResult(500);
            var videoForDisplay = Mapper.Map<VideoForDisplayDTO>(videoCreated);
            return CreatedAtRoute("GetVideo", new { username = username, videoId = videoCreated.VideoId }, videoForDisplay);
        }
    }
}
