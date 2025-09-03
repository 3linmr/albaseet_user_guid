using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Compound.CoreOne.Contracts.InvoiceSettlement;
using Compound.CoreOne.Models.Domain.InvoiceSettlement;
using Compound.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Purchases.CoreOne.Contracts;
using Purchases.CoreOne.Models.Domain;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Extensions;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service;
using Shared.Service.Logic.Approval;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.Service.Services.InvoiceSettlement
{
	public class PurchaseInvoiceSettlementService: BaseService<PurchaseInvoiceSettlement>, IPurchaseInvoiceSettlementService
	{
		private readonly IPurchaseInvoiceHeaderService _purchaseInvoiceHeaderService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IPaymentVoucherHeaderService _paymentVoucherHeaderService;
		private readonly IPaymentVoucherService _paymentVoucherService;
		private readonly IStringLocalizer<SalesInvoiceSettlementService> _localizer;
		private readonly IPurchaseValueService _purchaseValueService;

		public PurchaseInvoiceSettlementService(IRepository<PurchaseInvoiceSettlement> repository, IPurchaseInvoiceHeaderService purchaseInvoiceHeaderService, IHttpContextAccessor httpContextAccessor, IPaymentVoucherHeaderService paymentVoucherHeaderService, IPaymentVoucherService paymentVoucherService, IStringLocalizer<SalesInvoiceSettlementService> localizer, IPurchaseValueService purchaseValueService) : base(repository)
		{ 
			_purchaseInvoiceHeaderService = purchaseInvoiceHeaderService;
			_httpContextAccessor = httpContextAccessor;
			_paymentVoucherHeaderService = paymentVoucherHeaderService;
			_paymentVoucherService = paymentVoucherService;
			_localizer = localizer;
			_purchaseValueService = purchaseValueService;
		}

		public async Task<ResponseDto> IsSettlementOnInvoiceStarted(int purchaseInvoiceHeaderId)
		{
			var isStarted = await _repository.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).AnyAsync();

			return new ResponseDto { Success = isStarted, Message = _localizer[isStarted ? "SettlementStartedOnInvoice" : "NoSettlementsYet"] };
		}

		public IQueryable<PurchaseInvoiceSettlementDto> GetUnSettledInvoices(int? supplierId, int storeId, int? exceptPaymentVoucherHeaderId = null, IEnumerable<int>? purchaseInvoicesThatMustBeIncluded = null)
		{
			purchaseInvoicesThatMustBeIncluded ??= [];
			var purchaseInvoices = _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaders().Where(x => x.CreditPayment && (supplierId == null || x.SupplierId == supplierId) && x.StoreId == storeId);

			var settlements = _repository.GetAll().Where(x => x.PaymentVoucherHeaderId != exceptPaymentVoucherHeaderId).GroupBy(x => x.PurchaseInvoiceHeaderId).Select(x => new PurchaseInvoiceSettlementDto
			{
				PurchaseInvoiceHeaderId = x.Key,
				SettleValue = x.Sum(y => y.SettleValue)
			});

			return from purchaseInvoiceHeader in purchaseInvoices
				   from purchaseInvoiceValue in _purchaseValueService.GetOverallValueOfPurchaseInvoices().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId)
				   from purchaseInvoiceSettlement in settlements.Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
				   where purchaseInvoicesThatMustBeIncluded.Contains(purchaseInvoiceHeader.PurchaseInvoiceHeaderId) || purchaseInvoiceValue.OverallValue - (purchaseInvoiceSettlement.SettleValue != null /*must write it this way or else "Nullable object is null exception*/ ? purchaseInvoiceSettlement.SettleValue : 0) > 0
				   orderby purchaseInvoiceHeader.DueDate != null descending, purchaseInvoiceHeader.DueDate, purchaseInvoiceHeader.DocumentDate, purchaseInvoiceHeader.PurchaseInvoiceHeaderId
				   select new PurchaseInvoiceSettlementDto
				   {
					   PurchaseInvoiceSettlementId = -purchaseInvoiceHeader.PurchaseInvoiceHeaderId,
					   PurchaseInvoiceHeaderId = purchaseInvoiceHeader.PurchaseInvoiceHeaderId,
					   Prefix = purchaseInvoiceHeader.Prefix,
					   DocumentCode = purchaseInvoiceHeader.DocumentCode,
					   Suffix = purchaseInvoiceHeader.Suffix,
					   DocumentFullCode = purchaseInvoiceHeader.DocumentFullCode,
					   DocumentReference = purchaseInvoiceHeader.DocumentReference,
					   SupplierId = purchaseInvoiceHeader.SupplierId,
					   SupplierCode = purchaseInvoiceHeader.SupplierCode,
					   SupplierName = purchaseInvoiceHeader.SupplierName,
					   StoreId = purchaseInvoiceHeader.StoreId,
					   StoreName = purchaseInvoiceHeader.StoreName,
					   DocumentDate = purchaseInvoiceHeader.DocumentDate,
					   DueDate = purchaseInvoiceHeader.DueDate,
					   EntryDate = purchaseInvoiceHeader.EntryDate,
					   Reference = purchaseInvoiceHeader.Reference,
					   MenuCode = purchaseInvoiceHeader.MenuCode,
					   MenuName = purchaseInvoiceHeader.MenuName,
					   InvoiceTypeId = purchaseInvoiceHeader.InvoiceTypeId,
					   InvoiceTypeName = purchaseInvoiceHeader.InvoiceTypeName,
					   InvoiceValue = purchaseInvoiceValue.OverallValue,
					   PreviouslySettledValue = purchaseInvoiceSettlement.SettleValue != null ? purchaseInvoiceSettlement.SettleValue : 0,
					   RemainingValue = purchaseInvoiceValue.OverallValue - (purchaseInvoiceSettlement.SettleValue != null ? purchaseInvoiceSettlement.SettleValue : 0)
				   };
		}

		public async Task<decimal> GetPurchaseInvoiceSettledValue(int purchaseInvoiceHeaderId)
		{
			var settledValue = await _repository.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).SumAsync(x => x.SettleValue);
			return settledValue;
		}

		public IQueryable<PurchaseInvoiceSettledValueDto> GetPurchaseInvoiceSettledValues()
		{
			var settledValues = _repository.GetAll().GroupBy(x => x.PurchaseInvoiceHeaderId)
				.Select(x => new PurchaseInvoiceSettledValueDto
				{
					PurchaseInvoiceHeaderId = x.Key,
					SettledValue = x.Sum(y => y.SettleValue)
				});

			return settledValues;
		}

        public IQueryable<PaymentVoucherSettledValueDto> GetPaymentVoucherSettledValues()
        {
            var settledValues = _repository.GetAll().GroupBy(x => x.PaymentVoucherHeaderId)
                .Select(x => new PaymentVoucherSettledValueDto
                {
                    PaymentVoucherHeaderId = x.Key,
                    SettledValue = x.Sum(y => y.SettleValue)
                });

            return settledValues;
        }

		public async Task<IQueryable<PurchaseInvoiceSettlementDto>> GetPurchaseInvoicesForPaymentVoucher(int paymentVoucherHeaderId, bool allInvoices = true)
		{
			var paymentVoucherHeader = await _paymentVoucherHeaderService.GetPaymentVoucherHeaderById(paymentVoucherHeaderId);
			var storeId = paymentVoucherHeader.StoreId;
			var supplierId = paymentVoucherHeader.SupplierId;

			var purchaseInvoices = _purchaseInvoiceHeaderService.GetPurchaseInvoiceHeaders().Where(x => x.CreditPayment && x.SupplierId == supplierId && x.StoreId == storeId);

			var previouslSettlements = _repository.GetAll().Where(x => x.PaymentVoucherHeaderId != paymentVoucherHeaderId).GroupBy(x => x.PurchaseInvoiceHeaderId).Select(x => new PurchaseInvoiceSettlementDto
			{
				PurchaseInvoiceHeaderId = x.Key,
				SettleValue = x.Sum(y => y.SettleValue)
			});

			var currentSettlements = _repository.GetAll().Where(x => x.PaymentVoucherHeaderId == paymentVoucherHeaderId);

			return from purchaseInvoiceHeader in purchaseInvoices
				   from purchaseInvoiceValue in _purchaseValueService.GetOverallValueOfPurchaseInvoices().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId)
				   from previousSettlement in previouslSettlements.Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
				   from currentSettlement in currentSettlements.Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeader.PurchaseInvoiceHeaderId).DefaultIfEmpty()
				   where (previousSettlement.SettleValue != null ? previousSettlement.SettleValue : 0) < purchaseInvoiceValue.OverallValue && (allInvoices || currentSettlement.SettleValue != null)
				   orderby purchaseInvoiceHeader.DueDate != null descending, purchaseInvoiceHeader.DueDate, purchaseInvoiceHeader.DocumentDate, purchaseInvoiceHeader.PurchaseInvoiceHeaderId
				   select new PurchaseInvoiceSettlementDto
				   {
					   PurchaseInvoiceSettlementId = currentSettlement.PurchaseInvoiceSettlementId != null ? currentSettlement.PurchaseInvoiceSettlementId : -purchaseInvoiceHeader.PurchaseInvoiceHeaderId,
					   PurchaseInvoiceHeaderId = purchaseInvoiceHeader.PurchaseInvoiceHeaderId,
					   PaymentVoucherHeaderId = paymentVoucherHeaderId,
					   Prefix = purchaseInvoiceHeader.Prefix,
					   DocumentCode = purchaseInvoiceHeader.DocumentCode,
					   Suffix = purchaseInvoiceHeader.Suffix,
					   DocumentFullCode = purchaseInvoiceHeader.DocumentFullCode,
					   DocumentReference = purchaseInvoiceHeader.DocumentReference,
					   SupplierId = purchaseInvoiceHeader.SupplierId,
					   SupplierCode = purchaseInvoiceHeader.SupplierCode,
					   SupplierName = purchaseInvoiceHeader.SupplierName,
					   StoreId = purchaseInvoiceHeader.StoreId,
					   StoreName = purchaseInvoiceHeader.StoreName,
					   DocumentDate = purchaseInvoiceHeader.DocumentDate,
					   DueDate = purchaseInvoiceHeader.DueDate,
					   EntryDate = purchaseInvoiceHeader.EntryDate,
					   Reference = purchaseInvoiceHeader.Reference,
					   MenuCode = purchaseInvoiceHeader.MenuCode,
					   MenuName = purchaseInvoiceHeader.MenuName,
					   InvoiceTypeId = purchaseInvoiceHeader.InvoiceTypeId,
					   InvoiceTypeName = purchaseInvoiceHeader.InvoiceTypeName,
					   InvoiceValue = purchaseInvoiceValue.OverallValue,
					   PreviouslySettledValue = previousSettlement.SettleValue != null ? previousSettlement.SettleValue : 0,
					   SettleValue = currentSettlement.SettleValue != null ? currentSettlement.SettleValue : 0,
					   RemainingValue = purchaseInvoiceValue.OverallValue - (previousSettlement.SettleValue != null ? previousSettlement.SettleValue : 0),
					   RemarksAr = currentSettlement.RemarksAr != null ? currentSettlement.RemarksAr : null,
					   RemarksEn = currentSettlement.RemarksEn != null ? currentSettlement.RemarksEn : null,
					   CreatedAt = currentSettlement.CreatedAt != null ? currentSettlement.CreatedAt : null,
					   UserNameCreated = currentSettlement.UserNameCreated != null ? currentSettlement.UserNameCreated : null,
					   IpAddressCreated = currentSettlement.IpAddressCreated != null ? currentSettlement.IpAddressCreated : null,
				   };
		}

		public List<RequestChangesDto> GetRequestChangesWithPurchaseInvoiceSettlements(PaymentVoucherDto oldItem, PaymentVoucherDto newItem)
		{
			var requestChanges = _paymentVoucherService.GetPaymentVoucherRequestChanges(oldItem, newItem);

			var filteredOldItems = oldItem.PurchaseInvoiceSettlements.Where(x => x.SettleValue > 0).ToList();
			var filteredNewItems = newItem.PurchaseInvoiceSettlements.Where(x => x.SettleValue > 0).ToList();

			if (filteredOldItems.Any() && filteredNewItems.Any())
			{
				var oldCount = filteredOldItems.Count;
				var newCount = filteredNewItems.Count;
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(filteredOldItems[i], filteredNewItems[i]);
							requestChanges.AddRange(changes);
							index++;
							break;
						}
					}
				}
			}

			return requestChanges;
		}

		public async Task<PaymentVoucherDto> GetPaymentVoucherWithAllUnSettledPurchaseInvoices(int paymentVoucherHeaderId)
		{
			var paymentVoucher = await _paymentVoucherService.GetPaymentVoucher(paymentVoucherHeaderId);
			if (paymentVoucher.PaymentVoucherHeader != null)
			{
				paymentVoucher.PurchaseInvoiceSettlements = await (await GetPurchaseInvoicesForPaymentVoucher(paymentVoucherHeaderId, true)).ToListAsync();
			}
			return paymentVoucher;
		}

		public async Task<List<PurchaseInvoiceSettlementDto>> GetPurchaseInvoicesForPaymentVoucherRequest(int paymentVoucherHeaderId, int supplierId, int storeId, List<PurchaseInvoiceSettlementDto> settlements, bool allInvoices)
		{
			var purchaseInvoices = await GetUnSettledInvoices(supplierId, storeId, paymentVoucherHeaderId, settlements.Select(x => x.PurchaseInvoiceHeaderId)).ToListAsync();

			var result = from purchaseInvoice in purchaseInvoices
						 from purchaseInvoiceSettlement in settlements.Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoice.PurchaseInvoiceHeaderId).DefaultIfEmpty()
						 where allInvoices || purchaseInvoiceSettlement != null
						 orderby purchaseInvoice.DueDate != null descending, purchaseInvoice.DueDate, purchaseInvoice.DocumentDate, purchaseInvoice.PurchaseInvoiceHeaderId
						 select new PurchaseInvoiceSettlementDto
						 {
							 PurchaseInvoiceSettlementId = purchaseInvoiceSettlement != null ? purchaseInvoiceSettlement.PurchaseInvoiceSettlementId : -purchaseInvoice.PurchaseInvoiceHeaderId,
							 PurchaseInvoiceHeaderId = purchaseInvoice.PurchaseInvoiceHeaderId,
							 Prefix = purchaseInvoice.Prefix,
							 DocumentCode = purchaseInvoice.DocumentCode,
							 Suffix = purchaseInvoice.Suffix,
							 DocumentFullCode = purchaseInvoice.DocumentFullCode,
							 DocumentReference = purchaseInvoice.DocumentReference,
							 SupplierId = purchaseInvoice.SupplierId,
							 SupplierCode = purchaseInvoice.SupplierCode,
							 SupplierName = purchaseInvoice.SupplierName,
							 StoreId = purchaseInvoice.StoreId,
							 StoreName = purchaseInvoice.StoreName,
							 DocumentDate = purchaseInvoice.DocumentDate,
							 DueDate = purchaseInvoice.DueDate,
							 EntryDate = purchaseInvoice.EntryDate,
							 Reference = purchaseInvoice.Reference,
							 MenuCode = purchaseInvoice.MenuCode,
							 MenuName = purchaseInvoice.MenuName,
							 InvoiceTypeId = purchaseInvoice.InvoiceTypeId,
							 InvoiceTypeName = purchaseInvoice.InvoiceTypeName,
							 InvoiceValue = purchaseInvoice.InvoiceValue,
							 PreviouslySettledValue = purchaseInvoice.PreviouslySettledValue,
							 SettleValue = purchaseInvoiceSettlement != null ? purchaseInvoiceSettlement.SettleValue : 0,
							 RemainingValue = purchaseInvoice.InvoiceValue - purchaseInvoice.PreviouslySettledValue,
							 RemarksAr = purchaseInvoiceSettlement != null ? purchaseInvoiceSettlement.RemarksAr : null,
							 RemarksEn = purchaseInvoiceSettlement != null ? purchaseInvoiceSettlement.RemarksEn : null,
							 CreatedAt = purchaseInvoiceSettlement != null ? purchaseInvoiceSettlement.CreatedAt : null,
							 UserNameCreated = purchaseInvoiceSettlement != null ? purchaseInvoiceSettlement.UserNameCreated : null,
							 IpAddressCreated = purchaseInvoiceSettlement != null ? purchaseInvoiceSettlement.IpAddressCreated : null,
						 };

			return result.ToList();
		}


		public async Task<PaymentVoucherDto> GetPaymentVoucherWithPurchaseInvoices(int paymentVoucherHeaderId)
		{
			var paymentVoucher = await _paymentVoucherService.GetPaymentVoucher(paymentVoucherHeaderId);
			if (paymentVoucher.PaymentVoucherHeader != null)
			{
				paymentVoucher.PurchaseInvoiceSettlements = await (await GetPurchaseInvoicesForPaymentVoucher(paymentVoucherHeaderId, false)).ToListAsync();
			}
			return paymentVoucher;
		}

		public async Task<ResponseDto> SavePaymentVoucherWithInvoiceSettlements(PaymentVoucherDto paymentVoucher, bool hasApprove, bool approved, int? requestId)
		{
			paymentVoucher.PurchaseInvoiceSettlements = paymentVoucher.PurchaseInvoiceSettlements.Where(x => x.SettleValue > 0).ToList();
			var validationResult = await CheckPaymentVoucherExceedingInvoice(paymentVoucher.PaymentVoucherHeader!.PaymentVoucherHeaderId, paymentVoucher.PaymentVoucherHeader!.SupplierId, paymentVoucher.PaymentVoucherHeader!.StoreId, paymentVoucher.PurchaseInvoiceSettlements);
			if (validationResult.validation.Success == false) return validationResult.validation;

			var paymentVoucherResult = await _paymentVoucherService.SavePaymentVoucher(paymentVoucher, hasApprove, approved, requestId);
			if (paymentVoucherResult.Success)
			{
				await UpdatePurchaseInvoiceHasSettlementFlagsForSave(paymentVoucherResult.Id, paymentVoucher.PurchaseInvoiceSettlements);
				await UpdatePurchaseInvoiceSettlementCompletedFlagsForSave(paymentVoucherResult.Id, validationResult.completelySettledSalesInvoiceIds!);
				await SavePurchaseInvoiceSettlements(paymentVoucherResult.Id, paymentVoucher.PurchaseInvoiceSettlements);
			}
			return paymentVoucherResult;
		}

		private async Task<ValidationResultAndListOfInvoices> CheckPaymentVoucherExceedingInvoice(int paymentVoucherHeaderId, int? supplierId, int storeId, List<PurchaseInvoiceSettlementDto> purchaseInvoiceSettlements)
		{
			if (supplierId == 0) return new ValidationResultAndListOfInvoices { validation = new ResponseDto { Message = "SupplierId in header must not be 0" } };

			var purchaseInvoiceIds = purchaseInvoiceSettlements.Select(x => x.PurchaseInvoiceHeaderId).ToList();
			var purchaseInvoices = await GetUnSettledInvoices(supplierId, storeId, paymentVoucherHeaderId, purchaseInvoiceIds).Where(x => purchaseInvoiceIds.Contains(x.PurchaseInvoiceHeaderId)).ToListAsync();

			var exceedingOrCompleted = (from purchaseInvoiceSettlement in purchaseInvoiceSettlements
										from purchaseInvoice in purchaseInvoices.Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceSettlement.PurchaseInvoiceHeaderId).DefaultIfEmpty()
										where purchaseInvoice == null || (purchaseInvoiceSettlement.SettleValue >= purchaseInvoice.RemainingValue)
										select new { PurchaseInvoiceHeaderId = purchaseInvoiceSettlement.PurchaseInvoiceHeaderId, RemainingValue = purchaseInvoice?.RemainingValue, DocumentFullCode = purchaseInvoice?.DocumentFullCode ?? "0", purchaseInvoiceSettlement.SettleValue });

			var exceeding = exceedingOrCompleted.FirstOrDefault(x => x.RemainingValue == null || x.SettleValue > x.RemainingValue);
			if (exceeding != null)
			{
				if (exceeding.RemainingValue != null)
				{
					return new ValidationResultAndListOfInvoices { validation = new ResponseDto { Success = false, Message = _localizer["SettleExceedsRemainingValue", exceeding.SettleValue.ToNormalizedString(), exceeding.DocumentFullCode, ((decimal)exceeding.RemainingValue).ToNormalizedString()] } };
				}
				else
				{
					return new ValidationResultAndListOfInvoices { validation = new ResponseDto { Success = false, Message = _localizer["LinkToInvoiceNoLongerExist"] } };
				}
			}

			return new ValidationResultAndListOfInvoices { validation = new ResponseDto { Success = true }, completelySettledSalesInvoiceIds = exceedingOrCompleted.Select(x => x.PurchaseInvoiceHeaderId).ToList() };
		}

		public async Task<ResponseDto> DeletePaymentVoucherWithInvoiceSettlements(int paymentVoucherHeaderId)
		{
			await UpdatePurchaseInvoiceHasSettlementFlagsForDelete(paymentVoucherHeaderId);
			await UpdatePurchaseInvoiceSettlementCompletedFlagsForDelete(paymentVoucherHeaderId);
			await DeletePurchaseInvoiceSettlements(paymentVoucherHeaderId);
			return await _paymentVoucherService.DeletePaymentVoucher(paymentVoucherHeaderId);
		}

		private async Task UpdatePurchaseInvoiceHasSettlementFlagsForSave(int paymentVoucherHeaderId, List<PurchaseInvoiceSettlementDto> settlements)
		{
			var currentInvoiceIds = await _repository.GetAll().Where(x => x.PaymentVoucherHeaderId == paymentVoucherHeaderId).Select(x => x.PurchaseInvoiceHeaderId).ToListAsync();
			var futureInvoicesIds = settlements.Select(x => x.PurchaseInvoiceHeaderId).ToList();

			var addedInvoicesIds = futureInvoicesIds.Except(currentInvoiceIds).ToList();
			var removedInvoiceIds = currentInvoiceIds.Except(futureInvoicesIds).ToList();

			if (addedInvoicesIds.Count > 0)
			{
				await _purchaseInvoiceHeaderService.UpdateHasSettlementFlag(addedInvoicesIds, true);
			}

			if (removedInvoiceIds.Count > 0)
			{
				var invoiceIdsThatHaveOtherSettlments = await _repository.GetAll().Where(x => x.PaymentVoucherHeaderId != paymentVoucherHeaderId && removedInvoiceIds.Contains(x.PurchaseInvoiceHeaderId)).Select(x => x.PurchaseInvoiceHeaderId).ToListAsync();
				var toBeReopened = removedInvoiceIds.Except(invoiceIdsThatHaveOtherSettlments).ToList();

				await _purchaseInvoiceHeaderService.UpdateHasSettlementFlag(toBeReopened, false);
			}
		}

		private async Task UpdatePurchaseInvoiceSettlementCompletedFlagsForSave(int paymentVoucherHeaderId, List<int> completedSettlements)
		{
			var currentlyCompletedInvoiceIds = await (from settlement in _repository.GetAll().Where(x => x.PaymentVoucherHeaderId == paymentVoucherHeaderId)
													  from purchaseInvoice in _purchaseInvoiceHeaderService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == settlement.PurchaseInvoiceHeaderId && x.IsSettlementCompleted)
													  select settlement.PurchaseInvoiceHeaderId).ToListAsync();

			var futureCompletedInvoicesIds = completedSettlements;

			var addedInvoicesIds = futureCompletedInvoicesIds.Except(currentlyCompletedInvoiceIds).ToList();
			var removedInvoiceIds = currentlyCompletedInvoiceIds.Except(futureCompletedInvoicesIds).ToList();

			if (addedInvoicesIds.Count > 0)
			{
				await _purchaseInvoiceHeaderService.UpdateIsSettlementCompletedFlags(addedInvoicesIds, true);
			}

			if (removedInvoiceIds.Count > 0)
			{
				await _purchaseInvoiceHeaderService.UpdateIsSettlementCompletedFlags(removedInvoiceIds, false);
			}
		}

		private async Task UpdatePurchaseInvoiceHasSettlementFlagsForDelete(int paymentVoucherHeaderId)
		{
			var hasOtherSettlements = _repository.GetAll().Where(x => x.PaymentVoucherHeaderId != paymentVoucherHeaderId).Select(x => x.PurchaseInvoiceHeaderId);
			var toBeReopened = await _repository.GetAll().Where(x => x.PaymentVoucherHeaderId == paymentVoucherHeaderId && !hasOtherSettlements.Contains(x.PurchaseInvoiceHeaderId)).Select(x => x.PurchaseInvoiceHeaderId).ToListAsync();

			await _purchaseInvoiceHeaderService.UpdateHasSettlementFlag(toBeReopened, false);
		}

		private async Task UpdatePurchaseInvoiceSettlementCompletedFlagsForDelete(int paymentVoucherHeaderId)
		{
			var toBeReopened = await _repository.GetAll().Where(x => x.PaymentVoucherHeaderId == paymentVoucherHeaderId).Select(x => x.PurchaseInvoiceHeaderId).ToListAsync();

			await _purchaseInvoiceHeaderService.UpdateIsSettlementCompletedFlags(toBeReopened, false);
		}

		public async Task<List<PurchaseInvoiceSettlementDto>> SavePurchaseInvoiceSettlements(int paymentVoucherHeaderId, List<PurchaseInvoiceSettlementDto> purchaseInvoiceSettlements)
		{
			await DeletePurchaseInvoiceSettlements(purchaseInvoiceSettlements, paymentVoucherHeaderId);
			if (purchaseInvoiceSettlements.Any())
			{
				await EditPurchaseInvoiceSettlement(purchaseInvoiceSettlements);
				return await AddPurchaseInvoiceSettlement(purchaseInvoiceSettlements, paymentVoucherHeaderId);
			}
			return purchaseInvoiceSettlements;
		}

		public async Task<bool> DeletePurchaseInvoiceSettlements(int paymentVoucherHeaderId)
		{
			var data = await _repository.GetAll().Where(x => x.PaymentVoucherHeaderId == paymentVoucherHeaderId).ToListAsync();
			if (data.Any())
			{
				_repository.RemoveRange(data);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}

		private async Task<List<PurchaseInvoiceSettlementDto>> AddPurchaseInvoiceSettlement(List<PurchaseInvoiceSettlementDto> settlements, int paymentVoucherHeaderId)
		{
			var current = settlements.Where(x => x.PurchaseInvoiceSettlementId <= 0).ToList();
			var modelList = new List<PurchaseInvoiceSettlement>();
			var newId = await GetNextId();
			foreach (var settlement in current)
			{
				var model = new PurchaseInvoiceSettlement()
				{
					PurchaseInvoiceSettlementId = newId,
					PaymentVoucherHeaderId = paymentVoucherHeaderId,
					PurchaseInvoiceHeaderId = settlement.PurchaseInvoiceHeaderId,
					SettleValue = settlement.SettleValue,
					RemarksAr = settlement.RemarksAr,
					RemarksEn = settlement.RemarksEn,

					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor!.GetUserName(),
					IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				};

				settlement.PurchaseInvoiceSettlementId = newId;
				settlement.PaymentVoucherHeaderId = paymentVoucherHeaderId;

				modelList.Add(model);
				newId++;
			}

			if (modelList.Any())
			{
				await _repository.InsertRange(modelList);
				await _repository.SaveChanges();
			}
			return settlements;
		}

		private async Task<bool> EditPurchaseInvoiceSettlement(List<PurchaseInvoiceSettlementDto> purchaseInvoiceSettlements)
		{
			var current = purchaseInvoiceSettlements.Where(x => x.PurchaseInvoiceSettlementId > 0).ToList();
			var modelList = new List<PurchaseInvoiceSettlement>();
			foreach (var settlement in current)
			{
				var model = new PurchaseInvoiceSettlement()
				{
					PurchaseInvoiceSettlementId = settlement.PurchaseInvoiceSettlementId,
					PurchaseInvoiceHeaderId = settlement.PurchaseInvoiceHeaderId,
					PaymentVoucherHeaderId = settlement.PaymentVoucherHeaderId,
					SettleValue = settlement.SettleValue,
					RemarksAr = settlement.RemarksAr,
					RemarksEn = settlement.RemarksEn,

					CreatedAt = settlement.CreatedAt,
					UserNameCreated = settlement.UserNameCreated,
					IpAddressCreated = settlement.IpAddressCreated,
					ModifiedAt = DateHelper.GetDateTimeNow(),
					UserNameModified = await _httpContextAccessor!.GetUserName(),
					IpAddressModified = _httpContextAccessor?.GetIpAddress(),
					Hide = false
				};


				modelList.Add(model);
			}

			if (modelList.Any())
			{
				_repository.UpdateRange(modelList);
				await _repository.SaveChanges();
				return true;
			}
			return false;

		}

		private async Task<bool> DeletePurchaseInvoiceSettlements(List<PurchaseInvoiceSettlementDto> settlements, int headerId)
		{
			var current = _repository.GetAll().Where(x => x.PaymentVoucherHeaderId == headerId).AsNoTracking().ToList();
			var toBeDeleted = current.Where(p => settlements.All(p2 => p2.PurchaseInvoiceSettlementId != p.PurchaseInvoiceSettlementId)).ToList();
			if (toBeDeleted.Any())
			{
				_repository.RemoveRange(toBeDeleted);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}

		private async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.PurchaseInvoiceSettlementId) + 1; } catch { id = 1; }
			return id;
		}
	}
}
