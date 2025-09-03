using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Purchases.CoreOne.Contracts
{
    public interface IPurchaseOrderStatusService
    {
        /// <summary>
        ///     Update the status of Purchase Order Header Id
        /// </summary>
        /// <param name="purchaseOrderHeaderId"> The Id of the Purchase Order whose status should be updated</param>
        /// <param name="documentStatusId"> The status value that the Purchase Order should contain. To automatically determine the correct status, set this to -1 </param>
        /// <param name="menuCode"> The menuCode of the document that is being saved or deleted, it is not required but only used as optimization</param>
        Task UpdatePurchaseOrderStatus(int purchaseOrderHeaderId, int documentStatusId, int menuCode);
    }
}
