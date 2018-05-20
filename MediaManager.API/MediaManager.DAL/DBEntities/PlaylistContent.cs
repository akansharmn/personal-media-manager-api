using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MediaManager.DAL.DBEntities
{
    class PlaylistContent
    {
        [Key, Column(Order = 0)]
        public int PlaylistId { get; set; }

        public Playlist Playlist { get; set; }

        [Key, Column(Order = 1)]
        public int Id { get; set; }
    }
}
