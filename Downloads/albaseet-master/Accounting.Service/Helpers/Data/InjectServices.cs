using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Accounting.CoreOne.Contracts;
using Accounting.CoreOne.Contracts.Reports;
using Accounting.CoreOne.Models.Domain;
using Shared.CoreOne;
using Shared.Repository;
using Accounting.Service.Services;
using Accounting.Service.Services.Reports;
using Shared.Service.Services.Accounts;
using Shared.CoreOne.Contracts.Taxes;
using Compound.CoreOne.Contracts.Reports.Accounting;
using Compound.CoreOne.Models.Dtos.Reports.Accounting;
using Shared.CoreOne.Contracts.FixedAssets;
using Shared.CoreOne.Models.Domain.FixedAssets;
using Shared.Service.Services.FixedAssets;


namespace Accounting.Service.Helpers.Data
{
    public static class InjectServices
    {
        public static IServiceCollection InjectAccountingServices(this IServiceCollection services, IConfiguration configuration)
        {

			#region Domain

			services.AddScoped<IReceiptVoucherHeaderService, ReceiptVoucherHeaderService>();
			services.AddScoped(typeof(IRepository<ReceiptVoucherHeader>), typeof(Repository<ReceiptVoucherHeader>));

			services.AddScoped<IReceiptVoucherDetailService, ReceiptVoucherDetailService>();
			services.AddScoped(typeof(IRepository<ReceiptVoucherDetail>), typeof(Repository<ReceiptVoucherDetail>));

			services.AddScoped<IReceiptVoucherService, ReceiptVoucherService>();

			services.AddScoped<IPaymentVoucherHeaderService, PaymentVoucherHeaderService>();
			services.AddScoped(typeof(IRepository<PaymentVoucherHeader>), typeof(Repository<PaymentVoucherHeader>));

			services.AddScoped<IPaymentVoucherDetailService, PaymentVoucherDetailService>();
			services.AddScoped(typeof(IRepository<PaymentVoucherDetail>), typeof(Repository<PaymentVoucherDetail>));

			services.AddScoped<IPaymentVoucherService, PaymentVoucherService>();

            #endregion

            #region Fixed Asset
            
            services.AddScoped<IFixedAssetMovementHeaderService, FixedAssetMovementHeaderService>();
            services.AddScoped(typeof(IRepository<FixedAssetMovementHeader>), typeof(Repository<FixedAssetMovementHeader>));

            services.AddScoped<IFixedAssetMovementDetailService, FixedAssetMovementDetailService>();
            services.AddScoped(typeof(IRepository<FixedAssetMovementDetail>), typeof(Repository<FixedAssetMovementDetail>));

            services.AddScoped<IFixedAssetMovementService, FixedAssetMovementService>();

            services.AddScoped<IFixedAssetVoucherHeaderService, FixedAssetVoucherHeaderService>();
            services.AddScoped(typeof(IRepository<FixedAssetVoucherHeader>), typeof(Repository<FixedAssetVoucherHeader>));

            services.AddScoped<IFixedAssetVoucherDetailService, FixedAssetVoucherDetailService>();
            services.AddScoped(typeof(IRepository<FixedAssetVoucherDetail>), typeof(Repository<FixedAssetVoucherDetail>));

            services.AddScoped<IFixedAssetVoucherDetailPaymentService, FixedAssetVoucherDetailPaymentService>();
            services.AddScoped(typeof(IRepository<FixedAssetVoucherDetailPayment>), typeof(Repository<FixedAssetVoucherDetailPayment>));

            services.AddScoped<IFixedAssetVoucherService, FixedAssetVoucherService>();
            #endregion

            #region Reports

            services.AddScoped<IJournalLedgerService, JournalLedgerService>();
			//services.AddScoped<ItrailBalanceService, TrailbalanceService>();



			//services.AddTransient<ItrailBalanceService, TrailBalanceService>();
			

			#endregion

			return services;
        }
    }
}
