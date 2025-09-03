using Shared.CoreOne.Models.Domain.Basics;
using Shared.CoreOne.Models.Dtos.ViewModels.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Contracts.Basics
{
	public interface IInvoiceTypeService: IBaseService<InvoiceType>
	{
		IQueryable<InvoiceTypeDto> GetAllInvoiceTypes();
		IQueryable<InvoiceTypeDropDownDto> GetAllInvoiceTypesDropDown();
	}
}
