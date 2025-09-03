using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Items
{
    public class VendorService : BaseService<Vendor>, IVendorService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStringLocalizer<VendorService> _localizer;

        public VendorService(IRepository<Vendor> repository, IHttpContextAccessor httpContextAccessor, IStringLocalizer<VendorService> localizer) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _localizer = localizer;
        }

        public IQueryable<VendorDto> GetAllVendors()
        {
            return _repository.GetAll().Select(x => new VendorDto()
            {
                VendorId = x.VendorId,
                VendorCode = x.VendorCode,
                VendorNameAr = x.VendorNameAr,
                VendorNameEn = x.VendorNameEn,
                CompanyId = x.CompanyId,
                CreatedAt = x.CreatedAt,
                ModifiedAt = x.ModifiedAt,
                UserNameModified = x.UserNameModified,
                UserNameCreated = x.UserNameCreated,
            });
        }

        public IQueryable<VendorDto> GetCompanyVendors()
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            return GetAllVendors().Where(x => x.CompanyId == companyId);
        }

        public IQueryable<VendorDropDownDto> GetVendorsDropDown()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            return GetCompanyVendors().Where(x=>x.CompanyId == companyId).Select(x => new VendorDropDownDto()
            {
                VendorId = x.VendorId,
                VendorName = language == LanguageCode.Arabic ? x.VendorNameAr : x.VendorNameEn
            }).OrderBy(x => x.VendorName);
        }

        public Task<VendorDto?> GetVendorById(int id)
        {
            return GetAllVendors().FirstOrDefaultAsync(x => x.VendorId == id);
        }

        public async Task<ResponseDto> SaveVendor(VendorDto vendor)
        {
            var vendorExist = await IsVendorExist(vendor.VendorId, vendor.VendorNameAr, vendor.VendorNameEn);
            if (vendorExist.Success)
            {
                return new ResponseDto() { Id = vendorExist.Id, Success = false, Message = _localizer["VendorAlreadyExist"] };
            }
            else
            {
                if (vendor.VendorId == 0)
                {
                    return await CreateVendor(vendor);
                }
                else
                {
                    return await UpdateVendor(vendor);
                }
            }
        }

        public async Task<ResponseDto> IsVendorExist(int id, string? nameAr, string? nameEn)
        {
            var companyId = _httpContextAccessor.GetCurrentUserCompany();
            var vendor = await _repository.GetAll().FirstOrDefaultAsync(x => (x.VendorNameAr == nameAr || x.VendorNameEn == nameEn || x.VendorNameAr == nameEn || x.VendorNameEn == nameAr) && x.VendorId != id && x.CompanyId == companyId);
            if (vendor != null)
            {
                return new ResponseDto() { Id = vendor.VendorId, Success = true };
            }
            return new ResponseDto() { Id = 0, Success = false };
        }
        public async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.VendorId) + 1; } catch { id = 1; }
            return id;
        }

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.VendorCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateVendor(VendorDto vendor)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var companyId = _httpContextAccessor.GetCurrentUserCompany();

            var newVendor = new Vendor()
            {
                VendorId = await GetNextId(),
                VendorCode = await GetNextCode(companyId),
                VendorNameAr = vendor?.VendorNameAr?.Trim(),
                VendorNameEn = vendor?.VendorNameEn?.Trim(),
                CompanyId = companyId,
                CreatedAt = DateHelper.GetDateTimeNow(),
                UserNameCreated = await _httpContextAccessor!.GetUserName(),
                IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                Hide = false
            };

            var vendorValidator = await new VendorValidator(_localizer).ValidateAsync(newVendor);
            var validationResult = vendorValidator.IsValid;
            if (validationResult)
            {
                await _repository.Insert(newVendor);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = newVendor.VendorId, Success = true, Message = _localizer["NewVendorSuccessMessage", (language == LanguageCode.Arabic ? newVendor.VendorNameAr : newVendor.VendorNameEn) ?? "", newVendor.VendorCode] };
            }
            else
            {
                return new ResponseDto() { Id = newVendor.VendorId, Success = false, Message = vendorValidator.ToString("~") };
            }
        }

        public async Task<ResponseDto> UpdateVendor(VendorDto vendor)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var vendorDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.VendorId == vendor.VendorId);
            if (vendorDb != null)
            {
                vendorDb.VendorNameAr = vendor.VendorNameAr?.Trim();
                vendorDb.VendorNameEn = vendor.VendorNameEn?.Trim();
                vendorDb.ModifiedAt = DateHelper.GetDateTimeNow();
                vendorDb.UserNameModified = await _httpContextAccessor!.GetUserName();
                vendorDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

                var vendorValidator = await new VendorValidator(_localizer).ValidateAsync(vendorDb);
                var validationResult = vendorValidator.IsValid;
                if (validationResult)
                {
                    _repository.Update(vendorDb);
                    await _repository.SaveChanges();
                    return new ResponseDto() { Id = vendorDb.VendorId, Success = true, Message = _localizer["UpdateVendorSuccessMessage", (language == LanguageCode.Arabic ? vendorDb.VendorNameAr : vendorDb.VendorNameEn) ?? ""] };
                }
                else
                {
                    return new ResponseDto() { Id = 0, Success = false, Message = vendorValidator.ToString("~") };
                }
            }
            return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoVendorFound"] };
        }

        public async Task<ResponseDto> DeleteVendor(int id)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var vendorDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.VendorId == id);
            if (vendorDb != null)
            {
                _repository.Delete(vendorDb);
                await _repository.SaveChanges();
                return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteVendorSuccessMessage", (language == LanguageCode.Arabic ? vendorDb.VendorNameAr : vendorDb.VendorNameEn) ?? ""] };
            }
            return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoVendorFound"] };
        }
    }
}
