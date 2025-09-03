using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Items;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Items
{
	public class ItemBarCodeDto
	{
		public int ItemBarCodeId { get; set; }
		public int FromPackageId { get; set; }
		public int ToPackageId { get; set; }
		public int ItemId { get; set; }
		public string? FromPackageName { get; set; }
		public string? ToPackageName { get; set; }
		public decimal Packing { get; set; }
		public bool IsSingularPackage { get; set; }

		public string? BarCode { get; set; }
		public decimal ConsumerPrice { get; set; }
		public List<ItemBarCodeDetailDto>? ItemBarCodeDetails { get; set; }
	}
	public class ItemPackingVm
	{
		public int ItemPackingId { get; set; }

		public int FromPackageId { get; set; }
		public string? FromPackageName { get; set; }

		public int ToPackageId { get; set; }
		public string? ToPackageName { get; set; }

		public decimal Packing { get; set; }
	}

	public class ItemPackingDto
	{
		public decimal Packing { get; set; }
		public int ToPackage { get; set; }
	}

	public class ItemPackagesDto
	{
		public int ItemId { get; set; }
		public List<int>? Packages { get; set; }
	}

}
