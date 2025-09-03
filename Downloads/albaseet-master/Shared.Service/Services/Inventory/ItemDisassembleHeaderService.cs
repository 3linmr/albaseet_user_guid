using Shared.CoreOne.Contracts.Inventory;
using Shared.CoreOne.Models.Domain.Inventory;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Inventory;
using Shared.Helper.Models.Dtos;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.Helper.Logic;

namespace Shared.Service.Services.Inventory
{
	public class ItemDisassembleHeaderService : BaseService<ItemDisassembleHeader>, IItemDisassembleHeaderService
	{
		private readonly IStoreService _storeService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ItemDisassembleHeaderService(IRepository<ItemDisassembleHeader> repository,IStoreService storeService,IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_storeService = storeService;
			_httpContextAccessor = httpContextAccessor;
		}

		public IQueryable<ItemDisassembleHeaderDto> GetItemDisassembleHeaders()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var data =
				from itemDisassembleHeader in _repository.GetAll()
				from store in _storeService.GetAll().Where(x=>x.StoreId == itemDisassembleHeader.StoreId)
				select new ItemDisassembleHeaderDto
				{
					StoreId = itemDisassembleHeader.StoreId,
					RemarksAr = itemDisassembleHeader.RemarksAr,
					MenuCode = itemDisassembleHeader.MenuCode,
					StoreName = language == LanguageCode.Arabic ? store.StoreNameAr : store.StoreNameEn,
					RemarksEn = itemDisassembleHeader.RemarksEn,
					EntryDate = itemDisassembleHeader.EntryDate,
					IsAutomatic = itemDisassembleHeader.IsAutomatic,
					UserNameCreated = itemDisassembleHeader.UserNameCreated,
					ModifiedAt = itemDisassembleHeader.ModifiedAt,
					UserNameModified = itemDisassembleHeader.UserNameModified,
					ItemDisassembleCode = itemDisassembleHeader.ItemDisassembleCode,
					CreatedAt = itemDisassembleHeader.CreatedAt,
					IpAddressCreated = itemDisassembleHeader.IpAddressCreated,
					IpAddressModified = itemDisassembleHeader.IpAddressModified,
					DocumentDate = itemDisassembleHeader.DocumentDate,
					ItemDisassembleHeaderId = itemDisassembleHeader.ItemDisassembleHeaderId,
					MenuCodeName = "",
					ReferenceDetailCode = itemDisassembleHeader.ReferenceDetailId,
					ReferenceHeaderCode = itemDisassembleHeader.ReferenceHeaderId
				};
			return data;
		}

		public IQueryable<ItemDisassembleHeaderDto> GetItemDisassembleHeadersByStoreId(int storeId)
		{
			return GetItemDisassembleHeaders().Where(x => x.StoreId == storeId);
		}

		public async Task<ItemDisassembleHeaderDto> GetItemDisassembleHeader(int itemDisassembleHeaderId)
		{
			return await GetItemDisassembleHeaders().FirstOrDefaultAsync(x=>x.ItemDisassembleHeaderId == itemDisassembleHeaderId) ?? new ItemDisassembleHeaderDto();
		}

		public async Task<ResponseDto> SaveItemDisassembleHeader(ItemDisassembleHeaderDto model)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var newModel = new ItemDisassembleHeader()
			{
				ItemDisassembleHeaderId = await GetNextId(),
				ItemDisassembleCode = await GetNextCode(model.StoreId),
				RemarksAr = model?.RemarksAr?.Trim(),
				RemarksEn = model?.RemarksEn?.Trim(),
				CreatedAt = DateHelper.GetDateTimeNow(),
				UserNameCreated = await _httpContextAccessor!.GetUserName(),
				IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				Hide = false,
				MenuCode = model!.MenuCode,
				StoreId = model.StoreId,
				DocumentDate = model.DocumentDate,
				IsAutomatic = model.IsAutomatic,
				ReferenceDetailId = model.ReferenceDetailCode,
				ReferenceHeaderId = model.ReferenceHeaderCode,
				EntryDate = DateHelper.GetDateTimeNow()
			};

			await _repository.Insert(newModel);
			await _repository.SaveChanges();
			return new ResponseDto() { Id = newModel.ItemDisassembleHeaderId, Success = true, Message = "Success"};
		}

		public async Task<ResponseDto> DeleteItemDisassembleHeader(int itemDisassembleHeaderId)
		{
			var model =  await _repository.GetAll().FirstOrDefaultAsync(x=>x.ItemDisassembleHeaderId == itemDisassembleHeaderId);
			if (model != null)
			{
				_repository.Delete(model);
				await _repository.SaveChanges();
				return new ResponseDto() { Id = model.ItemDisassembleHeaderId, Success = true, Message = "Success"};
			}
			return new ResponseDto() { Id = 0, Success = false, Message = "Delete Failed" };
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.ItemDisassembleHeaderId) + 1; } catch { id = 1; }
			return id;
		}

		public async Task<int> GetNextCode(int storeId)
		{
			int id = 1;
			try { id = await _repository.GetAll().Where(x => x.StoreId == storeId).MaxAsync(a => a.ItemDisassembleCode) + 1; } catch { id = 1; }
			return id;
		}
	}
}
