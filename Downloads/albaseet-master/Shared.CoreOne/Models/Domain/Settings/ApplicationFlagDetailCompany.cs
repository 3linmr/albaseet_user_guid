using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Settings
{
	public class ApplicationFlagDetailCompany : BaseObject
	{
		[Column(Order = 1)]
		[Key,DatabaseGenerated(DatabaseGeneratedOption.None)] 
		public int ApplicationFlagDetailCompanyId { get; set; }

		[Column(Order = 2)]
		public int CompanyId { get; set; }

		[Column(Order = 3)]
		public int ApplicationFlagDetailId { get; set; }

		[Column(Order = 4)]
		public string? FlagValue { get; set; }


		[ForeignKey(nameof(CompanyId))]
		public Company? Company { get; set; }

		[ForeignKey(nameof(ApplicationFlagDetailId))]
		public ApplicationFlagDetail? ApplicationFlagDetail { get; set; }
	}
}
