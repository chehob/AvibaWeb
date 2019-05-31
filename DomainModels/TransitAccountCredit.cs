using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class TransitAccountCredit
    {
        [Key]
        public int TransitAccountCreditId { get; set; }

        // Sum to credit from transit account
        [Display(Name = "Сумма со счета")]
        //[AmountLessThan("Account", ErrorMessage = "Not valid")]
        public decimal AccountAmount { get; set; }

        // Additional sum without source
        [Display(Name = "Добавочная сумма")]
        public decimal AddAmount { get; set; }

        public int TransitAccountId { get; set; }
        [ForeignKey("TransitAccountId")]
        public virtual TransitAccount Account { get; set; }

        public int? LoanGroupId { get; set; }
        [ForeignKey("LoanGroupId")]
        public virtual LoanGroup LoanGroup { get; set; }

        public virtual ICollection<TransitAccountCreditOperation> Operations { get; set; }

        [Display(Name = "Комментарий")]
        public string Comment { get; set; }
    }
}
