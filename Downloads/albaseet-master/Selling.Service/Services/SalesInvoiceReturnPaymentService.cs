using Microsoft.AspNetCore.Http;
using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Contracts.Modules;
using Shared.CoreOne;
using Shared.Helper.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Service;
using Sales.CoreOne.Contracts;
using Microsoft.EntityFrameworkCore;
using Shared.Helper.Identity;

namespace Sales.Service.Services
{
	public class SalesInvoiceReturnPaymentService: BaseService<SalesInvoiceReturnPayment>, ISalesInvoiceReturnPaymentService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IPaymentMethodService _paymentMethodService;

		public SalesInvoiceReturnPaymentService(IRepository<SalesInvoiceReturnPayment> repository, IHttpContextAccessor httpContextAccessor, IPaymentMethodService paymentMethodService) : base(repository)
		{
			_httpContextAccessor = httpContextAccessor;
			_paymentMethodService = paymentMethodService;
		}

		public async Task<List<SalesInvoiceReturnPaymentDto>> GetSalesInvoiceReturnPayments(int salesInvoiceReturnHeaderId, int storeId)
		{
			var paymentMethods = (await _paymentMethodService.GetVoucherPaymentMethods(storeId, true, true));
			var salesInvoiceReturnPayments = await _repository.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeaderId).ToListAsync();

			var data = (from paymentMethod in paymentMethods
						from salesInvoiceReturnPayment in salesInvoiceReturnPayments.Where(x => x.PaymentMethodId == paymentMethod.PaymentMethodId).DefaultIfEmpty()
						select new SalesInvoiceReturnPaymentDto
						{
							SalesInvoiceReturnPaymentId = salesInvoiceReturnPayment != null ? salesInvoiceReturnPayment.SalesInvoiceReturnPaymentId : 0,
							SalesInvoiceReturnHeaderId = salesInvoiceReturnPayment != null ? salesInvoiceReturnPayment.SalesInvoiceReturnHeaderId : 0,
							PaymentMethodId = paymentMethod.PaymentMethodId,
							PaymentMethodName = paymentMethod.PaymentMethodName,
							AccountId = salesInvoiceReturnPayment != null ? salesInvoiceReturnPayment.AccountId : paymentMethod.AccountId,
							CurrencyId = salesInvoiceReturnPayment != null ? salesInvoiceReturnPayment.CurrencyId : paymentMethod.CurrencyId,
							CurrencyName = paymentMethod.CurrencyName,
							CurrencyRate = salesInvoiceReturnPayment != null ? salesInvoiceReturnPayment.CurrencyRate : paymentMethod.CurrencyRate,
							PaidValue = salesInvoiceReturnPayment != null ? salesInvoiceReturnPayment.PaidValue : 0,
							PaidValueAccount = salesInvoiceReturnPayment != null ? salesInvoiceReturnPayment.PaidValueAccount : 0,
							RemarksAr = salesInvoiceReturnPayment != null ? salesInvoiceReturnPayment.RemarksAr : null,
							RemarksEn = salesInvoiceReturnPayment != null ? salesInvoiceReturnPayment.RemarksEn : null,

							CreatedAt = salesInvoiceReturnPayment != null ? salesInvoiceReturnPayment.CreatedAt : null,
							IpAddressCreated = salesInvoiceReturnPayment != null ? salesInvoiceReturnPayment.IpAddressCreated : null,
							UserNameCreated = salesInvoiceReturnPayment != null ? salesInvoiceReturnPayment?.UserNameCreated : null
						}).ToList();

			var newId = -1;
			data.ForEach(x => x.SalesInvoiceReturnPaymentId = x.SalesInvoiceReturnPaymentId <= 0 ? newId-- : x.SalesInvoiceReturnPaymentId);

			return data!;
		}

		public async Task<List<SalesInvoiceReturnPaymentDto>> SaveSalesInvoiceReturnPayments(int salesInvoiceReturnHeaderId, List<SalesInvoiceReturnPaymentDto> salesInvoiceReturnPayments)
		{
			await DeleteSalesInvoiceReturnPayments(salesInvoiceReturnPayments, salesInvoiceReturnHeaderId);
			if (salesInvoiceReturnPayments.Any())
			{
				await EditSalesInvoiceReturnPayment(salesInvoiceReturnPayments);
				return await AddSalesInvoiceReturnPayment(salesInvoiceReturnPayments, salesInvoiceReturnHeaderId);
			}
			return salesInvoiceReturnPayments;
		}
		public async Task<bool> DeleteSalesInvoiceReturnPayments(int salesInvoiceReturnHeaderId)
		{
			var data = await _repository.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == salesInvoiceReturnHeaderId).ToListAsync();
			if (data.Any())
			{
				_repository.RemoveRange(data);
				await _repository.SaveChanges();
				return true;
			}
			return false;
		}

		private async Task<List<SalesInvoiceReturnPaymentDto>> AddSalesInvoiceReturnPayment(List<SalesInvoiceReturnPaymentDto> payments, int headerId)
		{
			var current = payments.Where(x => x.SalesInvoiceReturnPaymentId <= 0).ToList();
			var modelList = new List<SalesInvoiceReturnPayment>();
			var newId = await GetNextId();
			foreach (var payment in current)
			{
				var model = new SalesInvoiceReturnPayment()
				{
					SalesInvoiceReturnPaymentId = newId,
					SalesInvoiceReturnHeaderId = headerId,
					PaymentMethodId = payment.PaymentMethodId,
					AccountId = payment.AccountId,
					CurrencyId = payment.CurrencyId,
					CurrencyRate = payment.CurrencyRate,
					PaidValue = payment.PaidValue,
					PaidValueAccount = payment.PaidValueAccount,
					RemarksAr = payment.RemarksAr,
					RemarksEn = payment.RemarksEn,

					Hide = false,
					CreatedAt = DateHelper.GetDateTimeNow(),
					UserNameCreated = await _httpContextAccessor!.GetUserName(),
					IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
				};

				payment.SalesInvoiceReturnPaymentId = newId;
				payment.SalesInvoiceReturnHeaderId = headerId;

				modelList.Add(model);
				newId++;
			}

			if (modelList.Any())
			{
				await _repository.InsertRange(modelList);
				await _repository.SaveChanges();
			}
			return payments;
		}

		private async Task<bool> EditSalesInvoiceReturnPayment(List<SalesInvoiceReturnPaymentDto> salesInvoiceReturnPayments)
		{
			var current = salesInvoiceReturnPayments.Where(x => x.SalesInvoiceReturnPaymentId > 0).ToList();
			var modelList = new List<SalesInvoiceReturnPayment>();
			foreach (var payment in current)
			{
				var model = new SalesInvoiceReturnPayment()
				{
					SalesInvoiceReturnPaymentId = payment.SalesInvoiceReturnPaymentId,
					SalesInvoiceReturnHeaderId = payment.SalesInvoiceReturnHeaderId,
					PaymentMethodId = payment.PaymentMethodId,
					AccountId = payment.AccountId,
					CurrencyId = payment.CurrencyId,
					CurrencyRate = payment.CurrencyRate,
					PaidValue = payment.PaidValue,
					PaidValueAccount = payment.PaidValueAccount,
					RemarksAr = payment.RemarksAr,
					RemarksEn = payment.RemarksEn,

					CreatedAt = payment.CreatedAt,
					UserNameCreated = payment.UserNameCreated,
					IpAddressCreated = payment.IpAddressCreated,
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

		private async Task<bool> DeleteSalesInvoiceReturnPayments(List<SalesInvoiceReturnPaymentDto> payments, int headerId)
		{
			var current = _repository.GetAll().Where(x => x.SalesInvoiceReturnHeaderId == headerId).AsNoTracking().ToList();
			var toBeDeleted = current.Where(p => payments.All(p2 => p2.SalesInvoiceReturnPaymentId != p.SalesInvoiceReturnPaymentId)).ToList();
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
			try { id = await _repository.GetAll().MaxAsync(a => a.SalesInvoiceReturnPaymentId) + 1; } catch { id = 1; }
			return id;
		}
	}
}
