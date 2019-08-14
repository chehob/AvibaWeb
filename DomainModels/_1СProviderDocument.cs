using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class _1СProviderDocument
    {
        [Key]
        public int _1СProviderDocumentId { get; set; }

        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual Counterparty Provider { get; set; }

        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        public string DocumentName { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime DocumentDate { get; set; }

        public string _1CId { get; set; }

        public bool IsActive { get; set; }
    }
}
