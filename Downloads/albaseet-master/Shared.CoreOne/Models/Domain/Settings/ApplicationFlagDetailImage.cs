using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Settings
{
	public class ApplicationFlagDetailImage : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int ApplicationFlagDetailImageId { get; set; }

		[Column(Order = 2)]
		public int ApplicationFlagDetailCompanyId { get; set; }

		[StringLength(500)]
		[Column(Order = 3)]
		public string? FileName { get; set; }

		[StringLength(500)]
		[Column(Order = 4)]
		public string? FileType { get; set; }

		[Column(Order = 5)]
		public byte[]? Image { get; set; }


		[ForeignKey(nameof(ApplicationFlagDetailCompanyId))]
		public ApplicationFlagDetailCompany? ApplicationFlagDetailCompany { get; set; }
	}
}
