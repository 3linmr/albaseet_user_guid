using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Basics
{
	public class ShipmentTypeDto
	{
		public int ShipmentTypeId { get; set; }
		public int ShipmentTypeCode { get; set; }
		public string? ShipmentTypeNameAr { get; set; }
		public string? ShipmentTypeNameEn { get; set; }
		public string? ShipmentTypeName { get; set; }
		public int? CompanyId { get; set; }
	}

	public class ShipmentTypeDropDownDto
	{
		public int ShipmentTypeId { get; set; }
		public string? ShipmentTypeName { get; set; }
	}
}
