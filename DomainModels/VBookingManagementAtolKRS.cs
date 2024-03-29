﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class VBookingManagementAtolKRS
    {
        [Key]
        public int AtolServerId { get; set; }

        public string AtolServerName { get; set; }

        public decimal Amount { get; set; }

        public DateTime OperationDateTime { get; set; }
    }
}
