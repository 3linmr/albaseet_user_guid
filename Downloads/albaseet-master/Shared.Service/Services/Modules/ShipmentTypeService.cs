using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using Shared.Service.Services.Basics;
using Shared.Service.Validators;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
	public class ShipmentTypeService : BaseService<ShipmentType>, IShipmentTypeService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<ShipmentTypeService> _localizer;

		public ShipmentTypeService(IRepository<ShipmentType> repository,IHttpContextAccessor httpContextAccessor, IStringLocalizer<ShipmentTypeService> localizer) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
		}

		public IQueryable<ShipmentTypeDto> GetShipmentTypes()
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return _repository.GetAll().Where(x => x.CompanyId == companyId || x.CompanyId == null).Select(x => new ShipmentTypeDto()
			{
				ShipmentTypeId = x.ShipmentTypeId,
				ShipmentTypeCode = x.ShipmentTypeCode,
				ShipmentTypeNameAr =x.ShipmentTypeNameAr,
				ShipmentTypeNameEn = x.ShipmentTypeNameEn,
				ShipmentTypeName = language == LanguageCode.Arabic ? x.ShipmentTypeNameAr : x.ShipmentTypeNameEn,
				CompanyId = x.CompanyId,
			}).OrderBy(x => x.ShipmentTypeName);
		}

		public async Task<ShipmentTypeDto?> GetShipmentTypeById(int id)
		{
			return await GetShipmentTypes().FirstOrDefaultAsync(x => x.ShipmentTypeId == id);
		}

		public async Task<ResponseDto> SaveShipmentType(ShipmentTypeDto shipmentType)
		{
			var shipmentTypeExist = await IsShipmentTypeExist(shipmentType.ShipmentTypeId, shipmentType.ShipmentTypeNameAr, shipmentType.ShipmentTypeNameEn);
			if (shipmentTypeExist.Success)
			{
				return new ResponseDto() { Id = shipmentTypeExist.Id, Success = false, Message = _localizer["ShipmentTypeAlreadyExist"] };
			}
			else
			{
				if (shipmentType.ShipmentTypeId == 0)
				{
					return await CreateShipmentType(shipmentType);
				}
				else
				{
					return await UpdateShipmentType(shipmentType);
				}
			}
		}

		public async Task<ResponseDto> IsShipmentTypeExist(int id, string? nameAr, string? nameEn)
		{
			var companyId = _httpContextAccessor.GetCurrentUserCompany();
			var shipmentType = await _repository.GetAll().FirstOrDefaultAsync(x => (x.ShipmentTypeNameAr == nameAr || x.ShipmentTypeNameEn == nameEn || x.ShipmentTypeNameAr == nameEn || x.ShipmentTypeNameEn == nameAr) && x.ShipmentTypeId != id && x.CompanyId == companyId);
			if (shipmentType != null)
			{
				return new ResponseDto() { Id = shipmentType.ShipmentTypeId, Success = true };
			}
			return new ResponseDto() { Id = 0, Success = false };
		}

		public async Task<int> GetNextId()
		{
			var id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ShipmentTypeId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int companyId)
		{
			var id = 1;
			try { id = await _repository.GetAll().Where(x => x.CompanyId == companyId || x.CompanyId == null).MaxAsync(a => a.ShipmentTypeCode) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<ResponseDto> CreateShipmentType(ShipmentTypeDto shipmentType)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();

			var newShipmentType = new ShipmentType()
			{
				ShipmentTypeId = await GetNextId(),
				ShipmentTypeCode = await GetNextCode(companyId),
				ShipmentTypeNameAr = shipmentType?.ShipmentTypeNameAr?.Trim(),
				ShipmentTypeNameEn = shipmentType?.ShipmentTypeNameEn?.Trim(),
				CompanyId = companyId,
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false
			};

			var shipmentTypeValidator = await new ShipmentTypeValidator(_localizer).ValidateAsync(newShipmentType);
			var validationResult = shipmentTypeValidator.IsValid;
			if (validationResult)
			{
				await _repository.Insert(newShipmentType);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = newShipmentType.ShipmentTypeId, Success = true, Message = _localizer["NewShipmentTypeSuccessMessage", ((language == LanguageCode.Arabic ? newShipmentType.ShipmentTypeNameAr : newShipmentType.ShipmentTypeNameEn) ?? ""), newShipmentType.ShipmentTypeCode] };
			}
			else
			{
				return new ResponseDto() { Id = newShipmentType.ShipmentTypeId, Success = false, Message = shipmentTypeValidator.ToString("~") };
			}
		}

		public async Task<ResponseDto> UpdateShipmentType(ShipmentTypeDto shipmentType)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			var companyId = _httpContextAccessor.GetCurrentUserCompany();

			var shipmentTypeDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ShipmentTypeId == shipmentType.ShipmentTypeId);
			if (shipmentTypeDb != null)
			{
				shipmentTypeDb.ShipmentTypeNameAr = shipmentType.ShipmentTypeNameAr?.Trim();
				shipmentTypeDb.ShipmentTypeNameEn = shipmentType.ShipmentTypeNameEn?.Trim();
				shipmentTypeDb.CompanyId = companyId;
				shipmentTypeDb.ModifiedAt = DateHelper.GetDateTimeNow();
				shipmentTypeDb.UserNameModified = await _httpContextAccessor!.GetUserName();
				shipmentTypeDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();

				var shipmentTypeValidator = await new ShipmentTypeValidator(_localizer).ValidateAsync(shipmentTypeDb);
				var validationResult = shipmentTypeValidator.IsValid;
				if (validationResult)
				{
					_repository.Update(shipmentTypeDb);
					await _repository.SaveChanges();
					return new ResponseDto() { Id = shipmentTypeDb.ShipmentTypeId, Success = true, Message = _localizer["UpdateShipmentTypeSuccessMessage", ((language == LanguageCode.Arabic ? shipmentTypeDb.ShipmentTypeNameAr : shipmentTypeDb.ShipmentTypeNameEn) ?? "")] };
				}
				else
				{
					return new ResponseDto() { Id = 0, Success = false, Message = shipmentTypeValidator.ToString("~") };
				}
			}
			return new ResponseDto() { Id = 0, Success = false, Message = _localizer["NoShipmentTypeFound"] };
		}

		public async Task<ResponseDto> DeleteShipmentType(int id)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var shipmentTypeDb = await _repository.GetAll().FirstOrDefaultAsync(x => x.ShipmentTypeId == id);
			if (shipmentTypeDb != null)
			{
				_repository.Delete(shipmentTypeDb);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = id, Success = true, Message = _localizer["DeleteShipmentTypeSuccessMessage", ((language == LanguageCode.Arabic ? shipmentTypeDb.ShipmentTypeNameAr : shipmentTypeDb.ShipmentTypeNameEn) ?? "")] };
			}
			return new ResponseDto() { Id = id, Success = false, Message = _localizer["NoShipmentTypeFound"] };
		}
	}
}
