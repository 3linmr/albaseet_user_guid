using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Accounting
{
    public class StoreIncomeReportDto 
    {
        public short MenuCode { get; set; }
        public string? MenuName { get; set; }
        public int HeaderId { get; set; }
        public string? DocumentFullCode { get; set; }
        public DateTime DocumentDate { get; set; }
        public DateTime EntryDate { get; set; }
        public decimal NetValue { get; set; } //القيمة بعد الضريبة
        public int? ClientId { get; set; }
        public int? ClientCode { get; set; }
        public string? ClientName { get; set; }
        public int? SellerId { get; set; }
        public int? SellerCode { get; set; }
        public string? SellerName { get; set; }
        public int StoreId { get; set; }
        public string? StoreName { get; set; }

        public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
        public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }
    }

    public class StoreIncomeReportDataDto
    {
		public int HeaderId { get; set; }
		public short MenuCode { get; set; }
		public string? DocumentFullCode { get; set; }
		public DateTime DocumentDate { get; set; }
		public DateTime EntryDate { get; set; }
		public decimal NetValue { get; set; }
		public int? ClientId { get; set; }
		public int? SellerId { get; set; }
		public int StoreId { get; set; }

        public DateTime? CreatedAt { get; set; }
		public string? UserNameCreated { get; set; }
        public DateTime? ModifiedAt { get; set; }
		public string? UserNameModified { get; set; }
    }
}
