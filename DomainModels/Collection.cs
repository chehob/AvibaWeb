using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AvibaWeb.Models;

namespace AvibaWeb.DomainModels
{
    public enum OperationTypes
    {
        [Display(Name = "Инкассация")]
        Basic,
        [Display(Name = "Обмен")]
        Exchange,
    }

    public class Collection
    {
        public int CollectionId { get; set; }

        [Column(TypeName = "Money")]
        public decimal Amount { get; set; }

        public string DeskIssuedId { get; set; }
        [ForeignKey("DeskIssuedId")]
        public virtual Desk DeskIssued { get; set; }

        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual AppUser Provider { get; set; }

        public string CollectorId { get; set; }
        [ForeignKey("CollectorId")]
        public virtual AppUser Collector { get; set; }

        public PaymentTypes PaymentType { get; set; }

        public OperationTypes OperationType { get; set; }

        public string Comment { get; set; }

        public virtual ICollection<CollectionOperation> Operations { get; set; }
    }
}