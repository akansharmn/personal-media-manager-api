using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MediaManager.DAL.DBEntities
{
    [Table("VideoParticipant")]
    public class VideoParticipant
    {
      
        public Video Video { get; set; }

        [Key, Column(Order = 0)]
        public int VideoId { get; set; }

        public Participant Participant { get; set; }

        [Key, Column(Order = 1)]
        public int ParticipantId { get; set; }
    }
}
