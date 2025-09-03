using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Dtos.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.Service.Services
{
    public class StockOutQuantityService: IStockOutQuantityService
    {
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly IProformaInvoiceHeaderService _proformaInvoiceHeaderService;
        private readonly IProformaInvoiceDetailService _proformaInvoiceDetailService;
        private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;
        private readonly IStockOutDetailService _stockOutDetailService;
        private readonly IStockOutHeaderService _stockOutHeaderService;
        private readonly IStockOutReturnDetailService _stockOutReturnDetailService;
        private readonly IStockOutReturnHeaderService _stockOutReturnHeaderService;
        private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
        private readonly ISalesInvoiceReturnDetailService _salesInvoiceReturnDetailService;

        public StockOutQuantityService(ISalesInvoiceHeaderService salesInvoiceHeaderService, IProformaInvoiceHeaderService proformaInvoiceHeaderService, IProformaInvoiceDetailService proformaInvoiceDetailService, ISalesInvoiceDetailService salesInvoiceDetailService, IStockOutDetailService stockOutDetailService, IStockOutHeaderService stockOutHeaderService, IStockOutReturnDetailService stockOutReturnDetailService, IStockOutReturnHeaderService stockOutReturnHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService)
        {
            _salesInvoiceHeaderService = salesInvoiceHeaderService;
            _proformaInvoiceHeaderService = proformaInvoiceHeaderService;
            _proformaInvoiceDetailService = proformaInvoiceDetailService;
            _salesInvoiceDetailService = salesInvoiceDetailService;
            _stockOutDetailService = stockOutDetailService;
            _stockOutHeaderService = stockOutHeaderService;
            _stockOutReturnDetailService = stockOutReturnDetailService;
            _stockOutReturnHeaderService = stockOutReturnHeaderService;
            _salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
            _salesInvoiceReturnDetailService = salesInvoiceReturnDetailService;
        }

        public IQueryable<StockDisbursedFromProformaInvoiceDto> GetUnInvoicedDisbursedQuantityFromProformaInvoiceWithAllKeys(int proformaInvoiceHeaderId)
        {
            return GetFinalStocksDisbursedFromProformaInvoiceInternal(proformaInvoiceHeaderId, null, null, false, true);
        }

        public IQueryable<StockDisbursedFromProformaInvoiceDto> GetFinalStocksDisbursedFromProformaInvoice(int proformaInvoiceHeaderId)
        {
            return GetFinalStocksDisbursedFromProformaInvoiceInternal(proformaInvoiceHeaderId, null, null, null, false);
        }

        public IQueryable<StockDisbursedFromProformaInvoiceDto> GetFinalStocksDisbursedFromProformaInvoiceExceptStockOutHeaderId(int proformaInvoiceHeaderId, int exceptStockOutHeaderId)
        {
            return GetFinalStocksDisbursedFromProformaInvoiceInternal(proformaInvoiceHeaderId, exceptStockOutHeaderId, null, null, false);
        }

        public IQueryable<StockDisbursedFromProformaInvoiceDto> GetFinalStocksDisbursedFromProformaInvoiceExceptStockOutReturnHeaderId(int proformaInvoiceHeaderId, int exceptStockOutReturnHeaderId)
        {
            return GetFinalStocksDisbursedFromProformaInvoiceInternal(proformaInvoiceHeaderId, null, exceptStockOutReturnHeaderId, null, false);
        }

        private IQueryable<StockDisbursedFromProformaInvoiceDto> GetFinalStocksDisbursedFromProformaInvoiceInternal(int proformaInvoiceHeaderId, int? exceptStockOutHeaderId, int? exceptStockOutReturnHeaderId, bool? invoiced, bool useAllKeys)
        {
            var stockOutDisbursed = (
                   from stockOutDetail in _stockOutDetailService.GetAll().Where(x => x.StockOutHeaderId != exceptStockOutHeaderId)
                   from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutDetail.StockOutHeaderId && x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId && (invoiced == null || x.IsEnded == invoiced)).Select(x => x.ProformaInvoiceHeaderId)
                   select new StockDisbursedFromProformaInvoiceDto
                   {
                       ItemId = stockOutDetail.ItemId,
                       ItemPackageId = stockOutDetail.ItemPackageId,
                       CostCenterId = useAllKeys ? stockOutDetail.CostCenterId : null,
                       BatchNumber = useAllKeys ? stockOutDetail.BatchNumber : null,
                       ExpireDate = useAllKeys ? stockOutDetail.ExpireDate : null,
                       BarCode = stockOutDetail.BarCode,
                       SellingPrice = stockOutDetail.SellingPrice,
                       ItemDiscountPercent = stockOutDetail.ItemDiscountPercent,
                       QuantityDisbursed = stockOutDetail.Quantity,
                       BonusQuantityDisbursed = stockOutDetail.BonusQuantity
                   }
            ).Concat(
                   from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId != exceptStockOutReturnHeaderId)
                   from stockOutHeaderId in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnDetail.StockOutReturnHeaderId && x.StockOutHeaderId != exceptStockOutHeaderId).Select(x => x.StockOutHeaderId)
                   from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeaderId && x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId && (invoiced == null || x.IsEnded == invoiced)).Select(x => x.ProformaInvoiceHeaderId)
                   select new StockDisbursedFromProformaInvoiceDto
                   {
                       ItemId = stockOutReturnDetail.ItemId,
                       ItemPackageId = stockOutReturnDetail.ItemPackageId,
                       CostCenterId = useAllKeys ? stockOutReturnDetail.CostCenterId : null,
                       BatchNumber = useAllKeys ? stockOutReturnDetail.BatchNumber : null,
                       ExpireDate = useAllKeys ? stockOutReturnDetail.ExpireDate : null,
                       BarCode = stockOutReturnDetail.BarCode,
                       SellingPrice = stockOutReturnDetail.SellingPrice,
                       ItemDiscountPercent = stockOutReturnDetail.ItemDiscountPercent,
                       QuantityDisbursed = -stockOutReturnDetail.Quantity,
                       BonusQuantityDisbursed = -stockOutReturnDetail.BonusQuantity
                   }
                ).GroupBy(x => new { x.ItemId, x.ItemPackageId, x.BatchNumber, x.ExpireDate, x.CostCenterId, x.BarCode, x.SellingPrice, x.ItemDiscountPercent }).Select(
                    x => new StockDisbursedFromProformaInvoiceDto
                    {
                        ItemId = x.Key.ItemId,
                        ItemPackageId = x.Key.ItemPackageId,
                        CostCenterId = x.Key.CostCenterId,
                        BatchNumber = x.Key.BatchNumber,
                        ExpireDate = x.Key.ExpireDate,
                        BarCode = x.Key.BarCode,
                        SellingPrice = x.Key.SellingPrice,
                        ItemDiscountPercent = x.Key.ItemDiscountPercent,
                        QuantityDisbursed = x.Sum(y => y.QuantityDisbursed),
                        BonusQuantityDisbursed = x.Sum(y => y.BonusQuantityDisbursed)
                    }
                );

            return stockOutDisbursed;
        }

        public IQueryable<StockDisbursedFromSalesInvoiceDto> GetFinalStocksDisbursedFromSalesInvoice(int salesInvoiceHeaderId)
        {
            return GetFinalStocksDisbursedFromSalesInvoiceInternal(salesInvoiceHeaderId, null, null);
        }

        public IQueryable<StockDisbursedFromSalesInvoiceDto> GetFinalStocksDisbursedFromSalesInvoiceExceptStockOutHeaderId(int salesInvoiceHeaderId, int exceptStockOutHeaderId)
        {
            return GetFinalStocksDisbursedFromSalesInvoiceInternal(salesInvoiceHeaderId, exceptStockOutHeaderId, null);
        }

        public IQueryable<StockDisbursedFromSalesInvoiceDto> GetFinalStocksDisbursedFromSalesInvoiceExceptStockOutReturnHeaderId(int salesInvoiceHeaderId, int exceptStockOutReturnHeaderId)
        {
            return GetFinalStocksDisbursedFromSalesInvoiceInternal(salesInvoiceHeaderId, null, exceptStockOutReturnHeaderId);
        }

        private IQueryable<StockDisbursedFromSalesInvoiceDto> GetFinalStocksDisbursedFromSalesInvoiceInternal(int salesInvoiceHeaderId, int? exceptStockOutHeaderId, int? exceptStockOutReturnHeaderId)
        {
            var stockOutDisbursed = (
                   from stockOutDetail in _stockOutDetailService.GetAll().Where(x => x.StockOutHeaderId != exceptStockOutHeaderId)
                   from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutDetail.StockOutHeaderId && x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => x.SalesInvoiceHeaderId)
                   select new StockDisbursedFromSalesInvoiceDto
                   {
                       ItemId = stockOutDetail.ItemId,
                       ItemPackageId = stockOutDetail.ItemPackageId,
                       BatchNumber = stockOutDetail.BatchNumber,
                       ExpireDate = stockOutDetail.ExpireDate,
                       CostCenterId = stockOutDetail.CostCenterId,
                       BarCode = stockOutDetail.BarCode,
                       SellingPrice = stockOutDetail.SellingPrice,
                       ItemDiscountPercent = stockOutDetail.ItemDiscountPercent,
                       QuantityDisbursed = stockOutDetail.Quantity,
                       BonusQuantityDisbursed = stockOutDetail.BonusQuantity
                   }
            ).Concat(
                   from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId != exceptStockOutReturnHeaderId)
                   from stockOutHeaderId in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnDetail.StockOutReturnHeaderId && x.StockOutHeaderId != exceptStockOutHeaderId).Select(x => x.StockOutHeaderId)
                   from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeaderId && x.SalesInvoiceHeaderId == salesInvoiceHeaderId).Select(x => x.SalesInvoiceHeaderId)
                   select new StockDisbursedFromSalesInvoiceDto
                   {
                       ItemId = stockOutReturnDetail.ItemId,
                       ItemPackageId = stockOutReturnDetail.ItemPackageId,
                       BatchNumber = stockOutReturnDetail.BatchNumber,
                       ExpireDate = stockOutReturnDetail.ExpireDate,
                       CostCenterId = stockOutReturnDetail.CostCenterId,
                       BarCode = stockOutReturnDetail.BarCode,
                       SellingPrice = stockOutReturnDetail.SellingPrice,
                       ItemDiscountPercent = stockOutReturnDetail.ItemDiscountPercent,
                       QuantityDisbursed = -stockOutReturnDetail.Quantity,
                       BonusQuantityDisbursed = -stockOutReturnDetail.BonusQuantity
                   }
                ).GroupBy(x => new { x.ItemId, x.ItemPackageId, x.BatchNumber, x.ExpireDate, x.CostCenterId, x.BarCode, x.SellingPrice, x.ItemDiscountPercent }).Select(
                    x => new StockDisbursedFromSalesInvoiceDto
                    {
                        ItemId = x.Key.ItemId,
                        ItemPackageId = x.Key.ItemPackageId,
                        BatchNumber = x.Key.BatchNumber,
                        ExpireDate = x.Key.ExpireDate,
                        CostCenterId = x.Key.CostCenterId,
                        BarCode = x.Key.BarCode,
                        SellingPrice = x.Key.SellingPrice,
                        ItemDiscountPercent = x.Key.ItemDiscountPercent,
                        QuantityDisbursed = x.Sum(y => y.QuantityDisbursed),
                        BonusQuantityDisbursed = x.Sum(y => y.BonusQuantityDisbursed)
                    }
                );

            return stockOutDisbursed;
        }

        public IQueryable<StockDisbursedReturnedDto> GetStocksDisbursedReturnedFromStockOut(int stockOutHeaderId)
        {
            return GetStocksDisbursedReturnedFromStockOutInternal(stockOutHeaderId);
        }

        public IQueryable<StockDisbursedReturnedDto> GetStocksDisbursedReturnedFromStockOutExceptStockOutReturnHeaderId(int stockOutHeaderId, int exceptStockOutReturnHeaderId)
        {
            return GetStocksDisbursedReturnedFromStockOutInternal(stockOutHeaderId, exceptStockOutReturnHeaderId);
        }

        private IQueryable<StockDisbursedReturnedDto> GetStocksDisbursedReturnedFromStockOutInternal(int stockOutHeaderId, int? exceptStockOutReturnHeaderId = null)
        {
            var stockOutDisbursedReturned = (
                   from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId != exceptStockOutReturnHeaderId)
                   from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnDetail.StockOutReturnHeaderId && x.StockOutHeaderId == stockOutHeaderId).Select(x => x.StockOutHeaderId)
                   select new StockDisbursedReturnedDto
                   {
                       ItemId = stockOutReturnDetail.ItemId,
                       ItemPackageId = stockOutReturnDetail.ItemPackageId,
                       BatchNumber = stockOutReturnDetail.BatchNumber,
                       ExpireDate = stockOutReturnDetail.ExpireDate,
                       CostCenterId = stockOutReturnDetail.CostCenterId,
                       BarCode = stockOutReturnDetail.BarCode,
                       SellingPrice = stockOutReturnDetail.SellingPrice,
                       ItemDiscountPercent = stockOutReturnDetail.ItemDiscountPercent,
                       QuantityReturned = stockOutReturnDetail.Quantity,
                       BonusQuantityReturned = stockOutReturnDetail.BonusQuantity
                   }
                ).GroupBy(x => new { x.ItemId, x.ItemPackageId, x.BatchNumber, x.ExpireDate, x.CostCenterId, x.BarCode, x.SellingPrice, x.ItemDiscountPercent }).Select(
                    x => new StockDisbursedReturnedDto
                    {
                        ItemId = x.Key.ItemId,
                        ItemPackageId = x.Key.ItemPackageId,
                        BatchNumber = x.Key.BatchNumber,
                        ExpireDate = x.Key.ExpireDate,
                        CostCenterId = x.Key.CostCenterId,
                        BarCode = x.Key.BarCode,
                        SellingPrice = x.Key.SellingPrice,
                        ItemDiscountPercent = x.Key.ItemDiscountPercent,
                        QuantityReturned = x.Sum(y => y.QuantityReturned),
                        BonusQuantityReturned = x.Sum(y => y.BonusQuantityReturned)
                    }
                );

            return stockOutDisbursedReturned;
        }

        public IQueryable<StockDisbursedReturnedDto> GetStocksDisbursedReturnedFromSalesInvoice(int salesInvoiceHeaderId)
        {
            return GetStocksDisbursedReturnedFromSalesInvoiceInternal(salesInvoiceHeaderId, null, null);
        }

        public IQueryable<StockDisbursedReturnedDto> GetStocksDisbursedReturnedFromSalesInvoiceExceptStockOutReturnHeaderId(int salesInvoiceHeaderId, int exceptStockOutReturnHeaderId)
        {
            return GetStocksDisbursedReturnedFromSalesInvoiceInternal(salesInvoiceHeaderId, exceptStockOutReturnHeaderId, null);
        }

		public IQueryable<StockDisbursedReturnedDto> GetUnInvoicedStocksDisbursedReturnedFromSalesInvoice(int salesInvoiceHeaderId)
		{
			return GetStocksDisbursedReturnedFromSalesInvoiceInternal(salesInvoiceHeaderId, null, false);
		}

		private IQueryable<StockDisbursedReturnedDto> GetStocksDisbursedReturnedFromSalesInvoiceInternal(int salesInvoiceHeaderId, int? exceptStockOutReturnHeaderId, bool? invoiced)
        {
            var stockOutDisbursedReturned = (
                   from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId != exceptStockOutReturnHeaderId)
                   from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnDetail.StockOutReturnHeaderId && x.SalesInvoiceHeaderId == salesInvoiceHeaderId && (invoiced == null || x.IsEnded == invoiced)).Select(x => x.StockOutHeaderId)
                   select new StockDisbursedReturnedDto
                   {
                       ItemId = stockOutReturnDetail.ItemId,
                       ItemPackageId = stockOutReturnDetail.ItemPackageId,
                       BatchNumber = stockOutReturnDetail.BatchNumber,
                       ExpireDate = stockOutReturnDetail.ExpireDate,
                       CostCenterId = stockOutReturnDetail.CostCenterId,
                       BarCode = stockOutReturnDetail.BarCode,
                       SellingPrice = stockOutReturnDetail.SellingPrice,
                       ItemDiscountPercent = stockOutReturnDetail.ItemDiscountPercent,
                       QuantityReturned = stockOutReturnDetail.Quantity,
                       BonusQuantityReturned = stockOutReturnDetail.BonusQuantity
                   }
                ).GroupBy(x => new { x.ItemId, x.ItemPackageId, x.BatchNumber, x.ExpireDate, x.CostCenterId, x.BarCode, x.SellingPrice, x.ItemDiscountPercent }).Select(
                    x => new StockDisbursedReturnedDto
                    {
                        ItemId = x.Key.ItemId,
                        ItemPackageId = x.Key.ItemPackageId,
                        BatchNumber = x.Key.BatchNumber,
                        ExpireDate = x.Key.ExpireDate,
                        CostCenterId = x.Key.CostCenterId,
                        BarCode = x.Key.BarCode,
                        SellingPrice = x.Key.SellingPrice,
                        ItemDiscountPercent = x.Key.ItemDiscountPercent,
                        QuantityReturned = x.Sum(y => y.QuantityReturned),
                        BonusQuantityReturned = x.Sum(y => y.BonusQuantityReturned)
                    }
                );

            return stockOutDisbursedReturned;
        }

		//This function is for sales invoices that are on the way only
		public IQueryable<ParentQuantityDto> GetOverallQuantityAvailableFromSalesInvoices()
		{
			//formula: SalesInvoice quantities - StockOut quantities  + StockOutReturn quantities
			var quantityReceived = from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId != null)
								   from stockOutDetail in _stockOutDetailService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId)
								   select new ParentQuantityDto { ParentId = (int)stockOutHeader.SalesInvoiceHeaderId!, Quantity = -(stockOutDetail.Quantity + stockOutDetail.BonusQuantity) };

			var quantityReturned = from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId != null)
								   from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId)
								   from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId)
								   select new ParentQuantityDto { ParentId = (int)stockOutHeader.SalesInvoiceHeaderId!, Quantity = (stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity) };


			var salesInvoiceQuantity = from salesInvoiceDetail in _salesInvoiceDetailService.GetAll()
										  select new ParentQuantityDto { ParentId = salesInvoiceDetail.SalesInvoiceHeaderId, Quantity = salesInvoiceDetail.Quantity + salesInvoiceDetail.BonusQuantity };

			var overallAvailableQuantity = salesInvoiceQuantity.Concat(quantityReceived).Concat(quantityReturned).GroupBy(x => x.ParentId).Select(x => new ParentQuantityDto { ParentId = x.Key, Quantity = x.Sum(y => y.Quantity) });

			return overallAvailableQuantity;
		}

		public IQueryable<ParentQuantityDto> GetOverallQuantityAvailableFromProformaInvoices()
		{
			//formula: ProformaInvoice quantities - StockOut quantities  + StockOutReturn quantities
			var quantityReceived = from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId != null)
								   from stockOutDetail in _stockOutDetailService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId)
								   select new ParentQuantityDto { ParentId = (int)stockOutHeader.ProformaInvoiceHeaderId!, Quantity = -(stockOutDetail.Quantity + stockOutDetail.BonusQuantity) };

			var quantityReturned = from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId != null)
								   from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId)
								   from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId)
								   select new ParentQuantityDto { ParentId = (int)stockOutHeader.ProformaInvoiceHeaderId!, Quantity = (stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity) };


			var proformaInvoiceQuantity = from proformaInvoiceDetail in _proformaInvoiceDetailService.GetAll()
									   select new ParentQuantityDto { ParentId = proformaInvoiceDetail.ProformaInvoiceHeaderId, Quantity = proformaInvoiceDetail.Quantity + proformaInvoiceDetail.BonusQuantity };

			var overallAvailableQuantity = proformaInvoiceQuantity.Concat(quantityReceived).Concat(quantityReturned).GroupBy(x => x.ParentId).Select(x => new ParentQuantityDto { ParentId = x.Key, Quantity = x.Sum(y => y.Quantity) });

			return overallAvailableQuantity;
		}

		public IQueryable<ParentQuantityDto> GetOverallQuantityAvailableReturnFromStockOuts()
		{
			//formula: stockOut quantities - stockOutReturn quantities 
			var quantityReturned = from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutHeaderId != null)
								   from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId)
								   select new ParentQuantityDto { ParentId = (int)stockOutReturnHeader.StockOutHeaderId!, Quantity = -(stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity) };


			var stockOutQuantity = from stockOutDetail in _stockOutDetailService.GetAll()
								  select new ParentQuantityDto { ParentId = stockOutDetail.StockOutHeaderId, Quantity = stockOutDetail.Quantity + stockOutDetail.BonusQuantity };

			var overallAvailableQuantityReturn = stockOutQuantity.Concat(quantityReturned).GroupBy(x => x.ParentId).Select(x => new ParentQuantityDto { ParentId = x.Key, Quantity = x.Sum(y => y.Quantity) });

			return overallAvailableQuantityReturn;
		}

		public IQueryable<ParentQuantityDto> GetOverallQuantityAvailableReturnFromSalesInvoices()
		{
			//formula: SalesInvoice quantities - salesInvoiceReturnOnTheWay - StockOutReturn quantities 
			var quantityReturned = from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId != null)
								   from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId)
								   select new ParentQuantityDto { ParentId = (int)stockOutReturnHeader.SalesInvoiceHeaderId!, Quantity = - (stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity) };


			var salesInvoiceQuantity = from salesInvoiceDetail in _salesInvoiceDetailService.GetAll()
									   select new ParentQuantityDto { ParentId = salesInvoiceDetail.SalesInvoiceHeaderId, Quantity = salesInvoiceDetail.Quantity + salesInvoiceDetail.BonusQuantity };

			var reservationInvoiceCloseOutQuantity = from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll()
													 from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnDetail.SalesInvoiceReturnHeaderId && x.IsOnTheWay)
													 select new ParentQuantityDto
													 {
														 ParentId = salesInvoiceReturnHeader.SalesInvoiceHeaderId,
														 Quantity = - (salesInvoiceReturnDetail.Quantity + salesInvoiceReturnDetail.BonusQuantity)
													 };

			var overallAvailableQuantity = salesInvoiceQuantity.Concat(quantityReturned).Concat(reservationInvoiceCloseOutQuantity).GroupBy(x => x.ParentId).Select(x => new ParentQuantityDto { ParentId = x.Key, Quantity = x.Sum(y => y.Quantity) });

			return overallAvailableQuantity;
		}

		public IQueryable<ParentQuantityDto> GetOverallUnInvoicedQuantityFromProformaInvoices()
        {
			//formula: unInvoiced StockOut quantities  - unInvoiced StockOutReturn quantities
			var quantityReceived = from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId != null && !x.IsEnded)
								   from stockOutDetail in _stockOutDetailService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId)
								   select new ParentQuantityDto { ParentId = (int)stockOutHeader.ProformaInvoiceHeaderId!, Quantity = (stockOutDetail.Quantity + stockOutDetail.BonusQuantity) };

			var quantityReturned = from stockOutHeader in _stockOutHeaderService.GetAll().Where(x => x.ProformaInvoiceHeaderId != null && !x.IsEnded)
								   from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeader.StockOutHeaderId)
								   from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId)
								   select new ParentQuantityDto { ParentId = (int)stockOutHeader.ProformaInvoiceHeaderId!, Quantity = -(stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity) };

			var overallAvailableQuantity = quantityReceived.Concat(quantityReturned).GroupBy(x => x.ParentId).Select(x => new ParentQuantityDto { ParentId = x.Key, Quantity = x.Sum(y => y.Quantity) });

			return overallAvailableQuantity;
		}

		public IQueryable<ParentQuantityDto> GetOverallQuantityReturnedFromSalesInvoices()
		{
			//formula: StockOutReturn quantities 
			var quantityReturned = from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId != null)
								   from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId)
								   select new ParentQuantityDto { ParentId = (int)stockOutReturnHeader.SalesInvoiceHeaderId!, Quantity = (stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity) };

			var overallQuantityReturned = quantityReturned.GroupBy(x => x.ParentId).Select(x => new ParentQuantityDto { ParentId = x.Key, Quantity = x.Sum(y => y.Quantity) });

			return overallQuantityReturned;
		}

		public IQueryable<ParentQuantityDto> GetOverallUnInvoicedQuantityReturnedFromSalesInvoices()
		{
			//formula: UnInvoiced StockOutReturn quantities 
			var quantityReturned = from stockOutReturnHeader in _stockOutReturnHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId != null && !x.IsEnded)
								   from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeader.StockOutReturnHeaderId)
								   select new ParentQuantityDto { ParentId = (int)stockOutReturnHeader.SalesInvoiceHeaderId!, Quantity = (stockOutReturnDetail.Quantity + stockOutReturnDetail.BonusQuantity) };

			var overallQuantityReturned = quantityReturned.GroupBy(x => x.ParentId).Select(x => new ParentQuantityDto { ParentId = x.Key, Quantity = x.Sum(y => y.Quantity) });

			return overallQuantityReturned;
		}
	}
}
