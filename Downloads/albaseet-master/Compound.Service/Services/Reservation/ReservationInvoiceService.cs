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
	public class ReservationInvoiceService: IReservationInvoiceService
	{
		private readonly ISalesInvoiceHandlingService _salesInvoiceHandlingService;
		private readonly IDirectInternalTransferService _directInternalTransferService;
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly IStoreService _storeService;

		public ReservationInvoiceService(ISalesInvoiceHandlingService salesInvoiceHandlingService, IDirectInternalTransferService directInternalTransferService, IInternalTransferReceiveService internalTransferReceiveService, ISalesInvoiceHeaderService salesInvoiceHeaderService, IStoreService storeService)
		{
			_salesInvoiceHandlingService = salesInvoiceHandlingService;
			_directInternalTransferService = directInternalTransferService;
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_storeService = storeService;
		}

		public async Task<ResponseDto> DeleteReservationInvoice(int salesInvoiceHeaderId, int menuCode)
		{
			ResponseDto result;
			var internalTransferHeaderId = await _directInternalTransferService.GetInternalTransferIdFromReferenceAndMenuCode(salesInvoiceHeaderId, menuCode);
			
			result = await _directInternalTransferService.DeleteDirectInternalTransfer(internalTransferHeaderId);
			if(result.Success == false) return result;

			return await _salesInvoiceHandlingService.DeleteSalesInvoice(salesInvoiceHeaderId, menuCode);
		}

		public async Task<ResponseDto> SaveReservationInvoice(SalesInvoiceDto salesInvoice, bool hasApprove, bool isApproved, int? requestId)
		{
			var documentReference = await GetDirectSalesInvoiceDocumentReference(salesInvoice.SalesInvoiceHeader!.SalesInvoiceHeaderId, hasApprove, requestId, salesInvoice.SalesInvoiceHeader!.IsOnTheWay, salesInvoice.SalesInvoiceHeader!.CreditPayment);
			var reservationStoreId = await _storeService.GetReservedStoreByParentStoreId(salesInvoice.SalesInvoiceHeader!.StoreId);
			var menuCode = SalesInvoiceMenuCodeHelper.GetMenuCode(salesInvoice.SalesInvoiceHeader);

			var salesInvoiceResult = await _salesInvoiceHandlingService.SaveSalesInvoice(salesInvoice, hasApprove, isApproved, requestId, documentReference);
			if (salesInvoiceResult.Success == false) return salesInvoiceResult;

			var internalTransferResult = await SaveRelatedInternalTransferAndReceive(salesInvoice, salesInvoiceResult.Id, reservationStoreId, menuCode, documentReference);
			if (internalTransferResult.Success == false) return internalTransferResult;

			return salesInvoiceResult;
		}

		private async Task<ResponseDto> SaveRelatedInternalTransferAndReceive(SalesInvoiceDto salesInvoice, int resultSalesInvoiceHeaderId, int reservationStoreId, int menuCode, string? documentReference) {
			var salesInvoiceHeader = salesInvoice.SalesInvoiceHeader!;

			var internalTransferHeaderId = await _directInternalTransferService.GetInternalTransferIdFromReferenceAndMenuCode(salesInvoiceHeader.SalesInvoiceHeaderId, menuCode);

			var internalTransfer = new InternalTransferDto
			{
				InternalTransferHeader = new InternalTransferHeaderDto
				{
					InternalTransferHeaderId = internalTransferHeaderId,
					DocumentDate = salesInvoiceHeader.DocumentDate,
					FromStoreId = salesInvoiceHeader.StoreId,
					ToStoreId = reservationStoreId,
					Reference = salesInvoiceHeader.Reference,
					TotalConsumerValue = salesInvoice.SalesInvoiceDetails.Sum(x => x.TotalValue),
					TotalCostValue = salesInvoiceHeader.TotalCostValue,
					RemarksAr = salesInvoiceHeader.RemarksAr,
					RemarksEn = salesInvoiceHeader.RemarksEn,
					IsReturned = false,
					ReturnReason = null,
					MenuCode = (short)menuCode,
					ReferenceId = resultSalesInvoiceHeaderId,
					IsClosed = true,
					ArchiveHeaderId = null,
				},
				InternalTransferDetails = (from salesInvoiceDetail in salesInvoice.SalesInvoiceDetails select
										  new InternalTransferDetailDto
										  {
											  InternalTransferDetailId = 0,
											  InternalTransferHeaderId = internalTransferHeaderId,
											  ItemId = salesInvoiceDetail.ItemId,
											  ItemPackageId = salesInvoiceDetail.ItemPackageId,
											  BarCode = salesInvoiceDetail.BarCode,
											  Packing = salesInvoiceDetail.Packing,
											  Quantity = salesInvoiceDetail.Quantity + salesInvoiceDetail.BonusQuantity,
											  ConsumerPrice = salesInvoiceDetail.ConsumerPrice,
											  CostPrice = salesInvoiceDetail.CostPrice,
											  CostValue = salesInvoiceDetail.CostValue,
											  ExpireDate = salesInvoiceDetail.ExpireDate,
											  BatchNumber = salesInvoiceDetail.BatchNumber,
										  }).ToList()
			};

			return await _directInternalTransferService.SaveDirectInternalTransfer(internalTransfer, false, true, null, documentReference);
		}

		public async Task<string?> GetDirectSalesInvoiceDocumentReference(int salesInvoiceHeaderId, bool hasApprove, int? requestId, bool isOnTheWay, bool isCreditPayment)
		{
			if (salesInvoiceHeaderId != 0)
			{
				return null;
			}
			else if (hasApprove)
			{
				return $"{DocumentReferenceData.Approval}{requestId}";
			}
			else
			{
				return $"{SalesInvoiceMenuCodeHelper.GetDocumentReference(isOnTheWay, true, isCreditPayment)}{await _salesInvoiceHeaderService.GetNextId()}";
			}
		}
	}
}
