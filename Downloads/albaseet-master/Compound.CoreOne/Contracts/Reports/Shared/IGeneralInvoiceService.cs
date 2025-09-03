using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Models.Dtos.Reports.Shared;

namespace Compound.CoreOne.Contracts.Reports.Shared
{
	public interface IGeneralInvoiceService
	{
		IQueryable<GeneralInvoiceDto> GetGeneralInvoices();
	}
}
