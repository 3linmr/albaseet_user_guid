using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Accounts;
using Shared.CoreOne.Models.Domain.Accounts;

namespace Shared.Service.Services.Accounts
{
	public class AccountStoreService : BaseService<AccountStore>, IAccountStoreService
	{
		public AccountStoreService(IRepository<AccountStore> repository) : base(repository)
		{

		}
	}
}
