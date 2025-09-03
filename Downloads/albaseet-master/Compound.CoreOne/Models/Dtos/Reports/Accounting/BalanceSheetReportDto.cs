using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Accounting
{
	public class BalanceSheetReportDto
	{
        public int Serial { get; set; }
        public int ParentSerial { get; set; }
        public int? AccountId { get; set; }
        public int Level { get; set; }
        public int? MainAccountId { get; set; }
		public string? AccountNameAr { get; set; }
        public decimal? Value { get; set; } //القيمة
        public decimal? SubTotal { get; set; } //المجموع الفرعي
		public decimal? NetValue { get; set; } //الإجمالي
        public string? AccountNameEn { get; set; }
        public string? BalanceSheetFormatType { get; set; }
	}

	public static class BalanceSheetFormatType
    {
        public static readonly string Heading = "Heading";
        public static readonly string SubHeading = "SubHeading";
        public static readonly string SubTotal = "SubTotal";
        public static readonly string NetValue = "NetValue";
        public static readonly string NetProfit = "NetProfit";
        public static readonly string NetLoss = "NetLoss";
        public static readonly string Regular = "Regular";
    }
}
