using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Inventory.CoreOne.Contracts;
using Shared.CoreOne;
using Shared.Repository;
using Inventory.Service.Services;
using Inventory.CoreOne.Models.Domain;

namespace Inventory.Service.Helpers.Data
{
    public static class InjectServices
    {
        public static IServiceCollection InjectInventoryServices(this IServiceCollection services, IConfiguration configuration)
        {
	        //Stock Taking
            services.AddScoped<IStockTakingHeaderService, StockTakingHeaderService>();
	        services.AddScoped(typeof(IRepository<StockTakingHeader>), typeof(Repository<StockTakingHeader>));

	        services.AddScoped<IStockTakingDetailService, StockTakingDetailService>();
	        services.AddScoped(typeof(IRepository<StockTakingDetail>), typeof(Repository<StockTakingDetail>));

	        services.AddScoped<IStockTakingService, StockTakingService>();

            //InventoryIn
            services.AddScoped<IInventoryInHeaderService, InventoryInHeaderService>();
            services.AddScoped(typeof(IRepository<InventoryInHeader>), typeof(Repository<InventoryInHeader>));

            services.AddScoped<IInventoryInDetailService, InventoryInDetailService>();
            services.AddScoped(typeof(IRepository<InventoryInDetail>),typeof(Repository<InventoryInDetail>));

            services.AddScoped<IInventoryInService, InventoryInService>();

            //InventoryOut
            services.AddScoped<IInventoryOutHeaderService, InventoryOutHeaderService>();
            services.AddScoped(typeof(IRepository<InventoryOutHeader>), typeof(Repository<InventoryOutHeader>));

            services.AddScoped<IInventoryOutDetailService, InventoryOutDetailService>();
            services.AddScoped(typeof(IRepository<InventoryOutDetail>), typeof(Repository<InventoryOutDetail>));

            services.AddScoped<IInventoryOutService, InventoryOutService>();

            //InternalTransfer
            services.AddScoped<IInternalTransferHeaderService, InternalTransferHeaderService>();
            services.AddScoped(typeof(IRepository<InternalTransferHeader>), typeof(Repository<InternalTransferHeader>));

            services.AddScoped<IInternalTransferDetailService, InternalTransferDetailService>();
            services.AddScoped(typeof(IRepository<InternalTransferDetail>), typeof(Repository<InternalTransferDetail>));

            services.AddScoped<IInternalTransferService, InternalTransferService>();

            //InternalTransferReceive
            services.AddScoped<IInternalTransferReceiveHeaderService, InternalTransferReceiveHeaderService>();
            services.AddScoped(typeof(IRepository<InternalTransferReceiveHeader>), typeof(Repository<InternalTransferReceiveHeader>));

            services.AddScoped<IInternalTransferReceiveDetailService, InternalTransferReceiveDetailService>();
            services.AddScoped(typeof(IRepository<InternalTransferReceiveDetail>), typeof(Repository<InternalTransferReceiveDetail>));

            services.AddScoped<IInternalTransferReceiveService, InternalTransferReceiveService>();

            //DirectInternalTransferService
            services.AddScoped<IDirectInternalTransferService, DirectInternalTransferService>();

            services.AddScoped<IStockTakingCarryOverHeaderService, StockTakingCarryOverHeaderService>();
	        services.AddScoped(typeof(IRepository<StockTakingCarryOverHeader>), typeof(Repository<StockTakingCarryOverHeader>));

	        services.AddScoped<IStockTakingCarryOverDetailService, StockTakingCarryOverDetailService>();
	        services.AddScoped(typeof(IRepository<StockTakingCarryOverDetail>), typeof(Repository<StockTakingCarryOverDetail>));

	        services.AddScoped<IStockTakingCarryOverEffectDetailService, StockTakingCarryOverEffectDetailService>();
	        services.AddScoped(typeof(IRepository<StockTakingCarryOverEffectDetail>), typeof(Repository<StockTakingCarryOverEffectDetail>));

			services.AddScoped<IStockTakingCarryOverService, StockTakingCarryOverService>();

			return services;
        }
    }
}