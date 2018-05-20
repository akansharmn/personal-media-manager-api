using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaManager.DAL.DBEntities
{
    [Table("Participant")]
    public class Participant
    {
        [Key]
        public int ParticipantId { get; set; }

        public string ParticipantName { get; set; }

        public List<Video> videos { get; set; }

        public List<Audio> Audios { get; set; }

        public List<VideoParticipant> VideoParticipants { get; set; }

    }
}