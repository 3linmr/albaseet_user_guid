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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Contracts.Modules;
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
    public class ReceiptVoucherHeaderService : BaseService<ReceiptVoucherHeader>, IReceiptVoucherHeaderService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISellerService _sellerService;
        private readonly IStoreService _storeService;
        private readonly IMenuEncodingService _menuEncodingService;
        private readonly IStringLocalizer<ReceiptVoucherHeaderService> _localizer;
        private readonly IBranchService _branchService;
        private readonly ICompanyService _companyService;
        private readonly IClientService _clientService;
        private readonly IApplicationSettingService _applicationSettingService;

        public ReceiptVoucherHeaderService(IRepository<ReceiptVoucherHeader> repository, IHttpContextAccessor httpContextAccessor, ISellerService sellerService, IStoreService storeService, IMenuEncodingService menuEncodingService, IStringLocalizer<ReceiptVoucherHeaderService> localizer, IBranchService branchService, ICompanyService companyService, IClientService clientService, IApplicationSettingService applicationSettingService) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _sellerService = sellerService;
            _storeService = storeService;
            _menuEncodingService = menuEncodingService;
            _localizer = localizer;
            _branchService = branchService;
            _companyService = companyService;
            _clientService = clientService;
            _applicationSettingService = applicationSettingService;
        }

        public IQueryable<ReceiptVoucherHeaderDto> GetReceiptVoucherHeaders()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var data =
                from receiptVoucherHeader in _repository.GetAll()
                from store in _storeService.GetAll().Where(x => x.StoreId == receiptVoucherHeader.StoreId)
                from branch in _branchService.GetAll().Where(x => x.BranchId == store.BranchId)
                from company in _companyService.GetAll().Where(x => x.CompanyId == branch.CompanyId)
                from seller in _sellerService.GetAll().Where(x => x.SellerId == receiptVoucherHeader.SellerId).DefaultIfEmpty()
                from client in _clientService.GetAll().Where(x => x.ClientId == receiptVoucherHeader.ClientId).DefaultIfEmpty()
                select new ReceiptVoucherHeaderDto
                {
                    ReceiptVoucherHeaderId = receiptVoucherHeader.ReceiptVoucherHeaderId,
                    DocumentReference = receiptVoucherHeader.DocumentReference,
                    EntryDate = receiptVoucherHeader.EntryDate,
                    DocumentDate = receiptVoucherHeader.DocumentDate,
                    PaymentReference = receiptVoucherHeader.PaymentReference,
                    PeerReference = receiptVoucherHeader.PeerReference,
                    Prefix = receiptVoucherHeader.Prefix,
                    Suffix = receiptVoucherHeader.Suffix,
                    JournalHeaderId = receiptVoucherHeader.JournalHeaderId,
                    ReceiptVoucherCode = receiptVoucherHeader.ReceiptVoucherCode,
                    ReceiptVoucherCodeFull = $"{receiptVoucherHeader.Prefix}{receiptVoucherHeader.ReceiptVoucherCode}{receiptVoucherHeader.Suffix}",
                    RemarksAr = receiptVoucherHeader.RemarksAr,
                    RemarksEn = receiptVoucherHeader.RemarksEn,
                    SellerId = receiptVoucherHeader.SellerId ?? 0,
                    SellerCode = seller != null ? seller.SellerCode : null,
                    SellerName = seller != null ? language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn : "",
                    StoreId = receiptVoucherHeader.StoreId,
                    StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
                    ClientId = client != null ? receiptVoucherHeader.ClientId : null,
                    ClientCode = client != null ? client.ClientCode : null,
                    ClientName = client != null ? language == LanguageCode.Arabic ? client.ClientNameAr : client.ClientNameEn : null,
					ArchiveHeaderId = receiptVoucherHeader.ArchiveHeaderId,
                    StoreCurrencyId = company.CurrencyId,
                    TotalDebitValue = receiptVoucherHeader.TotalDebitValue,
                    TotalCreditValue = receiptVoucherHeader.TotalCreditValue,
                    TotalDebitValueAccount = receiptVoucherHeader.TotalDebitValueAccount,
                    TotalCreditValueAccount = receiptVoucherHeader.TotalCreditValueAccount,
                };
            return data;
        }

        public IQueryable<ReceiptVoucherHeaderDto>GetUserReceiptVoucherHeaders()
        {
			var userStore = _httpContextAccessor.GetCurrentUserStore();
			return GetReceiptVoucherHeaders().Where(x => x.StoreId == userStore);
		}

		public async Task<ReceiptVoucherHeaderDto> GetReceiptVoucherHeaderById(int headerId)
        {
            return await GetReceiptVoucherHeaders().FirstOrDefaultAsync(x => x.ReceiptVoucherHeaderId == headerId) ?? new ReceiptVoucherHeaderDto();
        }

        public async Task<DocumentCodeDto> GetReceiptVoucherCode(int storeId, DateTime documentDate)
        {
            var separateYears = await _applicationSettingService.SeparateYears(storeId);
            return await GetReceiptVoucherCodeInternal(storeId, separateYears, documentDate);
        }

        public async Task<DocumentCodeDto> GetReceiptVoucherCodeInternal(int storeId, bool separateYears, DateTime documentDate)
        {
            var encoding = await _menuEncodingService.GetMenuEncoding(storeId, MenuCodeData.ReceiptVoucher);
            var code = await GetNextReceiptVoucherCode(storeId, separateYears, documentDate, encoding.Prefix, encoding.Suffix);
            return new DocumentCodeDto() { NextCode = code, Prefix = encoding.Prefix, Suffix = encoding.Suffix };
        }

        public async Task<int> GetNextReceiptVoucherCode(int storeId, bool separateYears, DateTime documentDate, string? prefix, string? suffix)
        {
            int id = 1;
            try { id = await _repository.GetAll().Where(x => x.StoreId == storeId && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.Prefix == prefix && x.Suffix == suffix).MaxAsync(a => a.ReceiptVoucherCode) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.ReceiptVoucherHeaderId) + 1; } catch { id = 1; }
            return id;
        }

        public async Task<ResponseDto> SaveReceiptVoucherHeader(ReceiptVoucherHeaderDto receiptVoucher, bool hasApprove, bool approved, int? requestId)
        {
            var separateYears = await _applicationSettingService.SeparateYears(receiptVoucher.StoreId);

            if (hasApprove)
            {
                if (receiptVoucher.ReceiptVoucherHeaderId == 0)
                {
                    return await CreateReceiptVoucher(receiptVoucher, hasApprove, approved, requestId, separateYears);
                }
                else
                {
                    return await UpdateReceiptVoucher(receiptVoucher);
                }
            }
            else
            {
                var receiptVoucherExist = await IsReceiptVoucherCodeExist(receiptVoucher.ReceiptVoucherHeaderId, receiptVoucher.ReceiptVoucherCode, receiptVoucher.StoreId, receiptVoucher.Prefix, receiptVoucher.Suffix, separateYears, receiptVoucher.DocumentDate);
                if (receiptVoucherExist.Success)
                {
                    var nextReceiptVoucherCode = await GetNextReceiptVoucherCode(receiptVoucher.StoreId, separateYears, receiptVoucher.DocumentDate, receiptVoucher.Prefix, receiptVoucher.Suffix);
                    return new ResponseDto() { Id = nextReceiptVoucherCode, ResponseType = ResponseTypeData.Confirm, Success = false, Message = _localizer["ReceiptVoucherAlreadyExist", nextReceiptVoucherCode] };
                }
                else
                {
                    if (receiptVoucher.ReceiptVoucherHeaderId == 0)
                    {
                        return await CreateReceiptVoucher(receiptVoucher, hasApprove, approved, requestId, separateYears);
                    }
                    else
                    {
                        return await UpdateReceiptVoucher(receiptVoucher);
                    }
                }
            }
        }

        public async Task<bool> UpdateReceiptVoucherWithJournalHeaderId(int receiptVoucherHeaderId, int journalHeaderId)
        {
            var receiptVoucher = await _repository.GetAll().FirstOrDefaultAsync(x => x.ReceiptVoucherHeaderId == receiptVoucherHeaderId);
            if (receiptVoucher != null)
            {
                receiptVoucher.JournalHeaderId = journalHeaderId;
                _repository.Update(receiptVoucher);
                await _repository.SaveChanges();
            }
            return true;
        }

        public async Task<ResponseDto> CreateReceiptVoucher(ReceiptVoucherHeaderDto receiptVoucher, bool hasApprove, bool approved, int? requestId, bool separateYears)
        {
            int receiptVoucherCode;
            if (hasApprove && approved)
            {
                receiptVoucherCode = await GetNextReceiptVoucherCode(receiptVoucher.StoreId, separateYears, receiptVoucher.DocumentDate, receiptVoucher.Prefix, receiptVoucher.Suffix);
            }
            else
            {
                receiptVoucherCode = receiptVoucher.ReceiptVoucherCode;
            }

            var receiptVoucherId = await GetNextId();
            var newReceiptVoucher = new ReceiptVoucherHeader()
            {
                ReceiptVoucherHeaderId = receiptVoucherId,
                StoreId = receiptVoucher.StoreId,
                ClientId = receiptVoucher.ClientId,
                DocumentDate = receiptVoucher.DocumentDate,
                DocumentReference = hasApprove ? $"{DocumentReferenceData.Approval}{requestId}" : $"{DocumentReferenceData.ReceiptVoucher}{receiptVoucherId}",
                EntryDate = DateHelper.GetDateTimeNow(),
                PeerReference = receiptVoucher.PeerReference,
                Prefix = receiptVoucher.Prefix,
                Suffix = receiptVoucher.Suffix,
                TotalDebitValue = receiptVoucher.TotalDebitValue,
                TotalCreditValue = receiptVoucher.TotalCreditValue,
                TotalDebitValueAccount = receiptVoucher.TotalDebitValueAccount,
                TotalCreditValueAccount = receiptVoucher.TotalCreditValueAccount,
                RemarksAr = receiptVoucher.RemarksAr,
                RemarksEn = receiptVoucher.RemarksEn,
                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false,
                ReceiptVoucherCode = receiptVoucherCode,
                PaymentReference = receiptVoucher.PaymentReference,
                SellerId = receiptVoucher.SellerId != 0 ? receiptVoucher.SellerId : null,
                JournalHeaderId = null,
            };

            var receiptVoucherValidator = await new ReceiptVoucherValidator(_localizer).ValidateAsync(newReceiptVoucher);
            var validationResult = receiptVoucherValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newReceiptVoucher);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newReceiptVoucher.ReceiptVoucherHeaderId, Success = true, Message = _localizer["NewReceiptVoucherSuccessMessage", $"{receiptVoucher.Prefix}{newReceiptVoucher.ReceiptVoucherCode}{receiptVoucher.Suffix}"] };
            }
            else
            {
                return new ResponseDto() { Id = newReceiptVoucher.ReceiptVoucherHeaderId, Success = false, Message = receiptVoucherValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateReceiptVoucher(ReceiptVoucherHeaderDto receiptVoucher)
        {
            var receiptVoucherDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ReceiptVoucherHeaderId == receiptVoucher.ReceiptVoucherHeaderId);
            if (receiptVoucherDb != null)
            {
                receiptVoucherDb.Prefix = receiptVoucher.Prefix;
                receiptVoucherDb.ReceiptVoucherCode = receiptVoucher.ReceiptVoucherCode;
                receiptVoucherDb.Suffix = receiptVoucher.Suffix;
                receiptVoucherDb.PeerReference = receiptVoucher.PeerReference;
                receiptVoucherDb.RemarksAr = receiptVoucher.RemarksAr;
                receiptVoucherDb.RemarksEn = receiptVoucher.RemarksEn;
                receiptVoucherDb.StoreId = receiptVoucher.StoreId;
                receiptVoucherDb.ClientId = receiptVoucher.ClientId;
                receiptVoucherDb.TotalDebitValue = receiptVoucher.TotalDebitValue;
                receiptVoucherDb.TotalCreditValue = receiptVoucher.TotalCreditValue;
                receiptVoucherDb.TotalDebitValueAccount = receiptVoucher.TotalDebitValueAccount;
                receiptVoucherDb.TotalCreditValueAccount = receiptVoucher.TotalCreditValueAccount;
                receiptVoucherDb.DocumentDate = receiptVoucher.DocumentDate;
                //receiptVoucherDb.EntryDate = receiptVoucher.EntryDate;
                //receiptVoucherDb.DocumentReference = receiptVoucher.DocumentReference;
                receiptVoucherDb.PaymentReference = receiptVoucher.PaymentReference;
                receiptVoucherDb.SellerId = receiptVoucher.SellerId != 0 ? receiptVoucher.SellerId : null;
                //receiptVoucherDb.JournalHeaderId = receiptVoucher.JournalHeaderId;
                receiptVoucherDb.ModifiedAt = DateHelper.GetDateTimeNow();
                receiptVoucherDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                receiptVoucherDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

                var receiptVoucherValidator = await new ReceiptVoucherValidator(_localizer).ValidateAsync(receiptVoucherDb);
                var validationResult = receiptVoucherValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(receiptVoucherDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = receiptVoucherDb.ReceiptVoucherHeaderId, Success = true, Message = _localizer["UpdateReceiptVoucherSuccessMessage", $"{receiptVoucherDb.Prefix}{receiptVoucherDb.ReceiptVoucherCode}{receiptVoucherDb.Suffix}"] };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = receiptVoucherValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoReceiptVoucherFound"] };
        }

        public async Task<ResponseDto> IsReceiptVoucherCodeExist(int receiptVoucherHeaderId, int receiptVoucherCode, int storeId, string? prefix, string? suffix, bool separateYears, DateTime documentDate)
        {
            var receiptVoucher = await _repository.GetAll().FirstOrDefaultAsync(x => x.StoreId == storeId && x.ReceiptVoucherCode == receiptVoucherCode && x.Prefix == prefix && x.Suffix == suffix && (!separateYears || x.DocumentDate.Year == documentDate.Year) && x.ReceiptVoucherHeaderId != receiptVoucherHeaderId);
            if (receiptVoucher != null)
            {
                return new ResponseDto() { Id = receiptVoucher.ReceiptVoucherHeaderId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }
        public async Task<ResponseDto> DeleteReceiptVoucherHeader(int id)
        {
            var receiptVoucherDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ReceiptVoucherHeaderId == id);
            if (receiptVoucherDb != null)
            {
                _repository.Delete(receiptVoucherDb);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteReceiptVoucherMessage", $"{receiptVoucherDb.Prefix}{receiptVoucherDb.ReceiptVoucherCode}{receiptVoucherDb.Suffix}"] };
            }
            return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoReceiptVoucherFound"] };
        }
    }
}
