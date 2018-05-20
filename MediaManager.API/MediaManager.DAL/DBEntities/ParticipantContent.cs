using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MediaManager.DAL.DBEntities
{
    public class ParticipantContent
    {
        [Key, Column(Order = 0)]
        
        public int ParticipantId { get; set; }


        [Key, Column(Order = 1)]
        public int ContentId { get; set; }

        [Key, Column(Order = 2)]
        public int Id { get; set; }
    }
}
