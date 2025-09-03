using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.CoreOne.Models.Dtos.ViewModels
{
	public struct ValidationResultAndListOfInvoices
	{
		public required ResponseDto validation;
		public List<int>? completelySettledSalesInvoiceIds;
	}
}
