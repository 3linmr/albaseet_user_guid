using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class CurrencyDto
	{
		public short CurrencyId { get; set; }
		public string? CurrencyNameAr { get; set; }
		public string? CurrencyNameEn { get; set; }
		public string? IsoCode { get; set; }
		public string? Symbol { get; set; }
		public string? FractionalUnitAr { get; set; }
		public string? FractionalUnitEn { get; set; }
		public short NumberToBasic { get; set; }
	}

	public class CurrencyDropDownDto
	{
		public short CurrencyId { get; set; }
		public string? CurrencyName { get; set; }
	}
}
