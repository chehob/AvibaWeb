using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class CorporatorDocumentFeeItem
    {
        [Key]
        public int CorporatorDocumentFeeItemId { get; set; }

        public int CorporatorDocumentId { get; set; }
        [ForeignKey("CorporatorDocumentId")]
        public virtual CorporatorDocument CorporatorDocument { get; set; }

        public string Name { get; set; }

        public string FeeStr { get; set; }
    }
}
