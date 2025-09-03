using System;

namespace Compound.CoreOne.Models.Dtos.Reports.CostCenters
{
    public class CostCenterJournalReportDto
    {
        public short? MenuCode { get; set; }
        public string? MenuName { get; set; }
        public int? HeaderId { get; set; }

        public string? DocumentFullCode { get; set; }
        public DateTime? DocumentDate { get; set; }
        public DateTime? EntryDate { get; set; }

        public int? CostCenterId { get; set; }
        public string? CostCenterCode { get; set; }
        public string? CostCenterName { get; set; }

        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }

        public decimal CreditValue { get; set; }
        public decimal DebitValue { get; set; }
        public decimal ProfitAndLossValue { get; set; }

        public decimal? Quantity { get; set; }
        public decimal? Price { get; set; }
        public string? Reference { get; set; }
        public string? Remarks { get; set; }

        public int? StoreId { get; set; }
        public string? StoreName { get; set; }
        public int? BranchId { get; set; }
        public string? BranchName { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyName { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string? UserNameCreated { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
    }
}
