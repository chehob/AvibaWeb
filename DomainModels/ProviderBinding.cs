using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class ProviderBinding
    {
        [Key]
        public int ProviderBindingId { get; set; }

        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual Counterparty Provider { get; set; }

        public string Session { get; set; }
    }
}
