using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace CorrelationStation.Models
{
    public class UploadVM
    {
        [Required]
        public HttpPostedFileBase File { get; set; }
        public bool FileSaved { get; set; }

    }
}