using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Compound.Service.Services.Reports.Accounting
{
    public class BalanceSheetServices : IBalanceSheetServices
    {
        private readonly IAccountService accountService;

        public BalanceSheetServices(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        /// <summary>
        /// Generates a balance sheet report for a specific company and date.
        /// </summary>
        /// <param name="companyId">The ID of the company for which the balance sheet is generated.</param>
        /// <param name="asOfDate">The date for which the balance sheet is generated.</param>
        /// <returns>A <see cref="BalanceSheetDto"/> containing the assets, liabilities, and equity.</returns>
        public async Task<BalanceSheetDto> GenerateBalanceSheetAsync(int companyId, DateTime asOfDate)
        {
            // Fetch all accounts for the company
            var accounts = accountService.GetCompanyAccounts(companyId).ToList();

            // Separate accounts into assets, liabilities, and equity
            var assets = accounts.Where(a => a.AccountCategoryId == (byte)AccountCategory.Assets).ToList();
            var liabilities = accounts.Where(a => a.AccountCategoryId == (byte)AccountCategory.Liabilities).ToList();
            var equity = accounts.Where(a => a.AccountCategoryId == (byte)AccountCategory.Equity).ToList();

            // Calculate totals
            var totalAssets = assets.Sum(a => a.Balance);
            var totalLiabilities = liabilities.Sum(a => a.Balance);
            var totalEquity = equity.Sum(a => a.Balance);

            // Return the balance sheet
            return new BalanceSheetDto
            {
                Assets = assets.Select(a => new AccountBalanceDto
                {
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    Balance = a.Balance
                }).ToList(),
                Liabilities = liabilities.Select(a => new AccountBalanceDto
                {
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    Balance = a.Balance
                }).ToList(),
                Equity = equity.Select(a => new AccountBalanceDto
                {
                    AccountId = a.AccountId,
                    AccountName = a.AccountName,
                    Balance = a.Balance
                }).ToList(),
                TotalAssets = totalAssets,
                TotalLiabilities = totalLiabilities,
                TotalEquity = totalEquity
            };
        }
    }

    public interface IBalanceSheetServices
    {
        /// <summary>
        /// Generates a balance sheet report for a specific company and date.
        /// </summary>
        /// <param name="companyId">The ID of the company for which the balance sheet is generated.</param>
        /// <param name="asOfDate">The date for which the balance sheet is generated.</param>
        /// <returns>A <see cref="BalanceSheetDto"/> containing the assets, liabilities, and equity.</returns>
        Task<BalanceSheetDto> GenerateBalanceSheetAsync(int companyId, DateTime asOfDate);
    }

    /// <summary>
    /// Represents the balance sheet data.
    /// </summary>
    public class BalanceSheetDto
    {
        public List<AccountBalanceDto> Assets { get; set; } = new();
        public List<AccountBalanceDto> Liabilities { get; set; } = new();
        public List<AccountBalanceDto> Equity { get; set; } = new();
        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal TotalEquity { get; set; }
    }

    /// <summary>
    /// Represents the balance of an account.
    /// </summary>
    public class AccountBalanceDto
    {
        public int AccountId { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }

    /// <summary>
    /// Represents account categories for classification.
    /// </summary>
    public enum AccountCategory
    {
        Assets = 1,
        Liabilities = 2,
        Equity = 3
    }
}
