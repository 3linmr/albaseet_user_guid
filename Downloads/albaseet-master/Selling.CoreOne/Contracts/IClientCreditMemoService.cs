using Sales.CoreOne.Models.Domain;
using Sales.CoreOne.Models.Dtos.ViewModels;
using Shared.CoreOne;
using Shared.Helper.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sales.CoreOne.Contracts
{
	public interface IClientCreditMemoService: IBaseService<ClientCreditMemo>
	{
		IQueryable<ClientCreditMemoDto> GetClientCreditMemos();
		Task<ClientCreditMemoDto?> GetClientCreditMemoById(int clientCreditMemoId);
		IQueryable<ClientCreditMemoDto> GetUserClientCreditMemos();
		IQueryable<ClientCreditMemoDto> GetClientCreditMemosByStoreId(int storeId, int clientId);
		Task<DocumentCodeDto> GetClientCreditMemoCode(int storeId, DateTime documentDate);
		Task<ResponseDto> SaveClientCreditMemo(ClientCreditMemoDto clientCreditMemo, bool hasApprove, bool approved, int? requestId);
		Task<ResponseDto> DeleteClientCreditMemo(int clientCreditMemoId);
		Task<int> GetNextId();
	}
}
