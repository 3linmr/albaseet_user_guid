using Accounting.CoreOne.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using System.Diagnostics.Metrics;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.Service.Logic.Approval;
using Inventory.CoreOne.Models.StaticData;
using Shared.Helper.Logic;

namespace Accounting.Service.Services
{
	public class ReceiptVoucherService : IReceiptVoucherService
	{
		private readonly IReceiptVoucherHeaderService _receiptVoucherHeaderService;
		private readonly IReceiptVoucherDetailService _receiptVoucherDetailService;
		private readonly ICostCenterJournalDetailService _costCenterJournalDetailService;
		private readonly IJournalService _journalService;
		private readonly IMenuNoteService _menuNoteService;
		private readonly IPaymentMethodService _paymentMethodService;
		private readonly IStoreService _storeService;

		public ReceiptVoucherService(IReceiptVoucherHeaderService receiptVoucherHeaderService, IReceiptVoucherDetailService receiptVoucherDetailService, ICostCenterJournalDetailService costCenterJournalDetailService, IJournalService journalService, IMenuNoteService menuNoteService, IPaymentMethodService paymentMethodService, IStoreService storeService)
		{
			_receiptVoucherHeaderService = receiptVoucherHeaderService;
			_receiptVoucherDetailService = receiptVoucherDetailService;
			_costCenterJournalDetailService = costCenterJournalDetailService;
			_journalService = journalService;
			_menuNoteService = menuNoteService;
			_paymentMethodService = paymentMethodService;
			_storeService = storeService;
		}
		public List<RequestChangesDto> GetReceiptVoucherRequestChanges(ReceiptVoucherDto oldItem, ReceiptVoucherDto newItem)
		{

			var requestChanges = new List<RequestChangesDto>();
			var items = CompareLogic.GetDifferences(oldItem.ReceiptVoucherHeader, newItem.ReceiptVoucherHeader);
			requestChanges.AddRange(items);

			if (oldItem.ReceiptVoucherDetails.Any() && newItem.ReceiptVoucherDetails.Any())
			{
				var oldCount = oldItem.ReceiptVoucherDetails.Count(x => x.DebitValue > 0 || x.CreditValue > 0);
				var newCount = newItem.ReceiptVoucherDetails.Count(x => x.DebitValue > 0 || x.CreditValue > 0);
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.ReceiptVoucherDetails.Where(x => x.DebitValue > 0 || x.CreditValue > 0).ToList()[i], newItem.ReceiptVoucherDetails.Where(x => x.DebitValue > 0 || x.CreditValue > 0).ToList()[i]);
							requestChanges.AddRange(changes);
							index++;
							break;
						}
					}
				}
			}
			if (oldItem.CostCenterJournalDetails.Any() && newItem.CostCenterJournalDetails.Any())
			{
				var oldCount = oldItem.CostCenterJournalDetails.Count;
				var newCount = newItem.CostCenterJournalDetails.Count;
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.CostCenterJournalDetails[i], newItem.CostCenterJournalDetails[i]);
							requestChanges.AddRange(changes);
							index++;
							break;
						}
					}
				}
			}
			if (oldItem.MenuNotes != null && oldItem.MenuNotes.Any() && newItem.MenuNotes != null && newItem.MenuNotes.Any())
			{
				var oldCount = oldItem.MenuNotes.Count;
				var newCount = newItem.MenuNotes.Count;
				var index = 0;
				if (oldCount <= newCount)
				{
					for (int i = index; i < oldCount; i++)
					{
						for (int j = index; j < newCount; j++)
						{
							var changes = CompareLogic.GetDifferences(oldItem.MenuNotes[i], newItem.MenuNotes[i]);
							requestChanges.AddRange(changes);
							index++;
							break;
						}
					}
				}
			}
			return requestChanges;
		}

		public async Task<ReceiptVoucherDto> GetReceiptVoucher(int receiptVoucherHeaderId)
		{
			var header = await _receiptVoucherHeaderService.GetReceiptVoucherHeaderById(receiptVoucherHeaderId);
			var detail = await _receiptVoucherDetailService.GetReceiptVoucherDetailsWithPaymentMethods(header.StoreId, receiptVoucherHeaderId);
			var costCenters = await _costCenterJournalDetailService.GetCostCenterJournalDetails(header.JournalHeaderId.GetValueOrDefault());
			var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.ReceiptVoucher, receiptVoucherHeaderId).ToListAsync();
			return new ReceiptVoucherDto() { ReceiptVoucherHeader = header, ReceiptVoucherDetails = detail, CostCenterJournalDetails = costCenters, MenuNotes = menuNotes };
		}



		public async Task<ResponseDto> SaveReceiptVoucher(ReceiptVoucherDto receiptVoucher, bool hasApprove, bool approved, int? requestId)
		{
			if (receiptVoucher.ReceiptVoucherHeader != null)
			{
				var isUpdate = receiptVoucher.ReceiptVoucherHeader.ReceiptVoucherHeaderId > 0;
				var header = await _receiptVoucherHeaderService.SaveReceiptVoucherHeader(receiptVoucher.ReceiptVoucherHeader, hasApprove, approved, requestId);
				if (header.Success)
				{
					await _receiptVoucherDetailService.SaveReceiptVoucherDetail(receiptVoucher.ReceiptVoucherDetails, header.Id);
					var receiptsJournal = await RebuildReceiptVoucherDetails(receiptVoucher.ReceiptVoucherDetails, receiptVoucher.ReceiptVoucherHeader.StoreId);
					var journal = HandleJournal(receiptVoucher.ReceiptVoucherHeader, receiptsJournal, receiptVoucher.CostCenterJournalDetails, header.Id);
					var newJournal = await _journalService.SaveJournal(journal, hasApprove, approved, requestId);
					if (!isUpdate)
					{
						await _receiptVoucherHeaderService.UpdateReceiptVoucherWithJournalHeaderId(header.Id, newJournal.Id);
					}
					if (receiptVoucher.MenuNotes != null)
					{
						await _menuNoteService.SaveMenuNotes(receiptVoucher.MenuNotes, header.Id);
					}
				}
				return header;
			}
			return new ResponseDto() { Success = false, Message = "Receipt Voucher Header Is NULL" };
		}

		public JournalDto HandleJournal(ReceiptVoucherHeaderDto header, List<ReceiptVoucherDetailDto> details, List<CostCenterJournalDetailDto> costCenters, int receiptVoucherHeaderId)
		{
			return BuildJournal(header, details, costCenters, receiptVoucherHeaderId, header.JournalHeaderId.GetValueOrDefault());

		}
		public static JournalDto BuildJournal(ReceiptVoucherHeaderDto header, List<ReceiptVoucherDetailDto> details, List<CostCenterJournalDetailDto> costCenters, int receiptVoucherHeaderId, int journalHeaderId)
		{
			var distributed = costCenters.Any() && costCenters[0].Distributed;
			return new JournalDto()
			{
				JournalHeader = BuildJournalHeader(header!, receiptVoucherHeaderId, journalHeaderId),
				JournalDetails = BuildJournalDetail(details, journalHeaderId, costCenters.Any(), distributed),
				CostCenterJournalDetails = BuildCostCenterJournalDetail(costCenters)
			};
		}
		public static JournalHeaderDto BuildJournalHeader(ReceiptVoucherHeaderDto header, int receiptVoucherHeaderId, int journalHeaderId)
		{
			return new JournalHeaderDto()
			{
				MenuCode = MenuCodeData.ReceiptVoucher,
				MenuReferenceId = receiptVoucherHeaderId,
				IsSystematic = true,
				JournalTypeId = JournalTypeData.ReceiptVoucher,
				PeerReference = header.PeerReference,
				RemarksAr = header.RemarksAr,
				RemarksEn = header.RemarksEn,
				StoreId = header.StoreId,
				TicketDate = header.DocumentDate,
				JournalHeaderId = journalHeaderId,
                TotalDebitValue = header.TotalDebitValue,
                TotalCreditValue = header.TotalCreditValue,
                TotalDebitValueAccount = header.TotalDebitValueAccount,
                TotalCreditValueAccount = header.TotalCreditValueAccount
            };
		}

		public static List<JournalDetailDto> BuildJournalDetail(List<ReceiptVoucherDetailDto> details, int journalHeaderId, bool hasCostCenters, bool isCostCenterDistributed)
		{
			var modelList = new List<JournalDetailDto>();
			foreach (var detail in details)
			{
				var model = new JournalDetailDto()
				{
					JournalHeaderId = journalHeaderId,
					JournalDetailId = detail.CreditValue > 0 ? -1 : 0,
					AccountId = detail.AccountId,
					CurrencyId = detail.CurrencyId,
					DebitValue = detail.DebitValue,
					CreditValue = detail.CreditValue,
					DebitValueAccount = detail.DebitValueAccount,
					CreditValueAccount = detail.CreditValueAccount,
					CurrencyRate = detail.CurrencyRate,
					IsLinkedToCostCenters = hasCostCenters,
					IsCostCenterDistributed = isCostCenterDistributed,
					IsSystematic = true,
					RemarksAr = detail.RemarksAr,
					RemarksEn = detail.RemarksEn
				};
				modelList.Add(model);
			}
			return modelList;
		}

		public async Task<List<ReceiptVoucherDetailDto>> RebuildReceiptVoucherDetails(List<ReceiptVoucherDetailDto> receiptVoucherDetails, int storeId)
		{
			var modelList = receiptVoucherDetails.Where(x => x.CreditValue > 0).ToList();
			var rounding = await _storeService.GetStoreRounding(storeId);
			foreach (var detail in receiptVoucherDetails.Where(x => x.DebitValue > 0))
			{
				var paymentMethod = await _paymentMethodService.GetAll().FirstOrDefaultAsync(x => x.PaymentMethodId == detail.PaymentMethodId);
				if (paymentMethod != null)
				{

					var methodEntry = await _paymentMethodService.GetPaymentMethodsEntry(storeId,detail.PaymentMethodId.GetValueOrDefault(), detail.DebitValue);
					if (methodEntry != null)
					{
						var bankModel = new ReceiptVoucherDetailDto()
						{
							AccountId = detail.AccountId,
							CurrencyId = detail.CurrencyId,
							CurrencyRate = detail.CurrencyRate,
							PaymentMethodId = detail.PaymentMethodId,
							DebitValue = methodEntry.BankValue,
							DebitValueAccount = NumberHelper.RoundNumber(methodEntry.BankValue * detail.CurrencyRate, rounding),
							CreditValue = 0,
							CreditValueAccount = 0,
							RemarksAr = detail.RemarksAr,
							RemarksEn = detail.RemarksEn,
							ReceiptVoucherHeaderId = detail.ReceiptVoucherHeaderId,
							ReceiptVoucherDetailId = detail.ReceiptVoucherDetailId,
							CreatedAt = detail.CreatedAt,
							UserNameCreated = detail.UserNameCreated,
							IpAddressCreated = detail.IpAddressCreated
						};
						modelList.Add(bankModel);

						if (methodEntry.CommissionValue > 0)
						{
							var commissionModel = new ReceiptVoucherDetailDto()
							{
								AccountId = methodEntry.CommissionAccountId,
								CurrencyId = methodEntry.CommissionCurrencyId,
								CurrencyRate = methodEntry.CommissionCurrencyRate,
								PaymentMethodId = detail.PaymentMethodId,
								DebitValue = methodEntry.CommissionValue,
								DebitValueAccount = NumberHelper.RoundNumber(methodEntry.CommissionValue * methodEntry.CommissionCurrencyRate, rounding),
								CreditValue = 0,
								CreditValueAccount = 0,
								RemarksAr = $"{VoucherCommissionData.CommissionRemarksAr} {methodEntry.CommissionAccountNameAr}",
								RemarksEn = $"{VoucherCommissionData.CommissionRemarksEn} {methodEntry.CommissionAccountNameEn}",
								ReceiptVoucherHeaderId = detail.ReceiptVoucherHeaderId,
								ReceiptVoucherDetailId = detail.ReceiptVoucherDetailId,
								CreatedAt = detail.CreatedAt,
								UserNameCreated = detail.UserNameCreated,
								IpAddressCreated = detail.IpAddressCreated
							};
							modelList.Add(commissionModel);
						}

						if (methodEntry.CommissionTaxValue > 0)
						{
							var commissionTaxModel = new ReceiptVoucherDetailDto()
							{
								AccountId = methodEntry.CommissionTaxAccountId,
								CurrencyId = methodEntry.CommissionTaxCurrencyId,
								CurrencyRate = methodEntry.CommissionTaxCurrencyRate,
								PaymentMethodId = detail.PaymentMethodId,
								DebitValue = methodEntry.CommissionTaxValue,
								DebitValueAccount = NumberHelper.RoundNumber(methodEntry.CommissionTaxValue * methodEntry.CommissionTaxCurrencyRate, rounding),
								CreditValue = 0,
								CreditValueAccount = 0,
								RemarksAr = $"{VoucherCommissionData.CommissionTaxRemarksAr} {methodEntry.CommissionTaxAccountNameAr}",
								RemarksEn = $"{VoucherCommissionData.CommissionTaxRemarksEn} {methodEntry.CommissionTaxAccountNameEn}",
								ReceiptVoucherHeaderId = detail.ReceiptVoucherHeaderId,
								ReceiptVoucherDetailId = detail.ReceiptVoucherDetailId,
								CreatedAt = detail.CreatedAt,
								UserNameCreated = detail.UserNameCreated,
								IpAddressCreated = detail.IpAddressCreated
							};
							modelList.Add(commissionTaxModel);
						}

						if (methodEntry.FixedCommissionValue > 0)
						{
							var fixedCommissionModel = new ReceiptVoucherDetailDto()
							{
								AccountId = methodEntry.CommissionAccountId,
								CurrencyId = methodEntry.CommissionCurrencyId,
								CurrencyRate = methodEntry.CommissionCurrencyRate,
								PaymentMethodId = detail.PaymentMethodId,
								DebitValue = methodEntry.FixedCommissionValue,
								DebitValueAccount = NumberHelper.RoundNumber(methodEntry.FixedCommissionValue * methodEntry.CommissionCurrencyRate, rounding),
								CreditValue = 0,
								CreditValueAccount = 0,
								RemarksAr = $"{VoucherCommissionData.FixedCommissionRemarksAr} {methodEntry.CommissionAccountNameAr}",
								RemarksEn = $"{VoucherCommissionData.FixedCommissionRemarksEn} {methodEntry.CommissionAccountNameEn}",
								ReceiptVoucherHeaderId = detail.ReceiptVoucherHeaderId,
								ReceiptVoucherDetailId = detail.ReceiptVoucherDetailId,
								CreatedAt = detail.CreatedAt,
								UserNameCreated = detail.UserNameCreated,
								IpAddressCreated = detail.IpAddressCreated
							};
							modelList.Add(fixedCommissionModel);
						}


						if (methodEntry.FixedCommissionTaxValue > 0)
						{
							var fixedCommissionTaxModel = new ReceiptVoucherDetailDto()
							{
								AccountId = methodEntry.CommissionTaxAccountId,
								CurrencyId = methodEntry.CommissionTaxCurrencyId,
								CurrencyRate = methodEntry.CommissionTaxCurrencyRate,
								PaymentMethodId = detail.PaymentMethodId,
								DebitValue = methodEntry.FixedCommissionTaxValue,
								DebitValueAccount = NumberHelper.RoundNumber(methodEntry.FixedCommissionTaxValue * methodEntry.CommissionTaxCurrencyRate, rounding),
								CreditValue = 0,
								CreditValueAccount = 0,
								RemarksAr = $"{VoucherCommissionData.FixedCommissionTaxRemarksAr} {methodEntry.CommissionTaxAccountNameAr}",
								RemarksEn = $"{VoucherCommissionData.FixedCommissionTaxRemarksEn} {methodEntry.CommissionTaxAccountNameEn}",
								ReceiptVoucherHeaderId = detail.ReceiptVoucherHeaderId,
								ReceiptVoucherDetailId = detail.ReceiptVoucherDetailId,
								CreatedAt = detail.CreatedAt,
								UserNameCreated = detail.UserNameCreated,
								IpAddressCreated = detail.IpAddressCreated
							};
							modelList.Add(fixedCommissionTaxModel);
						}
					}
					else
					{
						modelList.Add(detail);
					}

				}
				else
				{
					modelList.Add(detail);
				}

			}
			return modelList;
		}
		public static List<CostCenterJournalDetailDto> BuildCostCenterJournalDetail(List<CostCenterJournalDetailDto> costCenters)
		{
			foreach (var x in costCenters)
			{
				x.JournalDetailId = -1;
				x.CostCenterJournalDetailId = 0;
				x.CreditValue = x.CreditValue;
				x.CostCenterId = x.CostCenterId;
				x.RemarksAr = x.RemarksAr;
				x.RemarksEn = x.RemarksEn;
			}
			return costCenters;
		}


		public async Task<ResponseDto> DeleteReceiptVoucher(int receiptVoucherHeaderId)
		{
			var receiptHeader = await _receiptVoucherHeaderService.GetReceiptVoucherHeaderById(receiptVoucherHeaderId);
			await _menuNoteService.DeleteMenuNotes(MenuCodeData.ReceiptVoucher, receiptVoucherHeaderId);
			await _receiptVoucherDetailService.DeleteReceiptVoucherDetail(receiptVoucherHeaderId);
			var result = await _receiptVoucherHeaderService.DeleteReceiptVoucherHeader(receiptVoucherHeaderId);
			await _journalService.DeleteJournal(receiptHeader.JournalHeaderId.GetValueOrDefault());
			return result;
		}
	}
}
