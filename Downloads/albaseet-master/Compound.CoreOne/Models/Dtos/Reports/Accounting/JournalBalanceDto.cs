using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Accounting
{
    public class JournalBalanceDto
    {
        public int JournalDetailId { get; set; }
        public decimal Balance { get; set; }
        public decimal BalanceAccount { get; set; }
    }
}
