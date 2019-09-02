using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AvibaWeb.Models;

namespace AvibaWeb.DomainModels
{
    public class Desk
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(10)]
        public string DeskId { get; set; }

        [Display(Name = "Наименование")]
        [StringLength(50)]
        public string Description { get; set; }

        [Display(Name = "Активен")]
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        public int? GroupId { get; set; }
        [ForeignKey("GroupId")]
        public virtual DeskGroup Group { get; set; }

        public int? BMDeskGroupId { get; set; }
        [ForeignKey("BMDeskGroupId")]
        public virtual BMDeskGroup BMDeskGroup { get; set; }

        public ICollection<SubagentDesk> SubagentDesks { get; set; }
    }
}