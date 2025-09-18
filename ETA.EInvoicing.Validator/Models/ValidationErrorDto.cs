using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETA.EInvoicing.Validator.Models
{
    public class ValidationErrorDto
    {
        public string Key { get; set; } = "";
        public string Message { get; set; } = "";
       
    }
}
