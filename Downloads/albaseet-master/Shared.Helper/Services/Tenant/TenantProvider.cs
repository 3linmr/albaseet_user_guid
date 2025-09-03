using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Helper.Models.UserDetail;

namespace Shared.Helper.Services.Tenant
{
	public class TenantProvider : ITenantProvider
	{
		public TenantDto CurrentTenant { get; set; }
	}
}
