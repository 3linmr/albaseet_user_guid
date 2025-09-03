using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
	public class SellerService : BaseService<Seller>, ISellerService
	{
		private readonly IStringLocalizer<SellerService> _localizer;
		private readonly ISellerTypeService _sellerTypeService;
		private readonly ISellerCommissionService _sellerCommissionService;
		private readonly ISellerCommissionMethodService _sellerCommissionMethodService;
		private readonly ICompanyService _companyService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IBranchService _branchService;
		private readonly IStoreService _storeService;

		public SellerService(IRepository<Seller> repository, IStringLocalizer<SellerService> localizer, ISellerTypeService sellerTypeService,ISellerCommissionService sellerCommissionService, ISellerCommissionMethodService sellerCommissionMethodService, ICompanyService companyService, IHttpContextAccessor httpContextAccessor, IBranchService branchService, IStoreService storeService) : base(repository)
		{
			_localizer = localizer;
			_sellerTypeService = sellerTypeService;
			_sellerCommissionService = sellerCommissionService;
			_sellerCommissionMethodService = sellerCommissionMethodService;
			_companyService = companyService;
			_httpContextAccessor = httpContextAccessor;
			_branchService = branchService;
			_storeService = storeService;
		}

		public IQueryable<SellerDto> GetAllSellers()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from seller in _repository.GetAll()
				from company in _companyService.GetAll().Where(x => x.CompanyId == seller.CompanyId)
				from sellerType in _sellerTypeService.GetSellerTypes().Where(x => x.SellerTypeId == seller.SellerTypeId)
				from sellerCommissionMethod in _sellerCommissionMethodService.GetAll().Where(x => x.SellerCommissionMethodId == seller.SellerCommissionMethodId).DefaultIfEmpty()
				select new SellerDto()
				{
					SellerId = seller.SellerId,
					SellerCode = seller.SellerCode,
					SellerNameAr = seller.SellerNameAr,
					SellerNameEn = seller.SellerNameEn,
					CompanyId = seller.CompanyId,
					CompanyName = language == LanguageCode.Arabic ? company.CompanyNameAr : company.CompanyNameEn,
					IsActiveName = (bool)seller.IsActive ? _localizer["Active"].Value : _localizer["InActive"].Value,
					Address = seller.Address,
					InActiveReasons = seller.InActiveReasons,
					EmployeeId = seller.EmployeeId,
					ContractDate = seller.ContractDate,
					ClientsDebitLimit = seller.ClientsDebitLimit,
					Email = seller.Email,
					IsActive = seller.IsActive,
					OutSourcingCompany = seller.OutSourcingCompany,
					SellerTypeId = seller.SellerTypeId,
					SellerTypeName = sellerType.SellerTypeName,
					WhatsApp = seller.WhatsApp,
					Phone = seller.Phone,
					ModifiedAt = seller.ModifiedAt,
					UserNameModified = seller.UserNameModified,
					CreatedAt = seller.CreatedAt,
					UserNameCreated = seller.UserNameCreated,
					ArchiveHeaderId = seller.ArchiveHeaderId,
					SellerCommissionMethodId = seller.SellerCommissionMethodId ?? 0,
					SellerCommissionMethodName = sellerCommissionMethod != null ? (language == LanguageCode.Arabic ? sellerCommissionMethod.SellerCommissionMethodNameAr : sellerCommissionMethod.SellerCommissionMethodNameEn) : ""
				};
			return data;
		}

        public IQueryable<SellerDto> GetUserSellers()
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			return GetAllSellers().Where(x => x.CompanyId == companyId);
		}

        public IQueryable<SellerDropDownDto> GetSellersDropDownByCompanyId(int companyId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var data =
               (from seller in _repository.GetAll().Where(x => x.IsActive && x.CompanyId == companyId)
                select new SellerDropDownDto()
                {
                    SellerId = seller.SellerId,
					SellerCode = seller.SellerCode,
                    SellerName = language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn
                }).OrderBy(x => x.SellerName);
            return data;
        }

        public IQueryable<SellerDropDownDto> GetSellersDropDownByStoreId(int storeId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();
            var data =
               (from seller in _repository.GetAll().Where(x => x.IsActive)
                from company in _companyService.GetAll().Where(x => x.CompanyId == seller.CompanyId)
                from branch in _branchService.GetAll().Where(x => x.CompanyId == company.CompanyId)
                from store in _storeService.GetAll().Where(x => x.BranchId == branch.BranchId && x.StoreId == storeId)
                select new SellerDropDownDto()
                {
                    SellerId = seller.SellerId,
					SellerCode = seller.SellerCode,
                    SellerName = language == LanguageCode.Arabic ? seller.SellerNameAr : seller.SellerNameEn
                }).OrderBy(x => x.SellerName);
            return data;
        }

        public async Task<SellerCommissionMethodChangeDto?> GetSellerCommissionMethodChangeData(int commissionMethodId)
        {
	        var isConnected = await _repository.GetAll().AnyAsync(x => x.SellerCommissionMethodId == commissionMethodId);
	        var canEdit = !isConnected;
			var maxCommission = 1;

			if (canEdit)
	        {
		        try
		        {
			        maxCommission = await _sellerCommissionService.GetAll()
				        .Where(x => x.SellerCommissionMethodId == commissionMethodId).MaxAsync(s => s.To);
			        maxCommission +=1;

		        }
		        catch (Exception e)
		        {
			        Console.WriteLine(e);
		        }
	        }
			return new SellerCommissionMethodChangeDto(){SellerCommissionMethodId = commissionMethodId,CanEdit = canEdit, MaxFromValue = maxCommission};
        }

        public Task<SellerDto?> GetSellerById(int id)
		{
			return GetAllSellers().FirstOrDefaultAsync(x => x.SellerId == id);
		}

		public async Task<List<SellerAutoCompleteDto>> GetSellersAutoComplete(string term)
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			if (language == LanguageCode.Arabic)
			{
				return await _repository.GetAll().Where(x => x.CompanyId == companyId && x.IsActive && (x.SellerNameAr!.ToLower().Contains(term.Trim().ToLower()) || x.SellerCode.ToString().Contains(term.Trim().ToLower()))).Select(s => new SellerAutoCompleteDto
				{
					SellerId = s.SellerId,
					SellerCode = s.SellerCode,
					SellerName = $"{s.SellerCode} - {s.SellerNameAr}",
					SellerNameAr = s.SellerNameAr,
					SellerNameEn = s.SellerNameEn
				}).Take(10).ToListAsync();
			}
			else
			{
				return await _repository.GetAll().Where(x => x.CompanyId == companyId && x.IsActive && (x.SellerNameEn!.ToLower().Contains(term.Trim().ToLower()) || x.SellerCode.ToString().Contains(term.Trim().ToLower())) ).Select(s => new SellerAutoCompleteDto
				{
					SellerId = s.SellerId,
					SellerCode = s.SellerCode,
					SellerName = $"{s.SellerCode} - {s.SellerNameEn}",
					SellerNameAr = s.SellerNameAr,
					SellerNameEn = s.SellerNameEn
				}).Take(10).ToListAsync();
			}
		}

		public async Task<List<SellerAutoCompleteDto>> GetSellersAutoCompleteByStoreIds(string term, List<int> storeIds)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyIds = _storeService.GetAllStores().Where(x => storeIds.Contains(x.StoreId)).Select(x => x.CompanyId);

			if (language == LanguageCode.Arabic)
			{
				return await (from seller in _repository.GetAll()
							  where companyIds.Contains(seller.CompanyId) && seller.IsActive && (seller.SellerNameAr!.ToLower().Contains(term.Trim().ToLower()) || seller.SellerCode.ToString().Contains(term.Trim().ToLower()))
							  select new SellerAutoCompleteDto
							  {
								  SellerId = seller.SellerId,
								  SellerCode = seller.SellerCode,
								  SellerName = $"{seller.SellerCode} - {seller.SellerNameAr}",
								  SellerNameAr = seller.SellerNameAr,
								  SellerNameEn = seller.SellerNameEn
							  }).Take(10).ToListAsync();
			}
			else
			{
				return await (from seller in _repository.GetAll()
							  where companyIds.Contains(seller.CompanyId) && seller.IsActive && (seller.SellerNameEn!.ToLower().Contains(term.Trim().ToLower()) || seller.SellerCode.ToString().Contains(term.Trim().ToLower()))
							  select new SellerAutoCompleteDto
							  {
								  SellerId = seller.SellerId,
								  SellerCode = seller.SellerCode,
								  SellerName = $"{seller.SellerCode} - {seller.SellerNameEn}",
								  SellerNameAr = seller.SellerNameAr,
								  SellerNameEn = seller.SellerNameEn
							  }).Take(10).ToListAsync();
			}
		}

		public async Task<ResponseDto> SaveSeller(SellerDto seller)
		{
			var sellerExist = await IsSellerExist(seller.SellerId, seller.SellerNameAr, seller.SellerNameEn);
			if (sellerExist.Success)
			{
				return new ResponseDto() { Id = sellerExist.Id, Success = false, Message = _localizer["SellerAlreadyExist"] };
			}
			else
			{
				if (seller.SellerId == 0)
				{
					return await CreateSeller(seller);
				}
				else
				{
					return await UpdateSeller(seller);
				}
			}
		}

		public async Task<ResponseDto> IsSellerExist(int id, string? nameAr, string? nameEn)
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var seller = await _repository.GetAll().FirstOrDefaultAsync(x => (x.SellerNameAr == nameAr || x.SellerNameEn == nameEn || x.SellerNameAr == nameEn || x.SellerNameEn == nameAr) && x.SellerId != id && x.CompanyId == companyId);
			if (seller != null)
			{
				return new ResponseDto() { Id = seller.SellerId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.SellerId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId).MaxAsync(a => a.SellerCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateSeller(SellerDto seller)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var newSeller = new Seller()
			{
				SellerId = await GetNextId(),
				SellerCode = await GetNextCode(seller!.CompanyId),
				SellerNameAr = seller?.SellerNameAr?.Trim(),
				SellerNameEn = seller?.SellerNameEn?.Trim(),
				CompanyId = seller!.CompanyId,
				Email = seller?.Email,
				Address = seller?.Address,
				InActiveReasons = seller?.InActiveReasons,
				EmployeeId = seller?.EmployeeId,
				ContractDate = seller!.ContractDate,
				ClientsDebitLimit = seller.ClientsDebitLimit,
				IsActive = seller.IsActive,
				OutSourcingCompany = seller.OutSourcingCompany,
				SellerTypeId = seller.SellerTypeId,
				SellerCommissionMethodId = seller?.SellerCommissionMethodId != 0 ? seller?.SellerCommissionMethodId : null,
				WhatsApp = seller?.WhatsApp,
				Phone = seller?.Phone,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
			};

			var sellerValidator = await new SellerValidator(_localizer).ValidateAsync(newSeller);
			var validationResult = sellerValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newSeller);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newSeller.SellerId, Success = true, Message = _localizer["NewSellerSuccessMessage", ((language == LanguageCode.Arabic ? newSeller.SellerNameAr : newSeller.SellerNameEn) ?? ""), newSeller.SellerCode] };
			}
			else
			{
				return new ResponseDto() { Id = newSeller.SellerId, Success = false, Message = sellerValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateSeller(SellerDto seller)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var sellerDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.SellerId == seller.SellerId);
			if (sellerDb != null)
			{
				sellerDb.SellerNameAr = seller.SellerNameAr?.Trim();
				sellerDb.SellerNameEn = seller.SellerNameEn?.Trim();
				sellerDb.CompanyId = seller!.CompanyId;
				sellerDb.Email = seller?.Email;
				sellerDb.IsActive = seller!.IsActive;
				sellerDb.Email = seller?.Email;
				sellerDb.Address = seller?.Address;
				sellerDb.InActiveReasons = seller?.InActiveReasons;
				sellerDb.EmployeeId = seller?.EmployeeId;
				sellerDb.ContractDate = seller!.ContractDate;
				sellerDb.ClientsDebitLimit = seller.ClientsDebitLimit;
				sellerDb.OutSourcingCompany = seller.OutSourcingCompany;
				sellerDb.SellerTypeId = seller.SellerTypeId;
				sellerDb.SellerCommissionMethodId = seller?.SellerCommissionMethodId != 0 ? seller?.SellerCommissionMethodId : null;
				sellerDb.WhatsApp = seller?.WhatsApp;
				sellerDb.Phone = seller?.Phone;
				sellerDb.ModifiedAt = DateHelper.GetDateTimeNow();
				sellerDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				sellerDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var sellerValidator = await new SellerValidator(_localizer).ValidateAsync(sellerDb);
				var validationResult = sellerValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(sellerDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = sellerDb.SellerId, Success = true, Message = _localizer["UpdateSellerSuccessMessage", ((language == LanguageCode.Arabic ? sellerDb.SellerNameAr : sellerDb.SellerNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = sellerValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoSellerFound"] };
		}

		public async Task<ResponseDto> DeleteSeller(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var sellerDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.SellerId == id);
			if (sellerDb != null)
			{
				_repository.Delete(sellerDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteSellerSuccessMessage", ((language == LanguageCode.Arabic ? sellerDb.SellerNameAr : sellerDb.SellerNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoSellerFound"] };
		}
	}
}
