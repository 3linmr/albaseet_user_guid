using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Models.Domain.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Service.Services.Accounts
{
	public class AccountTypeService : BaseService<AccountType>, IAccountTypeService
	{
		public AccountTypeService(IRepository<AccountType> repository) : base(repository)
		{
		}
	}
}
