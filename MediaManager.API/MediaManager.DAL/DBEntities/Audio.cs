using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MediaManager.DAL.DBEntities
{
    public class Audio 
    {
       
        public string Title { get; set; }

       

        public string Url { get; set; }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AudioId { get; set; }

        

        [ForeignKey("Author")]
        public int AuthorId { get; set; }

        public Participant Author { get; set; }

        public TimeSpan Duration { get; set; }

        public TimeSpan WatchOffset { get; set; }

        public DateTimeOffset UploadedDate { get; set; }

        [ForeignKey("Domain")]
        public int DomainId { get; set; }

        public Domain Domain { get; set; }

        public string Properties { get; set; }

        public DateTimeOffset LastWatchedDate { get; set; }
       
    }
}
