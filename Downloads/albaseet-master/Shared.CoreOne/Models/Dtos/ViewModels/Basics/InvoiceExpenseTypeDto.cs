using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;

namespace Shared.CoreOne.Models.Dtos.ViewModels.Basics
{
	public class InvoiceExpenseTypeDto
	{
		public int InvoiceExpenseTypeId { get; set; }
        public int InvoiceExpenseTypeCode { get; set; }
		public string? InvoiceExpenseTypeNameAr { get; set; }
		public string? InvoiceExpenseTypeNameEn { get; set; }
        public int CompanyId { get; set; }
    }

    public class InvoiceExpenseTypeDropDownDto
    {
        public int InvoiceExpenseTypeId { get; set; }
        public string? InvoiceExpenseTypeName { get; set; }
    }
}
