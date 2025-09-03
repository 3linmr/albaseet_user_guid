using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.Service;
using Shared.Helper.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Logic;

namespace Sales.Service.Services
{
    public class SalesInvoiceDetailTaxService: BaseService<SalesInvoiceDetailTax>, ISalesInvoiceDetailTaxService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISalesInvoiceDetailService _salesInvoiceDetailService;

        public SalesInvoiceDetailTaxService(IRepository<SalesInvoiceDetailTax> repository, IHttpContextAccessor httpContextAccessor, ISalesInvoiceDetailService salesInvoiceDetailService) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _salesInvoiceDetailService = salesInvoiceDetailService;
        }

        public IQueryable<SalesInvoiceDetailTaxDto> GetSalesInvoiceDetailTaxes(int salesInvoiceHeaderId)
        {
            var data =
                    (
                    from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId)
                    from salesInvoiceDetailTax in _repository.GetAll().Where(x => x.SalesInvoiceDetailId == salesInvoiceDetail.SalesInvoiceDetailId)
                    select new SalesInvoiceDetailTaxDto
                    {
                        SalesInvoiceDetailTaxId = salesInvoiceDetailTax.SalesInvoiceDetailTaxId,
                        SalesInvoiceDetailId = salesInvoiceDetailTax.SalesInvoiceDetailId,
                        TaxId = salesInvoiceDetailTax.TaxId,
                        TaxTypeId = salesInvoiceDetailTax.TaxTypeId,
                        CreditAccountId = salesInvoiceDetailTax.CreditAccountId,
                        TaxAfterVatInclusive = salesInvoiceDetailTax.TaxAfterVatInclusive,
                        TaxPercent = salesInvoiceDetailTax.TaxPercent,
                        TaxValue = salesInvoiceDetailTax.TaxValue
                    }
                );

            return data;
        }

        public async Task<bool> SaveSalesInvoiceDetailTaxes(int salesInvoiceHeaderId, List<SalesInvoiceDetailTaxDto> salesInvoiceDetailTaxes)
        {
            await DeleteSalesInvoiceDetailTax(salesInvoiceDetailTaxes, salesInvoiceHeaderId);
            if (salesInvoiceDetailTaxes.Any())
            {
                await AddSalesInvoiceDetailTax(salesInvoiceDetailTaxes);
                await EditSalesInvoiceDetailTax(salesInvoiceDetailTaxes);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteSalesInvoiceDetailTaxes(int salesInvoiceHeaderId)
        {
            var data = await (
                       from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId)
                       from salesInvoiceDetailTax in _repository.GetAll().Where(x => x.SalesInvoiceDetailId == salesInvoiceDetail.SalesInvoiceDetailId)
                       select salesInvoiceDetailTax
                      ).ToListAsync();

            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteSalesInvoiceDetailTax(List<SalesInvoiceDetailTaxDto> details, int headerId)
        {
            var current = await (
                from salesInvoiceDetail in _salesInvoiceDetailService.GetAll().Where(x => x.SalesInvoiceHeaderId == headerId)
                from salesInvoiceDetailTax in _repository.GetAll().Where(x => x.SalesInvoiceDetailId == salesInvoiceDetail.SalesInvoiceDetailId)
                select salesInvoiceDetailTax
                ).AsNoTracking().ToListAsync();


            var toBeDeleted = current.Where(p => details.All(p2 => p2.SalesInvoiceDetailTaxId != p.SalesInvoiceDetailTaxId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddSalesInvoiceDetailTax(List<SalesInvoiceDetailTaxDto> details)
        {
            var current = details.Where(x => x.SalesInvoiceDetailTaxId <= 0).ToList();
            var modelList = new List<SalesInvoiceDetailTax>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new SalesInvoiceDetailTax()
                {
                    SalesInvoiceDetailTaxId = newId,
                    SalesInvoiceDetailId = detail.SalesInvoiceDetailId,
                    TaxId = detail.TaxId,
                    TaxTypeId = detail.TaxTypeId,
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

        private async Task<bool> EditSalesInvoiceDetailTax(List<SalesInvoiceDetailTaxDto> salesInvoiceDetailTaxes)
        {
            var current = salesInvoiceDetailTaxes.Where(x => x.SalesInvoiceDetailTaxId > 0).ToList();
            var modelList = new List<SalesInvoiceDetailTax>();
            foreach (var detail in current)
            {
                var model = new SalesInvoiceDetailTax()
                {
                    SalesInvoiceDetailTaxId = detail.SalesInvoiceDetailTaxId,
                    SalesInvoiceDetailId = detail.SalesInvoiceDetailId,
                    TaxId = detail.TaxId,
                    TaxTypeId = detail.TaxTypeId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.SalesInvoiceDetailTaxId) + 1; } catch { id = 1; }
            return id;
        }
    }
}
