using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Menus;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Menus;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;

namespace Shared.Service.Services.Menus
{
	 public class MenuEncodingService : BaseService<MenuEncoding>,IMenuEncodingService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<MenuEncodingService> _localizer;

		public MenuEncodingService(IRepository<MenuEncoding> repository,IHttpContextAccessor httpContextAccessor,IStringLocalizer<MenuEncodingService>localizer) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
		}

		public IQueryable<MenuEncodingDto> GetAllMenuEncoding()
		{
			return _repository.GetAll().Select(x => new MenuEncodingDto()
			{
				MenuEncodingId = x.MenuEncodingId,
				MenuCode = x.MenuCode,
				StoreId = x.StoreId,
				Prefix = x.Prefix,
				Suffix = x.Suffix
			});
		}

		public IQueryable<MenuEncodingDto> GetAllMenuEncodingByStoreId(int storeId)
		{
			return GetAllMenuEncoding().Where(x => x.StoreId == storeId);
		}

		public async Task<MenuEncodingDto?> GetMenuEncodingById(int id)
		{
			return await GetAllMenuEncoding().FirstOrDefaultAsync(x => x.MenuEncodingId == id);
		}

		public async Task<MenuEncodingVm> GetMenuEncoding(int storeId, int menuCode)
		{
			return await  GetAllMenuEncoding().Where(x => x.MenuCode == menuCode &&  x.StoreId == storeId ).Select(s=> new MenuEncodingVm()
			{
				Prefix = s.Prefix,
				Suffix = s.Suffix
			}).FirstOrDefaultAsync() ?? new MenuEncodingVm();
		}

		public async Task<ResponseDto> SaveMenuEncoding(List<MenuEncodingDto> menuEncodings)
		{
			var modelListInsert = new List<MenuEncoding>();
			var modelListUpdate = new List<MenuEncoding>();

			var nextId = await GetNextId();
			foreach (var encoding in menuEncodings)
			{
				if (encoding.MenuEncodingId == 0)
				{
					var model = new MenuEncoding()
					{
						MenuEncodingId = nextId,
						MenuCode = encoding.MenuCode,
						StoreId = encoding.StoreId,
						Prefix = encoding.Prefix?.Trim(),
						Suffix = encoding.Suffix?.Trim(),
						CreatedAt = DateHelper.GetDateTimeNow(),
						UserNameCreated = await _httpContextAccessor!.GetUserName(),
						IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
						Hide = false
					};
					modelListInsert.Add(model);
					nextId++;
				}
				else
				{
					var modelDb = await _repository.GetAll().FirstOrDefaultAsync(x=>x.MenuEncodingId == encoding.MenuEncodingId);
					if (modelDb != null)
					{
						modelDb.Prefix = encoding.Prefix?.Trim();
						modelDb.Suffix = encoding.Suffix?.Trim();
						modelDb.ModifiedAt = DateHelper.GetDateTimeNow();
						modelDb.UserNameModified = await _httpContextAccessor!.GetUserName();
						modelDb.IpAddressModified = _httpContextAccessor?.GetIpAddress();
						modelListUpdate.Add(modelDb);
					}
				}
			}

			if (!modelListInsert.Any() && !modelListUpdate.Any())
			{
				return new ResponseDto() { Id = 0, Success = true, Message = _localizer["MenuEncodingNothingMessage"] };
			}
			else
			{
				if (modelListInsert.Any())
				{
					await _repository.InsertRange(modelListInsert);
					await _repository.SaveChanges();
				}

				if (modelListUpdate.Any())
				{
					_repository.UpdateRange(modelListUpdate);
					await _repository.SaveChanges();
				}
				return new ResponseDto() { Id = 0, Success = true, Message = _localizer["MenuEncodingSuccessMessage"] };
			}
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.MenuEncodingId) + 1; } catch { id = 1; }
			return id;
		}

	}
}
