using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Compound.CoreOne.Contracts.InvoiceSettlement;
using Compound.CoreOne.Models.Domain.InvoiceSettlement;
using Compound.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
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
	public class SalesInvoiceSettlementService: BaseService<SalesInvoiceSettlement>, ISalesInvoiceSettlementService
	{
		private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IReceiptVoucherHeaderService _receiptVoucherHeaderService;
		private readonly IReceiptVoucherService _receiptVoucherService;
		private readonly IStringLocalizer<SalesInvoiceSettlementService> _localizer;
		private readonly ISalesValueService _salesValueService;

		public SalesInvoiceSettlementService(IRepository<SalesInvoiceSettlement> repository, ISalesInvoiceHeaderService salesInvoiceHeaderService, IHttpContextAccessor httpContextAccessor, IReceiptVoucherHeaderService receiptVoucherHeaderService, IReceiptVoucherService receiptVoucherService, IStringLocalizer<SalesInvoiceSettlementService> localizer, ISalesValueService salesValueService) : base(repository)
		{ 
			_salesInvoiceHeaderService = salesInvoiceHeaderService;
			_httpContextAccessor = httpContextAccessor;
			_receiptVoucherHeaderService = receiptVoucherHeaderService;
			_receiptVoucherService = receiptVoucherService;
			_localizer = localizer;
			_salesValueService = salesValueService;
		}

		public async Task<ResponseDto> IsSettlementOnInvoiceStarted(int salesInvoiceHeaderId)
		{
			var isStarted = await _repository.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).AnyAsync();

			return new ResponseDto { Success = isStarted, Message = _localizer[isStarted ? "SettlementStartedOnInvoice" : "NoSettlementsYet"] };
		}

		public IQueryable<SalesInvoiceSettlementDto> GetUnSettledInvoices(int? clientId, int storeId, int? exceptReceiptVoucherHeaderId = null, IEnumerable<int>? salesInvoicesThatMustBeIncluded = null)
		{
			salesInvoicesThatMustBeIncluded ??= [];
			var salesInvoices = _salesInvoiceHeaderService.GetSalesInvoiceHeaders().Where(x => x.CreditPayment && (clientId == null || x.ClientId == clientId) && x.StoreId == storeId);

			var settlements = _repository.GetAll().Where(x => x.ReceiptVoucherHeaderId != exceptReceiptVoucherHeaderId).GroupBy(x => x.SalesInvoiceHeaderId).Select(x => new SalesInvoiceSettlementDto
			{
				SalesInvoiceHeaderId = x.Key,
				SettleValue = x.Sum(y => y.SettleValue)
			});

			return from salesInvoiceHeader in salesInvoices
				   from salesInvoiceValue in _salesValueService.GetOverallValueOfSalesInvoices().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)
				   from salesInvoiceSettlement in settlements.Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
				   where salesInvoicesThatMustBeIncluded.Contains(salesInvoiceHeader.SalesInvoiceHeaderId) || salesInvoiceValue.OverallNetValue - (salesInvoiceSettlement.SettleValue != null /*must write it this way or else "Nullable object is null exception*/ ? salesInvoiceSettlement.SettleValue : 0) > 0
				   orderby salesInvoiceHeader.DueDate != null descending, salesInvoiceHeader.DueDate, salesInvoiceHeader.DocumentDate, salesInvoiceHeader.SalesInvoiceHeaderId
				   select new SalesInvoiceSettlementDto
				   {
					   SalesInvoiceSettlementId = -salesInvoiceHeader.SalesInvoiceHeaderId,
					   SalesInvoiceHeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
					   Prefix = salesInvoiceHeader.Prefix,
					   DocumentCode = salesInvoiceHeader.DocumentCode,
					   Suffix = salesInvoiceHeader.Suffix,
					   DocumentFullCode = salesInvoiceHeader.DocumentFullCode,
					   DocumentReference = salesInvoiceHeader.DocumentReference,
					   ClientId = salesInvoiceHeader.ClientId,
					   ClientCode = salesInvoiceHeader.ClientCode,
					   ClientName = salesInvoiceHeader.ClientName,
					   SellerId = salesInvoiceHeader.SellerId,
					   SellerCode = salesInvoiceHeader.SellerCode,
					   SellerName = salesInvoiceHeader.SellerName,
					   StoreId = salesInvoiceHeader.StoreId,
					   StoreName = salesInvoiceHeader.StoreName,
					   DocumentDate = salesInvoiceHeader.DocumentDate,
					   DueDate = salesInvoiceHeader.DueDate,
					   EntryDate = salesInvoiceHeader.EntryDate,
					   Reference = salesInvoiceHeader.Reference,
					   MenuCode = salesInvoiceHeader.MenuCode,
					   MenuName = salesInvoiceHeader.MenuName,
					   InvoiceTypeId = salesInvoiceHeader.InvoiceTypeId,
					   InvoiceTypeName = salesInvoiceHeader.InvoiceTypeName,
					   InvoiceValue = salesInvoiceValue.OverallNetValue,
					   PreviouslySettledValue = salesInvoiceSettlement.SettleValue != null ? salesInvoiceSettlement.SettleValue : 0,
					   RemainingValue = salesInvoiceValue.OverallNetValue - (salesInvoiceSettlement.SettleValue != null ? salesInvoiceSettlement.SettleValue : 0)
				   };
		}

		public async Task<decimal> GetSalesInvoiceSettledValue(int salesInvoiceHeaderId)
		{
			var settledValue = await _repository.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).SumAsync(x => x.SettleValue);
			return settledValue;
		}

		public async Task<IQueryable<SalesInvoiceSettlementDto>> GetSalesInvoicesForReceiptVoucher(int receiptVoucherHeaderId, bool allInvoices = true)
		{
			var receiptVoucherHeader = await _receiptVoucherHeaderService.GetReceiptVoucherHeaderById(receiptVoucherHeaderId);
			var storeId = receiptVoucherHeader.StoreId;
			var clientId = receiptVoucherHeader.ClientId;

			var salesInvoices = _salesInvoiceHeaderService.GetSalesInvoiceHeaders().Where(x => x.CreditPayment && x.ClientId == clientId && x.StoreId == storeId);

			var previouslSettlements = _repository.GetAll().Where(x => x.ReceiptVoucherHeaderId != receiptVoucherHeaderId).GroupBy(x => x.SalesInvoiceHeaderId).Select(x => new SalesInvoiceSettlementDto
			{
				SalesInvoiceHeaderId = x.Key,
				SettleValue = x.Sum(y => y.SettleValue)
			});

			var currentSettlements = _repository.GetAll().Where(x => x.ReceiptVoucherHeaderId == receiptVoucherHeaderId);

			return from salesInvoiceHeader in salesInvoices
				   from salesInvoiceValue in _salesValueService.GetOverallValueOfSalesInvoices().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId)
				   from previousSettlement in previouslSettlements.Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
				   from currentSettlement in currentSettlements.Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeader.SalesInvoiceHeaderId).DefaultIfEmpty()
				   where (previousSettlement.SettleValue != null ? previousSettlement.SettleValue : 0) < salesInvoiceValue.OverallNetValue && (allInvoices || currentSettlement.SettleValue != null)
				   orderby salesInvoiceHeader.DueDate != null descending, salesInvoiceHeader.DueDate, salesInvoiceHeader.DocumentDate, salesInvoiceHeader.SalesInvoiceHeaderId
				   select new SalesInvoiceSettlementDto
				   {
					   SalesInvoiceSettlementId = currentSettlement.SalesInvoiceSettlementId != null ? currentSettlement.SalesInvoiceSettlementId : -salesInvoiceHeader.SalesInvoiceHeaderId,
					   SalesInvoiceHeaderId = salesInvoiceHeader.SalesInvoiceHeaderId,
					   ReceiptVoucherHeaderId = receiptVoucherHeaderId,
					   Prefix = salesInvoiceHeader.Prefix,
					   DocumentCode = salesInvoiceHeader.DocumentCode,
					   Suffix = salesInvoiceHeader.Suffix,
					   DocumentFullCode = salesInvoiceHeader.DocumentFullCode,
					   DocumentReference = salesInvoiceHeader.DocumentReference,
					   ClientId = salesInvoiceHeader.ClientId,
					   ClientCode = salesInvoiceHeader.ClientCode,
					   ClientName = salesInvoiceHeader.ClientName,
					   SellerId = salesInvoiceHeader.SellerId,
					   SellerCode = salesInvoiceHeader.SellerCode,
					   SellerName = salesInvoiceHeader.SellerName,
					   StoreId = salesInvoiceHeader.StoreId,
					   StoreName = salesInvoiceHeader.StoreName,
					   DocumentDate = salesInvoiceHeader.DocumentDate,
					   DueDate = salesInvoiceHeader.DueDate,
					   EntryDate = salesInvoiceHeader.EntryDate,
					   Reference = salesInvoiceHeader.Reference,
					   MenuCode = salesInvoiceHeader.MenuCode,
					   MenuName = salesInvoiceHeader.MenuName,
					   InvoiceTypeId = salesInvoiceHeader.InvoiceTypeId,
					   InvoiceTypeName = salesInvoiceHeader.InvoiceTypeName,
					   InvoiceValue = salesInvoiceValue.OverallNetValue,
					   PreviouslySettledValue = previousSettlement.SettleValue != null ? previousSettlement.SettleValue : 0,
					   SettleValue = currentSettlement.SettleValue != null ? currentSettlement.SettleValue : 0,
					   RemainingValue = salesInvoiceValue.OverallNetValue - (previousSettlement.SettleValue != null ? previousSettlement.SettleValue : 0),
					   RemarksAr = currentSettlement.RemarksAr != null ? currentSettlement.RemarksAr : null,
					   RemarksEn = currentSettlement.RemarksEn != null ? currentSettlement.RemarksEn : null,
					   CreatedAt = currentSettlement.CreatedAt != null ? currentSettlement.CreatedAt : null,
					   UserNameCreated = currentSettlement.UserNameCreated != null ? currentSettlement.UserNameCreated : null,
					   IpAddressCreated = currentSettlement.IpAddressCreated != null ? currentSettlement.IpAddressCreated : null,
				   };
		}

		public List<RequestChangesDto> GetRequestChangesWithSalesInvoiceSettlements(ReceiptVoucherDto oldItem, ReceiptVoucherDto newItem)
		{
			var requestChanges = _receiptVoucherService.GetReceiptVoucherRequestChanges(oldItem, newItem);

			var filteredOldItems = oldItem.SalesInvoiceSettlements.Where(x => x.SettleValue > 0).ToList();
			var filteredNewItems = newItem.SalesInvoiceSettlements.Where(x => x.SettleValue > 0).ToList();

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

		public async Task<ReceiptVoucherDto> GetReceiptVoucherWithAllUnSettledSalesInvoices(int receiptVoucherHeaderId)
		{
			var receiptVoucher = await _receiptVoucherService.GetReceiptVoucher(receiptVoucherHeaderId);
			if (receiptVoucher.ReceiptVoucherHeader != null)
			{
				receiptVoucher.SalesInvoiceSettlements = await (await GetSalesInvoicesForReceiptVoucher(receiptVoucherHeaderId, true)).ToListAsync();
			}
			return receiptVoucher;
		}

		public async Task<List<SalesInvoiceSettlementDto>> GetSalesInvoicesForReceiptVoucherRequest(int receiptVoucherHeaderId, int clientId, int storeId, List<SalesInvoiceSettlementDto> settlements, bool allInvoices)
		{
			var salesInvoices = await GetUnSettledInvoices(clientId, storeId, receiptVoucherHeaderId, settlements.Select(x => x.SalesInvoiceHeaderId)).ToListAsync();

			var result = from salesInvoice in salesInvoices
						 from salesInvoiceSettlement in settlements.Where(x => x.SalesInvoiceHeaderId == salesInvoice.SalesInvoiceHeaderId).DefaultIfEmpty()
						 where allInvoices || salesInvoiceSettlement != null
						 orderby salesInvoice.DueDate != null descending, salesInvoice.DueDate, salesInvoice.DocumentDate, salesInvoice.SalesInvoiceHeaderId
						 select new SalesInvoiceSettlementDto
						 {
							 SalesInvoiceSettlementId = salesInvoiceSettlement != null ? salesInvoiceSettlement.SalesInvoiceSettlementId : -salesInvoice.SalesInvoiceHeaderId,
							 SalesInvoiceHeaderId = salesInvoice.SalesInvoiceHeaderId,
							 Prefix = salesInvoice.Prefix,
							 DocumentCode = salesInvoice.DocumentCode,
							 Suffix = salesInvoice.Suffix,
							 DocumentFullCode = salesInvoice.DocumentFullCode,
							 DocumentReference = salesInvoice.DocumentReference,
							 ClientId = salesInvoice.ClientId,
							 ClientCode = salesInvoice.ClientCode,
							 ClientName = salesInvoice.ClientName,
							 SellerId = salesInvoice.SellerId,
							 SellerCode = salesInvoice.SellerCode,
							 SellerName = salesInvoice.SellerName,
							 StoreId = salesInvoice.StoreId,
							 StoreName = salesInvoice.StoreName,
							 DocumentDate = salesInvoice.DocumentDate,
							 DueDate = salesInvoice.DueDate,
							 EntryDate = salesInvoice.EntryDate,
							 Reference = salesInvoice.Reference,
							 MenuCode = salesInvoice.MenuCode,
							 MenuName = salesInvoice.MenuName,
							 InvoiceTypeId = salesInvoice.InvoiceTypeId,
							 InvoiceTypeName = salesInvoice.InvoiceTypeName,
							 InvoiceValue = salesInvoice.InvoiceValue,
							 PreviouslySettledValue = salesInvoice.PreviouslySettledValue,
							 SettleValue = salesInvoiceSettlement != null ? salesInvoiceSettlement.SettleValue : 0,
							 RemainingValue = salesInvoice.InvoiceValue - salesInvoice.PreviouslySettledValue,
							 RemarksAr = salesInvoiceSettlement != null ? salesInvoiceSettlement.RemarksAr : null,
							 RemarksEn = salesInvoiceSettlement != null ? salesInvoiceSettlement.RemarksEn : null,
							 CreatedAt = salesInvoiceSettlement != null ? salesInvoiceSettlement.CreatedAt : null,
							 UserNameCreated = salesInvoiceSettlement != null ? salesInvoiceSettlement.UserNameCreated : null,
							 IpAddressCreated = salesInvoiceSettlement!= null ? salesInvoiceSettlement.IpAddressCreated : null,
						 };

			return result.ToList();
		}

		public async Task<ReceiptVoucherDto> GetReceiptVoucherWithSalesInvoices(int receiptVoucherHeaderId)
		{
			var receiptVoucher = await _receiptVoucherService.GetReceiptVoucher(receiptVoucherHeaderId);
			if (receiptVoucher.ReceiptVoucherHeader != null)
			{
				receiptVoucher.SalesInvoiceSettlements = await (await GetSalesInvoicesForReceiptVoucher(receiptVoucherHeaderId, false)).ToListAsync();
			}
			return receiptVoucher;
		}

		public async Task<ResponseDto> SaveReceiptVoucherWithInvoiceSettlements(ReceiptVoucherDto receiptVoucher, bool hasApprove, bool approved, int? requestId)
		{
			receiptVoucher.SalesInvoiceSettlements = receiptVoucher.SalesInvoiceSettlements.Where(x => x.SettleValue > 0).ToList();
			var validationResult = await CheckReceiptVoucherExceedingInvoice(receiptVoucher.ReceiptVoucherHeader!.ReceiptVoucherHeaderId, receiptVoucher.ReceiptVoucherHeader!.ClientId, receiptVoucher.ReceiptVoucherHeader!.StoreId, receiptVoucher.SalesInvoiceSettlements);
			if (validationResult.validation.Success == false) return validationResult.validation;

			var receiptVoucherResult = await _receiptVoucherService.SaveReceiptVoucher(receiptVoucher, hasApprove, approved, requestId);
			if (receiptVoucherResult.Success)
			{
				await UpdateSalesInvoiceHasSettlementFlagsForSave(receiptVoucherResult.Id, receiptVoucher.SalesInvoiceSettlements);
				await UpdateSalesInvoiceSettlementCompletedFlagsForSave(receiptVoucherResult.Id, validationResult.completelySettledSalesInvoiceIds!);
				await SaveSalesInvoiceSettlements(receiptVoucherResult.Id, receiptVoucher.SalesInvoiceSettlements);
			}
			return receiptVoucherResult;
		}

		private async Task<ValidationResultAndListOfInvoices> CheckReceiptVoucherExceedingInvoice(int receiptVoucherHeaderId, int? clientId, int storeId, List<SalesInvoiceSettlementDto> salesInvoiceSettlements)
		{
			if (clientId == 0) return new ValidationResultAndListOfInvoices { validation = new ResponseDto { Message = "ClientId in header must not be 0" } };

			var salesInvoiceIds = salesInvoiceSettlements.Select(x => x.SalesInvoiceHeaderId).ToList();
			var salesInvoices = await GetUnSettledInvoices(clientId, storeId, receiptVoucherHeaderId, salesInvoiceIds).Where(x => salesInvoiceIds.Contains(x.SalesInvoiceHeaderId)).ToListAsync();

			var exceedingOrCompleted = (from salesInvoiceSettlement in salesInvoiceSettlements
										from salesInvoice in salesInvoices.Where(x => x.SalesInvoiceHeaderId == salesInvoiceSettlement.SalesInvoiceHeaderId).DefaultIfEmpty()
										where salesInvoice == null || (salesInvoiceSettlement.SettleValue >= salesInvoice.RemainingValue)
										select new { SalesInvoiceHeaderId = salesInvoiceSettlement.SalesInvoiceHeaderId, RemainingValue = salesInvoice?.RemainingValue, DocumentFullCode = salesInvoice?.DocumentFullCode ?? "0", salesInvoiceSettlement.SettleValue });

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

			return new ValidationResultAndListOfInvoices { validation = new ResponseDto { Success = true }, completelySettledSalesInvoiceIds = exceedingOrCompleted.Select(x => x.SalesInvoiceHeaderId).ToList() };
		}

		public async Task<ResponseDto> DeleteReceiptVoucherWithInvoiceSettlements(int receiptVoucherHeaderId)
		{
			await UpdateSalesInvoiceHasSettlementFlagsForDelete(receiptVoucherHeaderId);
			await UpdateSalesInvoiceSettlementCompletedFlagsForDelete(receiptVoucherHeaderId);
			await DeleteSalesInvoiceSettlements(receiptVoucherHeaderId);
			return await _receiptVoucherService.DeleteReceiptVoucher(receiptVoucherHeaderId);
		}

		private async Task UpdateSalesInvoiceHasSettlementFlagsForSave(int receiptVoucherHeaderId, List<SalesInvoiceSettlementDto> settlements)
		{
			var currentInvoiceIds = await _repository.GetAll().Where(x => x.ReceiptVoucherHeaderId == receiptVoucherHeaderId).Select(x => x.SalesInvoiceHeaderId).ToListAsync();
			var futureInvoicesIds = settlements.Select(x => x.SalesInvoiceHeaderId).ToList();

			var addedInvoicesIds = futureInvoicesIds.Except(currentInvoiceIds).ToList();
			var removedInvoiceIds = currentInvoiceIds.Except(futureInvoicesIds).ToList();

			if (addedInvoicesIds.Count > 0)
			{
				await _salesInvoiceHeaderService.UpdateHasSettlementFlag(addedInvoicesIds, true);
			}

			if (removedInvoiceIds.Count > 0)
			{
				var invoiceIdsThatHaveOtherSettlments = await _repository.GetAll().Where(x => x.ReceiptVoucherHeaderId != receiptVoucherHeaderId && removedInvoiceIds.Contains(x.SalesInvoiceHeaderId)).Select(x => x.SalesInvoiceHeaderId).ToListAsync();
				var toBeReopened = removedInvoiceIds.Except(invoiceIdsThatHaveOtherSettlments).ToList();

				await _salesInvoiceHeaderService.UpdateHasSettlementFlag(toBeReopened, false);
			}
		}

		private async Task UpdateSalesInvoiceSettlementCompletedFlagsForSave(int receiptVoucherHeaderId, List<int> completedSettlements)
		{
			var currentlyCompletedInvoiceIds = await (from settlement in _repository.GetAll().Where(x => x.ReceiptVoucherHeaderId == receiptVoucherHeaderId)
										   from salesInvoice in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == settlement.SalesInvoiceHeaderId && x.IsSettlementCompleted)
										   select settlement.SalesInvoiceHeaderId).ToListAsync();

			var futureCompletedInvoicesIds = completedSettlements;

			var addedInvoicesIds = futureCompletedInvoicesIds.Except(currentlyCompletedInvoiceIds).ToList();
			var removedInvoiceIds = currentlyCompletedInvoiceIds.Except(futureCompletedInvoicesIds).ToList();

			if (addedInvoicesIds.Count > 0)
			{
				await _salesInvoiceHeaderService.UpdateIsSettlementCompletedFlags(addedInvoicesIds, true);
			}

			if (removedInvoiceIds.Count > 0)
			{
				await _salesInvoiceHeaderService.UpdateIsSettlementCompletedFlags(removedInvoiceIds, false);
			}
		}

		private async Task UpdateSalesInvoiceHasSettlementFlagsForDelete(int receiptVoucherHeaderId)
		{
			var hasOtherSettlements = _repository.GetAll().Where(x => x.ReceiptVoucherHeaderId != receiptVoucherHeaderId).Select(x => x.SalesInvoiceHeaderId);
			var toBeReopened = await _repository.GetAll().Where(x => x.ReceiptVoucherHeaderId == receiptVoucherHeaderId && !hasOtherSettlements.Contains(x.SalesInvoiceHeaderId)).Select(x => x.SalesInvoiceHeaderId).ToListAsync();

			await _salesInvoiceHeaderService.UpdateHasSettlementFlag(toBeReopened, false);
		}

		private async Task UpdateSalesInvoiceSettlementCompletedFlagsForDelete(int receiptVoucherHeaderId)
		{
			var toBeReopened = await _repository.GetAll().Where(x => x.ReceiptVoucherHeaderId == receiptVoucherHeaderId).Select(x => x.SalesInvoiceHeaderId).ToListAsync();

			await _salesInvoiceHeaderService.UpdateIsSettlementCompletedFlags(toBeReopened, false);
		}

		public async Task<List<SalesInvoiceSettlementDto>> SaveSalesInvoiceSettlements(int receiptVoucherHeaderId, List<SalesInvoiceSettlementDto> salesInvoiceSettlements)
		{
			await DeleteSalesInvoiceSettlements(salesInvoiceSettlements, receiptVoucherHeaderId);
			if (salesInvoiceSettlements.Any())
			{
				await EditSalesInvoiceSettlement(salesInvoiceSettlements);
				return await AddSalesInvoiceSettlement(salesInvoiceSettlements, receiptVoucherHeaderId);
			}
			return salesInvoiceSettlements;
		}

		public async Task<bool> DeleteSalesInvoiceSettlements(int receiptVoucherHeaderId)
		{
			var data = await _repository.GetAll().Where(x => x.ReceiptVoucherHeaderId == receiptVoucherHeaderId).ToListAsync();
			if (data.Any())
			{
				_repository.RemoveRange(data);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}

		private async Task<List<SalesInvoiceSettlementDto>> AddSalesInvoiceSettlement(List<SalesInvoiceSettlementDto> settlements, int receiptVoucherHeaderId)
		{
			var current = settlements.Where(x => x.SalesInvoiceSettlementId <= 0).ToList();
			var modelList = new List<SalesInvoiceSettlement>();
			var newId = await GetNextId();
			foreach (var settlement in current)
			{
				var model = new SalesInvoiceSettlement()
				{
					SalesInvoiceSettlementId = newId,
					ReceiptVoucherHeaderId = receiptVoucherHeaderId,
					SalesInvoiceHeaderId = settlement.SalesInvoiceHeaderId,
					SettleValue = settlement.SettleValue,
					RemarksAr = settlement.RemarksAr,
					RemarksEn = settlement.RemarksEn,

					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor!.GetUserName(),
					IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				};

				settlement.SalesInvoiceSettlementId = newId;
				settlement.ReceiptVoucherHeaderId = receiptVoucherHeaderId;

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

		private async Task<bool> EditSalesInvoiceSettlement(List<SalesInvoiceSettlementDto> salesInvoiceSettlements)
		{
			var current = salesInvoiceSettlements.Where(x => x.SalesInvoiceSettlementId > 0).ToList();
			var modelList = new List<SalesInvoiceSettlement>();
			foreach (var settlement in current)
			{
				var model = new SalesInvoiceSettlement()
				{
					SalesInvoiceSettlementId = settlement.SalesInvoiceSettlementId,
					SalesInvoiceHeaderId = settlement.SalesInvoiceHeaderId,
					ReceiptVoucherHeaderId = settlement.ReceiptVoucherHeaderId,
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

		private async Task<bool> DeleteSalesInvoiceSettlements(List<SalesInvoiceSettlementDto> settlements, int headerId)
		{
			var current = _repository.GetAll().Where(x => x.ReceiptVoucherHeaderId == headerId).AsNoTracking().ToList();
			var toBeDeleted = current.Where(p => settlements.All(p2 => p2.SalesInvoiceSettlementId != p.SalesInvoiceSettlementId)).ToList();
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
			try { id = await _repository.GetAll().MaxAsync(a => a.SalesInvoiceSettlementId) + 1; } catch { id = 1; }
			return id;
		}


      
        public IQueryable<SalesInvoiceSettledValueDto> GetSalesInvoiceSettledValues()
        {
            var settledValues = _repository.GetAll().GroupBy(x => x.SalesInvoiceHeaderId)
                .Select(x => new SalesInvoiceSettledValueDto
                {
                    SalesInvoiceHeaderId = x.Key,
                    SettledValue = x.Sum(y => y.SettleValue)
                });

            return settledValues;
        }

        public IQueryable<ReceiptVoucherSettledValueDto> GetReceiptVoucherSettledValues()
        {
            var settledValues = _repository.GetAll().GroupBy(x => x.ReceiptVoucherHeaderId)
                .Select(x => new ReceiptVoucherSettledValueDto
                {
                    ReceiptVoucherHeaderId = x.Key,
                    SettledValue = x.Sum(y => y.SettleValue)
                });

            return settledValues;
        }
    }
}
