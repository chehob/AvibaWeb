using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class FinancialAccountOperation
    {
        [Key]
        public int FinancialAccountOperationId { get; set; }

        public decimal Amount { get; set; }

        public int FinancialAccountId { get; set; }
        [ForeignKey("FinancialAccountId")]
        public virtual FinancialAccount Account { get; set; }

        public int? TransferFinancialAccountId { get; set; }
        [ForeignKey("TransferFinancialAccountId")]
        public virtual FinancialAccount TransferAccount { get; set; }

        public string CounterpartyId { get; set; }
        [ForeignKey("CounterpartyId")]
        public virtual Counterparty Counterparty { get; set; }

        [MaxLength(128)]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual AppUser PayeeUser { get; set; }

        public DateTime OperationDateTime { get; set; }
        public DateTime InsertDateTime { get; set; }

        public string Description { get; set; }

        public string OrderNumber { get; set; }
    }
}
