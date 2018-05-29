using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.DAL.DBEntities;

namespace MediaManager.API.Repository
{
    public class ParticipantRepository
    {
        private DatabaseContext context;

        public ParticipantRepository(DatabaseContext ctx)
        {
            context = ctx;
        }
        public bool DeleteEntity(Participant entity)
        {
            try
            {
                var video = context.Participants.Where(x => x.ParticipantId == entity.ParticipantId);
                context.Entry(video).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                context.SaveChanges();
                return true;
            }
            catch(Exception)
            {
                return false;
            }
            

        }

        public bool EntityExists(int id)
        {
            return context.Videos.Any(x => x.VideoId == id);
        }

        public List<Participant> GetEntities(string user)
        {
            throw new NotImplementedException();
        }

        public Participant GetEntity(string participantName, int id )
        {
            return context.Participants.Where(x => x.ParticipantName == participantName).FirstOrDefault();
        }

        public async Task<Participant> PostEntity(Participant entity)
        {
            var entry = context.Participants.Add(entity);
            await context.SaveChangesAsync();
           
                return entry.Entity;
            return null;
        }

        public Participant PutEntity(Participant entity)
        {
            try
            {
                var entry = context.Entry(entity);
                entry.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                context.SaveChanges();
                return entry.Entity;
            }
            catch(Exception)
            {
                return null;
            }
        }

        public Participant UpdateEntity(Participant entity)
        {
            throw new NotImplementedException();
        }
    }
}
