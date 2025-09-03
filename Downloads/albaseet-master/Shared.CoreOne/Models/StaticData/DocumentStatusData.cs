using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.CoreOne.Models.StaticData
{
    public static class DocumentStatusData
    {
        public const int PurchaseOrderCreated = 1;
        public const int QuantityPartiallyReceived = 2;
        public const int QuantityFullyReceived = 3;
        public const int PurchaseInvoiceCreated = 4;

        public const int PurchaseInvoiceCreatedWaitingReceive = 7;
        public const int PartialQuantityReturnedWaitingPurchaseInvoiceReturn = 8;
        public const int FullQuantityReturnedWaitingPurchaseInvoiceReturn = 9;
        public const int PurchaseInvoiceReturnCreated = 10;

        public const int ProformaInvoiceCreated = 13;
        public const int QuantityPartiallyDisbursed = 14;
        public const int QuantityFullyDisbursed = 15;
        public const int SalesInvoiceCreated = 16;

        public const int PartialDisbursedQuantityReturnedWaitingSalesReturnInvoice = 19;
        public const int EntireQuantityDisbursedReturnedWaitingSalesReturnInvoice = 20;
        public const int SalesReturnInvoiceCreated = 21;

        public const int SupplierDebitMemoCreated = 24;
        public const int SupplierCreditMemoCreated = 25;
        public const int ClientDebitMemoCreated = 26;
        public const int ClientCreditMemoCreated = 27;
    }
}
