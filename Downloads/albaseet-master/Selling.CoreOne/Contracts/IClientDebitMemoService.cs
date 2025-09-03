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
	public interface IClientDebitMemoService: IBaseService<ClientDebitMemo>
	{
		IQueryable<ClientDebitMemoDto> GetClientDebitMemos();
		Task<ClientDebitMemoDto?> GetClientDebitMemoById(int clientDebitMemoId);
		IQueryable<ClientDebitMemoDto> GetUserClientDebitMemos();
		IQueryable<ClientDebitMemoDto> GetClientDebitMemosByStoreId(int storeId, int clientId);
		Task<DocumentCodeDto> GetClientDebitMemoCode(int storeId, DateTime documentDate);
		Task<ResponseDto> SaveClientDebitMemo(ClientDebitMemoDto clientDebitMemo, bool hasApprove, bool approved, int? requestId);
		Task<ResponseDto> DeleteClientDebitMemo(int clientDebitMemoId);
		Task<int> GetNextId();
	}
}
