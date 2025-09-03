using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Basics
{
	public class InvoiceTypeDto
	{
		public byte InvoiceTypeId { get; set; }
		public string? InvoiceTypeNameAr { get; set; }
		public string? InvoiceTypeNameEn { get; set; }
	}

	public class InvoiceTypeDropDownDto
	{
		public byte InvoiceTypeId { get; set; }
		public string? InvoiceTypeName { get; set; }
	}
}
