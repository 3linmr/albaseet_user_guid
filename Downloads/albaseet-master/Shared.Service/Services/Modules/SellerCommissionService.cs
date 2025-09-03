using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using static Shared.CoreOne.Models.StaticData.StaticData;

namespace Shared.Service.Services.Modules
{
	public class SellerCommissionService : BaseService<SellerCommission>, ISellerCommissionService
	{
		private readonly ISellerCommissionTypeService _sellerCommissionTypeService;
		private readonly ISellerCommissionMethodService _sellerCommissionMethodService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<SellerCommissionService> _localizer;

		public SellerCommissionService(IRepository<SellerCommission> repository,ISellerCommissionTypeService sellerCommissionTypeService,ISellerCommissionMethodService sellerCommissionMethodService,IHttpContextAccessor httpContextAccessor,IStringLocalizer<SellerCommissionService>localizer) : base(repository)
		{
			_sellerCommissionTypeService = sellerCommissionTypeService;
			_sellerCommissionMethodService = sellerCommissionMethodService;
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
		}

		public IQueryable<SellerCommissionDto> GetAllSellerCommissions()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var days = _localizer["Days"].Value;
			var data =
				from sellerCommission in _repository.GetAll()
				from sellerCommissionMethod in _sellerCommissionMethodService.GetAll().Where(x=>x.SellerCommissionMethodId == sellerCommission.SellerCommissionMethodId)
				from sellerCommissionType in _sellerCommissionTypeService.GetAll().Where(x=>x.SellerCommissionTypeId == sellerCommissionMethod.SellerCommissionTypeId)
				select new SellerCommissionDto()
				{
					SellerCommissionId = sellerCommission.SellerCommissionId,
					SellerCommissionMethodId = sellerCommission.SellerCommissionMethodId,
					SellerCommissionTypeName = language == LanguageCode.Arabic ? sellerCommissionType.SellerCommissionTypeNameAr : sellerCommissionType.SellerCommissionTypeNameEn,
					SellerCommissionMethodName = language == LanguageCode.Arabic ? sellerCommissionMethod.SellerCommissionMethodNameAr : sellerCommissionMethod.SellerCommissionMethodNameEn,
					FromText = sellerCommissionType.SellerCommissionTypeId == SellerCommissionTypeData.AgeOfDebt ? $"{sellerCommission.From} {days}" : $"{sellerCommission.From}",
					ToText = sellerCommissionType.SellerCommissionTypeId == SellerCommissionTypeData.AgeOfDebt ? $"{sellerCommission.To} {days}" : $"{sellerCommission.To}",
					From = sellerCommission.From,
					To = sellerCommission.To,
					CommissionPercent = sellerCommission.CommissionPercent,
					CommissionPercentText = $"{sellerCommission.CommissionPercent} %"
				};
			return data;
		}

		public IQueryable<SellerCommissionDto> GetSellerCommissionsByType(int sellerCommissionMethodId)
		{
			return GetAllSellerCommissions().Where(x=>x.SellerCommissionMethodId == sellerCommissionMethodId);
		}

		public async Task<List<SellerCommissionDto>> GetSellerCommissionsByCommissionMethodId(int sellerCommissionMethodId)
		{
			var data = await GetAllSellerCommissions().Where(x=>x.SellerCommissionMethodId == sellerCommissionMethodId).ToListAsync();
			var orderedData = data.OrderByDescending(x => x.From).ToList();
			// Modify the first product and keep the others unchanged
			var newList = orderedData
				.Select((product, index) =>
				{
					if (index == 0)
					{
						product.CanEdit = true;
					}
					// Return other products unchanged
					return product;
				})
				.ToList();

			return newList.OrderBy(x=>x.From).ToList();
		}

		public Task<SellerCommissionDto?> GetSellerCommissionById(int commissionId)
		{
			return GetAllSellerCommissions().FirstOrDefaultAsync(x => x.SellerCommissionId == commissionId);
		}

		public async Task<ResponseDto> SaveSellerCommission(SellerCommissionDto sellerCommission)
		{
			var commissionExit = await IsSellerCommissionExist(sellerCommission);
			if (commissionExit.Success)
			{
				return new ResponseDto() { Id = commissionExit.Id, Success = false, Message = _localizer["SellerCommissionAlreadyExist"] };
			}
			else
			{
				var commissionInRange = await IsSellerCommissionInTheSameRange(sellerCommission);

				if (commissionInRange)
				{
					return new ResponseDto() { Id = commissionExit.Id, Success = false, Message = _localizer["SellerCommissionInTheSameRange"] };
				}
				else
				{
					if (sellerCommission.SellerCommissionId == 0)
					{
						return await CreateSellerCommission(sellerCommission);
					}
					else
					{
						return await UpdateSellerCommission(sellerCommission);
					}
				}
			}
		}

		public async Task<ResponseDto> IsSellerCommissionExist(SellerCommissionDto sellerCommission)
		{
			var commission = await _repository.GetAll().FirstOrDefaultAsync(x => (x.SellerCommissionMethodId == sellerCommission.SellerCommissionMethodId && x.From == sellerCommission.From && x.To == sellerCommission.To && x.CommissionPercent == sellerCommission.CommissionPercent) && x.SellerCommissionId != sellerCommission.SellerCommissionId);
			if (commission != null)
			{
				return new ResponseDto() { Id = commission.SellerCommissionId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}
		public async Task<bool> IsSellerCommissionInTheSameRange(SellerCommissionDto sellerCommission)
		{
			var data = await  _repository.GetAll().Where(x => x.SellerCommissionMethodId == sellerCommission.SellerCommissionMethodId && x.SellerCommissionId != sellerCommission.SellerCommissionId).ToListAsync();
			return data.Any(x=> sellerCommission.From.InRange(x.From,x.To) || sellerCommission.To.InRange(x.From,x.To));
		}
		//public async Task<bool> IsSellerCommissionHasGap(SellerCommissionDto sellerCommission)
		//{
		//	var data = await _repository.GetAll().Where(x => x.SellerCommissionMethodId == sellerCommission.SellerCommissionMethodId && x.SellerCommissionId != sellerCommission.SellerCommissionId).ToListAsync();
		//	var maxValue = data.Max(x => x.To);
		//	if (sellerCommission.To + 1 > maxValue)
		//	{
		//		return true;
		//	}
		//	return false;
		//}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.SellerCommissionId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateSellerCommission(SellerCommissionDto commission)
		{
			var newCommission = new SellerCommission()
			{
				SellerCommissionId = await GetNextId(),
				SellerCommissionMethodId = commission.SellerCommissionMethodId,
				From = commission.From,
				To = commission.To,
				CommissionPercent = commission.CommissionPercent,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false
			};

			var commissionValidator = await new SellerCommissionValidator(_localizer).ValidateAsync(newCommission);
			var validationResult = commissionValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newCommission);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newCommission.SellerCommissionId, Success = true, Message = _localizer["NewSellerCommissionSuccessMessage"] };
			}
			else
			{
				return new ResponseDto() { Id = newCommission.SellerCommissionId, Success = false, Message = commissionValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateSellerCommission(SellerCommissionDto commission)
		{
			var commissionDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.SellerCommissionId == commission.SellerCommissionId);
			if (commissionDb != null)
			{
				commissionDb.SellerCommissionMethodId = commission.SellerCommissionMethodId;
				commissionDb.From = commission.From;
				commissionDb.To = commission.To;
				commissionDb.CommissionPercent = commission.CommissionPercent;
				commissionDb.ModifiedAt = DateHelper.GetDateTimeNow();
				commissionDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				commissionDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var commissionValidator = await new SellerCommissionValidator(_localizer).ValidateAsync(commissionDb);
				var validationResult = commissionValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(commissionDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = commissionDb.SellerCommissionId, Success = true, Message = _localizer["UpdateSellerCommissionSuccessMessage"] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = commissionValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoSellerCommissionFound"] };
		}

		public async Task<ResponseDto> DeleteSellerCommission(int id)
		{
			var sellerCommission = await _repository.GetAll().FirstOrDefaultAsync(x => x.SellerCommissionId == id);
			if (sellerCommission != null)
			{
				_repository.Delete(sellerCommission);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteSellerCommissionSuccessMessage"] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoSellerCommissionFound"] };
		}
	}
}
