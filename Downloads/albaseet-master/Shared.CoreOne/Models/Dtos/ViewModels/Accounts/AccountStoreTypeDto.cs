using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Accounts
{
	public class AccountStoreTypeDto
	{
		public int AccountStoreTypeId { get; set; }
		public string? AccountStoreTypeNameAr { get; set; }
		public string? AccountStoreTypeNameEn { get; set; }
	}
	public class AccountStoreTypeDropDownDto
	{
		public int AccountStoreTypeId { get; set; }
		public string? AccountStoreTypeName { get; set; }
	}
}
