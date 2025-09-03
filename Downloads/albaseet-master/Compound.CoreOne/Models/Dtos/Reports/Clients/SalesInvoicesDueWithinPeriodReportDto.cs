using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Clients
{
    public class SalesInvoicesDueWithinPeriodReportDto
    {

        public int SalesInvoiceHeaderId { get; set; }
        public string? CompanyName { get; set; }
        public string? BranchName { get; set; }
        public string? DocumentFullCode { get; set; }
        public short? MenuCode { get; set; }
        public string? MenuName { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime EntryDate { get; set; }
        public int ClientId { get; set; }
        public int ClientCode { get; set; }
        public string? ClientName { get; set; }
        public int? AccountId { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
        public int? SellerId { get; set; }
        public int? SellerCode { get; set; }
        public string? SellerName { get; set; }
        public string? UserNameCreated { get; set; }
        public DateTime? CreatedAt { get; set; }
        public decimal InvoiceValue { get; set; }
        public decimal SettledValue { get; set; }
        public decimal RemainingValue { get; set; }
        public DateTime? DueDate { get; set; }
        public int Age { get; set; }
        public string? Reference { get; set; }
        public string? RemarksAr { get; set; }
        public string? RemarksEn { get; set; }
        public int RemainingDays { get; set; }
        public int InvoiceDuration { get; set; } //المهلة
    }
}
