﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class CorporatorReceiptMultiPayment
    {
        [Key]
        public int CorporatorReceiptMultiPaymentId { get; set; }

        public decimal? Amount { get; set; }

        public string ErrorString { get; set; }

        public int FinancialAccountOperationId { get; set; }
        [ForeignKey("FinancialAccountOperationId")]
        public virtual FinancialAccountOperation FinancialAccountOperation { get; set; }
    }
}
