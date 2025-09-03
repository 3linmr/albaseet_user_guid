using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Items
{
	public class ItemBarCodeDetailDto
	{
		public int ItemBarCodeDetailId { get; set; }
		public int ItemBarCodeId { get; set; }
		public string? BarCode { get; set; }
		public decimal ConsumerPrice { get; set; }
		public bool IsSingularPackage { get; set; }
	}
}
