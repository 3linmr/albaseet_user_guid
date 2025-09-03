using Purchases.CoreOne.Contracts;
using Purchases.CoreOne.Models.Domain;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.Helper.Identity;
using Shared.Service;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.Helper.Logic;
using Shared.Service.Logic.Calculation;

namespace Purchases.Service.Services
{
	public class StockInDetailTaxService : BaseService<StockInDetailTax>, IStockInDetailTaxService
	{
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStockInDetailService _stockInDetailService;

		public StockInDetailTaxService(IRepository<StockInDetailTax> repository,IHttpContextAccessor httpContextAccessor, IStockInDetailService stockInDetailService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
            _stockInDetailService = stockInDetailService;
        }

		public IQueryable<StockInDetailTaxDto> GetStockInDetailTaxes(int stockInHeaderId)
		{
			var data =
			        (
                    from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId == stockInHeaderId)
                    from stockInDetailTax in _repository.GetAll().Where(x => x.StockInDetailId == stockInDetail.StockInDetailId)
                    select new StockInDetailTaxDto
				    {
                         StockInDetailTaxId = stockInDetailTax.StockInDetailTaxId,
                         StockInDetailId = stockInDetailTax.StockInDetailId,
                         TaxId = stockInDetailTax.TaxId,
                         DebitAccountId = stockInDetailTax.DebitAccountId,
                         TaxAfterVatInclusive = stockInDetailTax.TaxAfterVatInclusive,
                         TaxPercent = stockInDetailTax.TaxPercent,
                         TaxValue = stockInDetailTax.TaxValue
				    }
                );

			return data;
		}

        public async Task<bool> SaveStockInDetailTaxes(int stockInHeaderId, List<StockInDetailTaxDto> stockInDetailTaxes)
        {
            await DeleteStockInDetailTax(stockInDetailTaxes, stockInHeaderId);
            if (stockInDetailTaxes.Any())
            {
                await AddStockInDetailTax(stockInDetailTaxes);
                await EditStockInDetailTax(stockInDetailTaxes);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteStockInDetailTaxes(int stockInHeaderId)
        {
            var data = await (
                       from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId == stockInHeaderId)
                       from stockInDetailTax in _repository.GetAll().Where(x => x.StockInDetailId == stockInDetail.StockInDetailId)
                       select stockInDetailTax
                      ).ToListAsync();

            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteStockInDetailTax(List<StockInDetailTaxDto> details, int headerId)
        {
            var current = await (
                from stockInDetail in _stockInDetailService.GetAll().Where(x => x.StockInHeaderId == headerId)
                from stockInDetailTax in _repository.GetAll().Where(x => x.StockInDetailId == stockInDetail.StockInDetailId)
                select stockInDetailTax
                ).AsNoTracking().ToListAsync();


            var toBeDeleted = current.Where(p => details.All(p2 => p2.StockInDetailTaxId != p.StockInDetailTaxId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddStockInDetailTax(List<StockInDetailTaxDto> details)
        {
            var current = details.Where(x => x.StockInDetailTaxId <= 0).ToList();
            var modelList = new List<StockInDetailTax>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new StockInDetailTax()
                {
                    StockInDetailTaxId = newId,
                    StockInDetailId = detail.StockInDetailId,
                    TaxId = detail.TaxId,
                    DebitAccountId = detail.DebitAccountId,
                    TaxAfterVatInclusive = detail.TaxAfterVatInclusive,
                    TaxPercent = detail.TaxPercent,
                    TaxValue = detail.TaxValue,


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

        private async Task<bool> EditStockInDetailTax(List<StockInDetailTaxDto> stockInDetailTaxes)
        {            
            var current = stockInDetailTaxes.Where(x => x.StockInDetailTaxId > 0).ToList();
            var modelList = new List<StockInDetailTax>();
            foreach (var detail in current)
            {
                var model = new StockInDetailTax()
                {
                    StockInDetailTaxId = detail.StockInDetailTaxId,
                    StockInDetailId = detail.StockInDetailId,
                    TaxId = detail.TaxId,
                    DebitAccountId = detail.DebitAccountId,
                    TaxAfterVatInclusive = detail.TaxAfterVatInclusive,
                    TaxPercent = detail.TaxPercent,
                    TaxValue = detail.TaxValue,

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

        private async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.StockInDetailTaxId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
