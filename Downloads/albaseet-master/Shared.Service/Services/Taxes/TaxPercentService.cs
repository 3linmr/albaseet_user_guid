using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Taxes;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Domain.Taxes;
using Shared.CoreOne.Models.Dtos.ViewModels.Taxes;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Helper.Models.Dtos;

namespace Shared.Service.Services.Taxes
{
	public class TaxPercentService : BaseService<TaxPercent>, ITaxPercentService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IStringLocalizer<TaxPercentService> _localizer;
		private readonly ITaxService _taxService;

		public TaxPercentService(IRepository<TaxPercent> repository, IHttpContextAccessor httpContextAccessor, IStringLocalizer<TaxPercentService> localizer, ITaxService taxService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_localizer = localizer;
			_taxService = taxService;
		}

		public IQueryable<TaxPercentDto> GetAllTaxPercents()
		{
			return _repository.GetAll().Select(x => new TaxPercentDto()
			{
				TaxPercentId = x.TaxPercentId,
				TaxId = x.TaxId,
				FromDate = x.FromDate,
				Percent = x.Percent,
			});
		}

		public IQueryable<TaxPercentDto> GetTaxPercentsByTaxId(int taxId)
		{
			return GetAllTaxPercents().Where(x => x.TaxId == taxId);
		}

		public async Task<TaxPercentDto?> GetTaxPercentById(int id)
		{
			return await GetAllTaxPercents().FirstOrDefaultAsync(x => x.TaxPercentId == id);
		}

		public async Task<TaxPercentDto> GetCurrentTaxPercent(int taxId, DateTime currentDate)
		{
			var tax = await _repository.GetAll().Where(x => x.TaxId == taxId && x.FromDate <= currentDate)
				.OrderByDescending(x => x.FromDate).Take(1).Select(x => new TaxPercentDto
				{
					FromDate = x.FromDate,
					Percent = x.Percent,
					TaxId = x.TaxId,
					TaxPercentId = x.TaxPercentId
				}).FirstOrDefaultAsync();
			return tax ?? new TaxPercentDto();
		}

        public IQueryable<TaxPercentDto> GetAllCurrentTaxPercents(DateTime currentDate)
        {
			var tax = _repository.GetAll().Where(x => x.FromDate <= currentDate).GroupBy(x => new { x.TaxId })
				.Select(x => new TaxPercentDto
				{
					TaxId = x.Key.TaxId,
					Percent = x.OrderByDescending(x => x.FromDate).Select(x => x.Percent).FirstOrDefault()
				});

            return tax;
        }

        public IQueryable<CompanyVatPercentDto> GetAllCompanyVatPercents(DateTime currentDate)
        {
			var percent = from tax in _taxService.GetAll().Where(x => x.IsVatTax)
						  from taxPercent in _repository.GetAll().Where(x => x.TaxId == tax.TaxId && x.FromDate <= currentDate)
						  group taxPercent by new { tax.TaxId, tax.CompanyId } into g
						  select new CompanyVatPercentDto
						  {
							  TaxId = g.Key.TaxId,
							  CompanyId = g.Key.CompanyId,
							  VatPercent = g.OrderByDescending(x => x.FromDate).Select(x => x.Percent).FirstOrDefault()
						  };

			return percent;
		}

		public async Task<decimal> GetCurrentTaxPercentValue(int taxId, DateTime currentDate)
		{
			var taxPercent = await GetCurrentTaxPercent(taxId, currentDate);
			return taxPercent.Percent;
		}

		public async Task<decimal> GetVatTaxByCompanyId(int companyId, DateTime documentDate)
		{
			var vatTax = await _taxService.GetCompanyTaxes(companyId).FirstOrDefaultAsync(x => x.CompanyId == companyId && x.IsVatTax);
			if (vatTax != null)
			{
				return await GetCurrentTaxPercentValue(vatTax.TaxId, documentDate);
			}
			else
			{
				return 0;
			}
		}

		public async Task<decimal> GetVatTaxByStoreId(int storeId, DateTime documentDate)
		{
			var vatTax = await _taxService.GetStoreTaxes(storeId).FirstOrDefaultAsync(x => x.StoreId == storeId && x.IsVatTax);
			if (vatTax != null)
			{
				return await GetCurrentTaxPercentValue(vatTax.TaxId, documentDate);
			}
			else
			{
				return 0;
			}
		}

		public async Task<TaxDto> GetVatByCompanyId(int companyId,DateTime currentDate)
		{
			var tax =  await _taxService.GetCompanyTaxes(companyId).FirstOrDefaultAsync(x => x.IsVatTax) ?? new TaxDto();
			var taxPercent = await GetVatTaxByCompanyId(companyId,currentDate);
			var model = tax;
			model.Percent = taxPercent;
			return model;
		}

		public async Task<TaxDto> GetVatByStoreId(int storeId, DateTime currentDate)
		{
			var tax =  await _taxService.GetStoreTaxes(storeId).FirstOrDefaultAsync(x => x.IsVatTax) ?? new TaxDto();
			var taxPercent = await GetVatTaxByStoreId(storeId, currentDate);
			var model = tax;
			model.Percent = taxPercent;
			return model;
		}

		public async Task<ResponseDto> SaveTaxPercents(List<TaxPercentDto> percents, int taxId)
		{
			if (taxId == 0)
			{
				await CreateTaxPercents(percents, taxId);
				return new ResponseDto() { Success = true };
			}
			else
			{
				await DeleteTaxPercents(percents, taxId);
				await CreateTaxPercents(percents, taxId);
				await UpdateTaxPercents(percents, taxId);
				return new ResponseDto() { Success = true };

			}
		}

		public async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.TaxPercentId) + 1; } catch { id = 1; }
			return id;
		}
		public async Task<bool> CreateTaxPercents(List<TaxPercentDto> percents, int taxId)
		{
			var percentList = new List<TaxPercent>();
			var newId = await GetNextId();
			foreach (var percent in percents)
			{
				if (percent.TaxPercentId <= 0)
				{
					var newPercent = new TaxPercent()
					{
						TaxPercentId = newId,
						FromDate = percent.FromDate,
						TaxId = taxId,
						Percent = percent.Percent,
						CreatedAt = DateHelper.GetDateTimeNow(),
						UserNameCreated = await _httpContextAccessor!.GetUserName(),
						IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
						Hide = false
					};
					percentList.Add(newPercent);
					newId++;
				}
			}

			if (percentList.Any())
			{
				await _repository.InsertRange(percentList);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}
		public async Task<bool> UpdateTaxPercents(List<TaxPercentDto> percents, int taxId)
		{
			var currentPercents = percents.Where(x => x.TaxPercentId > 0 && x.TaxId > 0).ToList();
			var percentList = new List<TaxPercent>();
			foreach (var percent in currentPercents)
			{
				if (percent.TaxPercentId > 0)
				{
					var newNote = new TaxPercent()
					{
						TaxPercentId = percent.TaxPercentId,
						FromDate = percent.FromDate,
						TaxId = taxId,
						Percent = percent.Percent,
						ModifiedAt = DateHelper.GetDateTimeNow(),
						UserNameModified = await _httpContextAccessor!.GetUserName(),
						IpAddressModified = _httpContextAccessor?.GetIpAddress(),
						Hide = false
					};
					percentList.Add(newNote);
				}
			}

			if (percentList.Any())
			{
				_repository.UpdateRange(percentList);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}
		public async Task<bool> DeleteTaxPercents(List<TaxPercentDto> percents, int taxId)
		{
			if (percents.Any())
			{
				var currentPercents = await _repository.GetAll().Where(x => x.TaxId == taxId).AsNoTracking().ToListAsync();
				var notesToBeDeleted = currentPercents.Where(p => percents.All(p2 => p2.TaxPercentId != p.TaxPercentId)).ToList();
				if (notesToBeDeleted.Any())
				{
					_repository.RemoveRange(notesToBeDeleted);
					await _repository.SaveChanges();
					return true;
				}
			}
			return false;
		}

		public async Task<ResponseDto> DeleteTaxByTaxId(int id)
		{
			var data = await _repository.GetAll().Where(x => x.TaxId == id).ToListAsync();
			if (data.Any())
			{
				_repository.RemoveRange(data);
				await _repository.SaveChanges();
				return new ResponseDto() { Success = true };
			}
			return new ResponseDto() { Success = false };
		}
	}
}
