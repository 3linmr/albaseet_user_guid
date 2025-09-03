using Compound.CoreOne.Contracts.Print;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Compound.API.Controllers.Print
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class PrintInvoicesController(IPrintInvoiceService printInvoiceService) : ControllerBase
	{
		[HttpGet]
		[Route("GetSimplifiedInvoice")]
		public async Task<IActionResult> GetSimplifiedInvoice(int salesInvoiceHeaderId)
		{
			return Ok(await printInvoiceService.GetSimplifiedInvoice(salesInvoiceHeaderId));
		}
	}

}
