using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MediaManager.DAL.DBEntities
{
    [Table("Tag")]
    public class Tag
    {
     
        [Key, Column(Order = 0)]
        public string TagName { get; set; }

        [Key, Column(Order = 1)]
        public int ContentId { get; set; }

        [Key, Column(Order = 2)]
        public int Id { get; set; }
    }
}
