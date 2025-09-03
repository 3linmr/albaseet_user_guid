using Compound.CoreOne.Contracts.Reservation;
using Inventory.CoreOne.Contracts;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Sales.CoreOne.Models.StaticData;
using Sales.Service.Services;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.Service.Services.Reservation
{
	public class ReservationInvoiceCloseOutService: IReservationInvoiceCloseOutService
	{
		private readonly ISalesInvoiceReturnHandlingService _salesInvoiceReturnHandlingService;
		private readonly IDirectInternalTransferService _directInternalTransferService;
		private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
		private readonly IStoreService _storeService;

		public ReservationInvoiceCloseOutService(ISalesInvoiceReturnHandlingService salesInvoiceReturnHandlingService, IDirectInternalTransferService directInternalTransferService, IInternalTransferReceiveService internalTransferReceiveService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IStoreService storeService)
		{
			_salesInvoiceReturnHandlingService = salesInvoiceReturnHandlingService;
			_directInternalTransferService = directInternalTransferService;
			_salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
			_storeService = storeService;
		}

		public async Task<ResponseDto> DeleteReservationInvoiceCloseOut(int salesInvoiceReturnHeaderId, int menuCode)
		{
			ResponseDto result;
			var internalTransferHeaderId = await _directInternalTransferService.GetInternalTransferIdFromReferenceAndMenuCode(salesInvoiceReturnHeaderId, menuCode);
			
			result = await _directInternalTransferService.DeleteDirectInternalTransfer(internalTransferHeaderId);
			if(result.Success == false) return result;

			return await _salesInvoiceReturnHandlingService.DeleteSalesInvoiceReturn(salesInvoiceReturnHeaderId, menuCode);
		}

		public async Task<ResponseDto> SaveReservationInvoiceCloseOut(SalesInvoiceReturnDto salesInvoiceReturn, bool hasApprove, bool isApproved, int? requestId)
		{
			var documentReference = await GetDirectSalesInvoiceReturnDocumentReference(salesInvoiceReturn.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId, hasApprove, requestId, salesInvoiceReturn.SalesInvoiceReturnHeader!.IsOnTheWay, salesInvoiceReturn.SalesInvoiceReturnHeader!.CreditPayment);
			var reservationStoreId = await _storeService.GetReservedStoreByParentStoreId(salesInvoiceReturn.SalesInvoiceReturnHeader!.StoreId);
			var menuCode = SalesInvoiceReturnMenuCodeHelper.GetMenuCode(salesInvoiceReturn.SalesInvoiceReturnHeader);

			var salesInvoiceReturnResult = await _salesInvoiceReturnHandlingService.SaveSalesInvoiceReturn(salesInvoiceReturn, hasApprove, isApproved, requestId, documentReference);
			if (salesInvoiceReturnResult.Success == false) return salesInvoiceReturnResult;

			var internalTransferResult = await SaveRelatedInternalTransferAndReceive(salesInvoiceReturn, salesInvoiceReturnResult.Id, reservationStoreId, menuCode, documentReference);
			if (internalTransferResult.Success == false) return internalTransferResult;

			return salesInvoiceReturnResult;
		}

		private async Task<ResponseDto> SaveRelatedInternalTransferAndReceive(SalesInvoiceReturnDto salesInvoiceReturn, int resultSalesInvoiceReturnHeaderId, int reservationStoreId, int menuCode, string? documentReference) {
			var salesInvoiceReturnHeader = salesInvoiceReturn.SalesInvoiceReturnHeader!;

			var internalTransferHeaderId = await _directInternalTransferService.GetInternalTransferIdFromReferenceAndMenuCode(salesInvoiceReturnHeader.SalesInvoiceReturnHeaderId, menuCode);

			var internalTransfer = new InternalTransferDto
			{
				InternalTransferHeader = new InternalTransferHeaderDto
				{
					InternalTransferHeaderId = internalTransferHeaderId,
					DocumentDate = salesInvoiceReturnHeader.DocumentDate,
					FromStoreId = reservationStoreId,
					ToStoreId = salesInvoiceReturnHeader.StoreId,
					Reference = salesInvoiceReturnHeader.Reference,
					TotalConsumerValue = salesInvoiceReturn.SalesInvoiceReturnDetails.Sum(x => x.TotalValue),
					TotalCostValue = salesInvoiceReturnHeader.TotalCostValue,
					RemarksAr = salesInvoiceReturnHeader.RemarksAr,
					RemarksEn = salesInvoiceReturnHeader.RemarksEn,
					IsReturned = false,
					ReturnReason = null,
					MenuCode = (short)menuCode,
					ReferenceId = resultSalesInvoiceReturnHeaderId,
					IsClosed = true,
					ArchiveHeaderId = null,
				},
				InternalTransferDetails = (from salesInvoiceReturnDetail in salesInvoiceReturn.SalesInvoiceReturnDetails select
										  new InternalTransferDetailDto
										  {
											  InternalTransferDetailId = 0,
											  InternalTransferHeaderId = internalTransferHeaderId,
											  ItemId = salesInvoiceReturnDetail.ItemId,
											  ItemPackageId = salesInvoiceReturnDetail.ItemPackageId,
											  BarCode = salesInvoiceReturnDetail.BarCode,
											  Packing = salesInvoiceReturnDetail.Packing,
											  Quantity = salesInvoiceReturnDetail.Quantity + salesInvoiceReturnDetail.BonusQuantity,
											  ConsumerPrice = salesInvoiceReturnDetail.ConsumerPrice,
											  CostPrice = salesInvoiceReturnDetail.CostPrice,
											  CostValue = salesInvoiceReturnDetail.CostValue,
											  ExpireDate = salesInvoiceReturnDetail.ExpireDate,
											  BatchNumber = salesInvoiceReturnDetail.BatchNumber,
										  }).ToList()
			};

			return await _directInternalTransferService.SaveDirectInternalTransfer(internalTransfer, false, true, null, documentReference);
		}

		public async Task<string?> GetDirectSalesInvoiceReturnDocumentReference(int salesInvoiceReturnHeaderId, bool hasApprove, int? requestId, bool isOnTheWay, bool isCreditPayment)
		{
			if (salesInvoiceReturnHeaderId != 0)
			{
				return null;
			}
			else if (hasApprove)
			{
				return $"{DocumentReferenceData.Approval}{requestId}";
			}
			else
			{
				return $"{SalesInvoiceReturnMenuCodeHelper.GetDocumentReference(isOnTheWay, true, isCreditPayment)}{await _salesInvoiceReturnHeaderService.GetNextId()}";
			}
		}
	}
}
