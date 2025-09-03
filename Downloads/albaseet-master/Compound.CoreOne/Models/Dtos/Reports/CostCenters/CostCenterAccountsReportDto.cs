using System;

namespace Compound.CoreOne.Models.Dtos.Reports.CostCenters
{
    public class CostCenterAccountsReportDto
    {
        public int? CostCenterId { get; set; }
        public string? CostCenterCode { get; set; }
        public string? CostCenterName { get; set; }

        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }

        public decimal CreditValue { get; set; }
        public decimal DebitValue { get; set; }
        public decimal ProfitAndLossValue { get; set; }

        public decimal? Quantity { get; set; }

        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string? UserNameCreated { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
    }
}
