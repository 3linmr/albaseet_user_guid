using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Items
{
    public class ItemTypeService : BaseService<ItemType>, IItemTypeService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ItemTypeService(IRepository<ItemType> repository, IHttpContextAccessor httpContextAccessor) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public IQueryable<ItemTypeDto> GetItemTypes()
        {
            return _repository.GetAll().Select(x => new ItemTypeDto()
            {
                ItemTypeId = x.ItemTypeId,
                ItemTypeNameAr = x.ItemTypeNameAr,
                ItemTypeNameEn = x.ItemTypeNameEn
            });
        }

        public IQueryable<ItemTypeDropDownDto> GetItemTypesDropDown()
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            return GetItemTypes().Select(s => new ItemTypeDropDownDto()
            {
                ItemTypeId = s.ItemTypeId,
                ItemTypeName = language == LanguageCode.Arabic ? s.ItemTypeNameAr : s.ItemTypeNameEn
            }).OrderBy(x => x.ItemTypeName);
        }
    }
}
