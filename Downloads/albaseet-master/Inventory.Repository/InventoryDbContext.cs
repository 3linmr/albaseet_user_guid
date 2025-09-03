using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Inventory.CoreOne.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Repository
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options)
        {

        }

        #region Dbsets

        public DbSet<InternalTransferHeader>? InternalTransferHeaders { get; set; }
        public DbSet<InternalTransferDetail>? InternalTransferDetails { get; set; }
        public DbSet<ItemCost>? ItemCosts { get; set; }
        public DbSet<ItemCostHistory>? ItemCostHistories { get; set; }
        public DbSet<ItemCurrentBalance>? ItemCurrentBalances { get; set; }
        public DbSet<ItemCurrentBalanceHistory>? ItemCurrentBalanceHistories { get; set; }
        public DbSet<ItemImportExcel>? ItemImportExcels { get; set; }
        public DbSet<ItemImportExcelHistory>? ItemImportExcelHistories { get; set; }
        public DbSet<ItemOpenBalance>? ItemOpenBalances { get; set; }
        public DbSet<ItemOpenBalanceHistory>? ItemOpenBalanceHistories { get; set; }
    
        public DbSet<StockInInternalTransferDetail>? StockInInternalTransferDetails { get; set; }
        public DbSet<StockInInternalTransferHeader>? StockInInternalTransferHeaders { get; set; }
        public DbSet<StockTakingHeader>? StockTakingHeaders { get; set; }
        public DbSet<StockTakingDetail>? StockTakingDetails { get; set; }
        public DbSet<InventoryInHeader>? InventoryInHeaders { get; set; }
        public DbSet<InventoryInDetail>? InventoryInDetails { get; set; }
        public DbSet<InventoryOutHeader>? InventoryOutHeaders { get; set; }
        public DbSet<InventoryOutDetail>? InventoryOutDetails { get; set; }

      

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