using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Identity;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Logic;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Shared.Service.Services.Items
{
    public class ItemTaxService: BaseService<ItemTax>, IItemTaxService
    {
        private readonly ITaxService _taxService;
        private readonly ITaxPercentService _taxPercentService;
        private readonly IAccountService _accountService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ItemTaxService(IRepository<ItemTax> repository, ITaxService taxService, ITaxPercentService taxPercentService, IAccountService accountService, IHttpContextAccessor httpContextAccessor) : base(repository) 
        {
            _taxService = taxService;
            _taxPercentService = taxPercentService;
            _accountService = accountService;
            _httpContextAccessor = httpContextAccessor;
        }
        
        public async Task<List<ItemTaxDataDto>> GetItemTaxData(DateTime? currentDate = null)
        {
            currentDate ??= DateHelper.GetDateTimeNow();

            var itemTaxData = await (
                    from itemTax in _repository.GetAll()
                    from tax in _taxService.GetAll().Where(x => x.TaxId == itemTax.TaxId && !x.IsVatTax)
                    from drAccount in _accountService.GetAll().Where(x => x.AccountId == tax.DrAccount).DefaultIfEmpty()
                    from crAccount in _accountService.GetAll().Where(x => x.AccountId == tax.CrAccount).DefaultIfEmpty()
                    from taxPercent in _taxPercentService.GetAllCurrentTaxPercents((DateTime)currentDate).Where(x => x.TaxId == itemTax.TaxId)
                    select new ItemTaxDataDto
                    {
                        ItemId = itemTax.ItemId,
                        TaxId = itemTax.TaxId,
                        CreditAccountId = crAccount != null ? crAccount.AccountId : null,
                        DebitAccountId = drAccount != null ? drAccount.AccountId : null,
                        TaxAfterVatInclusive = tax.TaxAfterVatInclusive,
                        TaxPercent = taxPercent.Percent,
                    }
            ).ToListAsync();

            return itemTaxData;
        }

        public async Task<List<ItemTaxDataDto>> GetItemTaxDataByItemId(int itemId, DateTime? currentDate = null)
        {
            currentDate ??= DateHelper.GetDateTimeNow();

            var itemTaxData = await (
                    from itemTax in _repository.GetAll().Where(x => x.ItemId == itemId)
                    from tax in _taxService.GetAll().Where(x => x.TaxId == itemTax.TaxId && !x.IsVatTax)
                    from drAccount in _accountService.GetAll().Where(x => x.AccountId == tax.DrAccount).DefaultIfEmpty()
                    from crAccount in _accountService.GetAll().Where(x => x.AccountId == tax.CrAccount).DefaultIfEmpty()
                    from taxPercent in _taxPercentService.GetAllCurrentTaxPercents((DateTime)currentDate).Where(x => x.TaxId == itemTax.TaxId)
                    select new ItemTaxDataDto
                    {
                        ItemId = itemTax.ItemId,
                        TaxId = itemTax.TaxId,
                        TaxTypeId = tax.TaxTypeId,
                        CreditAccountId = crAccount != null ? crAccount.AccountId : null,
                        DebitAccountId = drAccount != null ? drAccount.AccountId : null,
                        TaxAfterVatInclusive = tax.TaxAfterVatInclusive,
                        TaxPercent = taxPercent.Percent,
                    }
            ).ToListAsync();

            return itemTaxData;
        }

        public async Task<List<ItemTaxDataDto>> GetItemTaxDataByItemIds(List<int> itemIds, DateTime? currentDate = null)
        {
            currentDate ??= DateHelper.GetDateTimeNow();

            var itemTaxData = await (
                    from itemTax in _repository.GetAll().Where(x => itemIds.Contains(x.ItemId))
                    from tax in _taxService.GetAll().Where(x => x.TaxId == itemTax.TaxId && !x.IsVatTax)
                    from drAccount in _accountService.GetAll().Where(x => x.AccountId == tax.DrAccount).DefaultIfEmpty()
                    from crAccount in _accountService.GetAll().Where(x => x.AccountId == tax.CrAccount).DefaultIfEmpty()
                    from taxPercent in _taxPercentService.GetAllCurrentTaxPercents((DateTime)currentDate).Where(x => x.TaxId == itemTax.TaxId)
                    select new ItemTaxDataDto
                    {
                        ItemId = itemTax.ItemId,
                        TaxId = itemTax.TaxId,
                        CreditAccountId = crAccount != null ? crAccount.AccountId : null,
                        DebitAccountId = drAccount != null ? drAccount.AccountId : null,
                        TaxAfterVatInclusive = tax.TaxAfterVatInclusive,
                        TaxPercent = taxPercent.Percent
                    }
            ).ToListAsync();

            return itemTaxData;
        }

        public async Task<bool> SaveItemTaxes(List<ItemTaxDto> itemTaxes, int itemId)
        {
            await DeleteItemTaxes(itemTaxes, itemId);
            if (itemTaxes.Any())
            {
                await AddItemTaxes(itemTaxes, itemId);
                await UpdateItemTaxes(itemTaxes, itemId);
            }
            return true;
        }

        private async Task<bool> DeleteItemTaxes(List<ItemTaxDto> itemTaxes, int itemId)
        {
            var current = await _repository.GetAll().Where(x => x.ItemId == itemId).AsNoTracking().ToListAsync();
            var toBeDeleted = current.Where(p => itemTaxes.All(p2 => p2.ItemTaxId != p.ItemTaxId));
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddItemTaxes(List<ItemTaxDto> itemTaxes, int itemId)
        {
            var current = itemTaxes.Where(x => x.ItemTaxId <= 0).ToList();
            var newId = await GetNextId();

            var modelList = new List<ItemTax>();

            foreach(var itemTax in current)
            {
                var model = new ItemTax
                {
                    ItemTaxId = newId,
                    ItemId = itemId,
                    TaxId = itemTax.TaxId,

                    Hide = false,
                    CreatedAt = DateHelper.GetDateTimeNow(),
                    UserNameCreated = await _httpContextAccessor!.GetUserName(),
                    IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                };

                modelList.Add(model);
                newId++;
            }
            
            if (modelList.Any())
            {
                await _repository.InsertRange(modelList);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> UpdateItemTaxes(List<ItemTaxDto> itemTaxes, int itemId)
        {
            var current = itemTaxes.Where(x => x.ItemTaxId > 0).ToList();
            var modelList = new List<ItemTax>();

            foreach (var itemTax in current)
            {
                var model = new ItemTax
                {
                    ItemTaxId = itemTax.ItemTaxId,
                    ItemId = itemId,
                    TaxId = itemTax.TaxId,

                    ModifiedAt = DateHelper.GetDateTimeNow(),
                    UserNameModified = await _httpContextAccessor!.GetUserName(),
                    IpAddressModified = _httpContextAccessor?.GetIpAddress(),
                    Hide = false
                };

                modelList.Add(model);
            }

            if (modelList.Any())
            {
                _repository.UpdateRange(modelList);
                await _repository.SaveChanges();
                return true;
            }

            return false;
        }

        public async Task<bool> DeleteItemTaxesByItemId(int itemId)
        {
            var data = await _repository.GetAll().Where(x => x.ItemId == itemId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.ItemTaxId) + 1; } catch { id = 1; }
            return id;
        }
    }
}
