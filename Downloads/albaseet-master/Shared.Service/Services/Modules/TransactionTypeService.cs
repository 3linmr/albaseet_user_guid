using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
    public class TransactionTypeService : BaseService<TransactionType>, ITransactionTypeService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public TransactionTypeService(IRepository<TransactionType> repository, IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public IQueryable<TransactionTypeDto> GetTransactionTypes()
		{
			return _repository.GetAll().Select(x => new TransactionTypeDto()
			{
				TransactionTypeId = x.TransactionTypeId,
				TransactionTypeNameAr = x.TransactionTypeNameAr,
				TransactionTypeNameEn = x.TransactionTypeNameEn
			});
		}

		public IQueryable<TransactionTypeDropDownDto> GetTransactionTypesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();
			return GetTransactionTypes().Select(x => new TransactionTypeDropDownDto()
			{
				TransactionTypeId = x.TransactionTypeId,
				TransactionTypeName = language == LanguageCode.Arabic ? x.TransactionTypeNameAr : x.TransactionTypeNameEn
			}).OrderBy(x => x.TransactionTypeName);
		}
	}
}
