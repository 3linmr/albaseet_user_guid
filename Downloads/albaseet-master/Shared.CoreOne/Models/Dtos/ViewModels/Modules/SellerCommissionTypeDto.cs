using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class SellerCommissionTypeDto
	{
		public byte SellerCommissionTypeId { get; set; }
		public string? SellerCommissionTypeName { get; set; }
	}
}
