using Microsoft.AspNetCore.Http;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shared.Helper.Identity;
using Shared.Helper.Logic;

namespace Sales.Service.Services
{
    public class StockOutReturnDetailTaxService : BaseService<StockOutReturnDetailTax>, IStockOutReturnDetailTaxService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStockOutReturnDetailService _stockOutReturnDetailService;

        public StockOutReturnDetailTaxService(IRepository<StockOutReturnDetailTax> repository, IHttpContextAccessor httpContextAccessor, IStockOutReturnDetailService stockOutReturnDetailService) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _stockOutReturnDetailService = stockOutReturnDetailService;
        }

        public IQueryable<StockOutReturnDetailTaxDto> GetStockOutReturnDetailTaxes(int stockOutReturnHeaderId)
        {
            var data =
                    (
                    from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeaderId)
                    from stockOutReturnDetailTax in _repository.GetAll().Where(x => x.StockOutReturnDetailId == stockOutReturnDetail.StockOutReturnDetailId)
                    select new StockOutReturnDetailTaxDto
                    {
                        StockOutReturnDetailTaxId = stockOutReturnDetailTax.StockOutReturnDetailTaxId,
                        StockOutReturnDetailId = stockOutReturnDetailTax.StockOutReturnDetailId,
                        TaxId = stockOutReturnDetailTax.TaxId,
                        DebitAccountId = stockOutReturnDetailTax.DebitAccountId,
                        TaxAfterVatInclusive = stockOutReturnDetailTax.TaxAfterVatInclusive,
                        TaxPercent = stockOutReturnDetailTax.TaxPercent,
                        TaxValue = stockOutReturnDetailTax.TaxValue
                    }
                );

            return data;
        }

        public async Task<bool> SaveStockOutReturnDetailTaxes(int stockOutReturnHeaderId, List<StockOutReturnDetailTaxDto> stockOutReturnDetailTaxes)
        {
            await DeleteStockOutReturnDetailTax(stockOutReturnDetailTaxes, stockOutReturnHeaderId);
            if (stockOutReturnDetailTaxes.Any())
            {
                await AddStockOutReturnDetailTax(stockOutReturnDetailTaxes);
                await EditStockOutReturnDetailTax(stockOutReturnDetailTaxes);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteStockOutReturnDetailTaxes(int stockOutReturnHeaderId)
        {
            var data = await (
                       from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == stockOutReturnHeaderId)
                       from stockOutReturnDetailTax in _repository.GetAll().Where(x => x.StockOutReturnDetailId == stockOutReturnDetail.StockOutReturnDetailId)
                       select stockOutReturnDetailTax
                      ).ToListAsync();

            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteStockOutReturnDetailTax(List<StockOutReturnDetailTaxDto> details, int headerId)
        {
            var current = await (
                from stockOutReturnDetail in _stockOutReturnDetailService.GetAll().Where(x => x.StockOutReturnHeaderId == headerId)
                from stockOutReturnDetailTax in _repository.GetAll().Where(x => x.StockOutReturnDetailId == stockOutReturnDetail.StockOutReturnDetailId)
                select stockOutReturnDetailTax
                ).AsNoTracking().ToListAsync();


            var toBeDeleted = current.Where(p => details.All(p2 => p2.StockOutReturnDetailTaxId != p.StockOutReturnDetailTaxId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddStockOutReturnDetailTax(List<StockOutReturnDetailTaxDto> details)
        {
            var current = details.Where(x => x.StockOutReturnDetailTaxId <= 0).ToList();
            var modelList = new List<StockOutReturnDetailTax>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new StockOutReturnDetailTax()
                {
                    StockOutReturnDetailTaxId = newId,
                    StockOutReturnDetailId = detail.StockOutReturnDetailId,
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

        private async Task<bool> EditStockOutReturnDetailTax(List<StockOutReturnDetailTaxDto> stockOutReturnDetailTaxes)
        {
            var current = stockOutReturnDetailTaxes.Where(x => x.StockOutReturnDetailTaxId > 0).ToList();
            var modelList = new List<StockOutReturnDetailTax>();
            foreach (var detail in current)
            {
                var model = new StockOutReturnDetailTax()
                {
                    StockOutReturnDetailTaxId = detail.StockOutReturnDetailTaxId,
                    StockOutReturnDetailId = detail.StockOutReturnDetailId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.StockOutReturnDetailTaxId) + 1; } catch { id = 1; }
            return id;
        }
    }
}
