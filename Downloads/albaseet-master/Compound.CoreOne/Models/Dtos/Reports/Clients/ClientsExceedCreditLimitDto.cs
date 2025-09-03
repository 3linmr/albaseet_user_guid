using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compound.CoreOne.Models.Dtos.Reports.Clients
{
    public class ClientsExceedCreditLimitDto
    {
        public int AccountId { get; set; }
        public string? AccountName { get; set; } // Customer's account name
        public int ClientId { get; set; }
        public int ClientCode { get; set; }
        public string? ClientName { get; set; }
        public string? LastInvoiceFullCode { get; set; } // Last issued invoice number
        public DateTime? LastInvoiceDate { get; set; } // Date of last issued invoice
        public decimal? LastInvoiceValue { get; set; } // Amount of the last invoice
        public string? LastReceiptVoucherFullCode { get; set; } // Last payment receipt number
        public decimal? LastReceiptVoucherValue { get; set; } // Last payment amount
        public decimal CreditLimitValues { get; set; } // Customer's credit limit
        public decimal Balance { get; set; } // Current balance of the customer
        public decimal OverdueAmount { get; set; } // Amount exceeding the credit limit
        //public int GracePeriodDays { get; set; } // Allowed grace period before late fees
        public DateTime? InvoiceDueDate { get; set; } // Invoice due date
        public int CreditLimitDays { get; set; } // Number of credit days allowed
        public int? DaysOverdue { get; set; } // Days overdue after grace period
        public int? InvoiceDuration { get; set; } //Days from date of invoice to due date
        //public string? TaxInfo { get; set; } // Tax-related information for the customer
        public DateTime? InvoiceDueDateFinal { get; set; }
        public int? SellerCode { get; set; }
        public string? SellerName { get; set; }

        public DateTime ContractDate { get; set; }
        public string? TaxCode { get; set; }
        public int? CountryId { get; set; }
        public string? CountryName { get; set; }
        public int? StateId { get; set; }
        public string? StateName { get; set; }
        public int? CityId { get; set; }
        public string? CityName { get; set; }
        public int? DistrictId { get; set; }
        public string? DistrictName { get; set; }
        public string? PostalCode { get; set; }
        public string? BuildingNumber { get; set; }
        public string? CommercialRegister { get; set; } //السجل التجاري
        public string? ClientStreet1 { get; set; }
        public string? ClientStreet2 { get; set; }
        public string? ClientAdditionalNumber { get; set; }
        public string? CountryCode { get; set; }
        public string? ClientAddress1 { get; set; }
        public string? ClientAddress2 { get; set; }
        public string? ClientAddress3 { get; set; }
        public string? ClientAddress4 { get; set; }
        public string? FirstResponsibleName { get; set; }
        public string? FirstResponsiblePhone { get; set; }
        public string? FirstResponsibleEmail { get; set; }
        public string? SecondResponsibleName { get; set; }
        public string? SecondResponsiblePhone { get; set; }
        public string? SecondResponsibleEmail { get; set; }
        public bool IsCredit { get; set; }
        public bool IsActive { get; set; }
        public string? InActiveReasons { get; set; }
        public string? IsActiveName { get; set; }

        public DateTime? CreatedAt { get; set; }
        public string? UserNameCreated { get; set; }
        public DateTime? ModifiedAt { get; set; }
        public string? UserNameModified { get; set; }
    }
}
