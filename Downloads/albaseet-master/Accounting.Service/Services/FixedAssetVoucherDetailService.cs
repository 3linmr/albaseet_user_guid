using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne;
using Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Helper.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Helper.Logic;
using Shared.Service.Services.Modules;
using Shared.CoreOne.Models.Domain.Journal;
using Microsoft.AspNetCore.Http.HttpResults;
using Shared.CoreOne.Models.Domain.FixedAssets;
using Shared.Helper.Models.Dtos;

namespace Accounting.Service.Services
{
    public class FixedAssetVoucherDetailService : BaseService<FixedAssetVoucherDetail>, IFixedAssetVoucherDetailService
    {
        private readonly IAccountService _accountService;
        private readonly ICurrencyService _currencyService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFixedAssetVoucherDetailPaymentService _fixedAssetVoucherDetailPaymentService;

        public FixedAssetVoucherDetailService(
            IRepository<FixedAssetVoucherDetail> repository, 
            IAccountService accountService, 
            ICurrencyService currencyService, 
            IHttpContextAccessor httpContextAccessor,
            IFixedAssetVoucherDetailPaymentService fixedAssetVoucherDetailPaymentService) : base(repository)
        {
            _accountService = accountService;
            _currencyService = currencyService;
            _httpContextAccessor = httpContextAccessor;
            _fixedAssetVoucherDetailPaymentService = fixedAssetVoucherDetailPaymentService;
        }

        public IQueryable<FixedAssetVoucherDetailDto> GetAllFixedAssetVoucherDetail()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data =
                from fixedAssetVoucherDetail in _repository.GetAll()
                join fixedAssetVoucherDetailPayment in _fixedAssetVoucherDetailPaymentService.GetAllFixedAssetVoucherDetailPayment()
                on fixedAssetVoucherDetail.FixedAssetVoucherDetailId equals fixedAssetVoucherDetailPayment.FixedAssetVoucherDetailId into paymentGroup
                select new FixedAssetVoucherDetailDto
                {
                    FixedAssetVoucherHeaderId = fixedAssetVoucherDetail.FixedAssetVoucherHeaderId,
                    FixedAssetVoucherDetailId = fixedAssetVoucherDetail.FixedAssetVoucherDetailId,
                    Prefix = fixedAssetVoucherDetail.FixedAssetVoucherHeader == null ? null : fixedAssetVoucherDetail.FixedAssetVoucherHeader!.Prefix,
                    DocumentCode = fixedAssetVoucherDetail.FixedAssetVoucherHeader == null ? null : fixedAssetVoucherDetail.FixedAssetVoucherHeader!.DocumentCode,
                    Suffix = fixedAssetVoucherDetail.FixedAssetVoucherHeader == null ? null : fixedAssetVoucherDetail.FixedAssetVoucherHeader!.Suffix,
                    FixedAssetId = fixedAssetVoucherDetail.FixedAssetId,
                    FixedAssetName = fixedAssetVoucherDetail.FixedAsset == null ? null : language == LanguageCode.Arabic ? fixedAssetVoucherDetail.FixedAsset!.FixedAssetNameAr : fixedAssetVoucherDetail!.FixedAsset!.FixedAssetNameEn,
                    DocumentDate = fixedAssetVoucherDetail.FixedAssetVoucherHeader != null ? fixedAssetVoucherDetail.FixedAssetVoucherHeader.DocumentDate : null,
                    DetailValue = fixedAssetVoucherDetail.DetailValue,
                    RemarksAr = fixedAssetVoucherDetail.RemarksAr,
                    RemarksEn = fixedAssetVoucherDetail.RemarksEn,
                    FixedAssetVoucherTypeId = fixedAssetVoucherDetail.FixedAssetVoucherHeader == null ? null : fixedAssetVoucherDetail.FixedAssetVoucherHeader!.FixedAssetVoucherTypeId,
                    JournalHeaderId = fixedAssetVoucherDetail.FixedAssetVoucherHeader == null ? null : fixedAssetVoucherDetail.FixedAssetVoucherHeader!.JournalHeaderId,
                    StoreId = fixedAssetVoucherDetail.FixedAssetVoucherHeader == null ? null : fixedAssetVoucherDetail.FixedAssetVoucherHeader!.StoreId,
                    FixedAssetVoucherDetailPayments = paymentGroup.Select(payment => new FixedAssetVoucherDetailPaymentDto
                    {
                        FixedAssetVoucherDetailPaymentId = payment.FixedAssetVoucherDetailPaymentId,
                        FixedAssetVoucherDetailId = payment.FixedAssetVoucherDetailId,
                        PaymentMethodId = payment.PaymentMethodId,
                        FixedAssetVoucherHeaderId = payment.FixedAssetVoucherHeaderId,
                        FixedAssetId = payment.FixedAssetId,
                        PaymentMethodName = payment.PaymentMethodName
                    }).ToList(),
                    CreatedAt = fixedAssetVoucherDetail.CreatedAt,
                    UserNameCreated = fixedAssetVoucherDetail.UserNameCreated,
                    IpAddressCreated = fixedAssetVoucherDetail.IpAddressCreated
                };
            return data;
		}

        public async Task<List<FixedAssetVoucherDetailDto>> GetFixedAssetVoucherDetail(int headerId)
        {
            return await GetAllFixedAssetVoucherDetail()
                .Where(x => x.FixedAssetVoucherHeaderId == headerId)
                .ToListAsync();
        }

        public async Task<List<FixedAssetVoucherDetailDto>> GetFixedAssetVoucherByFixedAssetId(int fixedAssetId)
        {
            return await GetAllFixedAssetVoucherDetail()
                .Where(x => x.FixedAssetId == fixedAssetId)
                .ToListAsync();
        }

        public async Task<FixedAssetVoucherDetailResponseDto> SaveFixedAssetVoucherDetail(List<FixedAssetVoucherDetailDto>? details, int headerId)
        {
            if (details != null)
            {
                if (details.Count != 0)
                {
                    await DeleteFixedAssetVoucherDetail(details, headerId);
                    var responseDetails = await AddFixedAssetVoucherDetail(details, headerId);
                    //await EditFixedAssetVoucherDetail(details);
                    return new FixedAssetVoucherDetailResponseDto() 
                    { 
                        Response = new ResponseDto() { Success = true } ,
                        FixedAssetVoucherDetails = responseDetails
                    };
                }
            }
            return new FixedAssetVoucherDetailResponseDto()
            {
                Response = new ResponseDto() { Success = false }
            };
        }

        public async Task<bool> DeleteFixedAssetVoucherDetail(List<FixedAssetVoucherDetailDto> details, int headerId)
        {
            if (details.Count != 0)
            {
                var current = _repository.GetAll().Where(x => x.FixedAssetVoucherHeaderId == headerId).ToList();
                //var toBeDeleted = current.Where(p => details.All(p2 => p2.FixedAssetVoucherDetailId != p.FixedAssetVoucherDetailId)).ToList();
                if (current.Count != 0)
                {
                    _repository.RemoveRange(current);
                    await _repository.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public async Task<List<FixedAssetVoucherDetailDto>> AddFixedAssetVoucherDetail(List<FixedAssetVoucherDetailDto> details, int headerId)
        {
            var modelList = new List<FixedAssetVoucherDetail>();
            var newId = await GetNextId();
            foreach (var detail in details)
            {
                if(detail.FixedAssetVoucherDetailId <= 0)
                {
                    detail.FixedAssetVoucherDetailId = newId;
                    detail.FixedAssetVoucherHeaderId = headerId;
                    var newNote = new FixedAssetVoucherDetail()
                    {
                        FixedAssetVoucherDetailId = newId,
                        FixedAssetVoucherHeaderId = headerId,
                        FixedAssetId = detail.FixedAssetId,
                        RemarksAr = detail.RemarksAr,
                        RemarksEn = detail.RemarksEn,
                        DetailValue = detail.DetailValue,
                        Hide = false,
                        CreatedAt = DateHelper.GetDateTimeNow(),
                        UserNameCreated = await _httpContextAccessor!.GetUserName(),
                        IpAddressCreated = _httpContextAccessor?.GetIpAddress()
                    };
                    modelList.Add(newNote);
                    newId++;
                }
               
            }

            if (modelList.Count != 0)
            {
                await _repository.InsertRange(modelList);
                await _repository.SaveChanges();
            }
            return details;

        }

        public async Task<bool> EditFixedAssetVoucherDetail(List<FixedAssetVoucherDetailDto> notes)
        {
            var current = notes.Where(x => x.FixedAssetVoucherDetailId > 0).ToList();
            var modelList = new List<FixedAssetVoucherDetail>();
            foreach (var detail in current)
            {
                var newNote = new FixedAssetVoucherDetail()
                {
                    FixedAssetVoucherDetailId = detail.FixedAssetVoucherDetailId,
                    FixedAssetVoucherHeaderId = detail.FixedAssetVoucherHeaderId,
                    FixedAssetId = detail.FixedAssetId,
                    RemarksAr = detail.RemarksAr,
                    RemarksEn = detail.RemarksEn,
                    DetailValue = detail.DetailValue,
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

            if (modelList.Count != 0)
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
	        try { id = await _repository.GetAll().MaxAsync(a => a.FixedAssetVoucherDetailId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<bool> DeleteFixedAssetVoucherDetail(int headerId)
        {
            var data = await _repository.GetAll().Where(x => x.FixedAssetVoucherHeaderId == headerId).ToListAsync();
            if (data.Count != 0)
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

    }
}