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
	public class StockInReturnDetailTaxService : BaseService<StockInReturnDetailTax>, IStockInReturnDetailTaxService
	{
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStockInReturnDetailService _stockInReturnDetailService;

		public StockInReturnDetailTaxService(IRepository<StockInReturnDetailTax> repository,IHttpContextAccessor httpContextAccessor, IStockInReturnDetailService stockInReturnDetailService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
            _stockInReturnDetailService = stockInReturnDetailService;
        }

		public IQueryable<StockInReturnDetailTaxDto> GetStockInReturnDetailTaxes(int stockInReturnHeaderId)
		{
			var data =
			        (
                    from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeaderId)
                    from stockInReturnDetailTax in _repository.GetAll().Where(x => x.StockInReturnDetailId == stockInReturnDetail.StockInReturnDetailId)
                    select new StockInReturnDetailTaxDto
				    {
                         StockInReturnDetailTaxId = stockInReturnDetailTax.StockInReturnDetailTaxId,
                         StockInReturnDetailId = stockInReturnDetailTax.StockInReturnDetailId,
                         TaxId = stockInReturnDetailTax.TaxId,
                         CreditAccountId = stockInReturnDetailTax.CreditAccountId,
                         TaxAfterVatInclusive = stockInReturnDetailTax.TaxAfterVatInclusive,
                         TaxPercent = stockInReturnDetailTax.TaxPercent,
                         TaxValue = stockInReturnDetailTax.TaxValue
				    }
                );

			return data;
		}

        public async Task<bool> SaveStockInReturnDetailTaxes(int stockInReturnHeaderId, List<StockInReturnDetailTaxDto> stockInReturnDetailTaxes)
        {
            await DeleteStockInReturnDetailTax(stockInReturnDetailTaxes, stockInReturnHeaderId);
            if (stockInReturnDetailTaxes.Any())
            {
                await AddStockInReturnDetailTax(stockInReturnDetailTaxes);
                await EditStockInReturnDetailTax(stockInReturnDetailTaxes);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteStockInReturnDetailTaxes(int stockInReturnHeaderId)
        {
            var data = await (
                       from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == stockInReturnHeaderId)
                       from stockInReturnDetailTax in _repository.GetAll().Where(x => x.StockInReturnDetailId == stockInReturnDetail.StockInReturnDetailId)
                       select stockInReturnDetailTax
                      ).ToListAsync();

            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteStockInReturnDetailTax(List<StockInReturnDetailTaxDto> details, int headerId)
        {
            var current = await (
                from stockInReturnDetail in _stockInReturnDetailService.GetAll().Where(x => x.StockInReturnHeaderId == headerId)
                from stockInReturnDetailTax in _repository.GetAll().Where(x => x.StockInReturnDetailId == stockInReturnDetail.StockInReturnDetailId)
                select stockInReturnDetailTax
                ).AsNoTracking().ToListAsync();


            var toBeDeleted = current.Where(p => details.All(p2 => p2.StockInReturnDetailTaxId != p.StockInReturnDetailTaxId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddStockInReturnDetailTax(List<StockInReturnDetailTaxDto> details)
        {
            var current = details.Where(x => x.StockInReturnDetailTaxId <= 0).ToList();
            var modelList = new List<StockInReturnDetailTax>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new StockInReturnDetailTax()
                {
                    StockInReturnDetailTaxId = newId,
                    StockInReturnDetailId = detail.StockInReturnDetailId,
                    TaxId = detail.TaxId,
                    CreditAccountId = detail.CreditAccountId,
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

        private async Task<bool> EditStockInReturnDetailTax(List<StockInReturnDetailTaxDto> stockInReturnDetailTaxes)
        {            
            var current = stockInReturnDetailTaxes.Where(x => x.StockInReturnDetailTaxId > 0).ToList();
            var modelList = new List<StockInReturnDetailTax>();
            foreach (var detail in current)
            {
                var model = new StockInReturnDetailTax()
                {
                    StockInReturnDetailTaxId = detail.StockInReturnDetailTaxId,
                    StockInReturnDetailId = detail.StockInReturnDetailId,
                    TaxId = detail.TaxId,
                    CreditAccountId = detail.CreditAccountId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.StockInReturnDetailTaxId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
