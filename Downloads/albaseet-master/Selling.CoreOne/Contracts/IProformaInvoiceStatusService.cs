using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
	public interface IProformaInvoiceStatusService
	{
		/// <summary>
		///     Update the status of Proforma Invoice Header Id
		/// </summary>
		/// <param name="proformaInvoiceHeaderId"> The Id of the Proforma Invoice whose status should be updated</param>
		/// <param name="documentStatusId"> The status value that the Proforma Invoice should contain. To automatically determine the correct status, set this to -1 </param>
		Task UpdateProformaInvoiceStatus(int proformaInvoiceHeaderId, int documentStatusId);
	}
}
