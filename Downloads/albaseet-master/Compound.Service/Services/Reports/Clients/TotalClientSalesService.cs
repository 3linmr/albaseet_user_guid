using Compound.CoreOne.Contracts.Reports.Clients;
using Compound.CoreOne.Models.Dtos.Reports.Clients;
using Microsoft.AspNetCore.Http;
using Sales.CoreOne.Contracts;
using Shared.CoreOne.Contracts.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.Service.Services.Reports.Clients
{
    public class TotalClientSalesService : ITotalClientSalesService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISalesInvoiceHeaderService _salesInvoiceHeaderService;
        private readonly ISalesInvoiceReturnHeaderService _salesInvoiceReturnHeaderService;
        private readonly IClientCreditMemoService _clientCreditMemoService;
        private readonly IClientDebitMemoService _clientDebitMemoService;
        private readonly IStoreService _storeService;
        private readonly IBranchService _branchService;
        private readonly IClientService _clientService;

        public TotalClientSalesService(IHttpContextAccessor httpContextAccessor, ISalesInvoiceHeaderService salesInvoiceHeaderService, ISalesInvoiceReturnHeaderService salesInvoiceReturnHeaderService, IClientCreditMemoService clientCreditMemoService, IClientDebitMemoService clientDebitMemoService, IStoreService storeService, IBranchService branchService, IClientService clientService)
        {
            _httpContextAccessor = httpContextAccessor;
            _salesInvoiceHeaderService = salesInvoiceHeaderService;
            _salesInvoiceReturnHeaderService = salesInvoiceReturnHeaderService;
            _clientCreditMemoService = clientCreditMemoService;
            _clientDebitMemoService = clientDebitMemoService;
            _storeService = storeService;
            _branchService = branchService;
            _clientService = clientService;
        }

        public IQueryable<TopSellingClientsReportValuesDto> GetTotalClientSales( DateTime? fromDate, DateTime? toDate, bool includeStore)
        {
            var salesInvoices = from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll()
                                where (fromDate == null || salesInvoiceHeader.DocumentDate >= fromDate) && (toDate == null || salesInvoiceHeader.DocumentDate <= toDate)
                                select new TopSellingClientsReportValuesDto
                                {
                                    StoreId = includeStore ? salesInvoiceHeader.StoreId : 0,
                                    ClientId = salesInvoiceHeader.ClientId,
                                    CashClientName = salesInvoiceHeader.ClientName,
                                    GrossValue = salesInvoiceHeader.GrossValue,
                                    CostValue = salesInvoiceHeader.TotalCostValue,
                                    InvoiceCount=1
                                };

            var salesInvoiceReturns = from salesInvoiceReturnHeader in _salesInvoiceReturnHeaderService.GetAll()
                                      where (fromDate == null || salesInvoiceReturnHeader.DocumentDate >= fromDate) && (toDate == null || salesInvoiceReturnHeader.DocumentDate <= toDate)
                                      select new TopSellingClientsReportValuesDto
                                      {
                                          StoreId = includeStore ? salesInvoiceReturnHeader.StoreId : 0,
                                          ClientId = salesInvoiceReturnHeader.ClientId,
                                          CashClientName = salesInvoiceReturnHeader.ClientName,
                                          GrossValue = -salesInvoiceReturnHeader.GrossValue,
                                          CostValue = -salesInvoiceReturnHeader.TotalCostValue,
                                          InvoiceCount = 0
                                      };

            var clientCreditMemos = from clientCreditMemo in _clientCreditMemoService.GetAll()
                                    from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == clientCreditMemo.SalesInvoiceHeaderId)
                                    where (fromDate == null || clientCreditMemo.DocumentDate >= fromDate) && (toDate == null || clientCreditMemo.DocumentDate <= toDate)
                                    select new TopSellingClientsReportValuesDto
                                    {
                                        StoreId = includeStore ? clientCreditMemo.StoreId : 0,
                                        ClientId = clientCreditMemo.ClientId,
                                        CashClientName = salesInvoiceHeader.ClientName,
                                        GrossValue = clientCreditMemo.MemoValue,
                                        CostValue = 0.0M,
                                        InvoiceCount=0
                                    };

            var clientDebitMemos = from clientDebitMemo in _clientDebitMemoService.GetAll()
                                   from salesInvoiceHeader in _salesInvoiceHeaderService.GetAll().Where(x => x.SalesInvoiceHeaderId == clientDebitMemo.SalesInvoiceHeaderId)
                                   where (fromDate == null || clientDebitMemo.DocumentDate >= fromDate) && (toDate == null || clientDebitMemo.DocumentDate <= toDate)
                                   select new TopSellingClientsReportValuesDto
                                   {
                                       StoreId = includeStore ? clientDebitMemo.StoreId : 0,
                                       ClientId = clientDebitMemo.ClientId,
                                       CashClientName = salesInvoiceHeader.ClientName,
                                       GrossValue = -clientDebitMemo.MemoValue,
                                       CostValue = 0.0M,
                                       InvoiceCount = 0
                                   };
            return salesInvoices.Concat(salesInvoiceReturns).Concat(clientCreditMemos).Concat(clientDebitMemos)
                .GroupBy(x => new { x.StoreId, x.ClientId, x.CashClientName }).Select(x => new TopSellingClientsReportValuesDto
                {
                    StoreId = x.Key.StoreId,
                    ClientId = x.Key.ClientId,
                    CashClientName = x.Key.CashClientName,
                    GrossValue = x.Sum(y => y.GrossValue),
                    CostValue = x.Sum(y => y.CostValue),
                    InvoiceCount = x.Sum(y => y.InvoiceCount)
                });
        }

     
    }
}
