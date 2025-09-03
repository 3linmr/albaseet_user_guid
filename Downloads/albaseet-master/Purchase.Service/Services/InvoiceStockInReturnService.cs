using Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne;
using Microsoft.EntityFrameworkCore;
using Purchases.CoreOne.Contracts;

namespace Purchases.Service.Services
{
	public class InvoiceStockInReturnService: BaseService<InvoiceStockInReturn>, IInvoiceStockInReturnService
	{
		public InvoiceStockInReturnService(IRepository<InvoiceStockInReturn> repository) : base(repository) { }

		public IQueryable<int> GetStockInReturnsLinkedToPurchaseInvoiceReturn(int purchaseInvoiceReturnHeaderId)
		{
			return _repository.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeaderId).Select(x => x.StockInReturnHeaderId);
		}

		public async Task LinkStockInReturnsToPurchaseInvoiceReturn(int purchaseInvoiceReturnHeaderId, List<int> stockInReturnHeaderIds)
		{
			var newInvoiceStockInReturns = stockInReturnHeaderIds.Select(x => new InvoiceStockInReturn { PurchaseInvoiceReturnHeaderId = purchaseInvoiceReturnHeaderId, StockInReturnHeaderId = x }).ToList();
			var newId = await GetNextId();

			newInvoiceStockInReturns.ForEach(x => x.InvoiceStockInReturnId = newId++);

			await _repository.InsertRange(newInvoiceStockInReturns);
			await _repository.SaveChanges();
		}

		public async Task UnlinkStockInReturnsFromPurchaseInvoiceReturn(int purchaseInvoiceReturnHeaderId)
		{
			var deleteInvoiceStockInReturns = await _repository.GetAll().Where(x => x.PurchaseInvoiceReturnHeaderId == purchaseInvoiceReturnHeaderId).ToListAsync();

			_repository.RemoveRange(deleteInvoiceStockInReturns);
			await _repository.SaveChanges();
		}

		public async Task<int> GetNextId()
		{
			try
			{
				return await _repository.GetAll().Select(x => x.InvoiceStockInReturnId).MaxAsync() + 1;
			}
			catch
			{
				return 1;
			}
		}
	}
}
