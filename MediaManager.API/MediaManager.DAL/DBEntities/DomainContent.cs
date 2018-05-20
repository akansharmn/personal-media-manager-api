using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MediaManager.DAL.DBEntities
{
    [Table("DomainContent")]
    public class DomainContent
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Domain")]
        public int DomainId { get; set; }

        
        public string DomainName { get; set; }

        
        public string Schema { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Content")]
        public int ContentId { get; set; }

       
        public string ContentName { get; set; }

        public Domain Domain { get; set; }

        public Content Content { get; set; }

        
    }
}