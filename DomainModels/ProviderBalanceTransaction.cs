using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class ProviderBalanceTransaction
    {
        public enum PBTType
        {
            Ticket,
            Penalty,
            Luggage
        }

        [Key]
        public int ProviderBalanceTransactionId { get; set; }

        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual ProviderBalance ProviderBalance { get; set; }

        [Column(TypeName = "Money")]
        public decimal OldBalance { get; set; }

        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        public int BookingOperationId { get; set; }

        public PBTType TypeId { get; set; }
    }
}
