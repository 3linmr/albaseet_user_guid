using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Purchases.CoreOne.Models.Domain;
using Shared.CoreOne.Models.Domain.Modules;

namespace Purchases.Repository
{
    public class PurchasesDbContext : DbContext
    {
        public PurchasesDbContext(DbContextOptions<PurchasesDbContext> options) : base(options)
        {

        }

        #region Dbsets

        public DbSet<ProductRequestHeader>? ProductRequestHeaders { get; set; }
        public DbSet<ProductRequestDetail>? ProductRequestDetails { get; set; }
        public DbSet<PurchaseExpenseType>? PurchaseExpenseTypes { get; set; }
        public DbSet<PurchaseOrderExpense>? PurchaseOrderExpenses { get; set; }
        public DbSet<ProductRequestPriceHeader>? ProductRequestPriceHeaders { get; set; }
        public DbSet<ProductRequestPriceDetail>? ProductRequestPriceDetails { get; set; }
        public DbSet<PurchaseOrderHeader>? PurchaseOrderHeaders { get; set; }
        public DbSet<PurchaseOrderDetail>? PurchaseOrderDetails { get; set; }
        public DbSet<PurchaseOrderDetailTax>? PurchaseOrderDetailTaxes { get; set; }
        public DbSet<StockInHeader>? StockInHeaders { get; set; }
        public DbSet<StockInDetail>? StockInDetails { get; set; }
        public DbSet<StockInDetailTax>? StockInDetailTaxes { get; set; }
        public DbSet<StockInReturnHeader>? StockInReturnHeaders { get; set; }
        public DbSet<StockInReturnDetail>? StockInReturnDetails { get; set; }
        public DbSet<StockInReturnReturnDetailTax>? StockInReturnDetailTaxes { get; set; }
        public DbSet<SupplierCreditNoticeHeader>? SupplierCreditNoticeHeaders { get; set; }
        public DbSet<SupplierCreditNoticeDetail>? SupplierCreditNoticeDetails { get; set; }
        public DbSet<SupplierCreditNoticeDetailTax>? SupplierCreditNoticeDetailTaxes { get; set; }
        public DbSet<SupplierDebitNoticeHeader>? SupplierDebitNoticeHeaders { get; set; }
        public DbSet<SupplierDebitNoticeDetail>? SupplierDebitNoticeDetails { get; set; }
        public DbSet<SupplierDebitNoticeDetailTax>? SupplierDebitNoticeDetailTaxes { get; set; }
        public DbSet<SupplierInvoiceHeader>? SupplierInvoiceHeaders { get; set; }
        public DbSet<SupplierInvoiceDetail>? SupplierInvoiceDetails { get; set; }
        public DbSet<SupplierInvoiceDetailTax>? SupplierInvoiceDetailTaxes { get; set; }
      
      

        #endregion



        protected override void OnModelCreating(ModelBuilder builder)
        {
            //Make All Relations Restrict
            foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }





            #region Data Seed


            //
            

            #endregion
        }
    }
}