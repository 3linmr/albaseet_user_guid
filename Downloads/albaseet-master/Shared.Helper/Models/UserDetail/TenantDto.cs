using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helper.Models.UserDetail
{
	public class TenantDto
	{
		// The companyId coming from the user claim.
		public int CompanyId { get; set; }

		// The database name is constructed by appending the CompanyId to "albaseet".
		public string DatabaseName => $"albaseet{CompanyId}";
	}
}
