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
	public class InvoiceStockOutService: BaseService<InvoiceStockOut>, IInvoiceStockOutService
	{
		public InvoiceStockOutService(IRepository<InvoiceStockOut> repository) : base(repository) { }

		public IQueryable<int> GetStockOutsLinkedToSalesInvoice(int salesInvoiceHeaderId)
		{
			return _repository.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => x.StockOutHeaderId);
		}

		public async Task LinkStockOutsToSalesInvoice(int salesInvoiceHeaderId, List<int> stockOutHeaderIds)
		{
			var newInvoiceStockOuts = stockOutHeaderIds.Select(x => new InvoiceStockOut { SalesInvoiceHeaderId = salesInvoiceHeaderId, StockOutHeaderId = x }).ToList();
			var newId = await GetNextId();

			newInvoiceStockOuts.ForEach(x => x.InvoiceStockOutId = newId++);

			await _repository.InsertRange(newInvoiceStockOuts);
			await _repository.SaveChanges();
		}

		public async Task UnlinkStockOutsFromSalesInvoice(int salesInvoiceHeaderId)
		{
			var deleteInvoiceStockOuts = await _repository.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).ToListAsync();

			_repository.RemoveRange(deleteInvoiceStockOuts);
			await _repository.SaveChanges();
		}

		public async Task<int> GetNextId()
		{
			try
			{
				return await _repository.GetAll().Select(x => x.InvoiceStockOutId).MaxAsync() + 1;
			}
			catch
			{
				return 1;
			}
		}
	}
}
