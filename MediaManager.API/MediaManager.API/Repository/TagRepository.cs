using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.DAL.DBEntities;
using Microsoft.EntityFrameworkCore;

namespace MediaManager.API.Repository
{
    /// <summary>
    /// The reository which supports CRUD operations on Tags
    /// </summary>
    public class TagRepository
    {
        private DatabaseContext context;

        /// <summary>
        /// Constructor of Tag Repository
        /// </summary>
        /// <param name="context">the database context</param>
        public TagRepository(DatabaseContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Gets tags for a contentid and id
        /// </summary>
        /// <param name="contentId">contentid</param>
        /// <param name="id">id</param>
        /// <returns></returns>
        public async Task<List<string>> GetTags(int contentId, int id)
        {
            if (contentId == 0 || id == 0)
                return null;
            try
            {

                var tags = await context.Tags.Where(x => x.ContentId == contentId && x.Id == id).Select(x => x.TagName).ToListAsync();
                return tags;
            }
            catch(Exception ex)
            {
                return null;
            }
            
        }

        /// <summary>
        /// Adds tags to an item
        /// </summary>
        /// <param name="contentId">ContentId</param>
        /// <param name="id">id</param>
        /// <param name="tags">list of latgs</param>
        /// <returns></returns>
        public async Task<bool> AddTags(int contentId, int id, IEnumerable<string> tags)
        {
            try
            {
                var table = MetadataStore.Metadata.Contents.Where(x => x.ContentId == contentId).Select(x => x.TableName).FirstOrDefault();
                if (string.IsNullOrEmpty(table))
                    return false;
                var type = MetadataStore.Metadata.TableDbSetMapping[table];
                var entity = context.Find(type, id);
                await context.SaveChangesAsync();
                if (entity == null)
                    return false;
                foreach (var tag in tags)
                {
                    await context.Tags.AddAsync(new Tag { TagName = tag, ContentId = contentId, Id = id });
                    await context.SaveChangesAsync();

                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes tags from an item
        /// </summary>
        /// <param name="contentId">content id</param>
        /// <param name="id">id of the item</param>
        /// <param name="tags">list of tags to be deleted</param>
        /// <returns></returns>
        public async Task<bool> DeleteTags(int contentId, int id, List<string> tags)
        {
            try
            {
                var table = MetadataStore.Metadata.Contents.Where(x => x.ContentId == contentId).Select(x => x.TableName).FirstOrDefault();
                if (string.IsNullOrEmpty(table))
                    return false;
                var type = MetadataStore.Metadata.TableDbSetMapping[table];
                var entity = context.Find(type, id);
                if (entity == null)
                    return false;
                foreach (var tag in tags)
                {
                    var tagEntity = await context.Tags.Where(x => x.ContentId == contentId && x.Id == id).FirstOrDefaultAsync();
                    if (tagEntity == null)
                        return false;
                    context.Tags.Remove(tagEntity);                    

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a Tag object 
        /// </summary>
        /// <param name="contentId">contentId</param>
        /// <param name="id">id</param>
        /// <returns>Tag object</returns>
        public async Task<Tag> GetEntity(int contentId, int id)
        {
            if (contentId == 0 || id == 0)
                return null;
            return await context.Tags.Where(x => x.ContentId == contentId && x.Id == id).FirstOrDefaultAsync();
        }
    }
}
