using Shared.CoreOne.Contracts.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne.Contracts.Settings;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Identity;
using Shared.CoreOne.Models.Domain.Modules;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Models.StaticData;
using static Shared.Service.Services.Items.ItemDivisionName;
using System.ComponentModel.Design;

namespace Shared.Service.Services.Items
{
	public class ItemDivisionService : IItemDivisionService
	{
		private readonly IApplicationSettingService _applicationSettingService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<ItemDivisionService> _localizer;

		public ItemDivisionService(IApplicationSettingService applicationSettingService,IHttpContextAccessor httpContextAccessor,IStringLocalizer<ItemDivisionService> localizer)
		{
			_applicationSettingService = applicationSettingService;
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
		}

		public async Task<ItemDivisionNamesDto> GetItemDivisions()
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var names = await _applicationSettingService.GetSettingDetail(ApplicationSettingHeaderData.ItemCategories).ToListAsync();

			var itemCategoryNameAr = names.Where(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.ItemCategoryAr).Select(x => x.FlagValue)
				.FirstOrDefault();
			var itemCategoryNameEn = names.Where(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.ItemCategoryEn).Select(x => x.FlagValue)
				.FirstOrDefault();

			var itemSubCategoryNameAr = names.Where(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.ItemSubCategoryAr).Select(x => x.FlagValue)
				.FirstOrDefault();
			var itemSubCategoryNameEn = names.Where(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.ItemSubCategoryEn).Select(x => x.FlagValue)
				.FirstOrDefault();

			var itemSectionNameAr = names.Where(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.ItemSectionAr).Select(x => x.FlagValue)
				.FirstOrDefault();
			var itemSectionNameEn = names.Where(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.ItemSectionEn).Select(x => x.FlagValue)
				.FirstOrDefault();

			var itemSubSectionNameAr = names.Where(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.ItemSubSectionAr).Select(x => x.FlagValue)
				.FirstOrDefault();
			var itemSubSectionNameEn = names.Where(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.ItemSubSectionEn).Select(x => x.FlagValue)
				.FirstOrDefault();

			var mainItemNameAr = names.Where(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.MainItemAr).Select(x => x.FlagValue)
				.FirstOrDefault();
			var mainItemNameEn = names.Where(x => x.ApplicationFlagDetailId == ApplicationSettingDetailData.MainItemEn).Select(x => x.FlagValue)
				.FirstOrDefault();


			return new ItemDivisionNamesDto()
			{
				ItemCategoryNameAr = itemCategoryNameAr,
				ItemCategoryNameEn = itemCategoryNameEn,
				ItemCategoryName = language == LanguageCode.Arabic ? itemCategoryNameAr : itemCategoryNameEn,
				ItemCategoryNameSelectAr = $"{_localizer["Choose"].Value} {itemCategoryNameAr}",
				ItemCategoryNameSelectEn = $"{_localizer["Choose"].Value} {itemCategoryNameEn}",
				ItemCategoryNameSelect = ComposeSelect(language, itemCategoryNameAr, itemCategoryNameEn, _localizer["Choose"].Value),
				ItemCategoryNameFirstAr = $"{_localizer["Choose"].Value} {itemCategoryNameAr} {_localizer["First"].Value}",
				ItemCategoryNameFirstEn = $"{_localizer["Choose"].Value} {itemCategoryNameEn} {_localizer["First"].Value}",
				ItemCategoryNameFirst = ComposeFirst(language, itemCategoryNameAr, itemCategoryNameEn, _localizer["Choose"].Value, _localizer["First"].Value),
				ItemSubCategoryNameAr = itemSubCategoryNameAr,
				ItemSubCategoryNameEn = itemSubCategoryNameEn,
				ItemSubCategoryName = language == LanguageCode.Arabic ? itemSubCategoryNameAr : itemSubCategoryNameEn,
				ItemSubCategoryNameSelectAr = $"{_localizer["Choose"].Value} {itemSubCategoryNameAr}",
				ItemSubCategoryNameSelectEn = $"{_localizer["Choose"].Value} {itemSubCategoryNameEn}",
				ItemSubCategoryNameSelect = ComposeSelect(language, itemSubCategoryNameAr, itemSubCategoryNameEn, _localizer["Choose"].Value),
				ItemSubCategoryNameFirstAr = $"{_localizer["Choose"].Value} {itemSubCategoryNameAr} {_localizer["First"].Value}",
				ItemSubCategoryNameFirstEn = $"{_localizer["Choose"].Value} {itemSubCategoryNameEn} {_localizer["First"].Value}",
				ItemSubCategoryNameFirst = ComposeFirst(language, itemSubCategoryNameAr, itemSubCategoryNameEn, _localizer["Choose"].Value, _localizer["First"].Value),
				ItemSectionNameAr = itemSectionNameAr,
				ItemSectionNameEn = itemSectionNameEn,
				ItemSectionName = language == LanguageCode.Arabic ? itemSectionNameAr : itemSectionNameEn,
				ItemSectionNameSelectAr = $"{_localizer["Choose"].Value} {itemSectionNameAr}",
				ItemSectionNameSelectEn = $"{_localizer["Choose"].Value} {itemSectionNameEn}",
				ItemSectionNameSelect = ComposeSelect(language, itemSectionNameAr, itemSectionNameEn, _localizer["Choose"].Value),
				ItemSectionNameFirstAr = $"{_localizer["Choose"].Value} {itemSectionNameAr} {_localizer["First"].Value}",
				ItemSectionNameFirstEn = $"{_localizer["Choose"].Value} {itemSectionNameEn} {_localizer["First"].Value}",
				ItemSectionNameFirst = ComposeFirst(language, itemSectionNameAr, itemSectionNameEn, _localizer["Choose"].Value, _localizer["First"].Value),
				ItemSubSectionNameAr = itemSubSectionNameAr,
				ItemSubSectionNameEn = itemSubSectionNameEn,
				ItemSubSectionName = language == LanguageCode.Arabic ? itemSubSectionNameAr : itemSubSectionNameEn,
				ItemSubSectionNameSelectAr = $"{_localizer["Choose"].Value} {itemSubSectionNameAr}",
				ItemSubSectionNameSelectEn = $"{_localizer["Choose"].Value} {itemSubSectionNameEn}",
				ItemSubSectionNameSelect = ComposeSelect(language, itemSubSectionNameAr, itemSubSectionNameEn, _localizer["Choose"].Value),
				ItemSubSectionNameFirstAr = $"{_localizer["Choose"].Value} {itemSubSectionNameAr} {_localizer["First"].Value}",
				ItemSubSectionNameFirstEn = $"{_localizer["Choose"].Value} {itemSubSectionNameEn} {_localizer["First"].Value}",
				ItemSubSectionNameFirst = ComposeFirst(language, itemSubSectionNameAr, itemSubSectionNameEn, _localizer["Choose"].Value, _localizer["First"].Value),
				MainItemNameAr = mainItemNameAr,
				MainItemNameEn = mainItemNameEn,
				MainItemName = language == LanguageCode.Arabic ? mainItemNameAr : mainItemNameEn,
				MainItemNameSelectAr = $"{_localizer["Choose"].Value} {mainItemNameAr}",
				MainItemNameSelectEn = $"{_localizer["Choose"].Value} {mainItemNameEn}",
				MainItemNameSelect = ComposeSelect(language, mainItemNameAr, mainItemNameEn, _localizer["Choose"].Value),
			};
		}
	}

	public static class ItemDivisionName
	{
		public static string ComposeSelect(string? language,string? nameAr, string? nameEn,string? choose)
		{
			return language == LanguageCode.Arabic ? $"{choose} {nameAr}" : $"{choose} {nameEn}";
		}
		public static string ComposeFirst(string? language, string? nameAr, string? nameEn, string? choose, string? first)
		{
			return language == LanguageCode.Arabic ? $"{choose} {nameAr} {first}" : $"{choose} {nameEn} {first}";
		}
	}

}
