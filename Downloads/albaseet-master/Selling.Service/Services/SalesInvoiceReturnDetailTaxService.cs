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
    public class SalesInvoiceReturnDetailTaxService: BaseService<SalesInvoiceReturnDetailTax>, ISalesInvoiceReturnDetailTaxService
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISalesInvoiceReturnDetailService _salesInvoiceReturnDetailService;

        public SalesInvoiceReturnDetailTaxService(IRepository<SalesInvoiceReturnDetailTax> repository, IHttpContextAccessor httpContextAccessor, ISalesInvoiceReturnDetailService salesInvoiceReturnDetailService) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _salesInvoiceReturnDetailService = salesInvoiceReturnDetailService;
        }

        public IQueryable<SalesInvoiceReturnDetailTaxDto> GetSalesInvoiceReturnDetailTaxes(int salesInvoiceReturnHeaderId)
        {
            var data =
                    (
                    from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeaderId)
                    from salesInvoiceReturnDetailTax in _repository.GetAll().Where(x => x.SalesInvoiceReturnDetailId == salesInvoiceReturnDetail.SalesInvoiceReturnDetailId)
                    select new SalesInvoiceReturnDetailTaxDto
                    {
                        SalesInvoiceReturnDetailTaxId = salesInvoiceReturnDetailTax.SalesInvoiceReturnDetailTaxId,
                        SalesInvoiceReturnDetailId = salesInvoiceReturnDetailTax.SalesInvoiceReturnDetailId,
                        TaxId = salesInvoiceReturnDetailTax.TaxId,
                        TaxTypeId = salesInvoiceReturnDetailTax.TaxTypeId,
                        DebitAccountId = salesInvoiceReturnDetailTax.DebitAccountId,
                        TaxAfterVatInclusive = salesInvoiceReturnDetailTax.TaxAfterVatInclusive,
                        TaxPercent = salesInvoiceReturnDetailTax.TaxPercent,
                        TaxValue = salesInvoiceReturnDetailTax.TaxValue
                    }
                );

            return data;
        }

        public async Task<bool> SaveSalesInvoiceReturnDetailTaxes(int salesInvoiceReturnHeaderId, List<SalesInvoiceReturnDetailTaxDto> salesInvoiceReturnDetailTaxes)
        {
            await DeleteSalesInvoiceReturnDetailTax(salesInvoiceReturnDetailTaxes, salesInvoiceReturnHeaderId);
            if (salesInvoiceReturnDetailTaxes.Any())
            {
                await AddSalesInvoiceReturnDetailTax(salesInvoiceReturnDetailTaxes);
                await EditSalesInvoiceReturnDetailTax(salesInvoiceReturnDetailTaxes);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteSalesInvoiceReturnDetailTaxes(int salesInvoiceReturnHeaderId)
        {
            var data = await (
                       from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeaderId)
                       from salesInvoiceReturnDetailTax in _repository.GetAll().Where(x => x.SalesInvoiceReturnDetailId == salesInvoiceReturnDetail.SalesInvoiceReturnDetailId)
                       select salesInvoiceReturnDetailTax
                      ).ToListAsync();

            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteSalesInvoiceReturnDetailTax(List<SalesInvoiceReturnDetailTaxDto> details, int headerId)
        {
            var current = await (
                from salesInvoiceReturnDetail in _salesInvoiceReturnDetailService.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == headerId)
                from salesInvoiceReturnDetailTax in _repository.GetAll().Where(x => x.SalesInvoiceReturnDetailId == salesInvoiceReturnDetail.SalesInvoiceReturnDetailId)
                select salesInvoiceReturnDetailTax
                ).AsNoTracking().ToListAsync();


            var toBeDeleted = current.Where(p => details.All(p2 => p2.SalesInvoiceReturnDetailTaxId != p.SalesInvoiceReturnDetailTaxId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddSalesInvoiceReturnDetailTax(List<SalesInvoiceReturnDetailTaxDto> details)
        {
            var current = details.Where(x => x.SalesInvoiceReturnDetailTaxId <= 0).ToList();
            var modelList = new List<SalesInvoiceReturnDetailTax>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new SalesInvoiceReturnDetailTax()
                {
                    SalesInvoiceReturnDetailTaxId = newId,
                    SalesInvoiceReturnDetailId = detail.SalesInvoiceReturnDetailId,
                    TaxId = detail.TaxId,
                    TaxTypeId = detail.TaxTypeId,
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

        private async Task<bool> EditSalesInvoiceReturnDetailTax(List<SalesInvoiceReturnDetailTaxDto> salesInvoiceReturnDetailTaxes)
        {
            var current = salesInvoiceReturnDetailTaxes.Where(x => x.SalesInvoiceReturnDetailTaxId > 0).ToList();
            var modelList = new List<SalesInvoiceReturnDetailTax>();
            foreach (var detail in current)
            {
                var model = new SalesInvoiceReturnDetailTax()
                {
                    SalesInvoiceReturnDetailTaxId = detail.SalesInvoiceReturnDetailTaxId,
                    SalesInvoiceReturnDetailId = detail.SalesInvoiceReturnDetailId,
                    TaxId = detail.TaxId,
					TaxTypeId = detail.TaxTypeId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.SalesInvoiceReturnDetailTaxId) + 1; } catch { id = 1; }
            return id;
        }
    }
}
