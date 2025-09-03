using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Accounts
{
	public class AccountType : BaseObject
	{
		[Column(Order = 1)]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		public byte AccountTypeId { get; set; }

		[Required, StringLength(50)]
		[Column(Order = 2)]
		public string? AccountTypeNameAr { get; set; }

		[Required, StringLength(50)]
		[Column(Order = 3)]
		public string? AccountTypeNameEn { get; set; }

		[Column(Order = 4)]
		public bool IsInternalSystem { get; set; }
	}
}
