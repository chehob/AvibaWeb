using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class CorporatorDocument
    {
        [Key]
        public int CorporatorDocumentId { get; set; }

        public string ITN { get; set; }
        [ForeignKey("ITN")]
        public virtual Counterparty Corporator { get; set; }

        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        public string Document { get; set; }

        public DateTime Date { get; set; }
    }
}
