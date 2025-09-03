using Shared.CoreOne.Models.Domain.Items;
using Shared.CoreOne.Models.Dtos.ViewModels.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Contracts.Items
{
    public interface IItemTaxService: IBaseService<ItemTax>
    {
        Task<List<ItemTaxDataDto>> GetItemTaxData(DateTime? currentDate = null);
        Task<List<ItemTaxDataDto>> GetItemTaxDataByItemId(int itemId, DateTime? currentDate = null);
        Task<List<ItemTaxDataDto>> GetItemTaxDataByItemIds(List<int> itemIds, DateTime? currentDate = null);
        Task<bool> SaveItemTaxes(List<ItemTaxDto> itemTaxes, int itemId);
        Task<bool> DeleteItemTaxesByItemId(int itemId);
    }
}
