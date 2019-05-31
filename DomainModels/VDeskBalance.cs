using System;
using System.ComponentModel.DataAnnotations;

namespace AvibaWeb.DomainModels
{
    public class VDeskBalance
    {
        [Key]
        public Guid Id { get; set; }
        public string DeskId { get; set; }
        public string DeskName { get; set; }
        public decimal Balance { get; set; }
    }
}