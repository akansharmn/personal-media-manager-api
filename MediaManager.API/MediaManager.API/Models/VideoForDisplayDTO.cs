using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.DAL.DBEntities;

namespace MediaManager.API.Models
{
    public class VideoForDisplayDTO : LinkedResourceBasedDTO
    {
       public int VideoId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Url { get; set; }

        

        public string AuthorName { get; set; }

        public TimeSpan Duration { get; set; }

        public TimeSpan WatchOffset { get; set; }

        public DateTime UploadedDate { get; set; }

        
         
      

        public string Properties { get; set; }



        public DateTime LastWatchedDate { get; set; }

        public List<string> Participant { get; set; }
    }
}
