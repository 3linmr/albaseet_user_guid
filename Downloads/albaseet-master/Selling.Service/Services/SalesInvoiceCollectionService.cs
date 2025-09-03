using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Sales.CoreOne.Contracts;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.Helper.Identity;
using Shared.Helper.Logic;
using Shared.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Shared.CoreOne.Models.StaticData.LanguageData;

namespace Sales.Service.Services
{
	public class SalesInvoiceCollectionService: BaseService<SalesInvoiceCollection>, ISalesInvoiceCollectionService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IPaymentMethodService _paymentMethodService;

		public SalesInvoiceCollectionService(IRepository<SalesInvoiceCollection> repository, IHttpContextAccessor httpContextAccessor, IPaymentMethodService paymentMethodService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_paymentMethodService = paymentMethodService;
		}

		public async Task<List<SalesInvoiceCollectionDto>> GetSalesInvoiceCollections(int salesInvoiceHeaderId)
		{
			var data = await (from paymentMethod in _paymentMethodService.GetAllPaymentMethods().AsQueryable()
						from salesInvoiceCollection in _repository.GetAll().Where(x => x.PaymentMethodId == paymentMethod.PaymentMethodId && x.SalesInvoiceHeaderId == salesInvoiceHeaderId).DefaultIfEmpty()
						select new SalesInvoiceCollectionDto
						{
							SalesInvoiceCollectionId = salesInvoiceCollection != null ? salesInvoiceCollection.SalesInvoiceCollectionId : 0,
							SalesInvoiceHeaderId = salesInvoiceCollection != null ? salesInvoiceCollection.SalesInvoiceHeaderId : 0,
							PaymentMethodId = paymentMethod.PaymentMethodId,
							PaymentMethodName = paymentMethod.PaymentMethodName,
							AccountId = salesInvoiceCollection != null ? salesInvoiceCollection.AccountId : paymentMethod.PaymentAccountId,
							CurrencyId = salesInvoiceCollection != null ? salesInvoiceCollection.CurrencyId : paymentMethod.CurrencyId,
							CurrencyName = paymentMethod.CurrencyName,
							CurrencyRate = salesInvoiceCollection != null ? salesInvoiceCollection.CurrencyRate : paymentMethod.CurrencyRate,
							CollectedValue = salesInvoiceCollection != null ? salesInvoiceCollection.CollectedValue : 0,
							CollectedValueAccount = salesInvoiceCollection != null ? salesInvoiceCollection.CollectedValueAccount : 0,
							RemarksAr = salesInvoiceCollection != null ? salesInvoiceCollection.RemarksAr : null,
							RemarksEn = salesInvoiceCollection != null ? salesInvoiceCollection.RemarksEn : null,

							CreatedAt = salesInvoiceCollection != null ? salesInvoiceCollection.CreatedAt : null,
							IpAddressCreated = salesInvoiceCollection != null ? salesInvoiceCollection.IpAddressCreated : null,
							UserNameCreated = salesInvoiceCollection != null ? salesInvoiceCollection.UserNameCreated : null
						}).ToListAsync();
			return data!;
		}

		public async Task<List<SalesInvoiceCollectionDto>> GetSalesInvoiceCollections(int salesInvoiceHeaderId, int storeId)
		{
			var paymentMethods = (await _paymentMethodService.GetVoucherPaymentMethods(storeId, false, false));
			var salesInvoiceCollections = await _repository.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).ToListAsync();

			var data = (from paymentMethod in paymentMethods
						from salesInvoiceCollection in salesInvoiceCollections.Where(x => x.PaymentMethodId == paymentMethod.PaymentMethodId).DefaultIfEmpty()
						select new SalesInvoiceCollectionDto
						{
							SalesInvoiceCollectionId = salesInvoiceCollection != null ? salesInvoiceCollection.SalesInvoiceCollectionId : 0,
							SalesInvoiceHeaderId = salesInvoiceCollection != null ? salesInvoiceCollection.SalesInvoiceHeaderId : 0,
							PaymentMethodId = paymentMethod.PaymentMethodId,
							PaymentMethodName = paymentMethod.PaymentMethodName,
							AccountId = salesInvoiceCollection != null ? salesInvoiceCollection.AccountId : paymentMethod.AccountId,
							CurrencyId = salesInvoiceCollection != null ? salesInvoiceCollection.CurrencyId : paymentMethod.CurrencyId,
							CurrencyName = paymentMethod.CurrencyName,
							CurrencyRate = salesInvoiceCollection != null ? salesInvoiceCollection.CurrencyRate : paymentMethod.CurrencyRate,
							CollectedValue = salesInvoiceCollection != null ? salesInvoiceCollection.CollectedValue : 0,
							CollectedValueAccount = salesInvoiceCollection != null ? salesInvoiceCollection.CollectedValueAccount : 0,
							RemarksAr = salesInvoiceCollection != null ? salesInvoiceCollection.RemarksAr : null,
							RemarksEn = salesInvoiceCollection != null ? salesInvoiceCollection.RemarksEn : null,

							CreatedAt = salesInvoiceCollection != null ? salesInvoiceCollection.CreatedAt : null,
							IpAddressCreated = salesInvoiceCollection != null ? salesInvoiceCollection.IpAddressCreated : null,
							UserNameCreated = salesInvoiceCollection != null ? salesInvoiceCollection?.UserNameCreated : null
						}).ToList();

			var newId = -1;
			data.ForEach(x => x.SalesInvoiceCollectionId = x.SalesInvoiceCollectionId <= 0 ? newId-- : x.SalesInvoiceCollectionId);

			return data!;
		}

		public async Task<List<SalesInvoiceCollectionDto>> SaveSalesInvoiceCollections(int salesInvoiceHeaderId, List<SalesInvoiceCollectionDto> salesInvoiceCollections)
		{
			await DeleteSalesInvoiceCollections(salesInvoiceCollections, salesInvoiceHeaderId);
			if (salesInvoiceCollections.Any())
			{
				await EditSalesInvoiceCollection(salesInvoiceCollections);
				return await AddSalesInvoiceCollection(salesInvoiceCollections, salesInvoiceHeaderId);
			}
			return salesInvoiceCollections;
		}

		public async Task<bool> DeleteSalesInvoiceCollections(int salesInvoiceHeaderId)
		{
			var data = await _repository.GetAll().Where(x => x.SalesInvoiceHeaderId == salesInvoiceHeaderId).ToListAsync();
			if (data.Any())
			{
				_repository.RemoveRange(data);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}

		private async Task<List<SalesInvoiceCollectionDto>> AddSalesInvoiceCollection(List<SalesInvoiceCollectionDto> collections, int headerId)
		{
			var current = collections.Where(x => x.SalesInvoiceCollectionId <= 0).ToList();
			var modelList = new List<SalesInvoiceCollection>();
			var newId = await GetNextId();
			foreach (var collection in current)
			{
				var model = new SalesInvoiceCollection()
				{
					SalesInvoiceCollectionId = newId,
					SalesInvoiceHeaderId = headerId,
					PaymentMethodId = collection.PaymentMethodId,
					AccountId = collection.AccountId,
					CurrencyId = collection.CurrencyId,
					CurrencyRate = collection.CurrencyRate,
					CollectedValue = collection.CollectedValue,
					CollectedValueAccount = collection.CollectedValueAccount,
					RemarksAr = collection.RemarksAr,
					RemarksEn = collection.RemarksEn,

					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor!.GetUserName(),
					IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				};

				collection.SalesInvoiceCollectionId = newId;
				collection.SalesInvoiceHeaderId = headerId;

				modelList.Add(model);
				newId++;
			}

			if (modelList.Any())
			{
				await _repository.InsertRange(modelList);
				await _repository.SaveChanges();
			}
			return collections;
		}

		private async Task<bool> EditSalesInvoiceCollection(List<SalesInvoiceCollectionDto> salesInvoiceCollections)
		{
			var current = salesInvoiceCollections.Where(x => x.SalesInvoiceCollectionId > 0).ToList();
			var modelList = new List<SalesInvoiceCollection>();
			foreach (var collection in current)
			{
				var model = new SalesInvoiceCollection()
				{
					SalesInvoiceCollectionId = collection.SalesInvoiceCollectionId,
					SalesInvoiceHeaderId = collection.SalesInvoiceHeaderId,
					PaymentMethodId = collection.PaymentMethodId,
					AccountId = collection.AccountId,
					CurrencyId = collection.CurrencyId,
					CurrencyRate = collection.CurrencyRate,
					CollectedValue = collection.CollectedValue,
					CollectedValueAccount = collection.CollectedValueAccount,
					RemarksAr = collection.RemarksAr,
					RemarksEn = collection.RemarksEn,

					CreatedAt = collection.CreatedAt,
					UserNameCreated = collection.UserNameCreated,
					IpAddressCreated = collection.IpAddressCreated,
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

		private async Task<bool> DeleteSalesInvoiceCollections(List<SalesInvoiceCollectionDto> collections, int headerId)
		{
			var current = _repository.GetAll().Where(x => x.SalesInvoiceHeaderId == headerId).AsNoTracking().ToList();
			var toBeDeleted = current.Where(p => collections.All(p2 => p2.SalesInvoiceCollectionId != p.SalesInvoiceCollectionId)).ToList();
			if (toBeDeleted.Any())
			{
				_repository.RemoveRange(toBeDeleted);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}

		private async Task<int> GetNextId()
		{
			int id = 1;
			try { id = await _repository.GetAll().MaxAsync(a => a.SalesInvoiceCollectionId) + 1; } catch { id = 1; }
			return id;
		}
	}
}
