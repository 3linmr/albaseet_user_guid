using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne;
using Shared.Helper.Logic;
using Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Helper.Identity;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Service.Services
{
    internal class FixedAssetVoucherDetailPaymentService : BaseService<FixedAssetVoucherDetailPayment>, IFixedAssetVoucherDetailPaymentService
    {
        private readonly IAccountService _accountService;
        private readonly ICurrencyService _currencyService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FixedAssetVoucherDetailPaymentService(IRepository<FixedAssetVoucherDetailPayment> repository, IAccountService accountService, ICurrencyService currencyService, IHttpContextAccessor httpContextAccessor) : base(repository)
        {
            _accountService = accountService;
            _currencyService = currencyService;
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<FixedAssetVoucherDetailPaymentDto> GetAllFixedAssetVoucherDetailPayment()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data =
                from fixedAssetVoucherDetailPayment in _repository.GetAll()
                select new FixedAssetVoucherDetailPaymentDto
                {
                    FixedAssetVoucherDetailId = fixedAssetVoucherDetailPayment.FixedAssetVoucherDetailId,
                    FixedAssetVoucherHeaderId = fixedAssetVoucherDetailPayment.FixedAssetVoucherDetail!.FixedAssetVoucherHeaderId,
                    FixedAssetId = fixedAssetVoucherDetailPayment.FixedAssetVoucherDetail!.FixedAssetId,
                    PaymentMethodId = fixedAssetVoucherDetailPayment.PaymentMethodId,
                    PaymentMethodName = fixedAssetVoucherDetailPayment.PaymentMethod == null ? null : language == LanguageCode.Arabic ? fixedAssetVoucherDetailPayment.PaymentMethod!.PaymentMethodNameAr : fixedAssetVoucherDetailPayment!.PaymentMethod!.PaymentMethodNameEn,
                    //PaymentMethodNameAr = fixedAssetVoucherDetailPayment.PaymentMethod == null ? null : fixedAssetVoucherDetailPayment.PaymentMethod!.PaymentMethodNameAr,
                    //PaymentMethodNameEn = fixedAssetVoucherDetailPayment.PaymentMethod == null ? null : fixedAssetVoucherDetailPayment!.PaymentMethod!.PaymentMethodNameEn,
                    //AccountId = fixedAssetVoucherDetailPayment.AccountId,
                    //AccountName = fixedAssetVoucherDetailPayment.Account == null ? null : language == LanguageCode.Arabic ? fixedAssetVoucherDetailPayment.Account!.AccountNameAr : fixedAssetVoucherDetailPayment!.Account!.AccountNameEn,
                    //AccountNameAr = fixedAssetVoucherDetailPayment.Account == null ? null : fixedAssetVoucherDetailPayment.Account!.AccountNameAr,
                    //AccountNameEn = fixedAssetVoucherDetailPayment.Account == null ? null : fixedAssetVoucherDetailPayment!.Account!.AccountNameEn,                    
                    //CurrencyId = fixedAssetVoucherDetailPayment.CurrencyId,
                    //CurrencyName = fixedAssetVoucherDetailPayment.Currency == null ? null : language == LanguageCode.Arabic ? fixedAssetVoucherDetailPayment.Currency!.CurrencyNameAr : fixedAssetVoucherDetailPayment!.Currency!.CurrencyNameEn,
                    //CurrencyNameAr = fixedAssetVoucherDetailPayment.Currency == null ? null : fixedAssetVoucherDetailPayment.Currency!.CurrencyNameAr,
                    //CurrencyNameEn = fixedAssetVoucherDetailPayment.Currency == null ? null : fixedAssetVoucherDetailPayment!.Currency!.CurrencyNameEn,
                    //CurrencyRate = fixedAssetVoucherDetailPayment.CurrencyRate,
                    CreditValue = fixedAssetVoucherDetailPayment.CreditValue,
                    //CreditValueAccount = fixedAssetVoucherDetailPayment.CreditValueAccount,
                    DebitValue = fixedAssetVoucherDetailPayment.DebitValue,
                    //DebitValueAccount = fixedAssetVoucherDetailPayment.DebitValueAccount,                    
                    CreatedAt = fixedAssetVoucherDetailPayment.CreatedAt,
                    UserNameCreated = fixedAssetVoucherDetailPayment.UserNameCreated,
                    IpAddressCreated = fixedAssetVoucherDetailPayment.IpAddressCreated,
                    ModifiedAt = fixedAssetVoucherDetailPayment.ModifiedAt,
                    UserNameModified = fixedAssetVoucherDetailPayment.UserNameModified,                    
                };
            return data;
        }

        public async Task<List<FixedAssetVoucherDetailPaymentDto>> GetFixedAssetVoucherDetailPayment(int detailId)
        {
            return await GetAllFixedAssetVoucherDetailPayment()
                .Where(x => x.FixedAssetVoucherDetailId == detailId)
                .ToListAsync();
        }

        public async Task<List<FixedAssetVoucherDetailPaymentDto>> GetFixedAssetVoucherDetailPaymentByFixedAssetId(int fixedAssetId)
        {
            return await GetAllFixedAssetVoucherDetailPayment()
                .Where(x => x.FixedAssetId == fixedAssetId)
                .ToListAsync();
        }

        public async Task<bool> SaveFixedAssetVoucherDetailPayment(List<FixedAssetVoucherDetailPaymentDto>? payments, int headerId)
        {
            if (payments != null)
            {
                if (payments.Any())
                {
                    var result = await DeleteFixedAssetVoucherDetailPayment(payments, headerId);
                    if (result)
                    {
                        result = await AddFixedAssetVoucherDetailPayment(payments, headerId);
                        if (result)
                            result = await EditFixedAssetVoucherDetailPayment(payments);
                    }
                    return result;
                }
            }
            return true;
        }

        public async Task<bool> DeleteFixedAssetVoucherDetailPayment(List<FixedAssetVoucherDetailPaymentDto> payments, int detailId)
        {
            if (payments.Any())
            {
                var current = _repository.GetAll().Where(x => x.FixedAssetVoucherDetailId == detailId).AsNoTracking().ToList();
                var toBeDeleted = current.Where(p => payments.All(p2 => p2.FixedAssetVoucherDetailPaymentId != p.FixedAssetVoucherDetailPaymentId)).ToList();
                if (toBeDeleted.Any())
                {
                    _repository.RemoveRange(toBeDeleted);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> AddFixedAssetVoucherDetailPayment(List<FixedAssetVoucherDetailPaymentDto> payments, int detailId)
        {
            var current = payments.Where(x => x.FixedAssetVoucherDetailPaymentId <= 0).ToList();
            var modelList = new List<FixedAssetVoucherDetailPayment>();
            var newId = await GetNextId();
            foreach (var payment in current)
            {
                var newNote = new FixedAssetVoucherDetailPayment()
                {
                    FixedAssetVoucherDetailPaymentId = newId,
                    FixedAssetVoucherDetailId = detailId,
                    //AccountId = payment.AccountId,
                    CreditValue = payment.CreditValue,
                    //CreditValueAccount = payment.CreditValueAccount,
                    DebitValue = payment.DebitValue,
                    //DebitValueAccount = payment.DebitValueAccount,
                    //CurrencyId = payment.CurrencyId,
                    //CurrencyRate = payment.CurrencyRate,
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

        public async Task<bool> EditFixedAssetVoucherDetailPayment(List<FixedAssetVoucherDetailPaymentDto> payments)
        {
            var current = payments.Where(x => x.FixedAssetVoucherDetailPaymentId > 0).ToList();
            var modelList = new List<FixedAssetVoucherDetailPayment>();
            foreach (var payment in current)
            {
                var newNote = new FixedAssetVoucherDetailPayment()
                {
                    FixedAssetVoucherDetailPaymentId = payment.FixedAssetVoucherDetailPaymentId,
                    FixedAssetVoucherDetailId = payment.FixedAssetVoucherDetailId,
                    //AccountId = payment.AccountId,
                    CreditValue = payment.CreditValue,
                    //CreditValueAccount = payment.CreditValueAccount,
                    DebitValue = payment.DebitValue,
                    //DebitValueAccount = payment.DebitValueAccount,
                    //CurrencyId = payment.CurrencyId,
                    //CurrencyRate = payment.CurrencyRate,
                    CreatedAt = payment.CreatedAt,
                    UserNameCreated = payment.UserNameCreated,
                    IpAddressCreated = payment.IpAddressCreated,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.FixedAssetVoucherDetailPaymentId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<bool> DeleteFixedAssetVoucherDetailPayment(int detailId)
        {
            var data = await _repository.GetAll().Where(x => x.FixedAssetVoucherDetailId == detailId).ToListAsync();
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
