using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class CurrencyRateDto
	{
		public int CurrencyRateId { get; set; }

		public short FromCurrencyId { get; set; }
		
		public string? FromCurrencyName { get; set; }

		public short ToCurrencyId { get; set; }
	
		public string? ToCurrencyName { get; set; }

		public decimal CurrencyRateValue { get; set; }

		public DateTime? CreatedAt { get; set; }
		public DateTime? ModifiedAt { get; set; }

		public string? UserNameCreated { get; set; }
		public string? UserNameModified { get; set; }
	}
}