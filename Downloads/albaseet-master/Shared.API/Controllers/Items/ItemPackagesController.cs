using DevExtreme.AspNet.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Shared.API.Models;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using Shared.Helper.Database;
using Shared.Helper.Models.Dtos;
using Shared.Service.Services.Modules;
using Shared.Helper.Identity;

namespace Shared.API.Controllers.Items
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ItemPackagesController : ControllerBase
    {
        private readonly IDatabaseTransaction _databaseTransaction;
        private readonly IStringLocalizer<HandelException> _exceptionLocalizer;
        private readonly IItemPackageService _itemPackageService;
        private readonly IItemBarCodeService _itemBarCodeService;
        private readonly IItemPackingService _itemPackingService;
		private readonly IHttpContextAccessor _httpContextAccessor;

        public ItemPackagesController(IDatabaseTransaction databaseTransaction, IStringLocalizer<HandelException> exceptionLocalizer, IItemPackageService itemPackageService,IItemBarCodeService itemBarCodeService,IItemPackingService itemPackingService, IHttpContextAccessor httpContextAccessor)
        {
            _databaseTransaction = databaseTransaction;
            _exceptionLocalizer = exceptionLocalizer;
            _itemPackageService = itemPackageService;
            _itemBarCodeService = itemBarCodeService;
            _itemPackingService = itemPackingService;
			_httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        [Route("ReadItemPackages")]
        public IActionResult ReadItemPackages(DataSourceLoadOptions loadOptions)
        {
            var data = DataSourceLoader.Load(_itemPackageService.GetCompanyItemPackages(), loadOptions);
            return Ok(data);
        }
        
        [HttpGet]
        [Route("ReadItemPackageDropDowns")]
        public IActionResult ReadItemPackageDropDowns(DataSourceLoadOptions loadOptions)
        {
            var data = DataSourceLoader.Load(_itemPackageService.GetItemPackagesDropDown(), loadOptions);
            return Ok(data);
        }

        [HttpGet]
        [Route("GetAllItemPackages")]
        public async Task<ActionResult<IEnumerable<ItemPackageDto>>> GetAllItemPackages()
        {
            var allItemPackages = await _itemPackageService.GetAllItemPackages().ToListAsync();
            return Ok(allItemPackages);
        }

        [HttpGet]
        [Route("GetItemPackagesDropDown")]
        public async Task<ActionResult<IEnumerable<ItemPackageDropDownDto>>> GetItemPackagesDropDown()
        {
            var allItemPackages = await _itemPackageService.GetItemPackagesDropDown().ToListAsync();
            return Ok(allItemPackages);
        }

        [HttpGet]
        [Route("GetItemPackagesTree")]
        public async Task<ActionResult<List<PackageTreeDto>>> GetItemPackagesTree(int itemId)
        {
	        var packages = await _itemBarCodeService.ItemPackagesTree(itemId);
	        return Ok(packages);
        }

		[HttpGet]
        [Route("GetItemSiblingPackages")]
        public async Task<ActionResult<IEnumerable<ItemPackageVm>>> GetItemSiblingPackages(int itemId, int packageId)
        {
            var packages = await _itemPackingService.GetItemSiblingPackages(itemId,packageId);
            return Ok(packages);
        }
        
        [HttpGet]
        [Route("GetItemPackagesLevel")]
        public async Task<ActionResult<IEnumerable<ItemPackageLevelDto>>> GetItemPackagesLevel(int itemId,int fromPackageId,int toPackageId)
        {
            var packages = await _itemBarCodeService.GetItemPackagesLevel(itemId,fromPackageId,toPackageId);
            return Ok(packages);
        }
        
        [HttpGet]
        [Route("GetItemSiblingPackagesDropDown")]
        public async Task<ActionResult<IEnumerable<ItemPackageDropDownDto>>> GetItemSiblingPackagesDropDown(int itemId, int packageId)
        {
            var packages = await _itemPackingService.GetItemSiblingPackagesDropDown(itemId,packageId);
            return Ok(packages);
        }
        
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetItemPackageById(int id)
        {
			var stateDb = await _itemPackageService.GetItemPackageById(id);
			var userCanLook = await _httpContextAccessor.UserCanLook(stateDb?.CompanyId ?? 0, 0);
            return Ok(userCanLook ? stateDb : new ItemPackageDto());
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] ItemPackageDto model)
        {
            ResponseDto response;
            try
            {
                await _databaseTransaction.BeginTransaction();
                response = await _itemPackageService.SaveItemPackage(model);
                if (response.Success)
                {
                    if (response.Success)
                    {
                        await _databaseTransaction.Commit();
                    }
                }
                else
                {
                    response.Success = false;
                    response.Message = response.Message;
                }
            }
            catch (Exception e)
            {
                await _databaseTransaction.Rollback();
                var handleException = new HandelException(_exceptionLocalizer);
                return Ok(handleException.Handle(e));
            }
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteItemPackage(int id)
        {
            ResponseDto response;
            try
            {
                await _databaseTransaction.BeginTransaction();
                response = await _itemPackageService.DeleteItemPackage(id);
                if (response.Success)
                {
	                await _databaseTransaction.Commit();
                }
                else
                {
	                await _databaseTransaction.Rollback();
                }
			}
            catch (Exception e)
            {
                await _databaseTransaction.Rollback();
                var handleException = new HandelException(_exceptionLocalizer);
                return Ok(handleException.Handle(e));
            }
            return Ok(response);
        }
    }
}
