using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reservation
{
	public interface IReservationInvoiceCloseOutService
	{
		Task<ResponseDto> SaveReservationInvoiceCloseOut(SalesInvoiceReturnDto salesInvoiceReturnDto, bool hasApprove, bool isApproved, int? requestId);
		Task<ResponseDto> DeleteReservationInvoiceCloseOut(int salesInvoiceReturnHeaderId, int menuCode);
	}
}
