using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.CoreOne.Models.Domain.Menus;
using Shared.CoreOne.Models.Dtos.ViewModels.Menus;
using Shared.Helper.Models.UserDetail;

namespace Shared.CoreOne.Contracts.Menus
{
	public interface IMenuService : IBaseService<Menu>
	{
		Task<List<DocumentDto>> GetDocuments();
		Task<List<MenuCodeDto>> GetAllMenus();
		Task<MenuCodeDto?> GetMenuByMenuCode(int menuCode);
		Task<List<MenuCodeDropDownDto>> GetAllMenusDropDown();
		Task<List<MenuCodeDropDownDto>> GetMenusHasApprovesDropDown();
		Task<List<MenuCodeDropDownDto>> GetMenusHasNotesDropDown();
		Task<List<MenuCodeDropDownDto>> GetMenusHasEncodingDropDown();
		Task<List<MenuCodeDropDownDto>> GetMenusShippingStatusDropDown();
		Task<List<MenuCodeDropDownDto>> GetMenusInventoryDocumentsDropDown();
		Task<List<MenuCodeDropDownDto>> GetMenusItemTradingMovementDocumentsDropDown();
		Task<List<MenuCodeDropDownDto>> GetMenusInvoiceDocumentsDropDown();
		Task<List<MenuCodeDropDownDto>> GetMenusStoreCashIncomeDocumentsDropDown();
		Task<List<MenuCodeDropDownDto>> GetMenusPaymentMethodsIncomeDocumentsDropDown();
		Task<List<MenuCodeDropDownDto>> GetMenusMissingNumberDocumentsDropDown();
	}
}
