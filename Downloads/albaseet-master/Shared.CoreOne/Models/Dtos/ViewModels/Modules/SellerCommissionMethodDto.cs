using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class SellerCommissionMethodDto
	{
		public short SellerCommissionMethodId { get; set; }
		public short SellerCommissionMethodCode { get; set; }
		public string? SellerCommissionMethodNameAr { get; set; }
		public string? SellerCommissionMethodNameEn { get; set; }
		public byte SellerCommissionTypeId { get; set; }
		public string? SellerCommissionTypeName { get; set; }
        public int CompanyId { get; set; }
        public bool IsActive { get; set; }
		public string? InActiveReasons { get; set; }
	}
	
	public class SellerCommissionMethodDropDownDto
	{
		public short SellerCommissionMethodId { get; set; }
		public byte SellerCommissionTypeId { get; set; }
		public string? SellerCommissionMethodName { get; set; }
		public bool IsActive { get; set; }
	}

	public class SellerCommissionMethodChangeDto
	{
		public int SellerCommissionMethodId { get; set; }
		public bool CanEdit { get; set; }
		public decimal MaxFromValue { get; set; }
	}
}
