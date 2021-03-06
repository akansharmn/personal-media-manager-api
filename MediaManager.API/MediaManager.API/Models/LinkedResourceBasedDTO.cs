﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaManager.API.Models
{
    /// <summary>
    /// Class containign Links
    /// </summary>
    public abstract class LinkedResourceBasedDTO
    {
        /// <summary>
        /// List of links
        /// </summary>
        public List<LinkDTO> Links { get; set; } = new List<LinkDTO>();
    }
}
