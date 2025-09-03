using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Models.Domain.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Accounts
{
	public class AccountStoreTypeService : BaseService<AccountStoreType>, IAccountStoreTypeService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public AccountStoreTypeService(IRepository<AccountStoreType> repository, IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public IQueryable<AccountStoreTypeDto> GetAccountStoreTypes()
		{
			return _repository.GetAll().Select(x => new AccountStoreTypeDto()
			{
				AccountStoreTypeId = x.AccountStoreTypeId,
				AccountStoreTypeNameAr = x.AccountStoreTypeNameAr,
				AccountStoreTypeNameEn = x.AccountStoreTypeNameEn
			});
		}

		public IQueryable<AccountStoreTypeDropDownDto> GetAccountStoreTypesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return GetAccountStoreTypes().Select(x => new AccountStoreTypeDropDownDto()
			{
				AccountStoreTypeId = x.AccountStoreTypeId,
				AccountStoreTypeName = language == LanguageCode.Arabic ? x.AccountStoreTypeNameAr : x.AccountStoreTypeNameEn
			});
		}
	}
}
