using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Compound.CoreOne.Contracts.Approval;
using Inventory.CoreOne.Contracts;
using Inventory.CoreOne.Models.Dtos.ViewModels;
using Purchases.CoreOne.Contracts;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.CostCenters;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.Helper.Extensions;
using Compound.CoreOne.Contracts.Reservation;
using Compound.CoreOne.Contracts.InvoiceSettlement;

namespace Compound.Service.Services.Approval
{
	public class RedirectApproveRequestService : IRedirectApproveRequestService
	{
		private readonly IItemService _itemService;
		private readonly IAccountEntityService _accountEntityService;
		private readonly ICostCenterService _costCenterService;
		private readonly IJournalService _journalService;
		private readonly ISalesInvoiceSettlementService _salesInvoiceSettlementService;
		private readonly IPurchaseInvoiceSettlementService _purchaseInvoiceSettlementService;
		private readonly IStockTakingService _stockTakingService;
		private readonly IInventoryInService _inventoryInService;
		private readonly IInventoryOutService _inventoryOutService;
		private readonly IInternalTransferReceiveService _internalTransferReceiveService;
		private readonly IInternalTransferService _internalTransferService;
		private readonly IStockTakingCarryOverService _stockTakingCarryOverService;
		private readonly IProductRequestService _productRequestService;
		private readonly IProductRequestPriceService _productRequestPriceService;
		private readonly ISupplierQuotationService _supplierQuotationService;
		private readonly IPurchaseOrderService _purchaseOrderService;
		private readonly IStringLocalizer<RedirectApproveRequestService> _localizer;
        private readonly IStockInHandlingService _stockInHandlingService;
		private readonly IStockInReturnHandlingService _stockInReturnHandlingService;
		private readonly IPurchaseInvoiceHandlingService _purchaseInvoiceHandlingService;
		private readonly IPurchaseInvoiceReturnHandlingService _purchaseInvoiceReturnHandlingService;
        private readonly ISupplierDebitMemoHandlingService _supplierDebitMemoHandlingService;
        private readonly ISupplierCreditMemoHandlingService _supplierCreditMemoHandlingService;
        private readonly IClientPriceRequestService _clientPriceRequestService;
        private readonly IClientQuotationService _clientQuotationService;
        private readonly IClientQuotationApprovalService _clientQuotationApprovalService;
        private readonly IProformaInvoiceService _proformaInvoiceService;
        private readonly IStockOutHandlingService _stockOutHandlingService;
        private readonly IStockOutReturnHandlingService _stockOutReturnHandlingService;
        private readonly ISalesInvoiceHandlingService _salesInvoiceHandlingService;
        private readonly ISalesInvoiceReturnHandlingService _salesInvoiceReturnHandlingService;
        private readonly IReservationInvoiceService _reservationInvoiceService;
        private readonly IStockOutFromReservationService _stockOutFromReservationService;
        private readonly IStockOutReturnFromReservationService _stockOutReturnFromReservationService;
        private readonly IReservationInvoiceCloseOutService _reservationInvoiceCloseOutService;
        private readonly IClientCreditMemoHandlingService _clientCreditMemoHandlingService;
        private readonly IClientDebitMemoHandlingService _clientDebitMemoHandlingService;

		public RedirectApproveRequestService(IStringLocalizer<RedirectApproveRequestService> localizer,IItemService itemService,IAccountEntityService accountEntityService,ICostCenterService costCenterService,IJournalService journalService,ISalesInvoiceSettlementService salesInvoiceSettlementService,IPurchaseInvoiceSettlementService purchaseInvoiceSettlementService,IStockTakingService stockTakingService, IStockTakingCarryOverService stockTakingCarryOverService, IInventoryInService inventoryInService, IInventoryOutService inventoryOutService, IInternalTransferReceiveService internalTransferReceiveService, IInternalTransferService internalTransferService, IProductRequestService productRequestService, IProductRequestPriceService productRequestPriceService, ISupplierQuotationService supplierQuotationService, IPurchaseOrderService purchaseOrderService, IStockInHandlingService stockInHandlingService, IStockInReturnHandlingService stockInReturnHandlingService, IPurchaseInvoiceHandlingService purchaseInvoiceHandlingService, IPurchaseInvoiceReturnHandlingService purchaseInvoiceReturnHandlingService, ISupplierDebitMemoHandlingService supplierDebitMemoService, ISupplierCreditMemoHandlingService supplierCreditMemoHandlingService, IClientPriceRequestService clientPriceRequestService, IClientQuotationService clientQuotationService, IClientQuotationApprovalService clientQuotationApprovalService, IProformaInvoiceService proformaInvoiceService, IStockOutHandlingService stockOutHandlingService, IStockOutReturnHandlingService stockOutReturnHandlingService, ISalesInvoiceHandlingService salesInvoiceHandlingService, ISalesInvoiceReturnHandlingService salesInvoiceReturnHandlingService, IReservationInvoiceService reservationInvoiceService, IStockOutFromReservationService stockOutFromReservationService, IStockOutReturnFromReservationService stockOutReturnFromReservationService, IReservationInvoiceCloseOutService reservationInvoiceCloseOutService, IClientCreditMemoHandlingService clientCreditMemoHandlingService, IClientDebitMemoHandlingService clientDebitMemoHandlingService)
		{
			_itemService = itemService;
			_accountEntityService = accountEntityService;
			_costCenterService = costCenterService;
			_journalService = journalService;
			_salesInvoiceSettlementService = salesInvoiceSettlementService;
			_purchaseInvoiceSettlementService = purchaseInvoiceSettlementService;
			_stockTakingService = stockTakingService;
			_stockTakingCarryOverService = stockTakingCarryOverService;
			_inventoryInService = inventoryInService;
			_inventoryOutService = inventoryOutService;
			_internalTransferReceiveService = internalTransferReceiveService;
			_internalTransferService = internalTransferService;
			_localizer = localizer;
			_productRequestService = productRequestService;
			_productRequestPriceService = productRequestPriceService;
			_supplierQuotationService = supplierQuotationService;
			_purchaseOrderService = purchaseOrderService;
            _stockInHandlingService = stockInHandlingService;
			_stockInReturnHandlingService = stockInReturnHandlingService;
			_purchaseInvoiceHandlingService = purchaseInvoiceHandlingService;
			_purchaseInvoiceReturnHandlingService = purchaseInvoiceReturnHandlingService;
            _supplierDebitMemoHandlingService = supplierDebitMemoService;
            _supplierCreditMemoHandlingService = supplierCreditMemoHandlingService;
            _clientPriceRequestService = clientPriceRequestService;
            _clientQuotationService = clientQuotationService;
            _clientQuotationApprovalService = clientQuotationApprovalService;
            _proformaInvoiceService = proformaInvoiceService;
            _stockOutHandlingService = stockOutHandlingService;
            _stockOutReturnHandlingService = stockOutReturnHandlingService;
            _salesInvoiceHandlingService = salesInvoiceHandlingService;
            _salesInvoiceReturnHandlingService = salesInvoiceReturnHandlingService;
            _reservationInvoiceService = reservationInvoiceService;
            _stockOutFromReservationService = stockOutFromReservationService;
            _stockOutReturnFromReservationService = stockOutReturnFromReservationService;
            _reservationInvoiceCloseOutService = reservationInvoiceCloseOutService;
            _clientCreditMemoHandlingService = clientCreditMemoHandlingService;
            _clientDebitMemoHandlingService = clientDebitMemoHandlingService;
		}

		public async Task<ResponseDto> RedirectApproveRequest(ApproveResponseDto request)
		{
			return request.MenuCode switch
			{
				MenuCodeData.Item => await HandleItem(request),
				MenuCodeData.Account => await HandleAccount(request),
				MenuCodeData.CostCenter => await HandleCostCenter(request),
				MenuCodeData.JournalEntry => await HandleJournalEntry(request),
				MenuCodeData.ReceiptVoucher => await HandleReceiveVoucher(request),
				MenuCodeData.PaymentVoucher => await HandlePaymentVoucher(request),
				MenuCodeData.StockTakingOpenBalance => await HandleStockTaking(request),
				MenuCodeData.StockTakingCurrentBalance => await HandleStockTaking(request),
				MenuCodeData.InventoryIn => await HandleInventoryIn(request),
				MenuCodeData.InventoryOut => await HandleInventoryOut(request),
				MenuCodeData.InternalReceiveItems => await HandleInternalTransferReceive(request),
				MenuCodeData.InternalTransferItems => await HandleInternalTransfer(request),
				MenuCodeData.ApprovalStockTakingAsOpenBalance => await HandleStockTakingCarryOver(request),
				MenuCodeData.ApprovalStockTakingAsCurrentBalance => await HandleStockTakingCarryOver(request),
				MenuCodeData.ProductRequest => await HandleProductRequest(request),
				MenuCodeData.ProductRequestPrice => await HandleProductRequestPrice(request),
				MenuCodeData.SupplierQuotation => await HandleSupplierQuotation(request),
				MenuCodeData.PurchaseOrder => await HandlePurchaseOrder(request),
				MenuCodeData.ReceiptStatement => await HandleStockIn(request, request.MenuCode),
				MenuCodeData.ReceiptFromPurchaseInvoiceOnTheWay => await HandleStockIn(request, request.MenuCode),
				MenuCodeData.ReceiptStatementReturn => await HandleStockInReturn(request, request.MenuCode),
				MenuCodeData.ReceiptFromPurchaseInvoiceOnTheWayReturn => await HandleStockInReturn(request, request.MenuCode),
				MenuCodeData.ReturnFromPurchaseInvoice => await HandleStockInReturn(request, request.MenuCode),
				MenuCodeData.PurchaseInvoiceInterim => await HandlePurchaseInvoice(request, request.MenuCode),
                MenuCodeData.CashPurchaseInvoice => await HandlePurchaseInvoice(request, request.MenuCode),
                MenuCodeData.CreditPurchaseInvoice => await HandlePurchaseInvoice(request, request.MenuCode),
                MenuCodeData.PurchaseInvoiceOnTheWayCash => await HandlePurchaseInvoice(request, request.MenuCode),
                MenuCodeData.PurchaseInvoiceOnTheWayCredit => await HandlePurchaseInvoice(request, request.MenuCode),
                MenuCodeData.PurchaseInvoiceReturn => await HandlePurchaseInvoiceReturn(request, request.MenuCode),
                MenuCodeData.CashPurchaseInvoiceReturn => await HandlePurchaseInvoiceReturn(request, request.MenuCode),
                MenuCodeData.CreditPurchaseInvoiceReturn => await HandlePurchaseInvoiceReturn(request, request.MenuCode),
                MenuCodeData.PurchaseInvoiceReturnOnTheWay => await HandlePurchaseInvoiceReturn(request, request.MenuCode),
                MenuCodeData.SupplierDebitMemo => await HandleSupplierDebitMemo(request),
                MenuCodeData.SupplierCreditMemo => await HandleSupplierCreditMemo(request),
                MenuCodeData.ClientPriceRequest => await HandleClientPriceRequest(request),
                MenuCodeData.ClientQuotation => await HandleClientQuotation(request),
				MenuCodeData.ClientQuotationApproval => await HandleClientQuotationApproval(request),
				MenuCodeData.ProformaInvoice => await HandleProformaInvoice(request),
                MenuCodeData.StockOutFromProformaInvoice => await HandleStockOut(request, request.MenuCode),
                MenuCodeData.StockOutFromReservation => await HandleStockOutFromReservation(request, request.MenuCode),
                MenuCodeData.StockOutReturnFromReservation => await HandleStockOutReturnFromReservation(request, request.MenuCode),
                MenuCodeData.StockOutReturnFromStockOut => await HandleStockOutReturn(request, request.MenuCode),
                MenuCodeData.StockOutReturnFromInvoice => await HandleStockOutReturn(request, request.MenuCode),
                MenuCodeData.SalesInvoiceInterim => await HandleSalesInvoice(request, request.MenuCode),
                MenuCodeData.CashSalesInvoice => await HandleSalesInvoice(request, request.MenuCode),
                MenuCodeData.CreditSalesInvoice => await HandleSalesInvoice(request, request.MenuCode),
                MenuCodeData.CashReservationInvoice => await HandleReservationInvoice(request, request.MenuCode),
                MenuCodeData.CreditReservationInvoice => await HandleReservationInvoice(request, request.MenuCode),
				MenuCodeData.SalesInvoiceReturn => await HandleSalesInvoiceReturn(request, request.MenuCode),
				MenuCodeData.CashSalesInvoiceReturn => await HandleSalesInvoiceReturn(request, request.MenuCode),
				MenuCodeData.CreditSalesInvoiceReturn => await HandleSalesInvoiceReturn(request, request.MenuCode),
				MenuCodeData.ReservationInvoiceCloseOut => await HandleReservationInvoiceCloseOut(request, request.MenuCode),
                MenuCodeData.ClientCreditMemo => await HandleClientCreditMemo(request),
                MenuCodeData.ClientDebitMemo => await HandleClientDebitMemo(request),
				_ => new ResponseDto() { Success = false, Message = _localizer["ApproveIsNotSupported"] }
			};
		}

		public async Task<ResponseDto> HandleItem(ApproveResponseDto request)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if ( data.CanBeConverted<ItemVm>())
				{
					var sendModel = data.ConvertToType<ItemVm>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _itemService.DeleteItemInFull(sendModel.Item.ItemId);
						}
						else
						{
							return await _itemService.SaveItemInFull(sendModel);
						}
					}
				}
			}
			return new ResponseDto(){Success = false,Message = _localizer["ApproveModelIsEmpty"] };
		}

		public async Task<ResponseDto> HandleAccount(ApproveResponseDto request)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<AccountDto>())
				{
					var sendModel = data.ConvertToType<AccountDto>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _accountEntityService.DeleteAccount(sendModel.AccountId);
						}
						else
						{
							return await _accountEntityService.SaveAccount(sendModel);
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
		}

		public async Task<ResponseDto> HandleCostCenter(ApproveResponseDto request)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<CostCenterDto>())
				{
					var sendModel = data.ConvertToType<CostCenterDto>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _costCenterService.DeleteCostCenterInFull(sendModel.CostCenterId);
						}
						else
						{
							return await _costCenterService.SaveCostCenterInFull(sendModel);
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
		}
		public async Task<ResponseDto> HandleJournalEntry(ApproveResponseDto request)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<JournalDto>())
				{
					var sendModel = data.ConvertToType<JournalDto>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _journalService.DeleteJournal(sendModel.JournalHeader!.JournalHeaderId);
						}
						else
						{
							return await _journalService.SaveJournal(sendModel,true,true,request.RequestId);
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
		}

		public async Task<ResponseDto> HandleReceiveVoucher(ApproveResponseDto request)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<ReceiptVoucherDto>())
				{
					var sendModel = data.ConvertToType<ReceiptVoucherDto>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _salesInvoiceSettlementService.DeleteReceiptVoucherWithInvoiceSettlements(sendModel.ReceiptVoucherHeader!.ReceiptVoucherHeaderId);
						}
						else
						{
							return await _salesInvoiceSettlementService.SaveReceiptVoucherWithInvoiceSettlements(sendModel, true, true, request.RequestId);
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
		}

		public async Task<ResponseDto> HandlePaymentVoucher(ApproveResponseDto request)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<PaymentVoucherDto>())
				{
					var sendModel = data.ConvertToType<PaymentVoucherDto>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _purchaseInvoiceSettlementService.DeletePaymentVoucherWithInvoiceSettlements(sendModel.PaymentVoucherHeader!.PaymentVoucherHeaderId);
						}
						else
						{
							return await _purchaseInvoiceSettlementService.SavePaymentVoucherWithInvoiceSettlements(sendModel, true, true, request.RequestId);
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
		}

		public async Task<ResponseDto> HandleStockTaking(ApproveResponseDto request)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<StockTakingDto>())
				{
					var sendModel = data.ConvertToType<StockTakingDto>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _stockTakingService.DeleteStockTaking(sendModel.StockTakingHeader!.StockTakingHeaderId);
						}
						else
						{
							return await _stockTakingService.SaveStockTaking(sendModel, true, true, request.RequestId);
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
		}

        public async Task<ResponseDto> HandleInventoryIn(ApproveResponseDto request)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<InventoryInDto>())
                {
                    var sendModel = data.ConvertToType<InventoryInDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _inventoryInService.DeleteInventoryIn(sendModel.InventoryInHeader!.InventoryInHeaderId);
                        }
                        else
                        {
                            return await _inventoryInService.SaveInventoryIn(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

        public async Task<ResponseDto> HandleInventoryOut(ApproveResponseDto request)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<InventoryOutDto>())
                {
                    var sendModel = data.ConvertToType<InventoryOutDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _inventoryOutService.DeleteInventoryOut(sendModel.InventoryOutHeader!.InventoryOutHeaderId);
                        }
                        else
                        {
                            return await _inventoryOutService.SaveInventoryOut(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

        public async Task<ResponseDto> HandleInternalTransferReceive(ApproveResponseDto request)
        {
            var data = request.UserRequest;
            if (data != null && request.ApproveRequestTypeId == ApproveRequestTypeData.Add)
            {
                if (data.CanBeConverted<InternalTransferReceiveDto>())
                {
                    var sendModel = data.ConvertToType<InternalTransferReceiveDto>();
                    if (sendModel != null)
                    {
						return await _internalTransferReceiveService.SaveInternalTransferReceive(sendModel, true, true, request.RequestId);
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

        public async Task<ResponseDto> HandleInternalTransfer(ApproveResponseDto request)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<InternalTransferDto>())
                {
                    var sendModel = data.ConvertToType<InternalTransferDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _internalTransferService.DeleteInternalTransfer(sendModel.InternalTransferHeader!.InternalTransferHeaderId);
                        }
                        else
                        {
                            return await _internalTransferService.SaveInternalTransfer(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

		public async Task<ResponseDto> HandleStockTakingCarryOver(ApproveResponseDto request)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<StockTakingCarryOverDto>())
				{
					var sendModel = data.ConvertToType<StockTakingCarryOverDto>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _stockTakingCarryOverService.DeleteStockTaking(sendModel.StockTakingCarryOverHeader!.StockTakingCarryOverHeaderId);
						}
						else
						{
							return await _stockTakingCarryOverService.SaveStockTaking(sendModel, true, true, request.RequestId);
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

        public async Task<ResponseDto> HandleProductRequest(ApproveResponseDto request)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<ProductRequestDto>())
                {
                    var sendModel = data.ConvertToType<ProductRequestDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _productRequestService.DeleteProductRequest(sendModel.ProductRequestHeader!.ProductRequestHeaderId);
                        }
                        else
                        {
                            return await _productRequestService.SaveProductRequest(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

        public async Task<ResponseDto> HandleProductRequestPrice(ApproveResponseDto request)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<ProductRequestPriceDto>())
                {
                    var sendModel = data.ConvertToType<ProductRequestPriceDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _productRequestPriceService.DeleteProductRequestPrice(sendModel.ProductRequestPriceHeader!.ProductRequestPriceHeaderId);
                        }
                        else
                        {
                            return await _productRequestPriceService.SaveProductRequestPrice(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

        public async Task<ResponseDto> HandleSupplierQuotation(ApproveResponseDto request)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<SupplierQuotationDto>())
                {
                    var sendModel = data.ConvertToType<SupplierQuotationDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _supplierQuotationService.DeleteSupplierQuotation(sendModel.SupplierQuotationHeader!.SupplierQuotationHeaderId);
                        }
                        else
                        {
                            return await _supplierQuotationService.SaveSupplierQuotation(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

        public async Task<ResponseDto> HandlePurchaseOrder(ApproveResponseDto request)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<PurchaseOrderDto>())
                {
                    var sendModel = data.ConvertToType<PurchaseOrderDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _purchaseOrderService.DeletePurchaseOrder(sendModel.PurchaseOrderHeader!.PurchaseOrderHeaderId);
                        }
                        else
                        {
                            return await _purchaseOrderService.SavePurchaseOrder(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

        public async Task<ResponseDto> HandleStockIn(ApproveResponseDto request, int menuCode)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<StockInDto>())
                {
                    var sendModel = data.ConvertToType<StockInDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _stockInHandlingService.DeleteStockIn(sendModel.StockInHeader!.StockInHeaderId, menuCode);
                        }
                        else
                        {
                            return await _stockInHandlingService.SaveStockIn(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

        public async Task<ResponseDto> HandleStockInReturn(ApproveResponseDto request, int menuCode)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<StockInReturnDto>())
                {
                    var sendModel = data.ConvertToType<StockInReturnDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _stockInReturnHandlingService.DeleteStockInReturn(sendModel.StockInReturnHeader!.StockInReturnHeaderId, menuCode);
                        }
                        else
                        {
                            return await _stockInReturnHandlingService.SaveStockInReturn(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

        public async Task<ResponseDto> HandlePurchaseInvoice(ApproveResponseDto request, int menuCode)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<PurchaseInvoiceDto>())
                {
                    var sendModel = data.ConvertToType<PurchaseInvoiceDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _purchaseInvoiceHandlingService.DeletePurchaseInvoice(sendModel.PurchaseInvoiceHeader!.PurchaseInvoiceHeaderId, menuCode);
                        }
                        else
                        {
                            return await _purchaseInvoiceHandlingService.SavePurchaseInvoice(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };

        }

        public async Task<ResponseDto> HandlePurchaseInvoiceReturn(ApproveResponseDto request, int menuCode)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<PurchaseInvoiceReturnDto>())
                {
                    var sendModel = data.ConvertToType<PurchaseInvoiceReturnDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _purchaseInvoiceReturnHandlingService.DeletePurchaseInvoiceReturn(sendModel.PurchaseInvoiceReturnHeader!.PurchaseInvoiceReturnHeaderId, menuCode);
                        }
                        else
                        {
                            return await _purchaseInvoiceReturnHandlingService.SavePurchaseInvoiceReturn(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

        public async Task<ResponseDto> HandleSupplierDebitMemo(ApproveResponseDto request)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<SupplierDebitMemoVm>())
                {
                    var sendModel = data.ConvertToType<SupplierDebitMemoVm>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _supplierDebitMemoHandlingService.DeleteSupplierDebitMemoInFull(sendModel.SupplierDebitMemo!.SupplierDebitMemoId);
                        }
                        else
                        {
                            return await _supplierDebitMemoHandlingService.SaveSupplierDebitMemoInFull(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

        public async Task<ResponseDto> HandleSupplierCreditMemo(ApproveResponseDto request)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<SupplierCreditMemoVm>())
                {
                    var sendModel = data.ConvertToType<SupplierCreditMemoVm>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _supplierCreditMemoHandlingService.DeleteSupplierCreditMemoInFull(sendModel.SupplierCreditMemo!.SupplierCreditMemoId);
                        }
                        else
                        {
                            return await _supplierCreditMemoHandlingService.SaveSupplierCreditMemoInFull(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

        public async Task<ResponseDto> HandleClientPriceRequest(ApproveResponseDto request)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<ClientPriceRequestDto>())
                {
                    var sendModel = data.ConvertToType<ClientPriceRequestDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _clientPriceRequestService.DeleteClientPriceRequest(sendModel.ClientPriceRequestHeader!.ClientPriceRequestHeaderId);
                        }
                        else
                        {
                            return await _clientPriceRequestService.SaveClientPriceRequest(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

        public async Task<ResponseDto> HandleClientQuotation(ApproveResponseDto request)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<ClientQuotationDto>())
                {
                    var sendModel = data.ConvertToType<ClientQuotationDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _clientQuotationService.DeleteClientQuotation(sendModel.ClientQuotationHeader!.ClientQuotationHeaderId);
                        }
                        else
                        {
                            return await _clientQuotationService.SaveClientQuotation(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

		public async Task<ResponseDto> HandleClientQuotationApproval(ApproveResponseDto request)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<ClientQuotationApprovalDto>())
				{
					var sendModel = data.ConvertToType<ClientQuotationApprovalDto>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _clientQuotationApprovalService.DeleteClientQuotationApproval(sendModel.ClientQuotationApprovalHeader!.ClientQuotationApprovalHeaderId);
						}
						else
						{
							return await _clientQuotationApprovalService.SaveClientQuotationApproval(sendModel, true, true, request.RequestId);
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
		}

		public async Task<ResponseDto> HandleProformaInvoice(ApproveResponseDto request)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<ProformaInvoiceDto>())
                {
                    var sendModel = data.ConvertToType<ProformaInvoiceDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _proformaInvoiceService.DeleteProformaInvoice(sendModel.ProformaInvoiceHeader!.ProformaInvoiceHeaderId);
                        }
                        else
                        {
                            return await _proformaInvoiceService.SaveProformaInvoice(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

        public async Task<ResponseDto> HandleStockOut(ApproveResponseDto request, int menuCode)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<StockOutDto>())
                {
                    var sendModel = data.ConvertToType<StockOutDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _stockOutHandlingService.DeleteStockOut(sendModel.StockOutHeader!.StockOutHeaderId, menuCode);
                        }
                        else
                        {
                            return await _stockOutHandlingService.SaveStockOut(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

		public async Task<ResponseDto> HandleStockOutFromReservation(ApproveResponseDto request, int menuCode)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<StockOutDto>())
				{
					var sendModel = data.ConvertToType<StockOutDto>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _stockOutFromReservationService.DeleteStockOutFromReservation(sendModel.StockOutHeader!.StockOutHeaderId, menuCode);
						}
						else
						{
							return await _stockOutFromReservationService.SaveStockOutFromReservation(sendModel, true, true, request.RequestId);
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
		}

		public async Task<ResponseDto> HandleStockOutReturn(ApproveResponseDto request, int menuCode)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<StockOutReturnDto>())
                {
                    var sendModel = data.ConvertToType<StockOutReturnDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _stockOutReturnHandlingService.DeleteStockOutReturn(sendModel.StockOutReturnHeader!.StockOutReturnHeaderId, menuCode);
                        }
                        else
                        {
                            return await _stockOutReturnHandlingService.SaveStockOutReturn(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

		public async Task<ResponseDto> HandleStockOutReturnFromReservation(ApproveResponseDto request, int menuCode)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<StockOutReturnDto>())
				{
					var sendModel = data.ConvertToType<StockOutReturnDto>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _stockOutReturnFromReservationService.DeleteStockOutReturnFromReservation(sendModel.StockOutReturnHeader!.StockOutReturnHeaderId, menuCode);
						}
						else
						{
							return await _stockOutReturnFromReservationService.SaveStockOutReturnFromReservation(sendModel, true, true, request.RequestId);
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
		}

		public async Task<ResponseDto> HandleSalesInvoice(ApproveResponseDto request, int menuCode)
        {
            var data = request.UserRequest;
            if (data != null)
            {
                if (data.CanBeConverted<SalesInvoiceDto>())
                {
                    var sendModel = data.ConvertToType<SalesInvoiceDto>();
                    if (sendModel != null)
                    {
                        if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
                        {
                            return await _salesInvoiceHandlingService.DeleteSalesInvoice(sendModel.SalesInvoiceHeader!.SalesInvoiceHeaderId, menuCode);
                        }
                        else
                        {
                            return await _salesInvoiceHandlingService.SaveSalesInvoice(sendModel, true, true, request.RequestId);
                        }
                    }
                }
            }
            return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
        }

		public async Task<ResponseDto> HandleReservationInvoice(ApproveResponseDto request, int menuCode)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<SalesInvoiceDto>())
				{
					var sendModel = data.ConvertToType<SalesInvoiceDto>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _reservationInvoiceService.DeleteReservationInvoice(sendModel.SalesInvoiceHeader!.SalesInvoiceHeaderId, menuCode);
						}
						else
						{
							return await _reservationInvoiceService.SaveReservationInvoice(sendModel, true, true, request.RequestId);
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
		}

		public async Task<ResponseDto> HandleSalesInvoiceReturn(ApproveResponseDto request, int menuCode)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<SalesInvoiceReturnDto>())
				{
					var sendModel = data.ConvertToType<SalesInvoiceReturnDto>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _salesInvoiceReturnHandlingService.DeleteSalesInvoiceReturn(sendModel.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId, menuCode);
						}
						else
						{
							return await _salesInvoiceReturnHandlingService.SaveSalesInvoiceReturn(sendModel, true, true, request.RequestId);
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
		}

		public async Task<ResponseDto> HandleReservationInvoiceCloseOut(ApproveResponseDto request, int menuCode)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<SalesInvoiceReturnDto>())
				{
					var sendModel = data.ConvertToType<SalesInvoiceReturnDto>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _reservationInvoiceCloseOutService.DeleteReservationInvoiceCloseOut(sendModel.SalesInvoiceReturnHeader!.SalesInvoiceReturnHeaderId, menuCode);
						}
						else
						{
							return await _reservationInvoiceCloseOutService.SaveReservationInvoiceCloseOut(sendModel, true, true, request.RequestId);
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
		}

		public async Task<ResponseDto> HandleClientCreditMemo(ApproveResponseDto request)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<ClientCreditMemoVm>())
				{
					var sendModel = data.ConvertToType<ClientCreditMemoVm>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _clientCreditMemoHandlingService.DeleteClientCreditMemoInFull(sendModel.ClientCreditMemo!.ClientCreditMemoId);
						}
						else
						{
							return await _clientCreditMemoHandlingService.SaveClientCreditMemoInFull(sendModel, true, true, request.RequestId);
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
		}

		public async Task<ResponseDto> HandleClientDebitMemo(ApproveResponseDto request)
		{
			var data = request.UserRequest;
			if (data != null)
			{
				if (data.CanBeConverted<ClientDebitMemoVm>())
				{
					var sendModel = data.ConvertToType<ClientDebitMemoVm>();
					if (sendModel != null)
					{
						if (request.ApproveRequestTypeId == ApproveRequestTypeData.Delete)
						{
							return await _clientDebitMemoHandlingService.DeleteClientDebitMemoInFull(sendModel.ClientDebitMemo!.ClientDebitMemoId);
						}
						else
						{
							return await _clientDebitMemoHandlingService.SaveClientDebitMemoInFull(sendModel, true, true, request.RequestId);
						}
					}
				}
			}
			return new ResponseDto() { Success = false, Message = _localizer["ApproveModelIsEmpty"] };
		}
	}
}
