using Microsoft.AspNetCore.Http;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Basics;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.Helper.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Basics
{
	public class InvoiceTypeService: BaseService<InvoiceType>, IInvoiceTypeService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		public InvoiceTypeService(IRepository<InvoiceType> repository, IHttpContextAccessor httpContextAccessor) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public IQueryable<InvoiceTypeDto> GetAllInvoiceTypes()
		{
			return _repository.GetAll().Select(x => new InvoiceTypeDto { 
				InvoiceTypeId = x.InvoiceTypeId, 
				InvoiceTypeNameAr = x.InvoiceTypeNameAr,
				InvoiceTypeNameEn = x.InvoiceTypeNameEn
			});
		}

		public IQueryable<InvoiceTypeDropDownDto> GetAllInvoiceTypesDropDown()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			return _repository.GetAll().Select(x => new InvoiceTypeDropDownDto
			{
				InvoiceTypeId = x.InvoiceTypeId,
				InvoiceTypeName = language == LanguageCode.Arabic ? x.InvoiceTypeNameAr : x.InvoiceTypeNameEn,
			});
		}
	}
}
