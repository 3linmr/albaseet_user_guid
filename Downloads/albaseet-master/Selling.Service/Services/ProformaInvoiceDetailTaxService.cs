using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
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

namespace Sales.Service.Services
{
	public class ProformaInvoiceDetailTaxService : BaseService<ProformaInvoiceDetailTax>, IProformaInvoiceDetailTaxService
	{
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProformaInvoiceDetailService _proformaInvoiceDetailService;

		public ProformaInvoiceDetailTaxService(IRepository<ProformaInvoiceDetailTax> repository,IHttpContextAccessor httpContextAccessor, IProformaInvoiceDetailService proformaInvoiceDetailService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
            _proformaInvoiceDetailService = proformaInvoiceDetailService;
        }

		public IQueryable<ProformaInvoiceDetailTaxDto> GetProformaInvoiceDetailTaxes(int proformaInvoiceHeaderId)
		{
			var data =
			        (
                    from proformaInvoiceDetail in _proformaInvoiceDetailService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
                    from proformaInvoiceDetailTax in _repository.GetAll().Where(x => x.ProformaInvoiceDetailId == proformaInvoiceDetail.ProformaInvoiceDetailId)
                    select new ProformaInvoiceDetailTaxDto
				    {
                         ProformaInvoiceDetailTaxId = proformaInvoiceDetailTax.ProformaInvoiceDetailTaxId,
                         ProformaInvoiceDetailId = proformaInvoiceDetailTax.ProformaInvoiceDetailId,
                         TaxId = proformaInvoiceDetailTax.TaxId,
                         CreditAccountId = proformaInvoiceDetailTax.CreditAccountId,
                         TaxAfterVatInclusive = proformaInvoiceDetailTax.TaxAfterVatInclusive,
                         TaxPercent = proformaInvoiceDetailTax.TaxPercent,
                         TaxValue = proformaInvoiceDetailTax.TaxValue
				    }
                );

			return data;
		}

        public async Task<bool> SaveProformaInvoiceDetailTaxes(int proformaInvoiceHeaderId, List<ProformaInvoiceDetailTaxDto> proformaInvoiceDetailTaxes)
        {
            await DeleteProformaInvoiceDetailTax(proformaInvoiceDetailTaxes, proformaInvoiceHeaderId);
            if (proformaInvoiceDetailTaxes.Any())
            {
                await AddProformaInvoiceDetailTax(proformaInvoiceDetailTaxes);
                await EditProformaInvoiceDetailTax(proformaInvoiceDetailTaxes);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteProformaInvoiceDetailTaxes(int proformaInvoiceHeaderId)
        {
            var data = await (
                       from proformaInvoiceDetail in _proformaInvoiceDetailService.GetAll().Where(x => x.ProformaInvoiceHeaderId == proformaInvoiceHeaderId)
                       from proformaInvoiceDetailTax in _repository.GetAll().Where(x => x.ProformaInvoiceDetailId == proformaInvoiceDetail.ProformaInvoiceDetailId)
                       select proformaInvoiceDetailTax
                      ).ToListAsync();

            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteProformaInvoiceDetailTax(List<ProformaInvoiceDetailTaxDto> details, int headerId)
        {
            var current = await (
                from proformaInvoiceDetail in _proformaInvoiceDetailService.GetAll().Where(x => x.ProformaInvoiceHeaderId == headerId)
                from proformaInvoiceDetailTax in _repository.GetAll().Where(x => x.ProformaInvoiceDetailId == proformaInvoiceDetail.ProformaInvoiceDetailId)
                select proformaInvoiceDetailTax
                ).AsNoTracking().ToListAsync();


            var toBeDeleted = current.Where(p => details.All(p2 => p2.ProformaInvoiceDetailTaxId != p.ProformaInvoiceDetailTaxId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddProformaInvoiceDetailTax(List<ProformaInvoiceDetailTaxDto> details)
        {
            var current = details.Where(x => x.ProformaInvoiceDetailTaxId <= 0).ToList();
            var modelList = new List<ProformaInvoiceDetailTax>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new ProformaInvoiceDetailTax()
                {
                    ProformaInvoiceDetailTaxId = newId,
                    ProformaInvoiceDetailId = detail.ProformaInvoiceDetailId,
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

        private async Task<bool> EditProformaInvoiceDetailTax(List<ProformaInvoiceDetailTaxDto> proformaInvoiceDetailTaxes)
        {            
            var current = proformaInvoiceDetailTaxes.Where(x => x.ProformaInvoiceDetailTaxId > 0).ToList();
            var modelList = new List<ProformaInvoiceDetailTax>();
            foreach (var detail in current)
            {
                var model = new ProformaInvoiceDetailTax()
                {
                    ProformaInvoiceDetailTaxId = detail.ProformaInvoiceDetailTaxId,
                    ProformaInvoiceDetailId = detail.ProformaInvoiceDetailId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.ProformaInvoiceDetailTaxId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
