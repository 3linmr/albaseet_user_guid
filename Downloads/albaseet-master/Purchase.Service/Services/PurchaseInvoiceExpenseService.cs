using Purchases.CoreOne.Contracts;
using Purchases.CoreOne.Models.Domain;
using Purchases.CoreOne.Models.Dtos.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Shared.CoreOne;
using Shared.CoreOne.Contracts.Items;
using Shared.Helper.Identity;
using Shared.Service;
using static Shared.CoreOne.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.CostCenters;
using Shared.Service.Logic.Calculation;
using System.Security.Cryptography.X509Certificates;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.Text.Json;
using Shared.Helper.Extensions;
using static Shared.Helper.Models.StaticData.LanguageData;
using Shared.CoreOne.Contracts.Basics;
using Shared.Helper.Logic;

namespace Purchases.Service.Services
{
    public class PurchaseInvoiceExpenseService : BaseService<PurchaseInvoiceExpense>, IPurchaseInvoiceExpenseService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IInvoiceExpenseTypeService _invoiceExpenseTypeService;

        public PurchaseInvoiceExpenseService(IRepository<PurchaseInvoiceExpense> repository, IHttpContextAccessor httpContextAccessor, IInvoiceExpenseTypeService invoiceExpenseTypeService) : base(repository)
        {
            _httpContextAccessor = httpContextAccessor;
            _invoiceExpenseTypeService = invoiceExpenseTypeService;
        }

        public async Task<List<PurchaseInvoiceExpenseDto>> GetPurchaseInvoiceExpenses(int purchaseInvoiceHeaderId)
        {
            var language = _httpContextAccessor.GetProgramCurrentLanguage();

            var purchaseInvoiceExpenses =
                await (from purchaseInvoiceExpense in _repository.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId)
                       from invoiceExpenseType in _invoiceExpenseTypeService.GetAll().Where(x => x.InvoiceExpenseTypeId == purchaseInvoiceExpense.InvoiceExpenseTypeId)
                       select new PurchaseInvoiceExpenseDto
                       {
                           PurchaseInvoiceExpenseId = purchaseInvoiceExpense.PurchaseInvoiceExpenseId,
                           PurchaseInvoiceHeaderId = purchaseInvoiceExpense.PurchaseInvoiceHeaderId,
                           InvoiceExpenseTypeId = purchaseInvoiceExpense.InvoiceExpenseTypeId,
                           InvoiceExpenseTypeName = language == LanguageCode.Arabic ? invoiceExpenseType.InvoiceExpenseTypeNameAr : invoiceExpenseType.InvoiceExpenseTypeNameEn,
                           ExpenseValue = purchaseInvoiceExpense.ExpenseValue,
                           RemarksAr = purchaseInvoiceExpense.RemarksAr,
                           RemarksEn = purchaseInvoiceExpense.RemarksEn,
                       }).ToListAsync();

            return purchaseInvoiceExpenses;
        }

        public async Task<bool> SavePurchaseInvoiceExpenses(int purchaseInvoiceHeaderId, List<PurchaseInvoiceExpenseDto> purchaseInvoiceExpenses)
        {
            await DeletePurchaseInvoiceExpenses(purchaseInvoiceExpenses, purchaseInvoiceHeaderId);
            if (purchaseInvoiceExpenses.Any())
            {
                await AddPurchaseInvoiceExpenses(purchaseInvoiceExpenses, purchaseInvoiceHeaderId);
                await EditPurchaseInvoiceExpenses(purchaseInvoiceExpenses);
                return true;
            }
            return false;
        }

        public async Task<bool> DeletePurchaseInvoiceExpenses(int purchaseInvoiceHeaderId)
        {
            var data = await _repository.GetAll().Where(x => x.PurchaseInvoiceHeaderId == purchaseInvoiceHeaderId).ToListAsync();
            if (data.Any())
            {
                _repository.RemoveRange(data);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> DeletePurchaseInvoiceExpenses(List<PurchaseInvoiceExpenseDto> expenses, int headerId)
        {
            var current = _repository.GetAll().Where(x => x.PurchaseInvoiceHeaderId == headerId).AsNoTracking().ToList();
            var toBeDeleted = current.Where(p => expenses.All(p2 => p2.PurchaseInvoiceExpenseId != p.PurchaseInvoiceExpenseId)).ToList();
            if (toBeDeleted.Any())
            {
                _repository.RemoveRange(toBeDeleted);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> AddPurchaseInvoiceExpenses(List<PurchaseInvoiceExpenseDto> expenses, int headerId)
        {
            var current = expenses.Where(x => x.PurchaseInvoiceExpenseId <= 0).ToList();
            var modelList = new List<PurchaseInvoiceExpense>();
            var newId = await GetNextId();
            foreach (var expense in current)
            {
                var model = new PurchaseInvoiceExpense()
                {
                    PurchaseInvoiceExpenseId = newId,
                    PurchaseInvoiceHeaderId = headerId,
                    InvoiceExpenseTypeId = expense.InvoiceExpenseTypeId,
                    ExpenseValue = expense.ExpenseValue,
                    RemarksAr = expense.RemarksAr,
                    RemarksEn = expense.RemarksEn,

                    Hide = false,
                    CreatedAt = DateHelper.GetDateTimeNow(),
                    UserNameCreated = await _httpContextAccessor!.GetUserName(),
                    IpAddressCreated = _httpContextAccessor?.GetIpAddress(),
                };

                modelList.Add(model);
                newId++;
            }

            if (modelList.Any())
            {
                await _repository.InsertRange(modelList);
                await _repository.SaveChanges();
                return true;
            }
            return false;
        }

        private async Task<bool> EditPurchaseInvoiceExpenses(List<PurchaseInvoiceExpenseDto> purchaseInvoiceExpenses)
        {
            var current = purchaseInvoiceExpenses.Where(x => x.PurchaseInvoiceExpenseId > 0).ToList();
            var modelList = new List<PurchaseInvoiceExpense>();
            foreach (var expense in current)
            {
                var model = new PurchaseInvoiceExpense()
                {
                    PurchaseInvoiceExpenseId = expense.PurchaseInvoiceExpenseId,
                    PurchaseInvoiceHeaderId = expense.PurchaseInvoiceHeaderId,
                    InvoiceExpenseTypeId = expense.InvoiceExpenseTypeId,
                    ExpenseValue = expense.ExpenseValue,
                    RemarksAr = expense.RemarksAr,
                    RemarksEn = expense.RemarksEn,

                    ModifiedAt = DateHelper.GetDateTimeNow(),
                    UserNameModified = await _httpContextAccessor!.GetUserName(),
                    IpAddressModified = _httpContextAccessor?.GetIpAddress(),
                    Hide = false
                };


                modelList.Add(model);
            }

            if (modelList.Any())
            {
                _repository.UpdateRange(modelList);
                await _repository.SaveChanges();
                return true;
            }
            return false;

        }

        private async Task<int> GetNextId()
        {
            int id = 1;
            try { id = await _repository.GetAll().MaxAsync(a => a.PurchaseInvoiceExpenseId) + 1; } catch { id = 1; }
            return id;
        }

    }
}
