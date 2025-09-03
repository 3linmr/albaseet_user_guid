using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Dtos.ViewModels.Accounts;

namespace Shared.CoreOne.Contracts.Accounts
{
	public interface IAccountTaxService
	{
		Task<AccountTaxDto> GetAccountTax(int storeId,int taxId, bool isDebit);
	}
}
