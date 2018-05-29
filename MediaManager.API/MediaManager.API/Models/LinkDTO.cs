using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MediaManager.API.Models
{
    /// <summary>
    /// Class to create a link
    /// </summary>
    public class LinkDTO
    {
        /// <summary>
        /// Link
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// Relation of the entity to parent entity
        /// </summary>
        public string Rel { get; set; }

        /// <summary>
        /// Pst method it supports
        /// </summary>
        public string Method { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="href">link</param>
        /// <param name="rel">relation</param>
        /// <param name="method">HTTP method</param>
        public LinkDTO(string href, string rel, string method)
        {
            this.Href = href;
            this.Rel = rel;
            this.Method = method;
        }
    }
}
