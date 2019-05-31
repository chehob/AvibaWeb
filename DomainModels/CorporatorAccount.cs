using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class CorporatorAccount
    {
        [Key]
        public int CorporatorAccountId { get; set; }

        public string ITN { get; set; }
        [ForeignKey("ITN")]
        public virtual Counterparty Corporator { get; set; }

        [Display(Name = "Наименование")]
        public string Description { get; set; }

        [Display(Name = "Банк")]
        public string BankName { get; set; }

        [Display(Name = "Официальное наименование")]
        public string OffBankName { get; set; }

        [Display(Name = "БИК")]
        public string BIK { get; set; }

        [Display(Name = "Корр счет")]
        public string CorrespondentAccount { get; set; }

        [Display(Name = "Активен")]
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Баланс")]
        [Column(TypeName = "Money")]
        public decimal Balance { get; set; }

        public DateTime? LastPaymentDate { get; set; }

        public DateTime? LastReceiptDate { get; set; }
    }
}
