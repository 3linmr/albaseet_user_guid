using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Shared
{
	public class QuantityPriceDto
	{
		public decimal Quantity { get; set; }
		public decimal Price { get; set; }
	}


	public class ItemQuantityPriceDto
	{
		public int ItemId { get; set; }
		public decimal Quantity { get; set; }
		public decimal Price { get; set; }
	}
	public class ItemQuantityDto
	{
		public int ItemId { get; set; }
		public decimal Quantity { get; set; }
	}
}
