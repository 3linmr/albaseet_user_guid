using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Accounting
{
	public class MissingDocumentCodesReportDto
	{
		public int Serial { get; set; }
		public short? MenuCode { get; set; }
		public string? MenuName { get; set; }
		public int? StoreId { get; set; }
		public string? StoreName { get; set; }
		public int? Year { get; set; }
		public string? Prefix { get; set; }
		public int? DocumentCode { get; set; }
		public string? Suffix { get; set; }
		public string? DocumentFullCode { get; set; }
		public int? Min { get; set; }
		public int? Max { get; set; }
		public int? Count { get; set; }
	}

	public class AllDocumentCodesDto
	{
		public short MenuCode { get; set; }
		public int StoreId { get; set; }
		public int Year { get; set; }
		public string? Prefix { get; set; }
		public string? Suffix { get; set; }
		public int DocumentCode { get; set; }
	}
}
