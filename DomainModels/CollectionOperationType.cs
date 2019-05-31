using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AvibaWeb.DomainModels
{
    public class CollectionOperationType
    {
        public enum COType
        {
            [Display(Name = "Ожидание")]
            New,
            [Display(Name = "Получено")]
            Accepted,
            [Display(Name = "Отказ")]
            Rejected,
            [Display(Name = "Отмена")]
            Cancelled
        }

        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public COType CollectionOperationTypeId { get; set; }

        public string Description { get; set; }
    }
}