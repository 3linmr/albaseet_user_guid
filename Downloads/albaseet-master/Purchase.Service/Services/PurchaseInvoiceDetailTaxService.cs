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
	public class PurchaseInvoiceDetailTaxService : BaseService<PurchaseInvoiceDetailTax>, IPurchaseInvoiceDetailTaxService
	{
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPurchaseInvoiceDetailService _purchaseInvoiceDetailService;

		public PurchaseInvoiceDetailTaxService(IRepository<PurchaseInvoiceDetailTax> repository,IHttpContextAccessor httpContextAccessor, IPurchaseInvoiceDetailService purchaseInvoiceDetailService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
            _purchaseInvoiceDetailService = purchaseInvoiceDetailService;
        }

		public IQueryable<PurchaseInvoiceDetailTaxDto> GetPurchaseInvoiceDetailTaxes(int purchaseInvoiceHeaderId)
		{
			var data =
			        (
                    from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId)
                    from purchaseInvoiceDetailTax in _repository.GetAll().Where(x => x.PurchaseInvoiceDetailId == purchaseInvoiceDetail.PurchaseInvoiceDetailId)
                    select new PurchaseInvoiceDetailTaxDto
				    {
                         PurchaseInvoiceDetailTaxId = purchaseInvoiceDetailTax.PurchaseInvoiceDetailTaxId,
                         PurchaseInvoiceDetailId = purchaseInvoiceDetailTax.PurchaseInvoiceDetailId,
                         TaxId = purchaseInvoiceDetailTax.TaxId,
                         TaxTypeId = purchaseInvoiceDetailTax.TaxTypeId,
                         DebitAccountId = purchaseInvoiceDetailTax.DebitAccountId,
                         TaxAfterVatInclusive = purchaseInvoiceDetailTax.TaxAfterVatInclusive,
                         TaxPercent = purchaseInvoiceDetailTax.TaxPercent,
                         TaxValue = purchaseInvoiceDetailTax.TaxValue
				    }
                );

			return data;
		}

        public async Task<bool> SavePurchaseInvoiceDetailTaxes(int purchaseInvoiceHeaderId, List<PurchaseInvoiceDetailTaxDto> purchaseInvoiceDetailTaxes)
        {
            await DeletePurchaseInvoiceDetailTax(purchaseInvoiceDetailTaxes, purchaseInvoiceHeaderId);
            if (purchaseInvoiceDetailTaxes.Any())
            {
                await AddPurchaseInvoiceDetailTax(purchaseInvoiceDetailTaxes);
                await EditPurchaseInvoiceDetailTax(purchaseInvoiceDetailTaxes);
                return true;
            }
            return false;
        }

        public async Task<bool> DeletePurchaseInvoiceDetailTaxes(int purchaseInvoiceHeaderId)
        {
            var data = await (
                       from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId)
                       from purchaseInvoiceDetailTax in _repository.GetAll().Where(x => x.PurchaseInvoiceDetailId == purchaseInvoiceDetail.PurchaseInvoiceDetailId)
                       select purchaseInvoiceDetailTax
                      ).ToListAsync();

            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeletePurchaseInvoiceDetailTax(List<PurchaseInvoiceDetailTaxDto> details, int headerId)
        {
            var current = await (
                from purchaseInvoiceDetail in _purchaseInvoiceDetailService.GetAll().Where(x => x.PurchaseInvoiceHeaderId == headerId)
                from purchaseInvoiceDetailTax in _repository.GetAll().Where(x => x.PurchaseInvoiceDetailId == purchaseInvoiceDetail.PurchaseInvoiceDetailId)
                select purchaseInvoiceDetailTax
                ).AsNoTracking().ToListAsync();


            var toBeDeleted = current.Where(p => details.All(p2 => p2.PurchaseInvoiceDetailTaxId != p.PurchaseInvoiceDetailTaxId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddPurchaseInvoiceDetailTax(List<PurchaseInvoiceDetailTaxDto> details)
        {
            var current = details.Where(x => x.PurchaseInvoiceDetailTaxId <= 0).ToList();
            var modelList = new List<PurchaseInvoiceDetailTax>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new PurchaseInvoiceDetailTax()
                {
                    PurchaseInvoiceDetailTaxId = newId,
                    PurchaseInvoiceDetailId = detail.PurchaseInvoiceDetailId,
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

        private async Task<bool> EditPurchaseInvoiceDetailTax(List<PurchaseInvoiceDetailTaxDto> purchaseInvoiceDetailTaxes)
        {            
            var current = purchaseInvoiceDetailTaxes.Where(x => x.PurchaseInvoiceDetailTaxId > 0).ToList();
            var modelList = new List<PurchaseInvoiceDetailTax>();
            foreach (var detail in current)
            {
                var model = new PurchaseInvoiceDetailTax()
                {
                    PurchaseInvoiceDetailTaxId = detail.PurchaseInvoiceDetailTaxId,
                    PurchaseInvoiceDetailId = detail.PurchaseInvoiceDetailId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.PurchaseInvoiceDetailTaxId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
