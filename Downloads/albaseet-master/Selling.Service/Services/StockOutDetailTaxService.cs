using Microsoft.AspNetCore.Http;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.Service;
using Microsoft.EntityFrameworkCore;
using Shared.Helper.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Logic;

namespace Sales.Service.Services
{
    public class StockOutDetailTaxService: BaseService<StockOutDetailTax>, IStockOutDetailTaxService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IStockOutDetailService _stockOutDetailService;

        public StockOutDetailTaxService(IRepository<StockOutDetailTax> repository, IHttpContextAccessor httpContextAccessor, IStockOutDetailService stockOutDetailService) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _stockOutDetailService = stockOutDetailService;
        }

        public IQueryable<StockOutDetailTaxDto> GetStockOutDetailTaxes(int stockOutHeaderId)
        {
            var data =
                    (
                    from stockOutDetail in _stockOutDetailService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeaderId)
                    from stockOutDetailTax in _repository.GetAll().Where(x => x.StockOutDetailId == stockOutDetail.StockOutDetailId)
                    select new StockOutDetailTaxDto
                    {
                        StockOutDetailTaxId = stockOutDetailTax.StockOutDetailTaxId,
                        StockOutDetailId = stockOutDetailTax.StockOutDetailId,
                        TaxId = stockOutDetailTax.TaxId,
                        CreditAccountId = stockOutDetailTax.CreditAccountId,
                        TaxAfterVatInclusive = stockOutDetailTax.TaxAfterVatInclusive,
                        TaxPercent = stockOutDetailTax.TaxPercent,
                        TaxValue = stockOutDetailTax.TaxValue
                    }
                );

            return data;
        }

        public async Task<bool> SaveStockOutDetailTaxes(int stockOutHeaderId, List<StockOutDetailTaxDto> stockOutDetailTaxes)
        {
            await DeleteStockOutDetailTax(stockOutDetailTaxes, stockOutHeaderId);
            if (stockOutDetailTaxes.Any())
            {
                await AddStockOutDetailTax(stockOutDetailTaxes);
                await EditStockOutDetailTax(stockOutDetailTaxes);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteStockOutDetailTaxes(int stockOutHeaderId)
        {
            var data = await (
                       from stockOutDetail in _stockOutDetailService.GetAll().Where(x => x.StockOutHeaderId == stockOutHeaderId)
                       from stockOutDetailTax in _repository.GetAll().Where(x => x.StockOutDetailId == stockOutDetail.StockOutDetailId)
                       select stockOutDetailTax
                      ).ToListAsync();

            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteStockOutDetailTax(List<StockOutDetailTaxDto> details, int headerId)
        {
            var current = await (
                from stockOutDetail in _stockOutDetailService.GetAll().Where(x => x.StockOutHeaderId == headerId)
                from stockOutDetailTax in _repository.GetAll().Where(x => x.StockOutDetailId == stockOutDetail.StockOutDetailId)
                select stockOutDetailTax
                ).AsNoTracking().ToListAsync();


            var toBeDeleted = current.Where(p => details.All(p2 => p2.StockOutDetailTaxId != p.StockOutDetailTaxId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddStockOutDetailTax(List<StockOutDetailTaxDto> details)
        {
            var current = details.Where(x => x.StockOutDetailTaxId <= 0).ToList();
            var modelList = new List<StockOutDetailTax>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new StockOutDetailTax()
                {
                    StockOutDetailTaxId = newId,
                    StockOutDetailId = detail.StockOutDetailId,
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

        private async Task<bool> EditStockOutDetailTax(List<StockOutDetailTaxDto> stockOutDetailTaxes)
        {
            var current = stockOutDetailTaxes.Where(x => x.StockOutDetailTaxId > 0).ToList();
            var modelList = new List<StockOutDetailTax>();
            foreach (var detail in current)
            {
                var model = new StockOutDetailTax()
                {
                    StockOutDetailTaxId = detail.StockOutDetailTaxId,
                    StockOutDetailId = detail.StockOutDetailId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.StockOutDetailTaxId) + 1; } catch { id = 1; }
            return id;
        }
    }
}
