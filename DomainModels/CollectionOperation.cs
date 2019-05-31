using System;
using System.ComponentModel.DataAnnotations.Schema;
using AvibaWeb.Models;

namespace AvibaWeb.DomainModels
{
    public class CollectionOperation
    {
        public int CollectionOperationId { get; set; }

        public int CollectionId { get; set; }
        [ForeignKey("CollectionId")]
        public virtual Collection Collection { get; set; }

        public CollectionOperationType.COType OperationTypeId { get; set; }
        [ForeignKey("OperationTypeId")]
        public virtual CollectionOperationType OperationType { get; set; }

        public DateTime OperationDateTime { get; set; }
    }
}