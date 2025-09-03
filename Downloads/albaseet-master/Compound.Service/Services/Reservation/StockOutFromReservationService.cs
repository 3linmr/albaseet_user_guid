using Compound.CoreOne.Contracts.Reservation;
using Inventory.CoreOne.Contracts;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Inventory.Service.Services;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Sales.Service.Services;
using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.Service.Services.Reservation
{
	public class StockOutFromReservationService : IStockOutFromReservationService
	{
		private readonly IStockOutHandlingService _stockOutHandlingService;
		private readonly IDirectInternalTransferService _directInternalTransferService;
		private readonly IStockOutHeaderService _stockOutHeaderService;
		private readonly IStoreService _storeService;
		private readonly IStockOutService _stockOutService;
		private readonly IItemCurrentBalanceService _itemCurrentBalanceService;
		private readonly IGenericMessageService _genericMessageService;

		public StockOutFromReservationService(IStockOutHandlingService stockOutHandlingService, IDirectInternalTransferService directInternalTransferService, IStockOutHeaderService stockOutHeaderService, IStoreService storeService, IStockOutService stockOutService, IItemCurrentBalanceService itemCurrentBalanceService, IGenericMessageService genericMessageService)
		{
			_stockOutHandlingService = stockOutHandlingService;
			_directInternalTransferService = directInternalTransferService;
			_stockOutHeaderService = stockOutHeaderService;
			_storeService = storeService;
			_stockOutService = stockOutService;
			_itemCurrentBalanceService = itemCurrentBalanceService;
			_genericMessageService = genericMessageService;
		}

		public async Task<ResponseDto> SaveStockOutFromReservation(StockOutDto stockOut, bool hasApprove, bool isApproved, int? requestId)
		{

			List<StockOutDetailDto>? oldStockOutDetails = null;
			int? oldParentStoreId = null;
			int? oldReservationStoreId = null;
			if (stockOut.StockOutHeader!.StockOutHeaderId != 0)
			{
				var oldStockOut = await _stockOutService.GetStockOut(stockOut.StockOutHeader.StockOutHeaderId);
				oldStockOutDetails = oldStockOut.StockOutDetails;
				oldParentStoreId = oldStockOut.StockOutHeader?.StoreId;
				oldReservationStoreId = await _storeService.GetReservedStoreByParentStoreId((int)oldParentStoreId!);
			}

			var newReservationStoreId = await _storeService.GetReservedStoreByParentStoreId(stockOut.StockOutHeader.StoreId);
			var documentReference = await GetStockOutDocumentReference(stockOut.StockOutHeader!.StockOutHeaderId, hasApprove, requestId);

			var stockOutResult = await _stockOutHandlingService.SaveStockOut(stockOut, hasApprove, isApproved, requestId, false, documentReference, false);
			if (stockOutResult.Success == false) return stockOutResult;

			var internalTransferResult = await SaveRelatedInternalTransferAndReceive(stockOut, stockOutResult.Id, newReservationStoreId, documentReference);
			if (internalTransferResult.Success == false) return internalTransferResult;


			await UpdateCurrentBalances(oldParentStoreId, oldReservationStoreId, stockOut.StockOutHeader.StoreId, newReservationStoreId, oldStockOutDetails, stockOut.StockOutDetails);

			return stockOutResult;
		}

		private async Task UpdateCurrentBalances(int? oldParentStoreId, int? oldReservationStoreId, int newParentStoreId, int newReservationStoreId, List<StockOutDetailDto>? oldStockOutDetails, List<StockOutDetailDto> newStockOutDetails)
		{
			var newItemBalances = GetItemCurrentBalancesFromStockOut(newParentStoreId, newReservationStoreId, newStockOutDetails);
			if (oldStockOutDetails is null || oldParentStoreId is null)
			{
				await InventoryOut(newParentStoreId, newReservationStoreId, newItemBalances.ParentStoreCurrentBalances, newItemBalances.ReservationStoreCurrentBalances);
			}
			else
			{
				if (oldParentStoreId == newParentStoreId && oldReservationStoreId == newReservationStoreId)
				{
					var oldItemBalances = GetItemCurrentBalancesFromStockOut((int)oldParentStoreId, (int)oldReservationStoreId, oldStockOutDetails);
					await InventoryInOut((int)oldParentStoreId, (int)oldReservationStoreId!, oldItemBalances, newItemBalances);
				}
				else
				{
					await DeductCurrentBalances((int)oldParentStoreId, (int)oldReservationStoreId!, oldStockOutDetails);
					await InventoryOut(newParentStoreId, newReservationStoreId, newItemBalances.ParentStoreCurrentBalances, newItemBalances.ReservationStoreCurrentBalances);
				}
			}
		}

		private async Task DeductCurrentBalances(int parentStoreId, int reservationStoreId, List<StockOutDetailDto> stockOutDetails)
		{
			var oldItemCurrentBalances = GetItemCurrentBalancesFromStockOut(parentStoreId, reservationStoreId, stockOutDetails);

			var newItemCurrentBalances = new ReservationStoreAndParentStoreCurrentBalancesDto
			{
				ParentStoreCurrentBalances = (from itemCurrentPrices in oldItemCurrentBalances.ParentStoreCurrentBalances
											select new ItemCurrentBalanceDto()
											{
												ItemCurrentBalanceId = itemCurrentPrices.ItemCurrentBalanceId,
												StoreId = itemCurrentPrices.StoreId,
												ItemId = itemCurrentPrices.ItemId,
												ItemPackageId = itemCurrentPrices.ItemPackageId,
												ExpireDate = itemCurrentPrices.ExpireDate,
												BatchNumber = itemCurrentPrices.BatchNumber,
												//Zero
											}).ToList(),

				ReservationStoreCurrentBalances = (from itemCurrentPrices in oldItemCurrentBalances.ReservationStoreCurrentBalances
										  select new ItemCurrentBalanceDto()
										  {
											  ItemCurrentBalanceId = itemCurrentPrices.ItemCurrentBalanceId,
											  StoreId = itemCurrentPrices.StoreId,
											  ItemId = itemCurrentPrices.ItemId,
											  ItemPackageId = itemCurrentPrices.ItemPackageId,
											  ExpireDate = itemCurrentPrices.ExpireDate,
											  BatchNumber = itemCurrentPrices.BatchNumber,
											  //Zero
										  }).ToList()
			};

			await InventoryInOut(parentStoreId, reservationStoreId, oldItemCurrentBalances, newItemCurrentBalances);
		}

		private ReservationStoreAndParentStoreCurrentBalancesDto GetItemCurrentBalancesFromStockOut(int parentStoreId, int reservationStoreId, List<StockOutDetailDto> stockOutDetails)
		{
			var currentBalancesParentStoreId = (from stockOutDetail in stockOutDetails
										  select new ItemCurrentBalanceDto()
										  {
											  StoreId = parentStoreId,
											  ItemId = stockOutDetail.ItemId,
											  ItemPackageId = stockOutDetail.ItemPackageId,
											  ExpireDate = stockOutDetail.ExpireDate,
											  BatchNumber = stockOutDetail.BatchNumber,
											  OutQuantity = stockOutDetail.Quantity + stockOutDetail.BonusQuantity,
											  InQuantity = stockOutDetail.Quantity + stockOutDetail.BonusQuantity
										  }).ToList();

			var currentBalancesReservationStoreId = (from stockOutDetail in stockOutDetails
													 select new ItemCurrentBalanceDto()
													 {
														 StoreId = reservationStoreId,
														 ItemId = stockOutDetail.ItemId,
														 ItemPackageId = stockOutDetail.ItemPackageId,
														 ExpireDate = stockOutDetail.ExpireDate,
														 BatchNumber = stockOutDetail.BatchNumber,
														 OutQuantity = stockOutDetail.Quantity + stockOutDetail.BonusQuantity
													 }).ToList();

			return new ReservationStoreAndParentStoreCurrentBalancesDto { ParentStoreCurrentBalances = currentBalancesParentStoreId, ReservationStoreCurrentBalances = currentBalancesReservationStoreId };
		}

		private async Task InventoryInOut(int parentStoreId, int reservationStoreId, ReservationStoreAndParentStoreCurrentBalancesDto oldItemCurrentBalances, ReservationStoreAndParentStoreCurrentBalancesDto newItemCurrentBalances)
		{
			await _itemCurrentBalanceService.InventoryInOut(parentStoreId, oldItemCurrentBalances.ParentStoreCurrentBalances, newItemCurrentBalances.ParentStoreCurrentBalances);
			await _itemCurrentBalanceService.InventoryInOut(reservationStoreId, oldItemCurrentBalances.ReservationStoreCurrentBalances, newItemCurrentBalances.ReservationStoreCurrentBalances);
		}

		private async Task InventoryOut(int parentStoreId, int reservationStoreId, List<ItemCurrentBalanceDto> parentStoreCurrentBalances, List<ItemCurrentBalanceDto> reservationStoreCurrentBalances)
		{
			var oldItemCurrentBalances = new ReservationStoreAndParentStoreCurrentBalancesDto
			{
				ParentStoreCurrentBalances = (from itemCurrentPrices in parentStoreCurrentBalances
											  select new ItemCurrentBalanceDto()
											  {
												  ItemCurrentBalanceId = itemCurrentPrices.ItemCurrentBalanceId,
												  StoreId = itemCurrentPrices.StoreId,
												  ItemId = itemCurrentPrices.ItemId,
												  ItemPackageId = itemCurrentPrices.ItemPackageId,
												  ExpireDate = itemCurrentPrices.ExpireDate,
												  BatchNumber = itemCurrentPrices.BatchNumber,
												  //Zero
											  }).ToList(),

				ReservationStoreCurrentBalances = (from itemCurrentPrices in reservationStoreCurrentBalances
												   select new ItemCurrentBalanceDto()
												   {
													   ItemCurrentBalanceId = itemCurrentPrices.ItemCurrentBalanceId,
													   StoreId = itemCurrentPrices.StoreId,
													   ItemId = itemCurrentPrices.ItemId,
													   ItemPackageId = itemCurrentPrices.ItemPackageId,
													   ExpireDate = itemCurrentPrices.ExpireDate,
													   BatchNumber = itemCurrentPrices.BatchNumber,
													   //Zero
												   }).ToList()
			};

			await _itemCurrentBalanceService.InventoryInOut(parentStoreId, oldItemCurrentBalances.ParentStoreCurrentBalances, parentStoreCurrentBalances);
			await _itemCurrentBalanceService.InventoryInOut(reservationStoreId, oldItemCurrentBalances.ReservationStoreCurrentBalances, reservationStoreCurrentBalances);
		}

		private async Task<string?> GetStockOutDocumentReference(int stockOutHeaderId, bool hasApprove, int? requestId)
		{
			if (stockOutHeaderId != 0)
			{
				return null;
			}
			else if (hasApprove)
			{
				return $"{DocumentReferenceData.Approval}{requestId}";
			}
			else
			{
				return $"{DocumentReferenceData.StockOutFromReservation}{await _stockOutHeaderService.GetNextId()}";
			}
		}

		private async Task<ResponseDto> SaveRelatedInternalTransferAndReceive(StockOutDto stockOut, int resultStockOutHeaderId, int reservationStoreId, string? documentReference)
		{
			var stockOutHeader = stockOut.StockOutHeader!;

			var internalTransferHeaderId = await _directInternalTransferService.GetInternalTransferIdFromReferenceAndMenuCode(stockOutHeader.StockOutHeaderId, MenuCodeData.StockOutFromReservation);

			var internalTransfer = new InternalTransferDto
			{
				InternalTransferHeader = new InternalTransferHeaderDto
				{
					InternalTransferHeaderId = internalTransferHeaderId,
					DocumentDate = stockOutHeader.DocumentDate,
					FromStoreId = reservationStoreId,
					ToStoreId = stockOutHeader.StoreId,
					Reference = stockOutHeader.Reference,
					TotalConsumerValue = stockOut.StockOutDetails.Sum(x => x.TotalValue),
					TotalCostValue = stockOutHeader.TotalCostValue,
					RemarksAr = stockOutHeader.RemarksAr,
					RemarksEn = stockOutHeader.RemarksEn,
					IsReturned = false,
					ReturnReason = null,
					MenuCode = MenuCodeData.StockOutFromReservation,
					ReferenceId = resultStockOutHeaderId,
					IsClosed = true,
					ArchiveHeaderId = null,
				},
				InternalTransferDetails = (from stockOutDetail in stockOut.StockOutDetails
										   select
										  new InternalTransferDetailDto
										  {
											  InternalTransferDetailId = 0,
											  InternalTransferHeaderId = internalTransferHeaderId,
											  ItemId = stockOutDetail.ItemId,
											  ItemPackageId = stockOutDetail.ItemPackageId,
											  BarCode = stockOutDetail.BarCode,
											  Packing = stockOutDetail.Packing,
											  Quantity = stockOutDetail.Quantity + stockOutDetail.BonusQuantity,
											  ConsumerPrice = stockOutDetail.ConsumerPrice,
											  CostPrice = stockOutDetail.CostPrice,
											  CostValue = stockOutDetail.CostValue,
											  ExpireDate = stockOutDetail.ExpireDate,
											  BatchNumber = stockOutDetail.BatchNumber,
										  }).ToList()
			};

			return await _directInternalTransferService.SaveDirectInternalTransferWithoutUpdatingBalances(internalTransfer, false, true, null, documentReference);
		}

		public async Task<ResponseDto> DeleteStockOutFromReservation(int stockOutHeaderId, int menuCode)
		{
			ResponseDto result;
			var internalTransferHeaderId = await _directInternalTransferService.GetInternalTransferIdFromReferenceAndMenuCode(stockOutHeaderId, menuCode);
			var stockOut = await _stockOutService.GetStockOut(stockOutHeaderId);
			if (stockOut.StockOutHeader == null) return new ResponseDto { Message = await _genericMessageService.GetMessage(menuCode, GenericMessageData.NotFound) };

			var reservationStoreId = await _storeService.GetReservedStoreByParentStoreId(stockOut.StockOutHeader!.StoreId);

			result = await _directInternalTransferService.DeleteDirectInternalTransferWithoutUpdatingBalances(internalTransferHeaderId);
			if (result.Success == false) return result;

			result = await _stockOutHandlingService.DeleteStockOut(stockOutHeaderId, menuCode, false);
			if (result.Success == false) return result;

			await DeductCurrentBalances(stockOut.StockOutHeader.StoreId, reservationStoreId, stockOut.StockOutDetails);

			return result;
		}
	}
}
