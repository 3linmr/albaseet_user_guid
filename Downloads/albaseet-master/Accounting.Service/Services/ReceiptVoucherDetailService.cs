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
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Service;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.Helper.Models.StaticData.LanguageData;

namespace Accounting.Service.Services
{
    public class ReceiptVoucherDetailService : BaseService<ReceiptVoucherDetail>, IReceiptVoucherDetailService
    {
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IAccountService _accountService;
        private readonly ICurrencyService _currencyService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReceiptVoucherDetailService(IRepository<ReceiptVoucherDetail> repository, IPaymentMethodService paymentMethodService, IAccountService accountService, ICurrencyService currencyService, IHttpContextAccessor httpContextAccessor) : base(repository)
        {
            _paymentMethodService = paymentMethodService;
            _accountService = accountService;
            _currencyService = currencyService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<ReceiptVoucherDetailDto> GetAllReceiptVoucherDetail()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data =
                from receiptVoucherDetail in _repository.GetAll()
                from account in _accountService.GetAll().Where(x => x.AccountId == receiptVoucherDetail.AccountId)
                from currency in _currencyService.GetAll().Where(x => x.CurrencyId == receiptVoucherDetail.CurrencyId)
                from paymentMethod in _paymentMethodService.GetAll().Where(x => x.PaymentMethodId == receiptVoucherDetail.PaymentMethodId).DefaultIfEmpty()
                select new ReceiptVoucherDetailDto
                {
                    ReceiptVoucherHeaderId = receiptVoucherDetail.ReceiptVoucherHeaderId,
                    ReceiptVoucherDetailId = receiptVoucherDetail.ReceiptVoucherDetailId,
                    AccountId = receiptVoucherDetail.AccountId,
                    CurrencyId = receiptVoucherDetail.CurrencyId,
                    PaymentMethodId = receiptVoucherDetail.PaymentMethodId,
                    DebitValue = receiptVoucherDetail.DebitValue,
                    CreditValue = receiptVoucherDetail.CreditValue,
                    DebitValueAccount = receiptVoucherDetail.DebitValueAccount,
                    CreditValueAccount = receiptVoucherDetail.CreditValueAccount,
                    CurrencyRate = receiptVoucherDetail.CurrencyRate,
                    RemarksAr = receiptVoucherDetail.RemarksAr,
                    RemarksEn = receiptVoucherDetail.RemarksEn,
                    CreatedAt = receiptVoucherDetail.CreatedAt,
                    UserNameCreated = receiptVoucherDetail.UserNameCreated,
                    IpAddressCreated = receiptVoucherDetail.IpAddressCreated,
                    PaymentMethodName = paymentMethod != null ? language == LanguageCode.Arabic ? paymentMethod.PaymentMethodNameAr : paymentMethod.PaymentMethodNameEn : "",
                    AccountCode = account.AccountCode,
                    AccountName = language == LanguageCode.Arabic ? account.AccountNameAr : account.AccountNameEn,
                    CurrencyName = language == LanguageCode.Arabic ? currency.CurrencyNameAr : currency.CurrencyNameEn
                };
            return data;
        }

        public async Task<List<ReceiptVoucherDetailDto>> GetReceiptVoucherDetail(int headerId)
        {
            return await GetAllReceiptVoucherDetail().Where(x => x.ReceiptVoucherHeaderId == headerId).ToListAsync();
        }

        public async Task<List<ReceiptVoucherDetailDto>> GetReceiptVoucherDetailsWithPaymentMethods(int storeId, int receiptVoucherHeaderId)
        {
            var paymentMethods = await _paymentMethodService.GetVoucherPaymentMethods(storeId,false, false);
            var details = await GetReceiptVoucherDetail(receiptVoucherHeaderId);
            var data =
                 (from payment in paymentMethods
                  from receiptVoucherDetail in details.Where(x => x.PaymentMethodId == payment.PaymentMethodId).DefaultIfEmpty()
                  select new ReceiptVoucherDetailDto
                  {
                      ReceiptVoucherHeaderId = receiptVoucherDetail != null ? receiptVoucherDetail.ReceiptVoucherHeaderId : 0,
                      ReceiptVoucherDetailId = receiptVoucherDetail != null ? receiptVoucherDetail.ReceiptVoucherDetailId : 0,
                      AccountId = payment.AccountId,
                      CurrencyId = payment.CurrencyId,
                      PaymentMethodId = payment.PaymentMethodId,
                      DebitValue = receiptVoucherDetail != null ? receiptVoucherDetail.DebitValue : 0,
                      CreditValue = receiptVoucherDetail != null ? receiptVoucherDetail.CreditValue : 0,
                      DebitValueAccount = receiptVoucherDetail != null ? receiptVoucherDetail.DebitValueAccount : 0,
                      CreditValueAccount = receiptVoucherDetail != null ? receiptVoucherDetail.CreditValueAccount : 0,
                      CurrencyRate = payment.CurrencyRate,
                      RemarksAr = receiptVoucherDetail != null ? receiptVoucherDetail.RemarksAr : "",
                      RemarksEn = receiptVoucherDetail != null ? receiptVoucherDetail.RemarksEn : "",
                      CreatedAt = receiptVoucherDetail != null ? receiptVoucherDetail.CreatedAt : null,
                      UserNameCreated = receiptVoucherDetail != null ? receiptVoucherDetail.UserNameCreated : null,
                      IpAddressCreated = receiptVoucherDetail != null ? receiptVoucherDetail.IpAddressCreated : null,
                      PaymentMethodName = payment.PaymentMethodName,
                      AccountCode = payment.AccountCode,
                      AccountName = payment.AccountName,
                      CurrencyName = payment.CurrencyName
                  }).ToList();
            var creditSide = details.Where(x => x.CreditValue > 0).ToList();
            data.AddRange(creditSide);
            return data;
        }

        public async Task<bool> SaveReceiptVoucherDetail(List<ReceiptVoucherDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                await DeleteReceiptVoucherDetail(details, headerId);
                await AddReceiptVoucherDetail(details, headerId);
                await EditReceiptVoucherDetail(details);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteReceiptVoucherDetail(List<ReceiptVoucherDetailDto> details, int headerId)
        {
            if (details.Any())
            {
                var current = _repository.GetAll().Where(x => x.ReceiptVoucherHeaderId == headerId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => details.All(p2 => p2.ReceiptVoucherDetailId != p.ReceiptVoucherDetailId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> AddReceiptVoucherDetail(List<ReceiptVoucherDetailDto> details, int headerId)
        {
            var current = details.Where(x => x.ReceiptVoucherDetailId <= 0).ToList();
            var modelList = new List<ReceiptVoucherDetail>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var newNote = new ReceiptVoucherDetail()
                {
                    ReceiptVoucherDetailId = newId,
                    ReceiptVoucherHeaderId = headerId,
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

        public async Task<bool> EditReceiptVoucherDetail(List<ReceiptVoucherDetailDto> notes)
        {
            var current = notes.Where(x => x.ReceiptVoucherDetailId > 0).ToList();
            var modelList = new List<ReceiptVoucherDetail>();
            foreach (var detail in current)
            {
                var newNote = new ReceiptVoucherDetail()
                {
                    ReceiptVoucherDetailId = detail.ReceiptVoucherDetailId,
                    ReceiptVoucherHeaderId = detail.ReceiptVoucherHeaderId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.ReceiptVoucherDetailId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<bool> DeleteReceiptVoucherDetail(int headerId)
        {
            var data = await _repository.GetAll().Where(x => x.ReceiptVoucherHeaderId == headerId).ToListAsync();
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
