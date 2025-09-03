using Sales.CoreOne.Contracts;
using Shared.CoreOne.Models.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sales.CoreOne.Models.Dtos.ViewModels;

namespace Sales.Service.Services
{
	public class ProformaInvoiceStatusService: IProformaInvoiceStatusService
	{
		private readonly IProformaInvoiceHeaderService _proformaInvoiceHeaderService;
		private readonly IProformaInvoiceDetailService _proformaInvoiceDetailService;
		private readonly IStockOutDetailService _stockOutDetailService;
		private readonly IStockOutReturnDetailService _stockOutReturnDetailService;
		private readonly IStockOutHeaderService _stockOutHeaderService;
		private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
		private readonly ISalesInvoiceReturnDetailService _salesInvoiceReturnDetailService;
		private readonly IClientDebitMemoService _clientDebitMemoService;
		private readonly IClientCreditMemoService _clientCreditMemoService;

		public ProformaInvoiceStatusService(IProformaInvoiceHeaderService proformaInvoiceHeaderService, IStockOutDetailService stockOutDetailService, IStockOutReturnDetailService stockOutReturnDetailService, IStockOutHeaderService stockOutHeaderService, IStockOutReturnHeaderService stockOutReturnHeaderService, IProformaInvoiceDetailService proformaInvoiceDetailService, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, ISalesInvoiceDetailService salesInvoiceDetailService, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService, IClientDebitMemoService clientDebitMemoService, IClientCreditMemoService clientCreditMemoService)
		{
			_proformaInvoiceHeaderService = proformaInvoiceHeaderService;
			_proformaInvoiceDetailService = proformaInvoiceDetailService;
			_stockOutDetailService = stockOutDetailService;
			_stockOutReturnDetailService = stockOutReturnDetailService;
			_stockOutHeaderService = stockOutHeaderService;
			_stockOutReturnHeaderService = stockOutReturnHeaderService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_salesInvoiceDetailService = salesInvoiceDetailService;
			_salesInvoiceReturnDetailService = salesInvoiceReturnDetailService;
			_clientDebitMemoService = clientDebitMemoService;
			_clientCreditMemoService = clientCreditMemoService;
		}

		public async Task UpdateProformaInvoiceStatus(int proformaInvoiceHeaderId, int documentStatusId)
		{
			int finalDocumentStatusId;

			if (documentStatusId == -1)
			{
				finalDocumentStatusId = await ComputeProformaInvoiceDocumentStatusId(proformaInvoiceHeaderId);
			}
			else
			{
				finalDocumentStatusId = documentStatusId;
			}

			await _proformaInvoiceHeaderService.UpdateDocumentStatus(proformaInvoiceHeaderId, finalDocumentStatusId);
		}

		public async Task<int> ComputeProformaInvoiceDocumentStatusId(int proformaInvoiceHeaderId)
		{
			var lastDescendant = await GetLastCreatedDescendant(proformaInvoiceHeaderId);

			return lastDescendant?.MenuCode switch
			{
				MenuCodeData.CashSalesInvoice => DocumentStatusData.SalesInvoiceCreated,
				MenuCodeData.CashReservationInvoice => DocumentStatusData.SalesInvoiceCreated,
				MenuCodeData.SalesInvoiceReturn => DocumentStatusData.SalesReturnInvoiceCreated,
				MenuCodeData.StockOutReturnFromInvoice => await GetStatusBasedOnQuantityReturnedFromSalesInvoice((int)lastDescendant.ParentId!),
				MenuCodeData.ReservationInvoiceCloseOut => DocumentStatusData.SalesReturnInvoiceCreated,
				MenuCodeData.StockOutFromProformaInvoice => await GetStatusBasedOnFinalQuantityDisbursedFromProformaInvoice((int)lastDescendant.ParentId!),
				MenuCodeData.StockOutFromReservation => await GetStatusBasedOnFinalQuantityDisbursedFromSalesInvoice((int)lastDescendant.ParentId!),
				MenuCodeData.ClientDebitMemo => DocumentStatusData.ClientDebitMemoCreated,
				MenuCodeData.ClientCreditMemo => DocumentStatusData.ClientCreditMemoCreated,
				_ => DocumentStatusData.ProformaInvoiceCreated
			};
		}

		private async Task<int> GetStatusBasedOnFinalQuantityDisbursedFromProformaInvoice(int proformaInvoiceHeaderId)
		{
			var finalQuantityDisbursedFromProformaInvoice = await GetFinalQuantityDisbursedFromProformaInvoice(proformaInvoiceHeaderId);
			var proformaInvoiceQuantity = await GetProformaInvoiceQuantity(proformaInvoiceHeaderId);
			if (finalQuantityDisbursedFromProformaInvoice == 0)
			{
				return DocumentStatusData.ProformaInvoiceCreated;
			}
			else if (finalQuantityDisbursedFromProformaInvoice < proformaInvoiceQuantity)
			{
				return DocumentStatusData.QuantityPartiallyDisbursed;
			}
			else
			{
				return DocumentStatusData.QuantityFullyDisbursed;
			}
		}

		private async Task<decimal> GetFinalQuantityDisbursedFromProformaInvoice(int proformaInvoiceHeaderId)
		{
			decimal quantityDisbursed = await (from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
											  from stockOutDetail in _stockOutDetailService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId)
											  select stockOutDetail.Quantity + stockOutDetail.BonusQuantity).SumAsync();

			decimal quantityReturned = await (from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
											  from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId)
											  from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId)
											  select stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity).SumAsync();

			return quantityDisbursed - quantityReturned;
		}

		private async Task<decimal> GetProformaInvoiceQuantity(int proformaInvoiceHeaderId)
		{
			return await _proformaInvoiceDetailService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId).Select(x => x.Quantity + x.BonusQuantity).SumAsync();
		}

		private async Task<int> GetStatusBasedOnFinalQuantityDisbursedFromSalesInvoice(int salesInvoiceHeaderId)
		{
			var finalQuantityDisbursedFromSalesInvoice = await GetFinalQuantityDisbursedFromSalesInvoice(salesInvoiceHeaderId);
			var salesInvoiceQuantity = await GetSalesInvoiceQuantity(salesInvoiceHeaderId);
			if (finalQuantityDisbursedFromSalesInvoice == 0)
			{
				return DocumentStatusData.SalesInvoiceCreated;
			}
			else if (finalQuantityDisbursedFromSalesInvoice < salesInvoiceQuantity)
			{
				return DocumentStatusData.QuantityPartiallyDisbursed;
			}
			else
			{
				return DocumentStatusData.QuantityFullyDisbursed;
			}
		}

		private async Task<int> GetStatusBasedOnQuantityReturnedFromSalesInvoice(int salesInvoiceHeaderId)
		{
			var salesInvoiceQuantity = await GetSalesInvoiceQuantity(salesInvoiceHeaderId);
			var reservationInvoiceCloseOutQuantity = await GetReservationInvoiceCloseOutQuantity(salesInvoiceHeaderId);
			var quantityReturned = await GetQuantityReturnedFromSalesInvoice(salesInvoiceHeaderId);
			
			if (quantityReturned == 0)
			{
				return DocumentStatusData.SalesInvoiceCreated;
			}
			else if (quantityReturned < salesInvoiceQuantity - reservationInvoiceCloseOutQuantity)
			{
				return DocumentStatusData.PartialDisbursedQuantityReturnedWaitingSalesReturnInvoice;
			}
			else
			{
				return DocumentStatusData.EntireQuantityDisbursedReturnedWaitingSalesReturnInvoice;
			}
		}

		private async Task<decimal> GetFinalQuantityDisbursedFromSalesInvoice(int salesInvoiceHeaderId)
		{
			decimal quantityDisbursed = await (from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId)
											   from stockOutDetail in _stockOutDetailService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId)
											   select stockOutDetail.Quantity + stockOutDetail.BonusQuantity).SumAsync();

			decimal quantityReturned = await (from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId)
											  from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId)
											  from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId)
											  select stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity).SumAsync();

			return quantityDisbursed - quantityReturned;
		}

		private async Task<decimal> GetSalesInvoiceQuantity(int salesInvoiceHeaderId)
		{
			return await _salesInvoiceDetailService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => x.Quantity + x.BonusQuantity).SumAsync();
		}

		private async Task<decimal> GetReservationInvoiceCloseOutQuantity(int salesInvoiceHeaderId)
		{
			return await (from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId && !x.IsOnTheWay)
						  from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId)
						  select salesInvoiceReturnDetail.Quantity + salesInvoiceReturnDetail.BonusQuantity).SumAsync();
		}

		private async Task<decimal> GetQuantityReturnedFromSalesInvoice(int salesInvoiceHeaderId)
		{
			return await (from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId)
						  from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId)
						  select stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity).SumAsync();
		}

		public async Task<DescendantDto?> GetLastCreatedDescendant(int proformaInvoiceHeaderId)
		{
			var stockOutsFromProforma = from stockOut in _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
										select new DescendantDto { MenuCode = MenuCodeData.StockOutFromProformaInvoice, Id = stockOut.StockOutHeaderId, ParentId = stockOut.ProformaInvoiceHeaderId, CreatedAt = stockOut.CreatedAt };

			var stockOutsFromReservation = from salesInvoice in _salesInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
										   from stockOut in _stockOutHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoice.SalesInvoiceHeaderId)
										   select new DescendantDto  { MenuCode = MenuCodeData.StockOutFromReservation, Id = stockOut.StockOutHeaderId, ParentId = stockOut.SalesInvoiceHeaderId, CreatedAt = stockOut.CreatedAt };

			var allStockOuts = stockOutsFromProforma.Concat(stockOutsFromReservation);

			var salesInvoices = from salesInvoice in _salesInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
								select new DescendantDto { MenuCode = salesInvoice.IsOnTheWay ? MenuCodeData.CashReservationInvoice : MenuCodeData.CashSalesInvoice, Id = salesInvoice.SalesInvoiceHeaderId, ParentId = salesInvoice.ProformaInvoiceHeaderId, CreatedAt = salesInvoice.CreatedAt };

			var salesInvoiceReturns = from salesInvoice in _salesInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
									  from salesInvoiceReturn in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoice.SalesInvoiceHeaderId)
									  select new DescendantDto { MenuCode = salesInvoiceReturn.IsOnTheWay ? MenuCodeData.ReservationInvoiceCloseOut : MenuCodeData.SalesInvoiceReturn, Id = salesInvoiceReturn.SalesInvoiceReturnHeaderId, ParentId = salesInvoiceReturn.SalesInvoiceHeaderId, CreatedAt = salesInvoiceReturn.CreatedAt };

			var stockOutReturnFromSalesInvoice = from salesInvoice in _salesInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
												 from stockOutReturn in _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoice.SalesInvoiceHeaderId)
												 select new DescendantDto { MenuCode = MenuCodeData.StockOutReturnFromInvoice, Id = stockOutReturn.StockOutReturnHeaderId, ParentId = stockOutReturn.SalesInvoiceHeaderId, CreatedAt = stockOutReturn.CreatedAt };

			var clientDebitMemos = from salesInvoice in _salesInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
								   from clientDebitMemo in _clientDebitMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoice.SalesInvoiceHeaderId)
								   select new DescendantDto { MenuCode = MenuCodeData.ClientDebitMemo, Id = clientDebitMemo.ClientDebitMemoId, ParentId = clientDebitMemo.SalesInvoiceHeaderId, CreatedAt = clientDebitMemo.CreatedAt };

			var clientCreditMemos = from salesInvoice in _salesInvoiceHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
									from clientCreditMemo in _clientCreditMemoService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoice.SalesInvoiceHeaderId)
									select new DescendantDto { MenuCode = MenuCodeData.ClientCreditMemo, Id = clientCreditMemo.ClientCreditMemoId, ParentId = clientCreditMemo.SalesInvoiceHeaderId, CreatedAt = clientCreditMemo.CreatedAt };

			return await allStockOuts.Concat(salesInvoices).Concat(salesInvoiceReturns).Concat(stockOutReturnFromSalesInvoice).Concat(clientDebitMemos).Concat(clientCreditMemos).OrderByDescending(x => x.CreatedAt).FirstOrDefaultAsync();
		}
	}
}
