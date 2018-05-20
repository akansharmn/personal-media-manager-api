using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaManager.API.Models
{
    public abstract class LinkedResourceBasedDTO
    {
        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
    }
}
