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
	public class ClientQuotationDetailTaxService : BaseService<ClientQuotationDetailTax>, IClientQuotationDetailTaxService
	{
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IClientQuotationDetailService _clientQuotationDetailService;

		public ClientQuotationDetailTaxService(IRepository<ClientQuotationDetailTax> repository,IHttpContextAccessor httpContextAccessor, IClientQuotationDetailService clientQuotationDetailService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
            _clientQuotationDetailService = clientQuotationDetailService;
        }

		public IQueryable<ClientQuotationDetailTaxDto> GetClientQuotationDetailTaxes(int clientQuotationHeaderId)
		{
			var data =
			        (
                    from clientQuotationDetail in _clientQuotationDetailService.GetAll().Where(x => x.ClientQuotationHeaderId == clientQuotationHeaderId)
                    from clientQuotationDetailTax in _repository.GetAll().Where(x => x.ClientQuotationDetailId == clientQuotationDetail.ClientQuotationDetailId)
                    select new ClientQuotationDetailTaxDto
				    {
                         ClientQuotationDetailTaxId = clientQuotationDetailTax.ClientQuotationDetailTaxId,
                         ClientQuotationDetailId = clientQuotationDetailTax.ClientQuotationDetailId,
                         TaxId = clientQuotationDetailTax.TaxId,
                         CreditAccountId = clientQuotationDetailTax.CreditAccountId,
                         TaxAfterVatInclusive = clientQuotationDetailTax.TaxAfterVatInclusive,
                         TaxPercent = clientQuotationDetailTax.TaxPercent,
                         TaxValue = clientQuotationDetailTax.TaxValue
				    }
                );

			return data;
		}

        public async Task<bool> SaveClientQuotationDetailTaxes(int clientQuotationHeaderId, List<ClientQuotationDetailTaxDto> clientQuotationDetailTaxes)
        {
            await DeleteClientQuotationDetailTax(clientQuotationDetailTaxes, clientQuotationHeaderId);
            if (clientQuotationDetailTaxes.Any())
            {
                await AddClientQuotationDetailTax(clientQuotationDetailTaxes);
                await EditClientQuotationDetailTax(clientQuotationDetailTaxes);
                return true;
            }
            return false;
        }

        public async Task<bool> DeleteClientQuotationDetailTaxes(int clientQuotationHeaderId)
        {
            var data = await (
                       from clientQuotationDetail in _clientQuotationDetailService.GetAll().Where(x => x.ClientQuotationHeaderId == clientQuotationHeaderId)
                       from clientQuotationDetailTax in _repository.GetAll().Where(x => x.ClientQuotationDetailId == clientQuotationDetail.ClientQuotationDetailId)
                       select clientQuotationDetailTax
                      ).ToListAsync();

            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeleteClientQuotationDetailTax(List<ClientQuotationDetailTaxDto> details, int headerId)
        {
            var current = await (
                from clientQuotationDetail in _clientQuotationDetailService.GetAll().Where(x => x.ClientQuotationHeaderId == headerId)
                from clientQuotationDetailTax in _repository.GetAll().Where(x => x.ClientQuotationDetailId == clientQuotationDetail.ClientQuotationDetailId)
                select clientQuotationDetailTax
                ).AsNoTracking().ToListAsync();


            var toBeDeleted = current.Where(p => details.All(p2 => p2.ClientQuotationDetailTaxId != p.ClientQuotationDetailTaxId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddClientQuotationDetailTax(List<ClientQuotationDetailTaxDto> details)
        {
            var current = details.Where(x => x.ClientQuotationDetailTaxId <= 0).ToList();
            var modelList = new List<ClientQuotationDetailTax>();
            var newId = await GetNextId();
            foreach (var detail in current)
            {
                var model = new ClientQuotationDetailTax()
                {
                    ClientQuotationDetailTaxId = newId,
                    ClientQuotationDetailId = detail.ClientQuotationDetailId,
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

        private async Task<bool> EditClientQuotationDetailTax(List<ClientQuotationDetailTaxDto> clientQuotationDetailTaxes)
        {            
            var current = clientQuotationDetailTaxes.Where(x => x.ClientQuotationDetailTaxId > 0).ToList();
            var modelList = new List<ClientQuotationDetailTax>();
            foreach (var detail in current)
            {
                var model = new ClientQuotationDetailTax()
                {
                    ClientQuotationDetailTaxId = detail.ClientQuotationDetailTaxId,
                    ClientQuotationDetailId = detail.ClientQuotationDetailId,
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
            try { id = await _repository.GetAll().MaxAsync(a => a.ClientQuotationDetailTaxId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
