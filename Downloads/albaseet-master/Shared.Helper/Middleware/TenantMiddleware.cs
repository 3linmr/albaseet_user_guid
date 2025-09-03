// Middleware/TenantMiddleware.cs

using Microsoft.AspNetCore.Http;
using Shared.Helper.Identity;
using Shared.Helper.Models.UserDetail;
using Shared.Helper.Services.Tenant;

namespace Shared.Helper.Middleware;

public class TenantMiddleware(RequestDelegate next)
{
	public async Task Invoke(HttpContext context, ITenantProvider tenantProvider)
	{
		if (context.User.Identity!.IsAuthenticated)
		{
			// Look for the "companyId" claim. Adjust the claim name if necessary.
			var companyIdClaim = context.User.FindFirst("company")?.Value != null || context.User.FindFirst("company")?.Value == "0" ? context.User.FindFirst("company")?.Value : null;
			var company = companyIdClaim ?? await IdentityHelper.GetCompanyId(Convert.ToInt32(companyIdClaim));
			if (company != null && int.TryParse(company, out int companyId))
			{
				tenantProvider.CurrentTenant = new TenantDto() { CompanyId = companyId };
			}
		}

		// Continue with the request pipeline.
		await next(context);
	}
}