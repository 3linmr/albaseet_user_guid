using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Domain.Basics
{
	public class InvoiceExpenseType : BaseObject
	{
		[Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
		[Column(Order = 1)] 
		public int InvoiceExpenseTypeId { get; set; }

		[Column(Order = 2)]
		public int InvoiceExpenseTypeCode { get; set; }

		[Required, StringLength(200)]
		[Column(Order = 3)]
		public string? InvoiceExpenseTypeNameAr { get; set; }

		[Required, StringLength(200)]
		[Column(Order = 4)]
		public string? InvoiceExpenseTypeNameEn { get; set; }

		[Column(Order = 5)]
        public int CompanyId { get; set; }


        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }
    }
}
