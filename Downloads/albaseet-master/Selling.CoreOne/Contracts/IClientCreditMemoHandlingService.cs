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
	public interface IClientCreditMemoHandlingService
	{
		public List<RequestChangesDto> GetClientCreditMemoRequestChanges(ClientCreditMemoVm oldItem, ClientCreditMemoVm newItem);
		public Task<ClientCreditMemoVm> GetClientCreditMemo(int clientCreditMemoId);
		public Task<ClientCreditMemoVm> GetClientCreditMemoFromSalesInvoice(int salesInvoiceHeaderId);
		public Task<ResponseDto> SaveClientCreditMemoInFull(ClientCreditMemoVm clientCreditMemo, bool hasApprove, bool approved, int? requestId);
		public Task<ResponseDto> DeleteClientCreditMemoInFull(int clientCreditMemoId);
	}
}
