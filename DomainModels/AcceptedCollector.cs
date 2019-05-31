using System.ComponentModel.DataAnnotations.Schema;
using AvibaWeb.Models;

namespace AvibaWeb.DomainModels
{
    public class AcceptedCollector
    {
        public string ProviderId { get; set; }
        [ForeignKey("ProviderId")]
        public virtual AppUser Provider { get; set; }

        public string CollectorId { get; set; }
        [ForeignKey("CollectorId")]
        public virtual AppUser Collector { get; set; }
    }
}