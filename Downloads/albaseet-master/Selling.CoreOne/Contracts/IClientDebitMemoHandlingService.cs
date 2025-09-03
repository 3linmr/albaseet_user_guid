using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne.Models.Dtos.ViewModels.Approval;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
	public interface IClientDebitMemoHandlingService
	{
		public List<RequestChangesDto> GetClientDebitMemoRequestChanges(ClientDebitMemoVm oldItem, ClientDebitMemoVm newItem);
		public Task<ClientDebitMemoVm> GetClientDebitMemo(int clientDebitMemoId);
		public Task<ClientDebitMemoVm> GetClientDebitMemoFromSalesInvoice(int salesInvoiceHeaderId);
		public Task<ResponseDto> SaveClientDebitMemoInFull(ClientDebitMemoVm clientDebitMemo, bool hasApprove, bool approved, int? requestId);
		public Task<ResponseDto> DeleteClientDebitMemoInFull(int clientDebitMemoId);
	}
}
