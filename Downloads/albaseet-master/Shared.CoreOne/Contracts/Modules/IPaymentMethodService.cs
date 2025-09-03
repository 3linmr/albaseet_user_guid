using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Modules;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using Shared.CoreOne.Models.Dtos.ViewModels.Journal;
using Shared.CoreOne.Models.Dtos.ViewModels.Modules;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Modules
{
	public interface IPaymentMethodService : IBaseService<PaymentMethod>
	{
		IQueryable<PaymentMethodDto> GetAllPaymentMethods();
		IQueryable<PaymentMethodDto> GetUserPaymentMethods();
        IQueryable<PaymentMethodDto> GetCompanyPaymentMethods(int companyId);
		IQueryable<PaymentMethodDropDownDto> GetPaymentMethodsDropDown(int companyId);
		IQueryable<AccountAutoCompleteDto> GetPaymentMethodsAccounts();
		IQueryable<AccountAutoCompleteDto> GetReceivingMethodsAccounts();
		Task<List<VoucherPaymentMethodDto>> GetVoucherPaymentMethods(int storeId,bool isPayment,bool isOnlyCash);
		Task<VoucherPaymentMethodDto?> GetVoucherPaymentMethod(int storeId,int paymentMethodId);
		Task<JournalDetailCalculationVm> GetPaymentMethodEntries(int storeId, int paymentMethodId, decimal debitValue,decimal creditValue);
		Task<PaymentMethodDto> GetPaymentMethodById(int id);
		Task<PaymentMethodEntryDto?> GetPaymentMethodsEntry(int storeId,int paymentMethodId,decimal entryValue);
		Task<PaymentMethodDto?> GetDefaultCashMethod();
		Task<ResponseDto> SavePaymentMethod(PaymentMethodDto currency);
		Task<ResponseDto> DeletePaymentMethod(int id);
	}
}
