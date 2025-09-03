using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models;

namespace Shared.CoreOne.Models.Domain.FixedAssets
{
	public class FixedAssetVoucherType : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public byte FixedAssetVoucherTypeId { get; set; }

		[Column(Order = 2)]
		[Required, StringLength(100)]
		public string? FixedAssetVoucherTypeNameAr { get; set; }

		[Column(Order = 3)]
		[Required, StringLength(100)]
		public string? FixedAssetVoucherTypeNameEn { get; set; }
	}
}
