using Microsoft.EntityFrameworkCore;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Shared.CoreOne;
using Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Service.Services
{
	public class InvoiceStockOutReturnService: BaseService<InvoiceStockOutReturn>, IInvoiceStockOutReturnService
	{
		public InvoiceStockOutReturnService(IRepository<InvoiceStockOutReturn> repository) : base(repository) { }

		public IQueryable<int> GetStockOutReturnsLinkedToSalesInvoiceReturn(int salesInvoiceReturnHeaderId)
		{
			return _repository.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeaderId).Select(x => x.StockOutReturnHeaderId);
		}

		public async Task LinkStockOutReturnsToSalesInvoiceReturn(int salesInvoiceReturnHeaderId, List<int> stockOutReturnHeaderIds)
		{
			var newInvoiceStockOutReturns = stockOutReturnHeaderIds.Select(x => new InvoiceStockOutReturn { SalesInvoiceReturnHeaderId = salesInvoiceReturnHeaderId, StockOutReturnHeaderId = x }).ToList();
			var newId = await GetNextId();

			newInvoiceStockOutReturns.ForEach(x => x.InvoiceStockOutReturnId = newId++);

			await _repository.InsertRange(newInvoiceStockOutReturns);
			await _repository.SaveChanges();
		}

		public async Task UnlinkStockOutReturnsFromSalesInvoiceReturn(int salesInvoiceReturnHeaderId)
		{
			var deleteInvoiceStockOutReturns = await _repository.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeaderId).ToListAsync();

			_repository.RemoveRange(deleteInvoiceStockOutReturns);
			await _repository.SaveChanges();
		}

		public async Task<int> GetNextId()
		{
			try
			{
				return await _repository.GetAll().Select(x => x.InvoiceStockOutReturnId).MaxAsync() + 1;
			}
			catch
			{
				return 1;
			}
		}
	}
}
