using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Models.Domain;
using Accounting.CoreOne.Models.Dtos.ViewModels;
using Accounting.Service.Validators;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.StaticData;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Models.StaticData;
using Shared.Service;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Settings;

namespace Accounting.Service.Services
{
    public class PaymentVoucherHeaderService : BaseService<PaymentVoucherHeader>, IPaymentVoucherHeaderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISellerService _sellerService;
        private readonly IStoreService _storeService;
        private readonly IMenuEncodingService _menuEncodingService;
        private readonly IStringLocalizer<PaymentVoucherHeaderService> _localizer;
        private readonly IBranchService _branchService;
        private readonly ICompanyService _companyService;
        private readonly ISupplierService _supplierService;
        private readonly IApplicationSettingService _applicationSettingService;

        public PaymentVoucherHeaderService(IRepository<PaymentVoucherHeader> repository, IHttpContextAccessor httpContextAccessor, ISellerService sellerService, IStoreService storeService, IMenuEncodingService menuEncodingService, IStringLocalizer<PaymentVoucherHeaderService> localizer, IBranchService branchService, ICompanyService companyService, ISupplierService supplierService, IApplicationSettingService applicationSettingService) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _sellerService = sellerService;
            _storeService = storeService;
            _menuEncodingService = menuEncodingService;
            _localizer = localizer;
            _branchService = branchService;
            _companyService = companyService;
            _supplierService = supplierService;
            _applicationSettingService = applicationSettingService;
        }

        public IQueryable<PaymentVoucherHeaderDto> GetPaymentVoucherHeaders()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data =
                from paymentVoucherHeader in _repository.GetAll()
                from store in _storeService.GetAll().Where(x => x.StoreId == paymentVoucherHeader.StoreId)
                from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
                from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
                from seller in _sellerService.GetAll().Where(x => x.SellerId == paymentVoucherHeader.SellerId).DefaultIfEmpty()
                from supplier in _supplierService.GetAll().Where(x => x.SupplierId == paymentVoucherHeader.SupplierId).DefaultIfEmpty()
				select new PaymentVoucherHeaderDto
                {
                    PaymentVoucherHeaderId = paymentVoucherHeader.PaymentVoucherHeaderId,
                    DocumentReference = paymentVoucherHeader.DocumentReference,
                    EntryDate = paymentVoucherHeader.EntryDate,
                    DocumentDate = paymentVoucherHeader.DocumentDate,
                    PaymentReference = paymentVoucherHeader.PaymentReference,
                    PeerReference = paymentVoucherHeader.PeerReference,
                    Prefix = paymentVoucherHeader.Prefix,
                    Suffix = paymentVoucherHeader.Suffix,
                    JournalHeaderId = paymentVoucherHeader.JournalHeaderId,
                    PaymentVoucherCode = paymentVoucherHeader.PaymentVoucherCode,
                    PaymentVoucherCodeFull = $"{paymentVoucherHeader.Prefix}{paymentVoucherHeader.PaymentVoucherCode}{paymentVoucherHeader.Suffix}",
                    RemarksAr = paymentVoucherHeader.RemarksAr,
                    RemarksEn = paymentVoucherHeader.RemarksEn,
                    SellerId = paymentVoucherHeader.SellerId ?? 0,
                    SellerCode = seller != null ? seller.SellerCode : null,
                    SellerName = seller != null ? language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn : "",
                    StoreId = paymentVoucherHeader.StoreId,
                    StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                    SupplierId = paymentVoucherHeader.SupplierId,
                    SupplierCode = supplier != null ? supplier.SupplierCode : null,
					SupplierName = supplier != null ? language == LanguageCode.Arabic ? supplier.SupplierNameAr : supplier.SupplierNameEn : null,
					ArchiveHeaderId = paymentVoucherHeader.ArchiveHeaderId,
                    StoreCurrencyId = company.CurrencyId,
                    TotalDebitValue = paymentVoucherHeader.TotalDebitValue,
                    TotalCreditValue = paymentVoucherHeader.TotalCreditValue,
                    TotalDebitValueAccount = paymentVoucherHeader.TotalDebitValueAccount,
                    TotalCreditValueAccount = paymentVoucherHeader.TotalCreditValueAccount,
                };
            return data;
        }

        public IQueryable<PaymentVoucherHeaderDto> GetUserPaymentVoucherHeaders()
        {
	        var userStore = _httpContextAccessor.GetCurrentUserStore();
			return GetPaymentVoucherHeaders().Where(x => x.StoreId == userStore);
		}

		public async Task<PaymentVoucherHeaderDto> GetPaymentVoucherHeaderById(int headerId)
        {
            return await GetPaymentVoucherHeaders().FirstOrDefaultAsync(x => x.PaymentVoucherHeaderId == headerId) ?? new PaymentVoucherHeaderDto();
        }

        public async Task<DocumentCodeDto> GetPaymentVoucherCode(int storeId, DateTime documentDate)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetPaymentVoucherCodeInternal(storeId, separateYears, documentDate);
        }

        public async Task<DocumentCodeDto> GetPaymentVoucherCodeInternal(int storeId, bool separateYears, DateTime documentDate)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.PaymentVoucher);
            var code = await GetNextPaymentVoucherCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        public async Task<int> GetNextPaymentVoucherCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
        {
            int id = 1;
            try { id = await _repository.GetAll().Where(x => x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix).MaxAsync(a => a.PaymentVoucherCode) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.PaymentVoucherHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> SavePaymentVoucherHeader(PaymentVoucherHeaderDto paymentVoucher, bool hasApprove, bool approved, int? requestId)
        {
            var separateYears = await _applicationSettingService.SeparateYears(paymentVoucher.StoreId);

            if (hasApprove)
            {
                if (paymentVoucher.PaymentVoucherHeaderId == 0)
                {
                    return await CreatePaymentVoucher(paymentVoucher, hasApprove, approved, requestId, separateYears);
                }
                else
                {
                    return await UpdatePaymentVoucher(paymentVoucher);
                }
            }
            else
            {
                var paymentVoucherExist = await IsPaymentVoucherCodeExist(paymentVoucher.PaymentVoucherHeaderId, paymentVoucher.PaymentVoucherCode, paymentVoucher.StoreId, paymentVoucher.Prefix, paymentVoucher.Suffix, separateYears, paymentVoucher.DocumentDate);
                if (paymentVoucherExist.Success)
                {
                    var nextPaymentVoucherCode = await GetNextPaymentVoucherCode(paymentVoucher.StoreId, separateYears, paymentVoucher.DocumentDate, paymentVoucher.Prefix, paymentVoucher.Suffix);
                    return new ResponseDto() { Id = nextPaymentVoucherCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = _localizer["PaymentVoucherAlreadyExist", nextPaymentVoucherCode] };
                }
                else
                {
                    if (paymentVoucher.PaymentVoucherHeaderId == 0)
                    {
                        return await CreatePaymentVoucher(paymentVoucher, hasApprove, approved, requestId, separateYears);
                    }
                    else
                    {
                        return await UpdatePaymentVoucher(paymentVoucher);
                    }
                }
            }
        }

        public async Task<bool> UpdatePaymentVoucherWithJournalHeaderId(int paymentVoucherHeaderId, int journalHeaderId)
        {
            var paymentVoucher = await _repository.GetAll().FirstOrDefaultAsync(x => x.PaymentVoucherHeaderId == paymentVoucherHeaderId);
            if (paymentVoucher != null)
            {
                paymentVoucher.JournalHeaderId = journalHeaderId;
                _repository.Update(paymentVoucher);
                await _repository.SaveChanges();
            }
            return true;
        }

        public async Task<ResponseDto> CreatePaymentVoucher(PaymentVoucherHeaderDto paymentVoucher, bool hasApprove, bool approved, int? requestId, bool separateYears)
        {
            int paymentVoucherCode;
            if (hasApprove && approved)
            {
                paymentVoucherCode = await GetNextPaymentVoucherCode(paymentVoucher.StoreId, separateYears, paymentVoucher.DocumentDate, paymentVoucher.Prefix, paymentVoucher.Suffix);
            }
            else
            {
                paymentVoucherCode = paymentVoucher.PaymentVoucherCode;
            }

            var paymentVoucherId = await GetNextId();
            var newPaymentVoucher = new PaymentVoucherHeader()
            {
                PaymentVoucherHeaderId = paymentVoucherId,
                StoreId = paymentVoucher.StoreId,
                SupplierId = paymentVoucher.SupplierId,
                DocumentDate = paymentVoucher.DocumentDate,
                DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.PaymentVoucher}{paymentVoucherId}",
                EntryDate = DateHelper.GetDateTimeNow(),
                PeerReference = paymentVoucher.PeerReference,
                Prefix = paymentVoucher.Prefix,
                Suffix = paymentVoucher.Suffix,
                RemarksAr = paymentVoucher.RemarksAr,
                RemarksEn = paymentVoucher.RemarksEn,
                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
                PaymentVoucherCode = paymentVoucherCode,
                PaymentReference = paymentVoucher.PaymentReference,
                SellerId = paymentVoucher.SellerId != 0 ? paymentVoucher.SellerId : null,
                JournalHeaderId = null,
                TotalDebitValue = paymentVoucher.TotalDebitValue,
                TotalCreditValue = paymentVoucher.TotalCreditValue,
                TotalDebitValueAccount = paymentVoucher.TotalDebitValueAccount,
                TotalCreditValueAccount = paymentVoucher.TotalCreditValueAccount,
            };

            var paymentVoucherValidator = await new PaymentVoucherValidator(_localizer).ValidateAsync(newPaymentVoucher);
            var validationResult = paymentVoucherValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newPaymentVoucher);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newPaymentVoucher.PaymentVoucherHeaderId, Success = true, Message = _localizer["NewPaymentVoucherSuccessMessage", $"{paymentVoucher.Prefix}{newPaymentVoucher.PaymentVoucherCode}{paymentVoucher.Suffix}"] };
            }
            else
            {
                return new ResponseDto() { Id = newPaymentVoucher.PaymentVoucherHeaderId, Success = false, Message = paymentVoucherValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdatePaymentVoucher(PaymentVoucherHeaderDto paymentVoucher)
        {
            var paymentVoucherDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.PaymentVoucherHeaderId == paymentVoucher.PaymentVoucherHeaderId);
            if (paymentVoucherDb != null)
            {
                paymentVoucherDb.Prefix = paymentVoucher.Prefix;
                paymentVoucherDb.PaymentVoucherCode = paymentVoucher.PaymentVoucherCode;
                paymentVoucherDb.Suffix = paymentVoucher.Suffix;
                paymentVoucherDb.PeerReference = paymentVoucher.PeerReference;
                paymentVoucherDb.TotalDebitValue = paymentVoucher.TotalDebitValue;
                paymentVoucherDb.TotalCreditValue = paymentVoucher.TotalCreditValue;
                paymentVoucherDb.TotalDebitValueAccount = paymentVoucher.TotalDebitValueAccount;
                paymentVoucherDb.TotalCreditValueAccount = paymentVoucher.TotalCreditValueAccount;
                paymentVoucherDb.RemarksAr = paymentVoucher.RemarksAr;
                paymentVoucherDb.RemarksEn = paymentVoucher.RemarksEn;
                paymentVoucherDb.StoreId = paymentVoucher.StoreId;
                paymentVoucherDb.DocumentDate = paymentVoucher.DocumentDate;
                //paymentVoucherDb.EntryDate = paymentVoucher.EntryDate;
                //paymentVoucherDb.DocumentReference = paymentVoucher.DocumentReference;
                paymentVoucherDb.PaymentReference = paymentVoucher.PaymentReference;
                paymentVoucherDb.SellerId = paymentVoucher.SellerId != 0 ? paymentVoucher.SellerId : null;
                paymentVoucherDb.SupplierId = paymentVoucher.SupplierId;

                //paymentVoucherDb.JournalHeaderId = paymentVoucher.JournalHeaderId;
                paymentVoucherDb.ModifiedAt = DateHelper.GetDateTimeNow();
                paymentVoucherDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                paymentVoucherDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

                var paymentVoucherValidator = await new PaymentVoucherValidator(_localizer).ValidateAsync(paymentVoucherDb);
                var validationResult = paymentVoucherValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(paymentVoucherDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = paymentVoucherDb.PaymentVoucherHeaderId, Success = true, Message = _localizer["UpdatePaymentVoucherSuccessMessage", $"{paymentVoucherDb.Prefix}{paymentVoucherDb.PaymentVoucherCode}{paymentVoucherDb.Suffix}"] };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = paymentVoucherValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoPaymentVoucherFound"] };
        }

        public async Task<ResponseDto> IsPaymentVoucherCodeExist(int paymentVoucherHeaderId, int paymentVoucherCode, int storeId, string? prefix, string? suffix, bool separateYears, DateTime documentDate)
        {
            var paymentVoucher = await _repository.GetAll().FirstOrDefaultAsync(x => x.StoreId == storeId && x.PaymentVoucherCode == paymentVoucherCode && x.Prefix == prefix && x.Suffix == suffix && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.PaymentVoucherHeaderId != paymentVoucherHeaderId);
            if (paymentVoucher != null)
            {
                return new ResponseDto() { Id = paymentVoucher.PaymentVoucherHeaderId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }
        public async Task<ResponseDto> DeletePaymentVoucherHeader(int id)
        {
            var paymentVoucherDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.PaymentVoucherHeaderId == id);
            if (paymentVoucherDb != null)
            {
                _repository.Delete(paymentVoucherDb);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeletePaymentVoucherMessage", $"{paymentVoucherDb.Prefix}{paymentVoucherDb.PaymentVoucherCode}{paymentVoucherDb.Suffix}"] };
            }
            return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoPaymentVoucherFound"] };
        }
    }
}
