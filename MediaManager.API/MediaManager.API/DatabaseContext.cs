using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.DAL.DBEntities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MediaManager.API
{
    /// <summary>
    /// DbContext class containing all the DbSet
    /// </summary>
    public class DatabaseContext : IdentityDbContext
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options">options</param>
        public DatabaseContext(DbContextOptions options) : base( options)
        {
            
        }
        /// <summary>
        /// Videos
        /// </summary>
        public DbSet<Video> Videos { get; set; }

        /// <summary>
        /// Audios
        /// </summary>
        public DbSet<Audio> Audios { get; set; }

        /// <summary>
        /// Tags
        /// </summary>
        public DbSet<Tag> Tags { get; set; }

        /// <summary>
        /// Users
        /// </summary>
        public new DbSet<User>  Users { get; set; }

        /// <summary>
        /// Domains
        /// </summary>
        public DbSet<Domain> Domains { get; set; }

        /// <summary>
        /// DomainContents
        /// </summary>
        public DbSet<DomainContent> DomainContents { get; set; }

        /// <summary>
        /// Playlists
        /// </summary>
        public DbSet<Playlist> Playlists { get; set; }

        /// <summary>
        /// Contents
        /// </summary>
        public DbSet<Content> Contents { get; set; }

        /// <summary>
        /// VideoParticipants
        /// </summary>
        public DbSet<VideoParticipant> VideoParticipants { get; set; }

        /// <summary>
        /// Prticipants
        /// </summary>
        public DbSet<Participant> Participants { get; set; }

        /// <summary>
        /// operations to be performed on creation of model
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tag>().HasKey(c => new { c.TagName, c.ContentId, c.Id });
            modelBuilder.Entity<User>().ToTable("User");
            modelBuilder.Entity<User>().HasKey(c => c.Username);
            modelBuilder.Entity<VideoParticipant>().HasKey(c => new { c.VideoId, c.ParticipantId});
            modelBuilder.Entity<DomainContent>().HasKey(c => new { c.ContentId, c.DomainId });
            modelBuilder.Entity<VideoParticipant>().HasOne(c => c.Participant).WithMany(x => x.VideoParticipants).HasForeignKey(x => x.ParticipantId);
            modelBuilder.Entity<VideoParticipant>().HasOne(c => c.Video).WithMany(x => x.VideoParticipants).HasForeignKey(x => x.VideoId);
            base.OnModelCreating(modelBuilder);
        }
    }
}
