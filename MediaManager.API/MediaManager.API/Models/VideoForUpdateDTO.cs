using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MediaManager.DAL.DBEntities;

namespace MediaManager.API.Controllers
{
    public class VideoForUpdateDTO
    {
        [Required]
        public int VideoId { get; set; }

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
        public string Username { get; set; }

        [Required]
        public string Domain { get; set; }
    }
}