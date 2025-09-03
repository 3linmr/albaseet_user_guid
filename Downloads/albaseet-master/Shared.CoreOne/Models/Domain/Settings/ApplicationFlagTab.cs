using Shared.CoreOne.Models.Domain.Menus;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Settings
{
	public class ApplicationFlagTab : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int ApplicationFlagTabId { get; set; }

		[Column(Order = 2)]
		[StringLength(200)]
		public string? ApplicationFlagTabNameAr { get; set; }

		[Column(Order = 3)]
		[StringLength(200)]
		public string? ApplicationFlagTabNameEn { get; set; }

		[Column(Order = 4)]
		public short Order { get; set; }
	}
}
