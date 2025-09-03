using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Journal
{
	public class JournalTypeDto
	{
		public byte JournalTypeId { get; set; }
		public string? JournalTypeNameAr { get; set; }
		public string? JournalTypeNameEn { get; set; }
	}

	public class JournalTypeDropDownDto
	{
		public byte JournalTypeId { get; set; }
		public string? JournalTypeName { get; set; }
	}
}
