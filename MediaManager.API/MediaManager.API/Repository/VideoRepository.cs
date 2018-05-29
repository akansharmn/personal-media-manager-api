using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.API.Helpers;
using MediaManager.API.Models;
using MediaManager.API.Services;
using MediaManager.DAL.DBEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MediaManager.API.Repository
{
    public class VideoRepository /*: IRepository<Video>*/
    {
        private IPropertyMappingService propertyMappingService;
        private DatabaseContext context;
        private ParticipantRepository participantRepository;

       
        public VideoRepository(DatabaseContext context, IPropertyMappingService propertyMappingService, ParticipantRepository participantRepository)
        {
            this.propertyMappingService = propertyMappingService;
            this.participantRepository = participantRepository;
            this.context = context;
        }
        public bool DeleteEntity(Video entity)
        {
            var video = context.Participants.Where(x => x.ParticipantId == entity.VideoId);
            context.Entry(video).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            context.SaveChanges();
            return true;
        }

        public bool EntityExists(int id)
        {
            return context.Videos.Any(x => x.VideoId == id);
        }

        public PageList<Video> GetEntities(string user, VideoResourceParameters parameters)
        {
            
            var collectionBeforePaging = context.Videos.OrderBy(x => x.UploadedDate).AsQueryable();

            collectionBeforePaging = context.Videos.ApplySort(parameters.OrderBy, propertyMappingService.GetPropertyMapping<VideoForDisplayDTO, Video>());
            if(!string.IsNullOrEmpty(parameters.Title))
            {
                var filterClause = parameters.Title.Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging.Where(x => x.Title.ToLowerInvariant() == filterClause);
            }

            if(!string.IsNullOrEmpty(parameters.searchQuery))
            {
                var searchQuery = parameters.searchQuery.Trim().ToLowerInvariant();
                collectionBeforePaging = collectionBeforePaging.Where(a => a.Properties.ToLowerInvariant().Contains(searchQuery) || a.Title.ToLowerInvariant().Contains(searchQuery) || a.Username.Contains(searchQuery));
            }
            return PageList<Video>.Create(collectionBeforePaging, parameters.pageNumber, parameters.PageSize);
           // return context.Videos.Where(x => x.Username == user).OrderBy(x => x.UploadedDate).Skip(parameters.PageSize * (parameters.pageNumber - 1)).Take(parameters.PageSize).ToList();
        }

        public Video GetEntity(string user)
        {
            
            return context.Videos.Where(x =>  x.Username == user).Include(x => x.VideoParticipants).ThenInclude(y => y.Participant).FirstOrDefault();
        }

        public Video GetEntity(int id)
        {

            return context.Videos.Where(x => x.VideoId == id).Include(x => x.VideoParticipants).ThenInclude(y => y.Participant).FirstOrDefault();
        }

        public async Task<Video> PostEntity(Video video, List<string> participants, string domain, string author)
        {
            if (!string.IsNullOrEmpty(domain))
            {
                video.Domain = MetadataStore.Metadata.Domains.Where(x => x.DomainName == domain).FirstOrDefault();

                if (video.Domain == null)
                    return null;
                video.DomainId = video.Domain.DomainId;
                video.Domain = null;
            }
            video.Author = participantRepository.GetEntity(author, 0);
            if (video.Author == null)
            {
                video.Author = await participantRepository.PostEntity(new Participant { ParticipantName = author });
                if (video.Author == null)
                    return null;
            }
            video.AuthorId = video.Author.ParticipantId;
            var post = context.Videos.Add(video);
            await context.SaveChangesAsync();
            //if (post.State != Microsoft.EntityFrameworkCore.EntityState.Added)
            //    return null;

            if (participants != null)
            {
                foreach (var participant in participants)
                {
                    var entry = context.Participants.Where(x => x.ParticipantName == participant).FirstOrDefault();
                    if (entry == null)
                    {
                        var result = await participantRepository.PostEntity(new Participant { ParticipantName = participant });
                        if (result == null)
                            return null;
                    }
                    context.VideoParticipants.Add(new VideoParticipant { VideoId = post.Entity.VideoId, ParticipantId = entry.ParticipantId });
                    await context.SaveChangesAsync();
                }
            }

            return post.Entity;
        }

        public async Task<Video> PutEntity(Video entity, string domain, string author)
        {
            try
            {
                if (!string.IsNullOrEmpty(domain))
                {
                    entity.DomainId = MetadataStore.Metadata.Domains.Where(x => x.DomainName == domain).Select(x => x.DomainId).FirstOrDefault();
                    if (entity.DomainId == 0)
                        return null;
                }

                entity.Author = participantRepository.GetEntity(author, 0);
                if (entity.Author == null)
                {
                    entity.Author = await participantRepository.PostEntity(new Participant { ParticipantName = author });
                    if (entity.Author == null)
                        return null;
                }

                entity.AuthorId = entity.Author.ParticipantId;
                //await context.SaveChangesAsync();
                context.Videos.Update(entity);
               
                await context.SaveChangesAsync();
               
                return entity;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

       public Video UpdateEntity(Video entity)
        {
            throw new NotImplementedException();
        }
    }
}
