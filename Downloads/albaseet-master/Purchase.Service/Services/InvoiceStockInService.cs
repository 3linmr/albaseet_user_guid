using Purchases.CoreOne.Contracts;
using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne;
using Shared.CoreOne.Models;
using Shared.Repository;
using Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Purchases.Service.Services
{
	public class InvoiceStockInService: BaseService<InvoiceStockIn>, IInvoiceStockInService
	{
		public InvoiceStockInService(IRepository<InvoiceStockIn> repository) : base(repository) { }

		public IQueryable<int> GetStockInsLinkedToPurchaseInvoice(int purchaseInvoiceHeaderId)
		{
			return _repository.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).Select(x => x.StockInHeaderId);
		}

		public async Task LinkStockInsToPurchaseInvoice(int purchaseInvoiceHeaderId, List<int> stockInHeaderIds)
		{
			var newInvoiceStockIns = stockInHeaderIds.Select(x => new InvoiceStockIn { PurchaseInvoiceHeaderId = purchaseInvoiceHeaderId, StockInHeaderId = x }).ToList();
			var newId = await GetNextId();

			newInvoiceStockIns.ForEach(x => x.InvoiceStockInId = newId++);

			await _repository.InsertRange(newInvoiceStockIns);
			await _repository.SaveChanges();
		}

		public async Task UnlinkStockInsFromPurchaseInvoice(int purchaseInvoiceHeaderId)
		{
			var deleteInvoiceStockIns = await _repository.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).ToListAsync();

			_repository.RemoveRange(deleteInvoiceStockIns);
			await _repository.SaveChanges();
		}

		public async Task<int> GetNextId()
		{
			try
			{
				return await _repository.GetAll().Select(x => x.InvoiceStockInId).MaxAsync() + 1;
			}
			catch
			{
				return 1;
			}
		}
	}
}
