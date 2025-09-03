using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class SellerTypeDto
	{
		public byte SellerTypeId { get; set; }
		public string? SellerTypeName { get; set; }
	}
}
