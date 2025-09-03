using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Taxes
{
	public class TaxTypeDto
	{
		public byte TaxTypeId { get; set; }
		public string? TaxTypeNameAr { get; set; }
		public string? TaxTypeNameEn { get; set; }
	}
	public class TaxTypeDropDownDto
	{
		public byte TaxTypeId { get; set; }
		public string? TaxTypeName { get; set; }
	}
}
