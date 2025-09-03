using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class SellerCommissionDto
	{
		public int SellerCommissionId { get; set; }
		public short SellerCommissionMethodId { get; set; }
		public string? SellerCommissionTypeName { get; set; }
		public string? SellerCommissionMethodName { get; set; }
		public int From { get; set; }
		public string? FromText { get; set; }
		public int To { get; set; }
		public string? ToText { get; set; }
		public decimal CommissionPercent { get; set; }
		public string? CommissionPercentText { get; set; }
		public bool CanEdit { get; set; }
	}
}
