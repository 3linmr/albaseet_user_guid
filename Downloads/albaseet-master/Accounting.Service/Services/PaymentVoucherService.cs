using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.Journal;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Service.Logic.Approval;
using Inventory.CoreOne.Models.StaticData;
using Shared.Helper.Logic;

namespace Accounting.Service.Services
{
    public class PaymentVoucherService : IPaymentVoucherService
    {
        private readonly IPaymentVoucherHeaderService _paymentVoucherHeaderService;
        private readonly IPaymentVoucherDetailService _paymentVoucherDetailService;
        private readonly ICostCenterJournalDetailService _costCenterJournalDetailService;
        private readonly IJournalService _journalService;
        private readonly IMenuNoteService _menuNoteService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IStoreService _storeService;

        public PaymentVoucherService(IPaymentVoucherHeaderService paymentVoucherHeaderService, IPaymentVoucherDetailService paymentVoucherDetailService, ICostCenterJournalDetailService costCenterJournalDetailService, IJournalService journalService, IMenuNoteService menuNoteService, IPaymentMethodService paymentMethodService, IStoreService storeService)
        {
            _paymentVoucherHeaderService = paymentVoucherHeaderService;
            _paymentVoucherDetailService = paymentVoucherDetailService;
            _costCenterJournalDetailService = costCenterJournalDetailService;
            _journalService = journalService;
            _menuNoteService = menuNoteService;
            _paymentMethodService = paymentMethodService;
            _storeService = storeService;
        }
        public List<RequestChangesDto> GetPaymentVoucherRequestChanges(PaymentVoucherDto oldItem, PaymentVoucherDto newItem)
        {
            var requestChanges = new List<RequestChangesDto>();
            var items = CompareLogic.GetDifferences(oldItem.PaymentVoucherHeader, newItem.PaymentVoucherHeader);
            requestChanges.AddRange(items);

            if (oldItem.PaymentVoucherDetails.Any() && newItem.PaymentVoucherDetails.Any())
            {
                var oldCount = oldItem.PaymentVoucherDetails.Count(x => x.DebitValue > 0 || x.CreditValue > 0);
                var newCount = newItem.PaymentVoucherDetails.Count(x => x.DebitValue > 0 || x.CreditValue > 0);
                var index = 0;
                if (oldCount <= newCount)
                {
                    for (int i = index; i < oldCount; i++)
                    {
                        for (int j = index; j < newCount; j++)
                        {
                            var changes = CompareLogic.GetDifferences(oldItem.PaymentVoucherDetails.Where(x => x.DebitValue > 0 || x.CreditValue > 0).ToList()[i], newItem.PaymentVoucherDetails.Where(x => x.DebitValue > 0 || x.CreditValue > 0).ToList()[i]);
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

        public async Task<PaymentVoucherDto> GetPaymentVoucher(int paymentVoucherHeaderId)
        {
            var header = await _paymentVoucherHeaderService.GetPaymentVoucherHeaderById(paymentVoucherHeaderId);
            var detail = await _paymentVoucherDetailService.GetPaymentVoucherDetailsWithPaymentMethods(header.StoreId, paymentVoucherHeaderId);
            var costCenters = await _costCenterJournalDetailService.GetCostCenterJournalDetails(header.JournalHeaderId.GetValueOrDefault());
            var menuNotes = await _menuNoteService.GetMenuNotes(MenuCodeData.PaymentVoucher, paymentVoucherHeaderId).ToListAsync();
            return new PaymentVoucherDto() { PaymentVoucherHeader = header, PaymentVoucherDetails = detail, CostCenterJournalDetails = costCenters, MenuNotes = menuNotes };
        }



        public async Task<ResponseDto> SavePaymentVoucher(PaymentVoucherDto paymentVoucher, bool hasApprove, bool approved, int? requestId)
        {
            if (paymentVoucher.PaymentVoucherHeader != null)
            {
                var isUpdate = paymentVoucher.PaymentVoucherHeader.PaymentVoucherHeaderId > 0;
                var header = await _paymentVoucherHeaderService.SavePaymentVoucherHeader(paymentVoucher.PaymentVoucherHeader, hasApprove, approved, requestId);
                if (header.Success)
                {
                    await _paymentVoucherDetailService.SavePaymentVoucherDetail(paymentVoucher.PaymentVoucherDetails, header.Id);
                    var paymentsJournal = await RebuildPaymentVoucherDetails(paymentVoucher.PaymentVoucherDetails, paymentVoucher.PaymentVoucherHeader.StoreId);
                    var journal = HandleJournal(paymentVoucher.PaymentVoucherHeader, paymentsJournal, paymentVoucher.CostCenterJournalDetails, header.Id);
                    var newJournal = await _journalService.SaveJournal(journal, hasApprove, approved, requestId);
                    if (!isUpdate)
                    {
                        await _paymentVoucherHeaderService.UpdatePaymentVoucherWithJournalHeaderId(header.Id, newJournal.Id);
                    }
                    if (paymentVoucher.MenuNotes != null)
                    {
                        await _menuNoteService.SaveMenuNotes(paymentVoucher.MenuNotes, header.Id);
                    }
                }
                return header;
            }
            return new ResponseDto() { Success = false, Message = "Payment Voucher Header Is NULL" };
        }

        public JournalDto HandleJournal(PaymentVoucherHeaderDto header, List<PaymentVoucherDetailDto> details, List<CostCenterJournalDetailDto> costCenters, int paymentVoucherHeaderId)
        {
            return BuildJournal(header, details, costCenters, paymentVoucherHeaderId, header.JournalHeaderId.GetValueOrDefault());

        }
        public static JournalDto BuildJournal(PaymentVoucherHeaderDto header, List<PaymentVoucherDetailDto> details, List<CostCenterJournalDetailDto> costCenters, int paymentVoucherHeaderId, int journalHeaderId)
        {
            var distributed = costCenters.Any() && costCenters[0].Distributed;
            return new JournalDto()
            {
                JournalHeader = BuildJournalHeader(header!, paymentVoucherHeaderId, journalHeaderId),
                JournalDetails = BuildJournalDetail(details, journalHeaderId, costCenters.Any(), distributed),
                CostCenterJournalDetails = BuildCostCenterJournalDetail(costCenters)
            };
        }
        public static JournalHeaderDto BuildJournalHeader(PaymentVoucherHeaderDto header, int paymentVoucherHeaderId, int journalHeaderId)
        {
            return new JournalHeaderDto()
            {
                MenuCode = MenuCodeData.PaymentVoucher,
                MenuReferenceId = paymentVoucherHeaderId,
                IsSystematic = true,
                JournalTypeId = JournalTypeData.PaymentVoucher,
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

        public static List<JournalDetailDto> BuildJournalDetail(List<PaymentVoucherDetailDto> details, int journalHeaderId, bool hasCostCenters, bool isCostCenterDistributed)
        {
            var modelList = new List<JournalDetailDto>();
            foreach (var detail in details)
            {
                var model = new JournalDetailDto()
                {
                    JournalHeaderId = journalHeaderId,
                    JournalDetailId = detail.DebitValue > 0 ? -1 : 0,
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

        public async Task<List<PaymentVoucherDetailDto>> RebuildPaymentVoucherDetails(List<PaymentVoucherDetailDto> paymentVoucherDetails, int storeId)
        {
            var modelList = paymentVoucherDetails.Where(x => x.CreditValue > 0).ToList();
            var rounding = await _storeService.GetStoreRounding(storeId);
            foreach (var detail in paymentVoucherDetails.Where(x => x.DebitValue > 0))
            {
                var paymentMethod = await _paymentMethodService.GetAll().FirstOrDefaultAsync(x => x.PaymentMethodId == detail.PaymentMethodId);
                if (paymentMethod != null)
                {
                    if (paymentMethod.PaymentTypeId == PaymentTypeData.BankAccount || paymentMethod.PaymentTypeId == PaymentTypeData.BankCard || paymentMethod.PaymentTypeId == PaymentTypeData.Installment)
                    {
                        var methodEntry = await _paymentMethodService.GetPaymentMethodsEntry(storeId,detail.PaymentMethodId.GetValueOrDefault(), detail.DebitValue);
                        if (methodEntry != null)
                        {
                            var bankModel = new PaymentVoucherDetailDto()
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
                                PaymentVoucherHeaderId = detail.PaymentVoucherHeaderId,
                                PaymentVoucherDetailId = detail.PaymentVoucherDetailId,
                                CreatedAt = detail.CreatedAt,
                                UserNameCreated = detail.UserNameCreated,
                                IpAddressCreated = detail.IpAddressCreated
                            };
                            modelList.Add(bankModel);

                            if (methodEntry.CommissionValue > 0)
                            {
	                            var commissionModel = new PaymentVoucherDetailDto()
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
									PaymentVoucherHeaderId = detail.PaymentVoucherHeaderId,
		                            PaymentVoucherDetailId = detail.PaymentVoucherDetailId,
		                            CreatedAt = detail.CreatedAt,
		                            UserNameCreated = detail.UserNameCreated,
		                            IpAddressCreated = detail.IpAddressCreated
	                            };
	                            modelList.Add(commissionModel);
                            }

                            if (methodEntry.CommissionTaxValue > 0)
                            {
	                            var commissionTaxModel = new PaymentVoucherDetailDto()
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
									PaymentVoucherHeaderId = detail.PaymentVoucherHeaderId,
		                            PaymentVoucherDetailId = detail.PaymentVoucherDetailId,
		                            CreatedAt = detail.CreatedAt,
		                            UserNameCreated = detail.UserNameCreated,
		                            IpAddressCreated = detail.IpAddressCreated
	                            };
	                            modelList.Add(commissionTaxModel);
                            }

                            if (methodEntry.FixedCommissionValue > 0)
                            {
	                            var fixedCommissionModel = new PaymentVoucherDetailDto()
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
									PaymentVoucherHeaderId = detail.PaymentVoucherHeaderId,
		                            PaymentVoucherDetailId = detail.PaymentVoucherDetailId,
		                            CreatedAt = detail.CreatedAt,
		                            UserNameCreated = detail.UserNameCreated,
		                            IpAddressCreated = detail.IpAddressCreated
	                            };
	                            modelList.Add(fixedCommissionModel);
                            }

                            if (methodEntry.FixedCommissionTaxValue > 0)
                            {
	                            var fixedCommissionTaxModel = new PaymentVoucherDetailDto()
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
									PaymentVoucherHeaderId = detail.PaymentVoucherHeaderId,
		                            PaymentVoucherDetailId = detail.PaymentVoucherDetailId,
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
                x.DebitValue = x.DebitValue;
                x.CostCenterId = x.CostCenterId;
                x.RemarksAr = x.RemarksAr;
                x.RemarksEn = x.RemarksEn;
            }
            return costCenters;
        }


        public async Task<ResponseDto> DeletePaymentVoucher(int paymentVoucherHeaderId)
        {
            var paymentHeader = await _paymentVoucherHeaderService.GetPaymentVoucherHeaderById(paymentVoucherHeaderId);
            await _paymentVoucherDetailService.DeletePaymentVoucherDetail(paymentVoucherHeaderId);
            var result = await _paymentVoucherHeaderService.DeletePaymentVoucherHeader(paymentVoucherHeaderId);
            await _journalService.DeleteJournal(paymentHeader.JournalHeaderId.GetValueOrDefault());
            return result;
        }
    }
}