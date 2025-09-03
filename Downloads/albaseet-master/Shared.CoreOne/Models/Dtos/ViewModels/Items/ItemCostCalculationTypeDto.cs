using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Items
{
	public class ItemCostCalculationTypeDto
	{
		public byte ItemCostCalculationTypeId { get; set; }
		public string? ItemCostCalculationTypeNameAr { get; set; }
		public string? ItemCostCalculationTypeNameEn { get; set; }
	}

	public class ItemCostCalculationTypeDropDownDto
	{
		public byte ItemCostCalculationTypeId { get; set; }
		public string? ItemCostCalculationTypeName { get; set; }
	}
}
