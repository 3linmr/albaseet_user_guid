using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.API.Models;
using Shared.CoreOne.Contracts.Admin;

namespace Shared.API.Controllers.Admin
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        [HttpGet]
        [Route("GetHomeSetting")]
        public async Task<IActionResult> GetHomeSetting()
        {
            var data = await _homeService.GetHomeSetting();
            return Ok(data);
        }
    }
}
