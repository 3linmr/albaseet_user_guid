using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Accounts
{
	public class AccountEntityDto
	{
		//public int EntityTypeId { get; set; }
		public string? EntityTypeName { get; set; }
		public string? EntityId { get; set; }
		public string? EntityName { get; set; }
		public string? EntityNameAr { get; set; }
		public string? EntityNameEn { get; set; }
		public string? TaxCode { get; set; }
		public string? EntityEmail { get; set; }
	}
}
