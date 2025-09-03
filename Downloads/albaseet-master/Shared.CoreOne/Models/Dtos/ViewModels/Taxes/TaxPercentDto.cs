using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Taxes
{
	public class TaxPercentDto
	{
		public int TaxPercentId { get; set; }
		public int TaxId { get; set; }
		public DateTime FromDate { get; set; }
		public decimal Percent { get; set; }
	}

	public class CompanyVatPercentDto
	{
		public int CompanyId { get; set; }
		public int TaxId { get; set; }
		public decimal VatPercent { get; set; }
	}
}
