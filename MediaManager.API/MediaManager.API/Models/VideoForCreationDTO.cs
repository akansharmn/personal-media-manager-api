using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using MediaManager.DAL.DBEntities;

namespace MediaManager.API
{
    public class VideoForCreationDTO
    {
        [Required]
        public string Title { get; set; }


        [Required]
        public string Url { get; set; }

        public List<string> Participants { get; set; }

        public string AuthorName { get; set; }

        public int Duration { get; set; }

        public int WatchOffset { get; set; }

        public DateTime UploadedDate { get; set; }

        public DateTime LastWatchedDate { get; set; }

        public string Properties { get; set; }


        [Required]
        public string Domain { get; set; }


        [Required]
        public string Username { get; set; }
    }
}
