using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.API.Helpers;
using MediaManager.API.Models;
using MediaManager.API.Services;
using MediaManager.DAL.DBEntities;

namespace MediaManager.API.Repository
{
    public class UserRepository 
    {
        private DatabaseContext context;
        private IPropertyMappingService propertyMappingService;

        public UserRepository(DatabaseContext ctx, IPropertyMappingService propertyMappingService)
        {
            context = ctx;
            this.propertyMappingService = propertyMappingService;
        }

        public bool DeleteEntity(string user)
        {
            try
            {
                var video = context.Users.Where(x => x.Username == user);
                context.Entry(video).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
                context.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool EntityExists(string user)
        {
            return context.Users.Any(x => x.Username == user);
        }

        public PageList<User> GetEntities(UserResourceParameter parameter)
        {
            var users = context.Users.AsQueryable<User>();
            if( !string.IsNullOrWhiteSpace(parameter.Username) )
            {
                users = users.Where(user => user.Username == parameter.Username.Trim());
            }

            if(!string.IsNullOrWhiteSpace(parameter.searchQuery))
            {
                var searchQuery = parameter.searchQuery.ToLowerInvariant().Trim();
                users = users.Where(user => user.Username.ToLowerInvariant().Contains(searchQuery) || user.Email.ToLowerInvariant().Contains(searchQuery) || user.Name.ToLowerInvariant().Contains(searchQuery));
            }

            users = users.ApplySort<User>(parameter.OrderBy, propertyMappingService.GetPropertyMapping<UserForDisplayDTO, User>());
            return PageList<User>.Create(users, parameter.PageNumber, parameter.PageSize);
          

        }

        public User GetEntity(string user, int id = 0)
        {
            return context.Users.Where(x => x.Username == user).FirstOrDefault();
        }

        public async Task<User> PostEntity(User entity)
        {
            try
            {
                 context.Users.Add(entity);
                await context.SaveChangesAsync();

                return entity;
                
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public User PutEntity(User entity)
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

        public async Task<User> UpdateEntity(User entity)
        {
            try
            {
                context.Users.Update(entity);
                await context.SaveChangesAsync();
                return entity;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
