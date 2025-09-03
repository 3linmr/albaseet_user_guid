using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.StaticData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Compound.CoreOne.Contracts.Reports.Inventory;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Menus;

namespace Compound.Service.Services.Reports.Inventory
{
	public class ItemReportDataService : IItemReportDataService
	{
		private readonly ITaxService _taxService;
		private readonly IItemAttributeService _itemAttributeService;
		private readonly IItemTaxService _itemTaxService;
		private readonly IItemService _itemService;
		private readonly IMenuNoteService _menuNoteService;
		private readonly IMenuNoteIdentifierService _menuNoteIdentifierService;
		private readonly IStoreService _storeService;
		private readonly IBranchService _branchService;
		private readonly ICompanyService _companyService;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public ItemReportDataService(ITaxService taxService, IItemAttributeService itemAttributeService, IItemTaxService itemTaxService, IItemService itemService, IMenuNoteService menuNoteService, IMenuNoteIdentifierService menuNoteIdentifierService, IStoreService storeService, IBranchService branchService, ICompanyService companyService, IHttpContextAccessor httpContextAccessor)
		{
			_taxService = taxService;
			_itemAttributeService = itemAttributeService;
			_itemTaxService = itemTaxService;
			_itemService = itemService;
			_menuNoteService = menuNoteService;
			_menuNoteIdentifierService = menuNoteIdentifierService;
			_storeService = storeService;
			_branchService = branchService;
			_companyService = companyService;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<Dictionary<int, string>> GetItemMenuNotes(List<int> companyIds)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var result = from menuNote in _menuNoteService.GetAll().Where(x => x.MenuCode == MenuCodeData.Item && x.ShowInReports == true)
			             from menuNoteIdentifier in _menuNoteIdentifierService.GetAll().Where(x => x.MenuNoteIdentifierId == menuNote.MenuNoteIdentifierId)
						 from item in _itemService.GetAll().Where(x => x.ItemId == menuNote.ReferenceId && companyIds.Contains(x.CompanyId))
						 group new { menuNote, menuNoteIdentifier } by menuNote.ReferenceId into g
						 select new
						 {
							 ItemId = g.Key,
							 ItemNote = string.Join(", ", g.Select(x => (language == LanguageCode.Arabic ? x.menuNoteIdentifier.MenuNoteIdentifierNameAr : x.menuNoteIdentifier.MenuNoteIdentifierNameEn) + " (" + x.menuNote.NoteValue + ")"))
						 };

			return await result.AsNoTracking().ToDictionaryAsync(x => x.ItemId, x => x.ItemNote);
		}


		public async Task<Dictionary<int, string>> GetItemMenuNotesByStoreIds(List<int> storeIds)
		{
			var companyIds = await _storeService.GetAllStores().Where(x => storeIds.Contains(x.StoreId)).Select(x => x.CompanyId).ToListAsync();
			return await GetItemMenuNotes(companyIds);
		}

		public async Task<string> GetItemMenuNotesByItemId(int itemId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var result = from menuNote in _menuNoteService.GetAll().Where(x => x.MenuCode == MenuCodeData.Item && x.ReferenceId == itemId && x.ShowInReports == true)
			             from menuNoteIdentifier in _menuNoteIdentifierService.GetAll().Where(x => x.MenuNoteIdentifierId == menuNote.MenuNoteIdentifierId)
						 from item in _itemService.GetAll().Where(x => x.ItemId == menuNote.ReferenceId)
						 group new { menuNote, menuNoteIdentifier } by menuNote.ReferenceId into g
						 select string.Join(", ", g.Select(x => (language == LanguageCode.Arabic ? x.menuNoteIdentifier.MenuNoteIdentifierNameAr : x.menuNoteIdentifier.MenuNoteIdentifierNameEn) + " (" + x.menuNote.NoteValue + ")"));

			return await result.AsNoTracking().FirstOrDefaultAsync() ?? string.Empty;
		}

		public async Task<Dictionary<int, string>> GetItemAttributes(List<int> companyIds)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var result = from attribute in _itemAttributeService.GetAllItemAttributes()
						 from item in _itemService.GetAll().Where(x => x.ItemId == attribute.ItemId && companyIds.Contains(x.CompanyId))
						 group attribute by attribute.ItemId into g
						 select new
						 {
							 ItemId = g.Key,
							 ItemNote = string.Join(", ", g.Select(x => x.ItemAttributeTypeName + " (" +  (language == LanguageCode.Arabic ? x.ItemAttributeNameAr : x.ItemAttributeNameEn) + ")"))
						 };

			return await result.AsNoTracking().ToDictionaryAsync(x => x.ItemId, x => x.ItemNote);
		}

		public async Task<Dictionary<int, string>> GetItemAttributesByStoreIds(List<int> storeIds)
		{
			var companyIds = await _storeService.GetAllStores().Where(x => storeIds.Contains(x.StoreId)).Select(x => x.CompanyId).ToListAsync();
			return await GetItemAttributes(companyIds);
		}

		public async Task<string> GetItemAttributesByItemId(int itemId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var result = from attribute in _itemAttributeService.GetAllItemAttributes()
						 from item in _itemService.GetAll().Where(x => x.ItemId == attribute.ItemId && x.ItemId == itemId)
						 group attribute by attribute.ItemId into g
						 select string.Join(", ", g.Select(x => x.ItemAttributeTypeName + " (" +  (language == LanguageCode.Arabic ? x.ItemAttributeNameAr : x.ItemAttributeNameEn) + ")"));

			return await result.AsNoTracking().FirstOrDefaultAsync() ?? string.Empty;
		}

		public async Task<Dictionary<int, string>> GetItemOtherTaxes(List<int> companyIds)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var result = from itemTax in _itemTaxService.GetAll()
						 from item in _itemService.GetAll().Where(x => x.ItemId == itemTax.ItemId && companyIds.Contains(x.CompanyId))
						 from tax in _taxService.GetAll().Where(x => x.TaxId == itemTax.TaxId)
						 group tax by itemTax.ItemId into g
						 select new
						 {
							 ItemId = g.Key,
							 ItemTaxes = string.Join(", ", g.Select(x => language == LanguageCode.Arabic ? x.TaxNameAr : x.TaxNameEn))
						 };

			return await result.AsNoTracking().ToDictionaryAsync(x => x.ItemId, x => x.ItemTaxes);
		}

		public async Task<Dictionary<int, string>> GetItemOtherTaxesByStoreId(List<int> storeIds)
		{
			var companyIds = await _storeService.GetAllStores().Where(x => storeIds.Contains(x.StoreId)).Select(x => x.CompanyId).ToListAsync();
			return await GetItemOtherTaxes(companyIds);
		}

		public async Task<string> GetItemOtherTaxesByItemId(int itemId)
		{
			var language = _httpContextAccessor.GetProgramCurrentLanguage();

			var result = from itemTax in _itemTaxService.GetAll()
						 from item in _itemService.GetAll().Where(x => x.ItemId == itemTax.ItemId && x.ItemId == itemId)
						 from tax in _taxService.GetAll().Where(x => x.TaxId == itemTax.TaxId)
						 group tax by itemTax.ItemId into g
						 select string.Join(", ", g.Select(x => language == LanguageCode.Arabic ? x.TaxNameAr : x.TaxNameEn)) ;

			return await result.AsNoTracking().FirstOrDefaultAsync() ?? string.Empty;
		}
	}
}
