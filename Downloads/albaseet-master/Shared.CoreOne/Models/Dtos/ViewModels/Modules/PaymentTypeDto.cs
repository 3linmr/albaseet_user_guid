using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class PaymentTypeDto
	{
		public byte PaymentTypeId { get; set; }
		public string? PaymentTypeCode { get; set; }
		public string? PaymentTypeNameAr { get; set; }
		public string? PaymentTypeNameEn { get; set; }
	}
	public class PaymentTypeDropDownDto
	{
		public byte PaymentTypeId { get; set; }
		public string? PaymentTypeName { get; set; }
	}
}
