using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AvibaWeb.DomainModels
{
    public class _1CUpload
    {
        [Key]
        public int _1CUploadId { get; set; }

        public int UploadVersion { get; set; }
    }
}
