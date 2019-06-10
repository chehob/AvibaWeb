using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvibaWeb.DomainModels
{
    public class FinancialAccount
    {
        [Key]
        public int FinancialAccountId { get; set; }

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

        [Display(Name = "Баланс")]
        [Column(TypeName = "Money")]
        public decimal Balance { get; set; }

        [Display(Name = "Активен")]
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        public int OrganizationId { get; set; }
        [ForeignKey("OrganizationId")]
        public virtual Organization Organization { get; set; }

        public DateTime? LastUploadDate { get; set; }
    }
}
