using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
	public interface ISalesInvoiceCollectionService: IBaseService<SalesInvoiceCollection>
	{
		Task<List<SalesInvoiceCollectionDto>> GetSalesInvoiceCollections(int salesInvoiceHeaderId);
		Task<List<SalesInvoiceCollectionDto>> GetSalesInvoiceCollections(int salesInvoiceHeaderId, int storeId);
		Task<List<SalesInvoiceCollectionDto>> SaveSalesInvoiceCollections(int salesInvoiceHeaderId, List<SalesInvoiceCollectionDto> salesInvoiceCollections);
		Task<bool> DeleteSalesInvoiceCollections(int salesInvoiceHeaderId);
	}
}
