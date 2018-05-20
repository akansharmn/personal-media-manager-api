using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MediaManager.DAL.DBEntities
{
    [Table("Video")]
    public class Video
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VideoId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Url { get; set; }

        public string Username { get; set; }

        [ForeignKey("Author")]
        public int AuthorId { get; set; }

        public Participant Author { get; set; }

        public int Duration { get; set; }

        public int WatchOffset { get; set; }

        public DateTime UploadedDate { get; set; }

        [ForeignKey("DomainContent")]
        public int DomainId { get; set; }

        //[ForeignKey("DomainContent")]
        //public int ContentId { get; set; }

        public Domain Domain { get; set; }

        public string Properties { get; set; }

       public List<VideoParticipant> VideoParticipants { get; set; }

        public DateTime LastWatchedDate { get; set; }

        
    }
}
