using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.CostCenters
{
    public class MainCostCenterReportDto
    {
        public int CostCenterId { get; set; }
        public string? CostCenterCode { get; set; }
        public string? CostCenterName { get; set; }
        public int CostCenterLevel { get; set; }
        public bool IsMainCostCenter { get; set; }
        public int? MainCostCenterId { get; set; }
        public string? MainCostCenterCode { get; set; }
        public string? MainCostCenterName { get; set; }
        public decimal OpenBalance { get; set; }
        public decimal DebitValue { get; set; }
        public decimal CreditValue { get; set; }
        public decimal CurrentBalance { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? UserNameCreated { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
    }
}
