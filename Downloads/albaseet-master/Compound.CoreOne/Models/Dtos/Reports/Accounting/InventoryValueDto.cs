using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Accounting
{
    internal class InventoryValueDto
    {
        
        public decimal InValue { get; set; }
        public decimal OurValue { get; set; }
        
        public decimal OpenInValue { get; set; }
        public decimal OpenOutValue { get; set; }
        
    }
}
