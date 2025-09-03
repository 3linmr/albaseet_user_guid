using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Modules
{
	public class PaymentTypeService : BaseService<PaymentType>,IPaymentTypeService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public PaymentTypeService(IRepository<PaymentType> repository,IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public IQueryable<PaymentTypeDto> GetPaymentTypes()
		{
			return _repository.GetAll().Select(x => new PaymentTypeDto()
			{
				PaymentTypeId = x.PaymentTypeId,
				PaymentTypeNameAr = x.PaymentTypeNameAr,
				PaymentTypeNameEn = x.PaymentTypeNameEn,
				PaymentTypeCode = x.PaymentTypeCode,
			});
		}

		public async Task<List<PaymentTypeDropDownDto>> GetPaymentTypesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return await GetPaymentTypes().Select(x => new PaymentTypeDropDownDto
			{
				PaymentTypeId = x.PaymentTypeId,
				PaymentTypeName = language == LanguageCode.Arabic ? x.PaymentTypeNameAr : x.PaymentTypeNameEn
			}).OrderBy(x => x.PaymentTypeName).ToListAsync();
		}
	}
}
