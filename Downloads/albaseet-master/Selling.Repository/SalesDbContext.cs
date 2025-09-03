using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Sales.CoreOne.Models.Domain;

namespace Sales.Repository
{
    public class SalesDbContext : DbContext
    {
        public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options)
        {

        }

        #region Dbsets

        public DbSet<ClientCreditNoticeHeader>? ClientCreditNoticeHeaders { get; set; }
        public DbSet<ClientCreditNoticeDetail>? ClientCreditNoticeDetails { get; set; }
        public DbSet<ClientCreditNoticeDetailTax>? ClientCreditNoticeDetailTaxes { get; set; }
        public DbSet<ClientDebitNoticeHeader>? ClientDebitNoticeHeaders { get; set; }
        public DbSet<ClientDebitNoticeDetail>? ClientDebitNoticeDetails { get; set; }
        public DbSet<ClientDebitNoticeDetailTax>? ClientDebitNoticeDetailTaxes { get; set; }
        public DbSet<ClientInvoiceHeader>? ClientInvoiceHeaders { get; set; }
        public DbSet<ClientInvoiceDetail>? ClientInvoiceDetails { get; set; }
        public DbSet<ClientInvoiceDetailTax>? ClientInvoiceDetailTaxes { get; set; }
        public DbSet<ProductReceiveHeader>? ProductReceiveHeaders { get; set; }
        public DbSet<ProductReceiveDetail>? ProductReceiveDetails { get; set; }
        public DbSet<ProductReceivePriceHeader>? ProductReceivePriceHeaders { get; set; }
        public DbSet<ProductReceivePriceDetail>? ProductReceivePriceDetails { get; set; }
        public DbSet<SellingOrderHeader>? SellingOrderHeaders { get; set; }
        public DbSet<SellingOrderDetail>? SellingOrderDetails { get; set; }
        public DbSet<SellingOrderDetailTax>? SellingOrderDetailTaxes { get; set; }
        public DbSet<StockOutHeader>? StockOutHeaders { get; set; }
        public DbSet<StockOutDetail>? StockOutDetails { get; set; }
        public DbSet<StockOutDetailTax>? StockOutDetailTaxes { get; set; }
        public DbSet<StockOutReturnHeader>? StockOutReturnHeaders { get; set; }
        public DbSet<StockOutReturnDetail>? StockOutReturnDetails { get; set; }
        public DbSet<StockOutReturnDetailTax>? StockOutReturnDetailTaxes { get; set; }
      
      

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