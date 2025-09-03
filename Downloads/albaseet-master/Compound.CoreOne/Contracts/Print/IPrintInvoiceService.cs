using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Models.Dtos.Print;

namespace Compound.CoreOne.Contracts.Print
{
	public interface IPrintInvoiceService
	{
		Task<SimplifiedInvoiceDto> GetSimplifiedInvoice(int salesInvoiceHeaderId);
		Task<TaxInvoiceDto> GetTaxInvoice(int salesInvoiceHeaderId);
	}
}
