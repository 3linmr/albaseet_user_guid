using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Logic.Calculation;
using Shared.Service.Validators;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.Helper.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
	public class PaymentMethodService : BaseService<PaymentMethod>,IPaymentMethodService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IPaymentTypeService _paymentTypeService;
		private readonly ICompanyService _companyService;
		private readonly IAccountService _accountService;
		private readonly IStringLocalizer<PaymentMethodService> _localizer;
		private readonly ICurrencyService _currencyService;
		private readonly ICurrencyRateService _currencyRateService;
		private readonly IBranchService _branchService;
		private readonly IStoreService _storeService;
		private readonly ITaxService _taxService;
		private readonly ITaxPercentService _taxPercentService;

		public PaymentMethodService(IRepository<PaymentMethod> repository,IHttpContextAccessor httpContextAccessor,IPaymentTypeService paymentTypeService,ICompanyService companyService,IAccountService accountService,IStringLocalizer<PaymentMethodService> localizer,ICurrencyService currencyService,ICurrencyRateService currencyRateService,IBranchService branchService,IStoreService storeService,ITaxService taxService,ITaxPercentService taxPercentService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_paymentTypeService = paymentTypeService;
			_companyService = companyService;
			_accountService = accountService;
			_localizer = localizer;
			_currencyService = currencyService;
			_currencyRateService = currencyRateService;
			_branchService = branchService;
			_storeService = storeService;
			_taxService = taxService;
			_taxPercentService = taxPercentService;
		}

		public IQueryable<PaymentMethodDto> GetAllPaymentMethods()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from paymentMethod in _repository.GetAll()
				from paymentType in _paymentTypeService.GetAll().Where(x=>x.PaymentTypeId == paymentMethod.PaymentTypeId)
				from company in _companyService.GetAll().Where(x => x.CompanyId == paymentMethod.CompanyId)
				from paymentAccount in _accountService.GetAll().Where(x=>x.AccountId == paymentMethod.PaymentAccountId)
				from currency in _currencyService.GetAll().Where(x => x.CurrencyId == paymentAccount.CurrencyId)
				from tax in _taxService.GetAll().Where(x=>x.TaxId == paymentMethod.TaxId).DefaultIfEmpty()
				from commissionAccount in _accountService.GetAll().Where(x=>x.AccountId == paymentMethod.CommissionAccountId).DefaultIfEmpty()
				from commissionTaxAccount in _accountService.GetAll().Where(x=>x.AccountId == paymentMethod.CommissionTaxAccountId).DefaultIfEmpty()
				from currencyRate in _currencyRateService.GetAll().Where(x => x.FromCurrencyId == company.CurrencyId && x.ToCurrencyId == currency.CurrencyId).DefaultIfEmpty()
				select new PaymentMethodDto
				{
					PaymentMethodId = paymentMethod.PaymentMethodId,
					PaymentMethodCode = paymentMethod.PaymentMethodCode,
					PaymentMethodName = language == LanguageCode.Arabic ? paymentMethod.PaymentMethodNameAr : paymentMethod.PaymentMethodNameEn,
					PaymentMethodNameAr = paymentMethod.PaymentMethodNameAr,
					PaymentMethodNameEn = paymentMethod.PaymentMethodNameEn,
					CompanyId = paymentMethod.CompanyId,
					CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
					FixedCommissionValue = paymentMethod.FixedCommissionValue,
					CommissionPercent = paymentMethod.CommissionPercent,
					MinCommissionValue = paymentMethod.MinCommissionValue,
					MaxCommissionValue = paymentMethod.MaxCommissionValue,
					PaymentAccountId = paymentMethod.PaymentAccountId,
					CommissionTaxAccountId = paymentMethod.CommissionTaxAccountId,
					CommissionAccountId = paymentMethod.CommissionAccountId,
					PaymentTypeId = paymentMethod.PaymentTypeId,
					PaymentTypeName = language == LanguageCode.Arabic ? paymentType.PaymentTypeNameAr : paymentType.PaymentTypeNameEn,
					PaymentAccountCode = paymentAccount.AccountCode,
					PaymentAccountName = language == LanguageCode.Arabic ? paymentAccount.AccountNameAr : paymentAccount.AccountNameEn,
                    CurrencyId = paymentAccount.CurrencyId,
                    CurrencyRate = currencyRate.CurrencyRateValue,
					CurrencyName = language == LanguageCode.Arabic ? currency.CurrencyNameAr : currency.CurrencyNameEn,
					CommissionAccountCode = commissionAccount != null ? commissionAccount.AccountCode : "",
					CommissionAccountName = commissionAccount != null ? (language == LanguageCode.Arabic ? commissionAccount.AccountNameAr : commissionAccount.AccountNameEn) : "",
					CommissionTaxAccountCode = commissionTaxAccount != null ? commissionTaxAccount.AccountCode : "",
					CommissionTaxAccountName = commissionTaxAccount != null ? (language == LanguageCode.Arabic ? commissionTaxAccount.AccountNameAr : commissionTaxAccount.AccountNameEn) : "",
					IsActive = paymentMethod.IsActive,
					InActiveReasons = paymentMethod.InActiveReasons,
					TaxId = paymentMethod.TaxId,
					TaxName = tax != null ? (language == LanguageCode.Arabic ? tax.TaxNameAr : tax.TaxNameEn) : "",
					IsPaymentMethod = paymentMethod.IsPaymentMethod,
					IsReceivingMethod = paymentMethod.IsReceivingMethod,
					CommissionHasVat = paymentMethod.CommissionHasVat,
					CommissionVatInclusive = paymentMethod.CommissionVatInclusive,
					FixedCommissionHasVat = paymentMethod.FixedCommissionHasVat,
					FixedCommissionVatInclusive = paymentMethod.FixedCommissionVatInclusive
				};
			return data;
		}

        public IQueryable<PaymentMethodDto> GetUserPaymentMethods()
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return GetAllPaymentMethods().Where(x => x.CompanyId == companyId);
		}


        public IQueryable<PaymentMethodDto> GetCompanyPaymentMethods(int companyId)
		{
			return GetAllPaymentMethods().Where(x => x.CompanyId == companyId && x.IsActive);
		}

		public IQueryable<PaymentMethodDropDownDto> GetPaymentMethodsDropDown(int companyId)
		{
			return GetAllPaymentMethods().Where(x => x.CompanyId == companyId).Select(x=> new PaymentMethodDropDownDto()
			{
				PaymentMethodId = x.PaymentMethodId,
				PaymentMethodName = x.PaymentAccountName
			}).OrderBy(x => x.PaymentMethodName);
		}

		public IQueryable<AccountAutoCompleteDto> GetPaymentMethodsAccounts()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();

			var data =
				from paymentMethod in _repository.GetAll().Where(x=>x.IsPaymentMethod && x.IsActive && x.CompanyId == companyId)
				from account in _accountService.GetAll().Where(x=>x.IsActive && x.CompanyId == paymentMethod.CompanyId && x.AccountId == paymentMethod.PaymentAccountId)
				group new { account , paymentMethod } by new { account.AccountId , account.AccountCode, account.AccountNameAr, account.AccountNameEn, account.CurrencyId, account.AccountTypeId } into g
				select new AccountAutoCompleteDto
				{
					AccountId = g.Key.AccountId,
					AccountCode = g.Key.AccountCode,
					AccountName =  language == LanguageCode.Arabic ? g.Key.AccountNameAr : g.Key.AccountNameEn,
					CurrencyId = g.Key.CurrencyId,
					AccountTypeId = g.Key.AccountTypeId
				};
			return data;
		}

		public IQueryable<AccountAutoCompleteDto> GetReceivingMethodsAccounts()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();

			var data =
				from paymentMethod in _repository.GetAll().Where(x => x.IsReceivingMethod && x.IsActive && x.CompanyId == companyId)
				from account in _accountService.GetAll().Where(x => x.IsActive && x.CompanyId == paymentMethod.CompanyId && x.AccountId == paymentMethod.PaymentAccountId)
				group new { account, paymentMethod } by new { account.AccountId, account.AccountCode, account.AccountNameAr, account.AccountNameEn, account.CurrencyId, account.AccountTypeId } into g
				select new AccountAutoCompleteDto
				{
					AccountId = g.Key.AccountId,
					AccountCode = g.Key.AccountCode,
					AccountName = language == LanguageCode.Arabic ? g.Key.AccountNameAr : g.Key.AccountNameEn,
					CurrencyId = g.Key.CurrencyId,
					AccountTypeId = g.Key.AccountTypeId
				};
			return data;
		}

		public async Task<List<VoucherPaymentMethodDto>> GetVoucherPaymentMethods(int storeId, bool isPayment, bool isOnlyCash)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				await (from paymentMethod in _repository.GetAll().Where(x=>x.IsActive)
				from paymentType in _paymentTypeService.GetAll().Where(x => x.PaymentTypeId == paymentMethod.PaymentTypeId)
				from company in _companyService.GetAll().Where(x => x.CompanyId == paymentMethod.CompanyId)
				from branch in _branchService.GetAll().Where(x=>x.CompanyId == company.CompanyId)
				from store in _storeService.GetAll().Where(x=>x.BranchId == branch.BranchId && x.StoreId == storeId) 
				from paymentAccount in _accountService.GetAll().Where(x => x.AccountId == paymentMethod.PaymentAccountId)
				from currency in _currencyService.GetAll().Where(x=>x.CurrencyId == paymentAccount.CurrencyId)
				from currencyRate in _currencyRateService.GetAll().Where(x=>x.FromCurrencyId == company.CurrencyId && x.ToCurrencyId == currency.CurrencyId ).DefaultIfEmpty()
				from commissionAccount in _accountService.GetAll().Where(x => x.AccountId == paymentMethod.CommissionAccountId).DefaultIfEmpty()
				from commissionTaxAccount in _accountService.GetAll().Where(x => x.AccountId == paymentMethod.CommissionTaxAccountId).DefaultIfEmpty()
				select new VoucherPaymentMethodDto
				{
					PaymentMethodId = paymentMethod.PaymentMethodId,
					PaymentMethodName = language == LanguageCode.Arabic ? paymentMethod.PaymentMethodNameAr : paymentMethod.PaymentMethodNameEn,
					AccountId = paymentMethod.PaymentAccountId,
					AccountCode = paymentAccount.AccountCode,
					AccountName = language == LanguageCode.Arabic ? paymentAccount.AccountNameAr : paymentAccount.AccountNameEn,
					CurrencyId = paymentAccount.CurrencyId,
					CurrencyName = language == LanguageCode.Arabic ? currency.CurrencyNameAr : currency.CurrencyNameEn,
					CurrencyRate = currencyRate != null ? currencyRate.CurrencyRateValue : 1,
					PaymentTypeId = paymentMethod.PaymentTypeId,
					FixedCommissionValue = paymentMethod.CommissionPercent,
					CommissionPercent = paymentMethod.CommissionPercent,
					MinCommissionValue = paymentMethod.MinCommissionValue,
					MaxCommissionValue = paymentMethod.MaxCommissionValue,
					CommissionAccountId = paymentMethod.CommissionAccountId,
					CommissionTaxAccountId = paymentMethod.CommissionTaxAccountId,
					CommissionTaxId = paymentMethod.TaxId,
					IsReceivingMethod = paymentMethod.IsReceivingMethod,
					IsPaymentMethod = paymentMethod.IsPaymentMethod,
				}).ToListAsync();

			if (isPayment)
			{
				return isOnlyCash ? data.Where(x => x.IsPaymentMethod && x.PaymentTypeId == PaymentTypeData.Cash).ToList() : data.Where(x => x.IsPaymentMethod).ToList();
			}
			else
			{
				return isOnlyCash ? data.Where(x => x.IsReceivingMethod && x.PaymentTypeId == PaymentTypeData.Cash).ToList() : data.Where(x => x.IsReceivingMethod).ToList();
			}
		}

		public async Task<VoucherPaymentMethodDto?> GetVoucherPaymentMethod(int storeId, int paymentMethodId)
		{
			var methods =  await GetVoucherPaymentMethods(storeId, false,false);
			return methods.FirstOrDefault(x => x.PaymentMethodId == paymentMethodId);
		}

		public async Task<JournalDetailCalculationVm> GetPaymentMethodEntries(int storeId, int paymentMethodId, decimal debitValue, decimal creditValue)
		{
			var modelList = new List<JournalDetailCalculationDto>();
			ResponseDto response;

			var rounding = await _storeService.GetStoreRounding(storeId);
			var paymentMethod = await GetVoucherPaymentMethod(storeId, paymentMethodId);

			if (paymentMethod != null)
			{
				var entryValue = debitValue + creditValue;
				var methodEntry = await GetPaymentMethodsEntry(storeId,paymentMethodId, entryValue);

				if (methodEntry != null)
				{
					if (methodEntry.FixedCommissionValue + methodEntry.FixedCommissionTaxValue >= (entryValue))
					{
						response = new ResponseDto() { Success = false, Message = _localizer["FixedCommissionMoreThanMethodValue"] };
					}
					else if (methodEntry.CommissionValue + methodEntry.CommissionTaxValue + methodEntry.FixedCommissionValue + methodEntry.FixedCommissionTaxValue >= (entryValue))
					{
						response = new ResponseDto() { Success = false, Message = _localizer["CommissionMoreThanMethodValue"] };
					}
					else
					{
						var bankModel = new JournalDetailCalculationDto()
						{
							AccountId = paymentMethod.AccountId,
							CurrencyId = paymentMethod.CurrencyId,
							CurrencyRate = paymentMethod.CurrencyRate,
							DebitValue = debitValue > 0 ? methodEntry.BankValue : 0,
							DebitValueAccount = debitValue > 0 ? NumberHelper.RoundNumber(methodEntry.BankValue * paymentMethod.CurrencyRate, rounding) : 0,
							CreditValue = creditValue > 0 ? methodEntry.BankValue : 0,
							CreditValueAccount = creditValue > 0 ? NumberHelper.RoundNumber(methodEntry.BankValue * paymentMethod.CurrencyRate, rounding) : 0,
							AutomaticRemarks = "PaymentMethodValue"
						};
						modelList.Add(bankModel);

						if (methodEntry.CommissionValue > 0)
						{
							var commissionModel = new JournalDetailCalculationDto()
							{
								AccountId = methodEntry.CommissionAccountId,
								CurrencyId = methodEntry.CommissionCurrencyId,
								CurrencyRate = methodEntry.CommissionCurrencyRate,
								DebitValue = debitValue > 0 ? methodEntry.CommissionValue : 0,
								DebitValueAccount = debitValue > 0 ? NumberHelper.RoundNumber(methodEntry.CommissionValue * methodEntry.CommissionCurrencyRate, rounding) : 0,
								CreditValue = creditValue > 0 ? methodEntry.CommissionValue : 0,
								CreditValueAccount = creditValue > 0 ? NumberHelper.RoundNumber(methodEntry.CommissionValue * methodEntry.CommissionCurrencyRate, rounding) : 0,
								AutomaticRemarks = "CommissionValue"
							};
							modelList.Add(commissionModel);
						}

						if (methodEntry.CommissionTaxValue > 0)
						{
							var commissionTaxModel = new JournalDetailCalculationDto()
							{
								AccountId = methodEntry.CommissionTaxAccountId,
								CommissionTaxId = methodEntry.CommissionTaxId,
								CommissionTaxTypeId = methodEntry.CommissionTaxTypeId,
								CurrencyId = methodEntry.CommissionTaxCurrencyId,
								CurrencyRate = methodEntry.CommissionTaxCurrencyRate,
								DebitValue = debitValue > 0 ? methodEntry.CommissionTaxValue : 0,
								DebitValueAccount = debitValue > 0 ? NumberHelper.RoundNumber(methodEntry.CommissionTaxValue * methodEntry.CommissionTaxCurrencyRate, rounding) : 0,
								CreditValue = creditValue > 0 ? methodEntry.CommissionTaxValue : 0,
								CreditValueAccount = creditValue > 0 ? NumberHelper.RoundNumber(methodEntry.CommissionTaxValue * methodEntry.CommissionTaxCurrencyRate, rounding) : 0,
								AutomaticRemarks = "CommissionTaxValue",
							};
							modelList.Add(commissionTaxModel);
						}

						if (methodEntry.FixedCommissionValue > 0)
						{
							var fixedCommissionModel = new JournalDetailCalculationDto()
							{
								AccountId = methodEntry.CommissionAccountId,
								CurrencyId = methodEntry.CommissionCurrencyId,
								CurrencyRate = methodEntry.CommissionCurrencyRate,
								DebitValue = debitValue > 0 ? methodEntry.FixedCommissionValue : 0,
								DebitValueAccount = debitValue > 0 ? NumberHelper.RoundNumber(methodEntry.FixedCommissionValue * methodEntry.CommissionCurrencyRate, rounding) : 0,
								CreditValue = creditValue > 0 ? methodEntry.FixedCommissionValue : 0,
								CreditValueAccount = creditValue > 0 ? NumberHelper.RoundNumber(methodEntry.FixedCommissionValue * methodEntry.CommissionCurrencyRate, rounding) : 0,
								AutomaticRemarks = "FixedCommissionValue"
							};
							modelList.Add(fixedCommissionModel);
						}


						if (methodEntry.FixedCommissionTaxValue > 0)
						{
							var fixedCommissionTaxModel = new JournalDetailCalculationDto()
							{
								AccountId = methodEntry.CommissionTaxAccountId,
								CommissionTaxId = methodEntry.CommissionTaxId,
								CommissionTaxTypeId = methodEntry.CommissionTaxTypeId,
								CurrencyId = methodEntry.CommissionTaxCurrencyId,
								CurrencyRate = methodEntry.CommissionTaxCurrencyRate,
								DebitValue = debitValue > 0 ? methodEntry.FixedCommissionTaxValue : 0,
								DebitValueAccount = debitValue > 0 ? NumberHelper.RoundNumber(methodEntry.FixedCommissionTaxValue * methodEntry.CommissionTaxCurrencyRate, rounding) : 0,
								CreditValue = creditValue > 0 ? methodEntry.FixedCommissionTaxValue : 0,
								CreditValueAccount = creditValue > 0 ? NumberHelper.RoundNumber(methodEntry.FixedCommissionTaxValue * methodEntry.CommissionTaxCurrencyRate, rounding) : 0,
								AutomaticRemarks = "FixedCommissionTaxValue"
							};
							modelList.Add(fixedCommissionTaxModel);
						}

						response = new ResponseDto() { Success = true, Message = _localizer["EntriesCreated"] };
					}
				}
				else
				{
					modelList.Add(new JournalDetailCalculationDto());
					response = new ResponseDto() { Success = false, Message = _localizer["SomethingWentWrongInPayment"] };
				}
			}
			else
			{
				modelList.Add(new JournalDetailCalculationDto());
				response = new ResponseDto() { Success = false, Message = _localizer["SomethingWentWrongInPayment"]};
			}
			return new JournalDetailCalculationVm(){JournalDetailCalculations = modelList ,Response = response};
		}

		public async Task<PaymentMethodDto> GetPaymentMethodById(int id)
		{
			return await GetAllPaymentMethods().FirstOrDefaultAsync(x => x.PaymentMethodId == id) ?? new PaymentMethodDto();
		}

		public async Task<PaymentMethodEntryDto?> GetPaymentMethodsEntry(int storeId ,int paymentMethodId,decimal entryValue)
		{
			var rounding = await _storeService.GetStoreRounding(storeId);

			var data =
				await (from method in _repository.GetAll().Where(x => x.PaymentMethodId == paymentMethodId)
				from company in _companyService.GetAll().Where(x=>x.CompanyId == method.CompanyId)
				from commissionAccount in _accountService.GetAll().Where(x => x.AccountId == method.CommissionAccountId && x.CompanyId == method.CompanyId).DefaultIfEmpty()
				from commissionCurrency in _currencyService.GetAll().Where(x=>x.CurrencyId == commissionAccount.CurrencyId).DefaultIfEmpty()
				from commissionCurrencyRate in _currencyRateService.GetAll().Where(x=>x.FromCurrencyId == company.CurrencyId && x.ToCurrencyId == commissionCurrency.CurrencyId).DefaultIfEmpty()
				from commissionTaxAccount in _accountService.GetAll().Where(x => x.AccountId == method.CommissionTaxAccountId && x.CompanyId == method.CompanyId).DefaultIfEmpty()
				from commissionTaxCurrencyRate in _currencyRateService.GetAll().Where(x => x.FromCurrencyId == company.CurrencyId && x.ToCurrencyId == commissionTaxAccount.CurrencyId).DefaultIfEmpty()
				from tax in _taxService.GetAll().Where(x => x.DrAccount == commissionTaxAccount.AccountId && x.CompanyId == method.CompanyId && x.TaxId == method.TaxId).DefaultIfEmpty()
					from taxPercent in _taxPercentService.GetAll().Where(x => x.TaxId == tax.TaxId && x.FromDate <= DateTime.Today)
						.OrderByDescending(x => x.FromDate).Take(1).DefaultIfEmpty()
				select new PaymentMethodEntryDto
				{
					CommissionAccountId = commissionAccount != null ? commissionAccount.AccountId : 0,
					CommissionAccountNameAr = commissionAccount != null ? commissionAccount.AccountNameAr : null,
					CommissionAccountNameEn = commissionAccount != null ? commissionAccount.AccountNameEn : null,
					CommissionTaxAccountId = commissionTaxAccount != null ? commissionTaxAccount.AccountId : 0,
					CommissionTaxId = method.TaxId,
					CommissionTaxTypeId = tax != null ? tax.TaxTypeId : null,
					CommissionTaxAccountNameAr = commissionTaxAccount != null ? commissionTaxAccount.AccountNameAr : null,
					CommissionTaxAccountNameEn = commissionTaxAccount != null ? commissionTaxAccount.AccountNameEn : null,
					CommissionCurrencyId = (short)(commissionAccount != null ? commissionAccount.CurrencyId : 0),
					CommissionTaxCurrencyId = (short)(commissionTaxAccount != null ? commissionTaxAccount.CurrencyId : 0),
					CommissionCurrencyRate = commissionCurrencyRate != null ? commissionCurrencyRate.CurrencyRateValue : 1,
					CommissionTaxCurrencyRate = commissionTaxCurrencyRate != null ?  commissionTaxCurrencyRate.CurrencyRateValue : 1,
					FixedCommissionValue = PaymentMethodCalculation.GetFixedCommissionValue(method.FixedCommissionValue,(taxPercent != null ? taxPercent.Percent : 0),method.FixedCommissionHasVat,method.FixedCommissionVatInclusive, rounding),
					CommissionValue = PaymentMethodCalculation.GetCommissionValue(method.CommissionPercent, (taxPercent != null ? taxPercent.Percent : 0), method.MinCommissionValue,method.MaxCommissionValue,entryValue, method.CommissionHasVat,method.CommissionVatInclusive, rounding),
					CommissionTaxValue = PaymentMethodCalculation.GetCommissionTaxValue(method.CommissionPercent, (taxPercent != null ? taxPercent.Percent : 0), method.MinCommissionValue, method.MaxCommissionValue, entryValue, method.CommissionHasVat, method.CommissionVatInclusive, rounding),
					FixedCommissionTaxValue = PaymentMethodCalculation.GetFixedCommissionTaxValue(method.FixedCommissionValue, (taxPercent != null ? taxPercent.Percent : 0), method.FixedCommissionHasVat,method.FixedCommissionVatInclusive, rounding),
					BankValue = PaymentMethodCalculation.GetValueAfterCommission(method.FixedCommissionValue,method.CommissionPercent, (taxPercent != null ? taxPercent.Percent : 0), method.MinCommissionValue,method.MaxCommissionValue,entryValue,method.FixedCommissionHasVat,method.FixedCommissionVatInclusive,method.CommissionHasVat,method.CommissionVatInclusive, rounding)
				}).FirstOrDefaultAsync();
			return data;
		}

		public async Task<PaymentMethodDto?> GetDefaultCashMethod()
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return await GetAllPaymentMethods().Where(x=>x.CompanyId == companyId && x.IsActive && x.PaymentTypeId == PaymentTypeData.Cash).FirstOrDefaultAsync();
		}

		//public static decimal GetCommissionBeforeTax(decimal commissionPercent,decimal taxPercent,decimal minCommissionValue,decimal maxCommissionValue,decimal entryValue,int numberToBasic)
		//{
		//	var rounding = NumberHelper.GetTrailingZerosFromInteger(numberToBasic);
		//	var commissionValue = NumberHelper.RoundNumber((entryValue * (commissionPercent / 100) * ((100- taxPercent) / 100)),rounding);
		//	if (commissionValue <= minCommissionValue)
		//	{
		//		return minCommissionValue;
		//	}
		//	else if (commissionValue >= maxCommissionValue)
		//	{
		//		return maxCommissionValue;
		//	}
		//	else
		//	{
		//		return commissionValue;
		//	}
		//}
		//public static decimal GetFixedCommissionBeforeTax(decimal fixedCommissionValue,decimal taxPercent,int numberToBasic)
		//{
		//	var rounding = NumberHelper.GetTrailingZerosFromInteger(numberToBasic);
		//	var commissionValue = NumberHelper.RoundNumber((fixedCommissionValue  * ((100 - taxPercent) / 100)),rounding);
		//	return commissionValue;
		//}
		//public static decimal GetCommissionTaxValue(decimal commissionPercent, decimal taxPercent, decimal minCommissionValue, decimal maxCommissionValue, decimal entryValue, int numberToBasic)
		//{
		//	var rounding = NumberHelper.GetTrailingZerosFromInteger(numberToBasic);
		//	var commissionValue = GetCommissionBeforeTax(commissionPercent, taxPercent, minCommissionValue, maxCommissionValue, entryValue, numberToBasic);
		//	var tax = NumberHelper.RoundNumber((commissionValue * (taxPercent / 100)),rounding);
		//	return tax;
		//}
		//public static decimal GetFixedCommissionTaxValue(decimal fixedCommissionValue, decimal taxPercent,int numberToBasic)
		//{
		//	var rounding = NumberHelper.GetTrailingZerosFromInteger(numberToBasic);
		//	var tax = NumberHelper.RoundNumber((fixedCommissionValue * (taxPercent / 100)), rounding);
		//	return tax;
		//}
		//public static decimal GetBankAfterCommission(decimal fixedCommissionValue, decimal commissionPercent, decimal taxPercent, decimal minCommissionValue, decimal maxCommissionValue, decimal entryValue, int numberToBasic)
		//{
		//	var commission = GetCommissionBeforeTax(commissionPercent, taxPercent, minCommissionValue, maxCommissionValue,entryValue,numberToBasic);
		//	var tax = GetCommissionTaxValue(commissionPercent, taxPercent, minCommissionValue, maxCommissionValue, entryValue, numberToBasic);
		//	var fixedCommission = GetFixedCommissionBeforeTax(fixedCommissionValue, taxPercent, numberToBasic);
		//	var fixedTax = GetFixedCommissionTaxValue(fixedCommissionValue, taxPercent, numberToBasic);
		//	return entryValue - commission - tax - fixedCommission - fixedTax;
		//}

		public async Task<ResponseDto> SavePaymentMethod(PaymentMethodDto paymentMethod)
		{
			var validationResult = ValidatePaymentMethod(paymentMethod);
			if (!validationResult.Success)
			{
				return validationResult;
			}

			var paymentMethodExist = await IsPaymentMethodExist(paymentMethod.PaymentMethodId, paymentMethod.CompanyId,paymentMethod.PaymentMethodNameAr, paymentMethod.PaymentMethodNameEn);
			if (paymentMethodExist.Success)
			{
				return new ResponseDto() { Id = paymentMethodExist.Id, Success = false, Message = _localizer["PaymentMethodAlreadyExist"] };
			}
			else
			{
				if (paymentMethod.PaymentMethodId == 0)
				{
					return await CreatePaymentMethod(paymentMethod);
				}
				else
				{
					return await UpdatePaymentMethod(paymentMethod);
				}
			}
		}

		public ResponseDto ValidatePaymentMethod(PaymentMethodDto paymentMethod)
		{
			if ((paymentMethod.CommissionAccountId != null && paymentMethod.CommissionAccountId == paymentMethod.PaymentAccountId) ||
				(paymentMethod.CommissionAccountId != null && paymentMethod.CommissionTaxAccountId != null && paymentMethod.CommissionAccountId == paymentMethod.CommissionTaxAccountId) ||
				(paymentMethod.CommissionTaxAccountId != null && paymentMethod.CommissionTaxAccountId == paymentMethod.PaymentAccountId))
			{
				return new ResponseDto { Success = false, Message = _localizer["AllAccountsShouldBeDifferent"] };
			}

			return new ResponseDto { Success = true };
		}

		public async Task<ResponseDto> IsPaymentMethodExist(int id,int companyId, string? nameAr, string? nameEn)
		{
			var paymentMethod = await _repository.GetAll().FirstOrDefaultAsync(x => (x.PaymentMethodNameAr == nameAr || x.PaymentMethodNameEn == nameEn || x.PaymentMethodNameAr == nameEn || x.PaymentMethodNameEn == nameAr) && x.PaymentMethodId != id && x.CompanyId == companyId);
			if (paymentMethod != null)
			{
				return new ResponseDto() { Id = paymentMethod.PaymentMethodId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.PaymentMethodId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.PaymentMethodCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreatePaymentMethod(PaymentMethodDto paymentMethod)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var newPaymentMethod = new PaymentMethod()
			{
				PaymentMethodId = await GetNextId(),
				PaymentMethodCode = await GetNextCode(paymentMethod!.CompanyId),
				PaymentMethodNameAr = paymentMethod?.PaymentMethodNameAr?.Trim(),
				PaymentMethodNameEn = paymentMethod?.PaymentMethodNameEn?.Trim(),
				CompanyId = paymentMethod!.CompanyId,
				TaxId = paymentMethod.TaxId,
				PaymentTypeId = paymentMethod.PaymentTypeId,
				CommissionAccountId = paymentMethod.CommissionAccountId,
				CommissionTaxAccountId = paymentMethod.CommissionTaxAccountId,
				FixedCommissionValue = paymentMethod.FixedCommissionValue,
				CommissionPercent = paymentMethod.CommissionPercent,
				MinCommissionValue = paymentMethod.MinCommissionValue,
				MaxCommissionValue = paymentMethod.MaxCommissionValue,
				PaymentAccountId = paymentMethod.PaymentAccountId,
				IsActive = paymentMethod.IsActive,
				InActiveReasons = paymentMethod.InActiveReasons,
				IsPaymentMethod = paymentMethod.IsPaymentMethod,
				IsReceivingMethod = paymentMethod.IsReceivingMethod,
				CommissionHasVat = paymentMethod.CommissionHasVat,
				CommissionVatInclusive = paymentMethod.CommissionVatInclusive,
				FixedCommissionHasVat = paymentMethod.FixedCommissionHasVat,
				FixedCommissionVatInclusive = paymentMethod.FixedCommissionVatInclusive,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			var paymentMethodValidator = await new PaymentMethodValidator(_localizer).ValidateAsync(newPaymentMethod);
			var validationResult = paymentMethodValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newPaymentMethod);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newPaymentMethod.PaymentMethodId, Success = true, Message = _localizer["NewPaymentMethodSuccessMessage", ((language == LanguageCode.Arabic ? newPaymentMethod.PaymentMethodNameAr : newPaymentMethod.PaymentMethodNameEn) ?? ""), newPaymentMethod.PaymentMethodCode] };
			}
			else
			{
				return new ResponseDto() { Id = newPaymentMethod.PaymentMethodId, Success = false, Message = paymentMethodValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdatePaymentMethod(PaymentMethodDto paymentMethod)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var paymentMethodDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.PaymentMethodId == paymentMethod.PaymentMethodId);
			if (paymentMethodDb != null)
			{
				paymentMethodDb.PaymentMethodNameAr = paymentMethod.PaymentMethodNameAr?.Trim();
				paymentMethodDb.PaymentMethodNameEn = paymentMethod.PaymentMethodNameEn?.Trim();
				paymentMethodDb.CompanyId = paymentMethod!.CompanyId;
				paymentMethodDb.TaxId = paymentMethod.TaxId;
				paymentMethodDb.PaymentTypeId = paymentMethod.PaymentTypeId;
				paymentMethodDb.CommissionAccountId = paymentMethod.CommissionAccountId;
				paymentMethodDb.CommissionTaxAccountId = paymentMethod.CommissionTaxAccountId;
				paymentMethodDb.FixedCommissionValue = paymentMethod.FixedCommissionValue;
				paymentMethodDb.CommissionPercent = paymentMethod.CommissionPercent;
				paymentMethodDb.MinCommissionValue = paymentMethod.MinCommissionValue;
				paymentMethodDb.MaxCommissionValue = paymentMethod.MaxCommissionValue;
				paymentMethodDb.PaymentAccountId = paymentMethod.PaymentAccountId;
				paymentMethodDb.IsActive = paymentMethod.IsActive;
				paymentMethodDb.InActiveReasons = paymentMethod.InActiveReasons;
				paymentMethodDb.IsPaymentMethod = paymentMethod.IsPaymentMethod;
				paymentMethodDb.IsReceivingMethod = paymentMethod.IsReceivingMethod;
				paymentMethodDb.CommissionHasVat = paymentMethod.CommissionHasVat;
				paymentMethodDb.CommissionVatInclusive = paymentMethod.CommissionVatInclusive;
				paymentMethodDb.FixedCommissionHasVat = paymentMethod.FixedCommissionHasVat;
				paymentMethodDb.FixedCommissionVatInclusive = paymentMethod.FixedCommissionVatInclusive;
				paymentMethodDb.ModifiedAt = DateHelper.GetDateTimeNow();
				paymentMethodDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				paymentMethodDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var paymentMethodValidator = await new PaymentMethodValidator(_localizer).ValidateAsync(paymentMethodDb);
				var validationResult = paymentMethodValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(paymentMethodDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = paymentMethodDb.PaymentMethodId, Success = true, Message = _localizer["UpdatePaymentMethodSuccessMessage", ((language == LanguageCode.Arabic ? paymentMethodDb.PaymentMethodNameAr : paymentMethodDb.PaymentMethodNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = paymentMethodValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoPaymentMethodFound"] };
		}

		public async Task<ResponseDto> DeletePaymentMethod(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var paymentMethodDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.PaymentMethodId == id);
			if (paymentMethodDb != null)
			{
				_repository.Delete(paymentMethodDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeletePaymentMethodSuccessMessage", ((language == LanguageCode.Arabic ? paymentMethodDb.PaymentMethodNameAr : paymentMethodDb.PaymentMethodNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoPaymentMethodFound"] };
		}
	}
}
