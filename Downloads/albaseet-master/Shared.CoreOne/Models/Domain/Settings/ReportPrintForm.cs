using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Settings
{
	public class ReportPrintForm : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public int ReportPrintFormId { get; set; }

		[StringLength(200)]
		[Column(Order = 2)]
		public string? ReportPrintFormNameAr { get; set; }

		[StringLength(200)]
		[Column(Order = 3)] 
		public string? ReportPrintFormNameEn { get; set; }
	}
}
