using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MediaManager.DAL.DBEntities
{
    public class Playlist
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PlaylistId { get; set; }

        [Required]
        public string PlaylistName { get; set; }

        [Required]
        public int ContentId { get; set; }        
        
       
        public string Description { get; set; }



        
    }
}
