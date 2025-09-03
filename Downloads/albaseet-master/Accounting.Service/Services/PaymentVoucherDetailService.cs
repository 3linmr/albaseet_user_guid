using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Modules;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Service;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Accounting.Service.Services
{
    public class PaymentVoucherDetailService : BaseService<PaymentVoucherDetail>, IPaymentVoucherDetailService
    {
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IAccountService _accountService;
        private readonly ICurrencyService _currencyService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentVoucherDetailService(IRepository<PaymentVoucherDetail> repository, IPaymentMethodService paymentMethodService, IAccountService accountService, ICurrencyService currencyService, IHttpContextAccessor httpContextAccessor) : base(repository)
        {
            _paymentMethodService = paymentMethodService;
            _accountService = accountService;
            _currencyService = currencyService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<PaymentVoucherDetailDto> GetAllPaymentVoucherDetail()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data =
                from paymentVoucherDetail in _repository.GetAll()
                from account in _accountService.GetAll().Where(x => x.AccountId == paymentVoucherDetail.AccountId)
                from currency in _currencyService.GetAll().Where(x => x.CurrencyId == paymentVoucherDetail.CurrencyId)
                from paymentMethod in _paymentMethodService.GetAll().Where(x => x.PaymentMethodId == paymentVoucherDetail.PaymentMethodId).DefaultIfEmpty()
                select new PaymentVoucherDetailDto
                {
                    PaymentVoucherHeaderId = paymentVoucherDetail.PaymentVoucherHeaderId,
                    PaymentVoucherDetailId = paymentVoucherDetail.PaymentVoucherDetailId,
                    AccountId = paymentVoucherDetail.AccountId,
                    CurrencyId = paymentVoucherDetail.CurrencyId,
                    PaymentMethodId = paymentVoucherDetail.PaymentMethodId,
                    DebitValue = paymentVoucherDetail.DebitValue,
                    CreditValue = paymentVoucherDetail.CreditValue,
                    DebitValueAccount = paymentVoucherDetail.DebitValueAccount,
                    CreditValueAccount = paymentVoucherDetail.CreditValueAccount,
                    CurrencyRate = paymentVoucherDetail.CurrencyRate,
                    RemarksAr = paymentVoucherDetail.RemarksAr,
                    RemarksEn = paymentVoucherDetail.RemarksEn,
                    CreatedAt = paymentVoucherDetail.CreatedAt,
                    UserNameCreated = paymentVoucherDetail.UserNameCreated,
                    IpAddressCreated = paymentVoucherDetail.IpAddressCreated,
                    PaymentMethodName = paymentMethod != null ? language == LanguageCode.Arabic ? paymentMethod.PaymentMethodNameAr : paymentMethod.PaymentMethodNameEn : "",
                    AccountCode = account.AccountCode,
                    AccountName = language == LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn,
                    CurrencyName = language == LanguageCode.Arabic ? currency.CurrencyNameAr : currency.CurrencyNameEn
                };
            return data;
        }

        public async Task<List<PaymentVoucherDetailDto>> GetPaymentVoucherDetail(int headerId)
        {
            return await GetAllPaymentVoucherDetail().Where(x => x.PaymentVoucherHeaderId == headerId).ToListAsync();
        }

        public async Task<List<PaymentVoucherDetailDto>> GetPaymentVoucherDetailsWithPaymentMethods(int storeId, int paymentVoucherHeaderId)
        {
            var paymentMethods = await _paymentMethodService.GetVoucherPaymentMethods(storeId,true,false);
            var details = await GetPaymentVoucherDetail(paymentVoucherHeaderId);
            var data =
                 (from payment in paymentMethods
                  from paymentVoucherDetail in details.Where(x => x.PaymentMethodId == payment.PaymentMethodId).DefaultIfEmpty()
                  select new PaymentVoucherDetailDto
                  {
                      PaymentVoucherHeaderId = paymentVoucherDetail != null ? paymentVoucherDetail.PaymentVoucherHeaderId : 0,
                      PaymentVoucherDetailId = paymentVoucherDetail != null ? paymentVoucherDetail.PaymentVoucherDetailId : 0,
                      AccountId = payment.AccountId,
                      CurrencyId = payment.CurrencyId,
                      PaymentMethodId = payment.PaymentMethodId,
                      DebitValue = paymentVoucherDetail != null ? paymentVoucherDetail.DebitValue : 0,
                      CreditValue = paymentVoucherDetail != null ? paymentVoucherDetail.CreditValue : 0,
                      DebitValueAccount = paymentVoucherDetail != null ? paymentVoucherDetail.DebitValueAccount : 0,
                      CreditValueAccount = paymentVoucherDetail != null ? paymentVoucherDetail.CreditValueAccount : 0,
                      CurrencyRate = payment.CurrencyRate,
                      RemarksAr = paymentVoucherDetail != null ? paymentVoucherDetail.RemarksAr : "",
                      RemarksEn = paymentVoucherDetail != null ? paymentVoucherDetail.RemarksEn : "",
                      CreatedAt = paymentVoucherDetail != null ? paymentVoucherDetail.CreatedAt : null,
                      UserNameCreated = paymentVoucherDetail != null ? paymentVoucherDetail.UserNameCreated : null,
                      IpAddressCreated = paymentVoucherDetail != null ? paymentVoucherDetail.IpAddressCreated : null,
                      PaymentMethodName = payment.PaymentMethodName,
                      AccountCode = payment.AccountCode,
                      AccountName = payment.AccountName,
                      CurrencyName = payment.CurrencyName
                  }).ToList();
            var debitSide = details.Where(x => x.DebitValue > 0).ToList();
            data.AddRange(debitSide);
            return data;
        }

        public async Task<bool> SavePaymentVoucherDetail(List<PaymentVoucherDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                await DeletePaymentVoucherDetail(details, headerId);
                await AddPaymentVoucherDetail(details, headerId);
                await EditPaymentVoucherDetail(details);
                return true;
            }
            return false;
        }

        public async Task<bool> DeletePaymentVoucherDetail(List<PaymentVoucherDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.PaymentVoucherHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.PaymentVoucherDetailId != p.PaymentVoucherDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> AddPaymentVoucherDetail(List<PaymentVoucherDetailDto> details, int headerId)
        {
            var current = details.Where(x => x.PaymentVoucherDetailId <= 0).ToList();
            var modelList = new List<PaymentVoucherDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var newNote = new PaymentVoucherDetail()
                {
                    PaymentVoucherDetailId = newId,
                    PaymentVoucherHeaderId = headerId,
                    PaymentMethodId = detail.PaymentMethodId,
                    AccountId = detail.AccountId,
                    CurrencyId = detail.CurrencyId,
                    CreditValue = detail.CreditValue,
                    CurrencyRate = detail.CurrencyRate,
                    DebitValue = detail.DebitValue,
                    CreditValueAccount = detail.CreditValueAccount,
                    DebitValueAccount = detail.DebitValueAccount,
                    RemarksAr = detail.RemarksAr,
                    RemarksEn = detail.RemarksEn,
                    Hide = false,
                    CreatedAt = DateHelper.GetDateTimeNow(),
                    UserNameCreated = await _httpContextAccessor!.GetUserName(),
                    IpAddressCreated = _httpContextAccessor?.GetIpAddress()
                };
                modelList.Add(newNote);
                newId++;
            }

            if (modelList.Any())
            {
                await _repository.InsertRange(modelList);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        public async Task<bool> EditPaymentVoucherDetail(List<PaymentVoucherDetailDto> notes)
        {
            var current = notes.Where(x => x.PaymentVoucherDetailId > 0).ToList();
            var modelList = new List<PaymentVoucherDetail>();
            foreach (var detail in current)
            {
                var newNote = new PaymentVoucherDetail()
                {
                    PaymentVoucherDetailId = detail.PaymentVoucherDetailId,
                    PaymentVoucherHeaderId = detail.PaymentVoucherHeaderId,
                    PaymentMethodId = detail.PaymentMethodId,
                    AccountId = detail.AccountId,
                    CurrencyId = detail.CurrencyId,
                    CreditValue = detail.CreditValue,
                    CurrencyRate = detail.CurrencyRate,
                    DebitValue = detail.DebitValue,
                    CreditValueAccount = detail.CreditValueAccount,
                    DebitValueAccount = detail.DebitValueAccount,
                    RemarksAr = detail.RemarksAr,
                    RemarksEn = detail.RemarksEn,
                    CreatedAt = detail.CreatedAt,
                    UserNameCreated = detail.UserNameCreated,
                    IpAddressCreated = detail.IpAddressCreated,
                    ModifiedAt = DateHelper.GetDateTimeNow(),
                    UserNameModified = await _httpContextAccessor!.GetUserName(),
                    IpAddressModified = _httpContextAccessor?.GetIpAddress(),
                    Hide = false
                };
                modelList.Add(newNote);
            }

            if (modelList.Any())
            {
                _repository.UpdateRange(modelList);
                await _repository.SaveChanges();
                return true;
            }
            return false;

        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.PaymentVoucherDetailId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<bool> DeletePaymentVoucherDetail(int headerId)
        {
            var data = await _repository.GetAll().Where(x => x.PaymentVoucherHeaderId == headerId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }
    }
}