using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using Shared.Helper.Models.Dtos;

namespace Shared.CoreOne.Contracts.Basics
{
    public interface IInvoiceExpenseTypeService : IBaseService<InvoiceExpenseType>
    {
        IQueryable<InvoiceExpenseTypeDto> GetAllInvoiceExpenseTypes();
        IQueryable<InvoiceExpenseTypeDto> GetCompanyInvoiceExpenseTypes();
        IQueryable<InvoiceExpenseTypeDropDownDto> GetAllInvoiceExpenseTypesDropDown();
        Task<InvoiceExpenseTypeDto?> GetInvoiceExpenseTypeById(int id);
        Task<ResponseDto> SaveInvoiceExpenseType(InvoiceExpenseTypeDto invoiceExpenseType);
        Task<ResponseDto> DeleteInvoiceExpenseType(int id);
    }
}
