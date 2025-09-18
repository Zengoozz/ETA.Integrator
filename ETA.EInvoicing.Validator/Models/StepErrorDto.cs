using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETA.EInvoicing.Validator.Models
{
    public class StepErrorDto
    {
        public int StepNumber { get; set; } 
        public string StepName { get; set; } = "";
        public string ValidationType { set; get; } = "";
        public List<ValidationErrorDto> Errors { get; set; } = new();
    }
}
