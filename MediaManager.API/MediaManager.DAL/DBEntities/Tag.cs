using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace MediaManager.DAL.DBEntities
{
    public class Tag
    {
        
        public string TagName { get; set; }


        public int ContentId { get; set; }

        public int Id { get; set; }
    }
}
