using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Menus
{
	public class MenuCodeDto
	{
		public short MenuCode { get; set; }
		public string? MenuName { get; set; }
		public string? MenuNameAr { get; set; }
		public string? MenuNameEn { get; set; }
		public bool HasApprove { get; set; }
		public bool HasNotes { get; set; }
		public bool HasEncoding { get; set; }
		public bool IsFavorite { get; set; }
	}
	public class MenuCodeDropDownDto
	{
		public short MenuCode { get; set; }
		public string? MenuName { get; set; }
	}
}
