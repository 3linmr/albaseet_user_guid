using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Contracts.Reservation
{
	public interface IReservationInvoiceService
	{
		Task<ResponseDto> SaveReservationInvoice(SalesInvoiceDto salesInvoice, bool hasApprove, bool isApproved, int? requestId);
		Task<ResponseDto> DeleteReservationInvoice(int salesInvoiceHeaderId, int menuCode);
	}
}
