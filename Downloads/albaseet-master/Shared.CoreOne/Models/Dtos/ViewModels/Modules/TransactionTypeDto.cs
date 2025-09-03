using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Modules
{
	public class TransactionTypeDto
	{
		public byte TransactionTypeId { get; set; }
		public string? TransactionTypeNameAr { get; set; }
		public string? TransactionTypeNameEn { get; set; }
	}

	public class TransactionTypeDropDownDto
	{
		public byte TransactionTypeId { get; set; }
		public string? TransactionTypeName { get; set; }
	}
}