using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.Domain.Basics
{
	public class InvoiceType : BaseObject
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Column(Order = 1)]
		public byte InvoiceTypeId { get; set; }

		[Required, StringLength(50)]
		[Column(Order = 2)]
		public string? InvoiceTypeNameAr { get; set; }

		[Required, StringLength(50)]
		[Column(Order = 3)]
		public string? InvoiceTypeNameEn { get; set; }
	}
}
