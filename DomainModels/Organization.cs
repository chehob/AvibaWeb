using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvibaWeb.DomainModels
{
    public class Organization
    {
        [Key]
        public int OrganizationId { get; set; }

        [Display(Name = "Наименование")]
        public string Description { get; set; }

        [Display(Name = "Активен")]
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Имя файла росписи")]
        public string SignatureFileName { get; set; }

        [Display(Name = "Имя файла печати")]
        public string StampFileName { get; set; }

        [Display(Name = "Имя представителя")]
        public string HeadName { get; set; }

        [Display(Name = "Должность")]
        public string HeadTitle { get; set; }

        public string CounterpartyId { get; set; }
        [ForeignKey("CounterpartyId")]
        public virtual Counterparty Counterparty { get; set; }

        public virtual ICollection<FinancialAccount> Accounts { get; set; }

        public int ReceiptNumberGroupId { get; set; }

        public string CorpReceiptPrefix { get; set; }
    }
}
