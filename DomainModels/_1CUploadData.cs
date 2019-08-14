using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class _1CUploadData
    {
        [Key]
        public int _1CUploadDataId { get; set; }

        public string CounterpartyId { get; set; }
        [ForeignKey("CounterpartyId")]
        public virtual Counterparty Principal { get; set; }

        public string PrincipalDocumentName { get; set; }
    }
}
