using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Contracts.Accounts
{
    public interface IClientBalanceService
    {
        public IQueryable<ClientBalanceDto> GetClientBalanceService(DateTime? toDate, bool includeStores);
        public Task<decimal> GetClientBalanceByAccountId(int? accountId);
    }
}
