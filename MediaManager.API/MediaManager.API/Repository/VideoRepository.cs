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


       
        public VideoRepository(DatabaseContext context, IPropertyMappingService propertyMappingService)
        {
            this.propertyMappingService = propertyMappingService;
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
            if(string.IsNullOrEmpty(parameters.Title))
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

        public Video GetEntity(string user, int id)
        {
            
            return context.Videos.Where(x => x.VideoId == id && x.Username == user).Include(x => x.VideoParticipants).ThenInclude(y => y.Participant).FirstOrDefault();
        }

        public Video PostEntity(Video entity, List<string> participants)
        {
            var post = context.Videos.Add(entity);
            if (post.State != Microsoft.EntityFrameworkCore.EntityState.Added)
                return null;

            if (participants != null)
            {
                foreach (var participant in participants)
                {
                    var entry = context.Participants.Where(x => x.ParticipantName == participant).FirstOrDefault();
                    if (entry == null)
                    {
                        var result = context.Participants.Add(new Participant { ParticipantName = participant });
                        if (result.State != EntityState.Added)
                        {
                            return null;
                        }
                        entry = result.Entity;
                    }
                    context.VideoParticipants.Add(new VideoParticipant { VideoId = post.Entity.VideoId, ParticipantId = entry.ParticipantId });
                }
            }

            return post.Entity;
        }

        public Video PutEntity(Video entity)
        {
            try
            {
                var entry = context.Entry(entity);
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                return entry.Entity;
            }
            catch (Exception)
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
