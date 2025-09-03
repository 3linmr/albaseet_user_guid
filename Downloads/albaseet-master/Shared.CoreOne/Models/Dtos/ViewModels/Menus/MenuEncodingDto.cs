using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Menus
{
	public class MenuEncodingDto
	{
		public int MenuEncodingId { get; set; }

		public int StoreId { get; set; }

		public string? StoreNameAr { get; set; }

		public string? StoreNameEn { get; set; }

		public short MenuCode { get; set; }

		public string? MenuNameAr { get; set; }
		public string? MenuNameEn { get; set; }

		public string? Prefix { get; set; }

		public string? Suffix { get; set; }
	}

	public class MenuEncodingVm
	{
		public string? Prefix { get; set; }
		public string? Suffix { get; set; }
	}
}
