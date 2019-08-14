using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using static AvibaWeb.DomainModels.CorporatorReceipt;

namespace AvibaWeb.DomainModels
{
    public class Counterparty
    {
        public enum CEntityType
        {
            [Display(Name = "Физическое лицо")]
            Individual = 0,
            [Display(Name = "Юридическое лицо")]
            Entity = 1
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "ИНН")]
        public string ITN { get; set; }

        [Display(Name = "Наименование")]
        public string Name { get; set; }

        [Display(Name = "Корреспондентский счет")]
        public string CorrespondentAccount { get; set; }

        [Display(Name = "КПП")]
        public string KPP { get; set; }

        [Display(Name = "БИК")]
        public string BIK { get; set; }

        [Display(Name = "ОГРН")]
        public string OGRN { get; set; }

        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Display(Name = "E-mail")]
        public string Email { get; set; }

        [Display(Name = "Наименование банка")]
        public string BankName { get; set; }

        [Display(Name = "Расчетный счет")]
        public string BankAccount { get; set; }

        [Display(Name = "Имя представителя")]
        public string ManagementName { get; set; }

        [Display(Name = "Должность представителя")]
        public string ManagementPosition { get; set; }

        public CEntityType EntityType { get; set; } = CEntityType.Entity;

        public CR1CStatus _1CStatus { get; set; }

        [Display(Name = "Тип")]
        public int? TypeId { get; set; }
        [ForeignKey("TypeId")]
        public virtual CounterpartyType Type { get; set; }

        public int? LoanGroupId { get; set; }
        [ForeignKey("LoanGroupId")]
        public virtual LoanGroup LoanGroup { get; set; }

        [Display(Name = "Подразделение")]
        public int? ExpenditureDeskGroupId { get; set; }
        [ForeignKey("ExpenditureDeskGroupId")]
        public virtual DeskGroup ExpenditureDeskGroup { get; set; }

        [Display(Name = "Статья расходов")]
        public int? ExpenditureObjectId { get; set; }
        [ForeignKey("ExpenditureObjectId")]
        public virtual ExpenditureObject ExpenditureObject { get; set; }

        public virtual ProviderBinding ProviderBinding { get; set; }

        public virtual ProviderBalance ProviderBalance { get; set; }

        public virtual SubagentData SubagentData { get; set; }

        public virtual CorporatorAccount CorporatorAccount { get; set; }
    }
}
