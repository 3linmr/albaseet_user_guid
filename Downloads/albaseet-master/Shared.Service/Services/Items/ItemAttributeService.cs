using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Domain.Taxes;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Taxes;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Items
{
	public class ItemAttributeService : BaseService<ItemAttribute>, IItemAttributeService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IItemAttributeTypeService _itemAttributeTypeService;

		public ItemAttributeService(IRepository<ItemAttribute> repository,IHttpContextAccessor httpContextAccessor,IItemAttributeTypeService itemAttributeTypeService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_itemAttributeTypeService = itemAttributeTypeService;
		}

		public IQueryable<ItemAttributeDto> GetAllItemAttributes()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from itemAttribute in _repository.GetAll()
				from itemAttributeType in _itemAttributeTypeService.GetAll().Where(x=>x.ItemAttributeTypeId == itemAttribute.ItemAttributeTypeId)
				select new ItemAttributeDto
				{
					ItemAttributeId = itemAttribute.ItemAttributeId,
					ItemAttributeTypeId = itemAttribute.ItemAttributeTypeId,
					ItemAttributeNameAr = itemAttribute.ItemAttributeNameAr,
					ItemAttributeNameEn = itemAttribute.ItemAttributeNameEn,
					ItemId = itemAttribute.ItemId,
					ItemAttributeTypeName = language == LanguageCode.Arabic ? itemAttributeType.ItemAttributeTypeNameAr : itemAttributeType.ItemAttributeTypeNameEn
				};
			return data;
		}

		public IQueryable<ItemAttributeDto> GetItemAttributesByItemId(int itemId)
		{
			return GetAllItemAttributes().Where(x => x.ItemId == itemId);
		}

		public Task<ItemAttributeDto?> GetItemAttributeById(int id)
		{
			return GetAllItemAttributes().FirstOrDefaultAsync(x => x.ItemAttributeId == id);
		}

		public async Task<ResponseDto> SaveItemAttributes(List<ItemAttributeDto> attributes, int itemId)
		{
			if (attributes.Any())
			{
				if (itemId == 0)
				{
					await CreateItemAttribute(attributes, itemId);
					return new ResponseDto() { Success = true };
				}
				else
				{
					await DeleteItemAttribute(attributes, itemId);
					await CreateItemAttribute(attributes, itemId);
					await UpdateItemAttribute(attributes, itemId);
					return new ResponseDto() { Success = true };
				}
			}
			else
			{
				return new ResponseDto() { Success = false };
			}
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemAttributeId) + 1; } catch { id = 1; }
			return id;
		}
		public async Task<bool> CreateItemAttribute(List<ItemAttributeDto> attributes, int itemId)
		{
			var attributeList = new List<ItemAttribute>();
			var newId = await GetNextId();
			foreach (var attribute in attributes)
			{
				if (attribute.ItemAttributeId <= 0)
				{
					var newPercent = new ItemAttribute()
					{
						ItemAttributeId = newId,
						ItemAttributeTypeId = attribute.ItemAttributeTypeId,
						ItemId = itemId,
						ItemAttributeNameAr = attribute.ItemAttributeNameAr?.Trim(),
						ItemAttributeNameEn = attribute.ItemAttributeNameEn?.Trim(),
						CreatedAt = DateHelper.GetDateTimeNow(),
						UserNameCreated = await _httpContextAccessor!.GetUserName(),
						IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
						Hide = false,
					};
					attributeList.Add(newPercent);
					newId++;
				}
			}

			if (attributeList.Any())
			{
				await _repository.InsertRange(attributeList);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}
		public async Task<bool> UpdateItemAttribute(List<ItemAttributeDto> attributes, int itemId)
		{
			var currentPercents = attributes.Where(x => x.ItemAttributeId > 0 && x.ItemId > 0).ToList();
			var attributeList = new List<ItemAttribute>();
			foreach (var attribute in currentPercents)
			{
				if (attribute.ItemAttributeId > 0)
				{
					var newNote = new ItemAttribute()
					{
						ItemAttributeId = attribute.ItemAttributeId,
						ItemAttributeTypeId = attribute.ItemAttributeTypeId,
						ItemId = itemId,
						ItemAttributeNameAr = attribute.ItemAttributeNameAr?.Trim(),
						ItemAttributeNameEn = attribute.ItemAttributeNameEn?.Trim(),
						ModifiedAt = DateHelper.GetDateTimeNow(),
						UserNameModified = await _httpContextAccessor!.GetUserName(),
						IpAddressModified = _httpContextAccessor?.GetIpAddress(),
						Hide = false
					};
					attributeList.Add(newNote);
				}
			}

			if (attributeList.Any())
			{
				_repository.UpdateRange(attributeList);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}
		public async Task<bool> DeleteItemAttribute(List<ItemAttributeDto> attributes, int itemId)
		{
			if (attributes.Any())
			{
				var currentPercents = await _repository.GetAll().Where(x => x.ItemId == itemId).AsNoTracking().ToListAsync();
				var notesToBeDeleted = currentPercents.Where(p => attributes.All(p2 => p2.ItemAttributeId != p.ItemAttributeId)).ToList();
				if (notesToBeDeleted.Any())
				{
					_repository.RemoveRange(notesToBeDeleted);
					await _repository.SaveChanges();
					return true;
				}
			}
			return false;
		}

		public async Task<ResponseDto> DeleteItemAttributesByItemId(int itemId)
		{
			var data = await _repository.GetAll().Where(x => x.ItemId == itemId).ToListAsync();
			if (data.Any())
			{
				_repository.RemoveRange(data);
				await _repository.SaveChanges();
				return new ResponseDto() { Success = true };
			}
			return new ResponseDto() { Success = false };
		}
	}
}
