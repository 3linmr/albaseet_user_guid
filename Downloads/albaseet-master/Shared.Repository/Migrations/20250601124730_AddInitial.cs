using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shared.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AccountLedgers",
                columns: table => new
                {
                    AccountLedgerId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    AccountLedgerNameAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountLedgerNameEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLedgers", x => x.AccountLedgerId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AccountStoreTypes",
                columns: table => new
                {
                    AccountStoreTypeId = table.Column<int>(type: "int", nullable: false),
                    AccountStoreTypeNameAr = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountStoreTypeNameEn = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStoreTypes", x => x.AccountStoreTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AccountTypes",
                columns: table => new
                {
                    AccountTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    AccountTypeNameAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountTypeNameEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsInternalSystem = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountTypes", x => x.AccountTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApplicationFlagTabs",
                columns: table => new
                {
                    ApplicationFlagTabId = table.Column<int>(type: "int", nullable: false),
                    ApplicationFlagTabNameAr = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApplicationFlagTabNameEn = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<short>(type: "smallint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationFlagTabs", x => x.ApplicationFlagTabId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApplicationFlagTypes",
                columns: table => new
                {
                    ApplicationFlagTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ApplicationFlagTypeNameAr = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApplicationFlagTypeNameEn = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationFlagTypes", x => x.ApplicationFlagTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApproveRequestTypes",
                columns: table => new
                {
                    ApproveRequestTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    ApproveRequestTypeNameAr = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApproveRequestTypeNameEn = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproveRequestTypes", x => x.ApproveRequestTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ColumnIdentifiers",
                columns: table => new
                {
                    ColumnIdentifierId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ColumnIdentifierNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ColumnIdentifierNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColumnIdentifiers", x => x.ColumnIdentifierId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    CurrencyNameAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CurrencyNameEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsoCode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Symbol = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FractionalUnitAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FractionalUnitEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumberToBasic = table.Column<short>(type: "smallint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.CurrencyId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DocumentStatuses",
                columns: table => new
                {
                    DocumentStatusId = table.Column<int>(type: "int", nullable: false),
                    DocumentStatusNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentStatusNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentStatuses", x => x.DocumentStatusId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "DocumentTypes",
                columns: table => new
                {
                    DocumentTypeId = table.Column<short>(type: "smallint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    DocumentTypeNameAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentTypeNameEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTypes", x => x.DocumentTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EntityTypes",
                columns: table => new
                {
                    EntityTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    EntityTypeNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityTypeNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntityTypes", x => x.EntityTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FixedAssetVoucherTypes",
                columns: table => new
                {
                    FixedAssetVoucherTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    FixedAssetVoucherTypeNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FixedAssetVoucherTypeNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedAssetVoucherTypes", x => x.FixedAssetVoucherTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InvoiceTypes",
                columns: table => new
                {
                    InvoiceTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    InvoiceTypeNameAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InvoiceTypeNameEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceTypes", x => x.InvoiceTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemCostCalculationType",
                columns: table => new
                {
                    ItemCostCalculationTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    ItemCostCalculationTypeNameAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ItemCostCalculationTypeNameEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCostCalculationType", x => x.ItemCostCalculationTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemTypes",
                columns: table => new
                {
                    ItemTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ItemTypeNameAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ItemTypeNameEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTypes", x => x.ItemTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JournalTypes",
                columns: table => new
                {
                    JournalTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    JournalTypeNameAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JournalTypeNameEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalTypes", x => x.JournalTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    MenuCode = table.Column<short>(type: "smallint", nullable: false),
                    MenuNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MenuNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MenuUrl = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HasApprove = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HasNotes = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HasEncoding = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsFavorite = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsReport = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.MenuCode);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NotificationTypes",
                columns: table => new
                {
                    NotificationTypeId = table.Column<int>(type: "int", nullable: false),
                    NotificationTypeNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NotificationTypeNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AutomatedNotifyBeforeDays = table.Column<short>(type: "smallint", nullable: false),
                    NotifyAfterDays = table.Column<short>(type: "smallint", nullable: false),
                    IsHighPriority = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationTypes", x => x.NotificationTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaymentTypes",
                columns: table => new
                {
                    PaymentTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    PaymentTypeCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentTypeNameAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentTypeNameEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentTypes", x => x.PaymentTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReportPrintForms",
                columns: table => new
                {
                    ReportPrintFormId = table.Column<int>(type: "int", nullable: false),
                    ReportPrintFormNameAr = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReportPrintFormNameEn = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPrintForms", x => x.ReportPrintFormId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SellerCommissionTypes",
                columns: table => new
                {
                    SellerCommissionTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    SellerCommissionTypeNameAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SellerCommissionTypeNameEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerCommissionTypes", x => x.SellerCommissionTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SellerTypes",
                columns: table => new
                {
                    SellerTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    SellerTypeNameAr = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SellerTypeNameEn = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerTypes", x => x.SellerTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockTypes",
                columns: table => new
                {
                    StockTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    StockTypeNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StockTypeNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTypes", x => x.StockTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StoreClassifications",
                columns: table => new
                {
                    StoreClassificationId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ClassificationNameAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClassificationNameEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreClassifications", x => x.StoreClassificationId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SystemTasks",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false),
                    TaskNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaskNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsCompleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemTasks", x => x.TaskId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TaxTypes",
                columns: table => new
                {
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TaxTypeNameAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxTypeNameEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxTypes", x => x.TaxTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TransactionTypes",
                columns: table => new
                {
                    TransactionTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TransactionTypeNameAr = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TransactionTypeNameEn = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransactionTypes", x => x.TransactionTypeId);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AccountCategories",
                columns: table => new
                {
                    AccountCategoryId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    AccountLedgerId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    AccountCategoryNameAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountCategoryNameEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountCategories", x => x.AccountCategoryId);
                    table.ForeignKey(
                        name: "FK_AccountCategories_AccountLedgers_AccountLedgerId",
                        column: x => x.AccountLedgerId,
                        principalTable: "AccountLedgers",
                        principalColumn: "AccountLedgerId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApplicationFlagHeaders",
                columns: table => new
                {
                    ApplicationFlagHeaderId = table.Column<int>(type: "int", nullable: false),
                    ApplicationFlagTabId = table.Column<int>(type: "int", nullable: false),
                    ApplicationFlagHeaderNameAr = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApplicationFlagHeaderNameEn = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<short>(type: "smallint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationFlagHeaders", x => x.ApplicationFlagHeaderId);
                    table.ForeignKey(
                        name: "FK_ApplicationFlagHeaders_ApplicationFlagTabs_ApplicationFlagTa~",
                        column: x => x.ApplicationFlagTabId,
                        principalTable: "ApplicationFlagTabs",
                        principalColumn: "ApplicationFlagTabId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CompanyNameAr = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyNameEn = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    Phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WhatsApp = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Website = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LogoUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HeaderUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FooterUrl = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InActiveReasons = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                    table.ForeignKey(
                        name: "FK_Companies_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    CountryNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CountryNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CurrencyId = table.Column<short>(type: "smallint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.CountryId);
                    table.ForeignKey(
                        name: "FK_Countries_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CurrencyRates",
                columns: table => new
                {
                    CurrencyRateId = table.Column<int>(type: "int", nullable: false),
                    FromCurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    ToCurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    CurrencyRateValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrencyRates", x => x.CurrencyRateId);
                    table.ForeignKey(
                        name: "FK_CurrencyRates_Currencies_FromCurrencyId",
                        column: x => x.FromCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CurrencyRates_Currencies_ToCurrencyId",
                        column: x => x.ToCurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArchiveHeaders",
                columns: table => new
                {
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: false),
                    HeaderNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HeaderNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchiveHeaders", x => x.ArchiveHeaderId);
                    table.ForeignKey(
                        name: "FK_ArchiveHeaders_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NotificationHeaders",
                columns: table => new
                {
                    NotificationHeaderId = table.Column<int>(type: "int", nullable: false),
                    NotificationTypeId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Body = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FromUserName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsHighPriority = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    SendLater = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NotifyTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationHeaders", x => x.NotificationHeaderId);
                    table.ForeignKey(
                        name: "FK_NotificationHeaders_NotificationTypes_NotificationTypeId",
                        column: x => x.NotificationTypeId,
                        principalTable: "NotificationTypes",
                        principalColumn: "NotificationTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApplicationFlagDetails",
                columns: table => new
                {
                    ApplicationFlagDetailId = table.Column<int>(type: "int", nullable: false),
                    ApplicationFlagHeaderId = table.Column<int>(type: "int", nullable: false),
                    ApplicationFlagTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    FlagNameAr = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FlagNameEn = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FlagValue = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<short>(type: "smallint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationFlagDetails", x => x.ApplicationFlagDetailId);
                    table.ForeignKey(
                        name: "FK_ApplicationFlagDetails_ApplicationFlagHeaders_ApplicationFla~",
                        column: x => x.ApplicationFlagHeaderId,
                        principalTable: "ApplicationFlagHeaders",
                        principalColumn: "ApplicationFlagHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationFlagDetails_ApplicationFlagTypes_ApplicationFlagT~",
                        column: x => x.ApplicationFlagTypeId,
                        principalTable: "ApplicationFlagTypes",
                        principalColumn: "ApplicationFlagTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Approves",
                columns: table => new
                {
                    ApproveId = table.Column<int>(type: "int", nullable: false),
                    ApproveCode = table.Column<int>(type: "int", nullable: false),
                    ApplicationId = table.Column<int>(type: "int", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ApproveNameAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApproveNameEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OnAdd = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    OnEdit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    OnDelete = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsStopped = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Approves", x => x.ApproveId);
                    table.ForeignKey(
                        name: "FK_Approves_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Approves_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Branches",
                columns: table => new
                {
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    BranchCode = table.Column<int>(type: "int", nullable: false),
                    BranchNameAr = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BranchNameEn = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    BranchPhone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BranchWhatsApp = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BranchEmail = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BranchWebsite = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BranchAddress = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponsibleNameAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponsibleNameEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponsiblePhone = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponsibleWhatsApp = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponsibleEmail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponsibleAddress = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InActiveReasons = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Branches", x => x.BranchId);
                    table.ForeignKey(
                        name: "FK_Branches_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InvoiceExpenseTypes",
                columns: table => new
                {
                    InvoiceExpenseTypeId = table.Column<int>(type: "int", nullable: false),
                    InvoiceExpenseTypeCode = table.Column<int>(type: "int", nullable: false),
                    InvoiceExpenseTypeNameAr = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InvoiceExpenseTypeNameEn = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceExpenseTypes", x => x.InvoiceExpenseTypeId);
                    table.ForeignKey(
                        name: "FK_InvoiceExpenseTypes_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemAttributeTypes",
                columns: table => new
                {
                    ItemAttributeTypeId = table.Column<int>(type: "int", nullable: false),
                    ItemAttributeTypeCode = table.Column<int>(type: "int", nullable: false),
                    ItemAttributeTypeNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ItemAttributeTypeNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAttributeTypes", x => x.ItemAttributeTypeId);
                    table.ForeignKey(
                        name: "FK_ItemAttributeTypes_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemCategories",
                columns: table => new
                {
                    ItemCategoryId = table.Column<int>(type: "int", nullable: false),
                    ItemCategoryCode = table.Column<int>(type: "int", nullable: false),
                    CategoryNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CategoryNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCategories", x => x.ItemCategoryId);
                    table.ForeignKey(
                        name: "FK_ItemCategories_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemPackages",
                columns: table => new
                {
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    ItemPackageCode = table.Column<int>(type: "int", nullable: false),
                    PackageNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PackageNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PackageCode = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPackages", x => x.ItemPackageId);
                    table.ForeignKey(
                        name: "FK_ItemPackages_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MenuCompanies",
                columns: table => new
                {
                    MenuCompanyId = table.Column<int>(type: "int", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    MenuNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MenuNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsFavorite = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuCompanies", x => x.MenuCompanyId);
                    table.ForeignKey(
                        name: "FK_MenuCompanies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuCompanies_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MenuNoteIdentifiers",
                columns: table => new
                {
                    MenuNoteIdentifierId = table.Column<int>(type: "int", nullable: false),
                    MenuNoteIdentifierCode = table.Column<int>(type: "int", nullable: false),
                    MenuNoteIdentifierNameAr = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MenuNoteIdentifierNameEn = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ColumnIdentifierId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuNoteIdentifiers", x => x.MenuNoteIdentifierId);
                    table.ForeignKey(
                        name: "FK_MenuNoteIdentifiers_ColumnIdentifiers_ColumnIdentifierId",
                        column: x => x.ColumnIdentifierId,
                        principalTable: "ColumnIdentifiers",
                        principalColumn: "ColumnIdentifierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuNoteIdentifiers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuNoteIdentifiers_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReportPrintSettings",
                columns: table => new
                {
                    ReportPrintSettingId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: false),
                    InstitutionNameAr = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InstitutionNameEn = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InstitutionOtherNameAr = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InstitutionOtherNameEn = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address1Ar = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address1En = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address2Ar = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address2En = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address3Ar = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address3En = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TopNotesFirstPageOnlyAr = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TopNotesFirstPageOnlyEn = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BottomNotesLastPageOnlyAr = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BottomNotesLastPageOnlyEn = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TopNotesAllPagesAr = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TopNotesAllPagesEn = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BottomNotesAllPagesAr = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BottomNotesAllPagesEn = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrintBusinessName = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PrintDateTime = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PrintUserName = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PrintInstitutionLogo = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InstitutionNameFont = table.Column<int>(type: "int", nullable: false),
                    InstitutionOtherNameFont = table.Column<int>(type: "int", nullable: false),
                    Address1Font = table.Column<int>(type: "int", nullable: false),
                    Address2Font = table.Column<int>(type: "int", nullable: false),
                    Address3Font = table.Column<int>(type: "int", nullable: false),
                    TopNotesFirstPageOnlyFont = table.Column<int>(type: "int", nullable: false),
                    BottomNotesLastPageOnlyFont = table.Column<int>(type: "int", nullable: false),
                    TopNotesAllPagesFont = table.Column<int>(type: "int", nullable: false),
                    BottomNotesAllPagesFont = table.Column<int>(type: "int", nullable: false),
                    ReportNameFont = table.Column<int>(type: "int", nullable: false),
                    ReportPeriodFont = table.Column<int>(type: "int", nullable: false),
                    GridFont = table.Column<int>(type: "int", nullable: false),
                    BusinessFont = table.Column<int>(type: "int", nullable: false),
                    DetailRounding = table.Column<int>(type: "int", nullable: false),
                    SumRounding = table.Column<int>(type: "int", nullable: false),
                    PrintFormId = table.Column<int>(type: "int", nullable: false),
                    TopMargin = table.Column<int>(type: "int", nullable: false),
                    RightMargin = table.Column<int>(type: "int", nullable: false),
                    BottomMargin = table.Column<int>(type: "int", nullable: false),
                    LeftMargin = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportPrintSettings", x => x.ReportPrintSettingId);
                    table.ForeignKey(
                        name: "FK_ReportPrintSettings_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportPrintSettings_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportPrintSettings_ReportPrintForms_PrintFormId",
                        column: x => x.PrintFormId,
                        principalTable: "ReportPrintForms",
                        principalColumn: "ReportPrintFormId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SellerCommissionMethod",
                columns: table => new
                {
                    SellerCommissionMethodId = table.Column<short>(type: "smallint", nullable: false),
                    SellerCommissionMethodCode = table.Column<short>(type: "smallint", nullable: false),
                    SellerCommissionMethodNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SellerCommissionMethodNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SellerCommissionTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InActiveReasons = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerCommissionMethod", x => x.SellerCommissionMethodId);
                    table.ForeignKey(
                        name: "FK_SellerCommissionMethod_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SellerCommissionMethod_SellerCommissionTypes_SellerCommissio~",
                        column: x => x.SellerCommissionTypeId,
                        principalTable: "SellerCommissionTypes",
                        principalColumn: "SellerCommissionTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ShipmentTypes",
                columns: table => new
                {
                    ShipmentTypeId = table.Column<int>(type: "int", nullable: false),
                    ShipmentTypeCode = table.Column<int>(type: "int", nullable: false),
                    ShipmentTypeNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShipmentTypeNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentTypes", x => x.ShipmentTypeId);
                    table.ForeignKey(
                        name: "FK_ShipmentTypes_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ShippingStatuses",
                columns: table => new
                {
                    ShippingStatusId = table.Column<int>(type: "int", nullable: false),
                    ShippingStatusCode = table.Column<int>(type: "int", nullable: false),
                    ShippingStatusNameAr = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShippingStatusNameEn = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: false),
                    StatusOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InActiveReasons = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShippingStatuses", x => x.ShippingStatusId);
                    table.ForeignKey(
                        name: "FK_ShippingStatuses_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ShippingStatuses_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Vendors",
                columns: table => new
                {
                    VendorId = table.Column<int>(type: "int", nullable: false),
                    VendorCode = table.Column<int>(type: "int", nullable: false),
                    VendorNameAr = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VendorNameEn = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendors", x => x.VendorId);
                    table.ForeignKey(
                        name: "FK_Vendors_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    StateId = table.Column<int>(type: "int", nullable: false),
                    StateNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StateNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CountryId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.StateId);
                    table.ForeignKey(
                        name: "FK_States_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ArchiveDetails",
                columns: table => new
                {
                    ArchiveDetailId = table.Column<int>(type: "int", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileExtension = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileType = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileUrl = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArchiveDetails", x => x.ArchiveDetailId);
                    table.ForeignKey(
                        name: "FK_ArchiveDetails_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CostCenters",
                columns: table => new
                {
                    CostCenterId = table.Column<int>(type: "int", nullable: false),
                    CostCenterCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CostCenterNameAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CostCenterNameEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IsMainCostCenter = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MainCostCenterId = table.Column<int>(type: "int", nullable: true),
                    CostCenterLevel = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsLastLevel = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsPrivate = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InActiveReasons = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HasRemarks = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsNonEditable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NotesAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NotesEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenters", x => x.CostCenterId);
                    table.ForeignKey(
                        name: "FK_CostCenters_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostCenters_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemImportExcelHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    ItemCount = table.Column<int>(type: "int", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemImportExcelHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ItemImportExcelHistories_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "NotificationDetails",
                columns: table => new
                {
                    NotificationDetailId = table.Column<int>(type: "int", nullable: false),
                    NotificationHeaderId = table.Column<int>(type: "int", nullable: false),
                    ToUserName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsRead = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReadTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationDetails", x => x.NotificationDetailId);
                    table.ForeignKey(
                        name: "FK_NotificationDetails_NotificationHeaders_NotificationHeaderId",
                        column: x => x.NotificationHeaderId,
                        principalTable: "NotificationHeaders",
                        principalColumn: "NotificationHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApplicationFlagDetailCompanies",
                columns: table => new
                {
                    ApplicationFlagDetailCompanyId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ApplicationFlagDetailId = table.Column<int>(type: "int", nullable: false),
                    FlagValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationFlagDetailCompanies", x => x.ApplicationFlagDetailCompanyId);
                    table.ForeignKey(
                        name: "FK_ApplicationFlagDetailCompanies_ApplicationFlagDetails_Applic~",
                        column: x => x.ApplicationFlagDetailId,
                        principalTable: "ApplicationFlagDetails",
                        principalColumn: "ApplicationFlagDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApplicationFlagDetailCompanies_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApplicationFlagDetailSelects",
                columns: table => new
                {
                    ApplicationFlagDetailSelectId = table.Column<int>(type: "int", nullable: false),
                    ApplicationFlagDetailId = table.Column<int>(type: "int", nullable: false),
                    SelectId = table.Column<short>(type: "smallint", nullable: false),
                    SelectNameAr = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SelectNameEn = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Order = table.Column<short>(type: "smallint", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationFlagDetailSelects", x => x.ApplicationFlagDetailSelectId);
                    table.ForeignKey(
                        name: "FK_ApplicationFlagDetailSelects_ApplicationFlagDetails_Applicat~",
                        column: x => x.ApplicationFlagDetailId,
                        principalTable: "ApplicationFlagDetails",
                        principalColumn: "ApplicationFlagDetailId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApproveStep",
                columns: table => new
                {
                    ApproveStepId = table.Column<int>(type: "int", nullable: false),
                    ApproveId = table.Column<int>(type: "int", nullable: false),
                    StepNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StepNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ApproveOrder = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ApproveCount = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    AllCountShouldApprove = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsLastStep = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproveStep", x => x.ApproveStepId);
                    table.ForeignKey(
                        name: "FK_ApproveStep_Approves_ApproveId",
                        column: x => x.ApproveId,
                        principalTable: "Approves",
                        principalColumn: "ApproveId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemSubCategories",
                columns: table => new
                {
                    ItemSubCategoryId = table.Column<int>(type: "int", nullable: false),
                    ItemSubCategoryCode = table.Column<int>(type: "int", nullable: false),
                    ItemCategoryId = table.Column<int>(type: "int", nullable: false),
                    SubCategoryNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubCategoryNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSubCategories", x => x.ItemSubCategoryId);
                    table.ForeignKey(
                        name: "FK_ItemSubCategories_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemSubCategories_ItemCategories_ItemCategoryId",
                        column: x => x.ItemCategoryId,
                        principalTable: "ItemCategories",
                        principalColumn: "ItemCategoryId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MenuNotes",
                columns: table => new
                {
                    MenuNoteId = table.Column<int>(type: "int", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: false),
                    MenuNoteIdentifierId = table.Column<int>(type: "int", nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: false),
                    NoteValue = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Note = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShowInReports = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ShowOnPrint = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ShowOnSelection = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuNotes", x => x.MenuNoteId);
                    table.ForeignKey(
                        name: "FK_MenuNotes_MenuNoteIdentifiers_MenuNoteIdentifierId",
                        column: x => x.MenuNoteIdentifierId,
                        principalTable: "MenuNoteIdentifiers",
                        principalColumn: "MenuNoteIdentifierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuNotes_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SellerCommissions",
                columns: table => new
                {
                    SellerCommissionId = table.Column<int>(type: "int", nullable: false),
                    SellerCommissionMethodId = table.Column<short>(type: "smallint", nullable: false),
                    From = table.Column<int>(type: "int", nullable: false),
                    To = table.Column<int>(type: "int", nullable: false),
                    CommissionPercent = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SellerCommissions", x => x.SellerCommissionId);
                    table.ForeignKey(
                        name: "FK_SellerCommissions_SellerCommissionMethod_SellerCommissionMet~",
                        column: x => x.SellerCommissionMethodId,
                        principalTable: "SellerCommissionMethod",
                        principalColumn: "SellerCommissionMethodId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Sellers",
                columns: table => new
                {
                    SellerId = table.Column<int>(type: "int", nullable: false),
                    SellerCode = table.Column<int>(type: "int", nullable: false),
                    SellerTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    SellerNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SellerNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContractDate = table.Column<DateTime>(type: "Date", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    OutSourcingCompany = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SellerCommissionMethodId = table.Column<short>(type: "smallint", nullable: true),
                    Phone = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    WhatsApp = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientsDebitLimit = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InActiveReasons = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sellers", x => x.SellerId);
                    table.ForeignKey(
                        name: "FK_Sellers_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sellers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sellers_SellerCommissionMethod_SellerCommissionMethodId",
                        column: x => x.SellerCommissionMethodId,
                        principalTable: "SellerCommissionMethod",
                        principalColumn: "SellerCommissionMethodId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Sellers_SellerTypes_SellerTypeId",
                        column: x => x.SellerTypeId,
                        principalTable: "SellerTypes",
                        principalColumn: "SellerTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    CityId = table.Column<int>(type: "int", nullable: false),
                    CityNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CityNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StateId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.CityId);
                    table.ForeignKey(
                        name: "FK_Cities_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "StateId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    AccountCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    AccountNameAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountNameEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountCategoryId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    AccountTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: true),
                    IsMainAccount = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MainAccountId = table.Column<int>(type: "int", nullable: true),
                    AccountLevel = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsLastLevel = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    HasCostCenter = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    IsPrivate = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InActiveReasons = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HasRemarks = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsNonEditable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsNonDeletable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsCreatedAutomatically = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NotesAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NotesEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InternalReferenceAccountId = table.Column<int>(type: "int", nullable: true),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accounts", x => x.AccountId);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountCategories_AccountCategoryId",
                        column: x => x.AccountCategoryId,
                        principalTable: "AccountCategories",
                        principalColumn: "AccountCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_AccountTypes_AccountTypeId",
                        column: x => x.AccountTypeId,
                        principalTable: "AccountTypes",
                        principalColumn: "AccountTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Accounts_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApplicationFlagDetailImages",
                columns: table => new
                {
                    ApplicationFlagDetailImageId = table.Column<int>(type: "int", nullable: false),
                    ApplicationFlagDetailCompanyId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FileType = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Image = table.Column<byte[]>(type: "longblob", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationFlagDetailImages", x => x.ApplicationFlagDetailImageId);
                    table.ForeignKey(
                        name: "FK_ApplicationFlagDetailImages_ApplicationFlagDetailCompanies_A~",
                        column: x => x.ApplicationFlagDetailCompanyId,
                        principalTable: "ApplicationFlagDetailCompanies",
                        principalColumn: "ApplicationFlagDetailCompanyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApproveStatuses",
                columns: table => new
                {
                    ApproveStatusId = table.Column<int>(type: "int", nullable: false),
                    ApproveStepId = table.Column<int>(type: "int", nullable: false),
                    StatusNameAr = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StatusNameEn = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Approved = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Pending = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Rejected = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproveStatuses", x => x.ApproveStatusId);
                    table.ForeignKey(
                        name: "FK_ApproveStatuses_ApproveStep_ApproveStepId",
                        column: x => x.ApproveStepId,
                        principalTable: "ApproveStep",
                        principalColumn: "ApproveStepId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemSections",
                columns: table => new
                {
                    ItemSectionId = table.Column<int>(type: "int", nullable: false),
                    ItemSectionCode = table.Column<int>(type: "int", nullable: false),
                    ItemSubCategoryId = table.Column<int>(type: "int", nullable: false),
                    SectionNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SectionNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSections", x => x.ItemSectionId);
                    table.ForeignKey(
                        name: "FK_ItemSections_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemSections_ItemSubCategories_ItemSubCategoryId",
                        column: x => x.ItemSubCategoryId,
                        principalTable: "ItemSubCategories",
                        principalColumn: "ItemSubCategoryId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    DistrictId = table.Column<int>(type: "int", nullable: false),
                    DistrictNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DistrictNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CityId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.DistrictId);
                    table.ForeignKey(
                        name: "FK_Districts_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Banks",
                columns: table => new
                {
                    BankId = table.Column<int>(type: "int", nullable: false),
                    BankCode = table.Column<int>(type: "int", nullable: false),
                    BankNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BankNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    AccountNumber = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IBAN = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Fax = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Website = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponsibleName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponsiblePhone = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponsibleFax = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ResponsibleEmail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    VisaFees = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InActiveReasons = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banks", x => x.BankId);
                    table.ForeignKey(
                        name: "FK_Banks_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Banks_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Banks_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    ClientCode = table.Column<int>(type: "int", nullable: false),
                    ClientNameAr = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientNameEn = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContractDate = table.Column<DateTime>(type: "Date", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    CreditLimitDays = table.Column<int>(type: "int", nullable: false),
                    DebitLimitDays = table.Column<int>(type: "int", nullable: false),
                    CreditLimitValues = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: true),
                    EmployeeId = table.Column<int>(type: "int", nullable: true),
                    TaxCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    DistrictId = table.Column<int>(type: "int", nullable: true),
                    PostalCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BuildingNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CommercialRegister = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Street1 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Street2 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdditionalNumber = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CountryCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address1 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address2 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address3 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address4 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstResponsibleName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstResponsiblePhone = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstResponsibleEmail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecondResponsibleName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecondResponsiblePhone = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecondResponsibleEmail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsCredit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InActiveReasons = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientId);
                    table.ForeignKey(
                        name: "FK_Clients_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Clients_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "SellerId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FixedAssets",
                columns: table => new
                {
                    FixedAssetId = table.Column<int>(type: "int", nullable: false),
                    FixedAssetCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FixedAssetNameAr = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FixedAssetNameEn = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    IsMainFixedAsset = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MainFixedAssetId = table.Column<int>(type: "int", nullable: true),
                    FixedAssetLevel = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false),
                    IsLastLevel = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    DepreciationAccountId = table.Column<int>(type: "int", nullable: true),
                    CumulativeDepreciationAccountId = table.Column<int>(type: "int", nullable: true),
                    AnnualDepreciationPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    IsPrivate = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InActiveReasonsAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InActiveReasonsEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HasRemarks = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsNonEditable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NotesAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NotesEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedAssets", x => x.FixedAssetId);
                    table.ForeignKey(
                        name: "FK_FixedAssets_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssets_Accounts_CumulativeDepreciationAccountId",
                        column: x => x.CumulativeDepreciationAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssets_Accounts_DepreciationAccountId",
                        column: x => x.DepreciationAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssets_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssets_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    SupplierCode = table.Column<int>(type: "int", nullable: false),
                    SupplierNameAr = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SupplierNameEn = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ContractDate = table.Column<DateTime>(type: "Date", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: true),
                    CreditLimitDays = table.Column<int>(type: "int", nullable: false),
                    CreditLimitValues = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DebitLimitDays = table.Column<int>(type: "int", nullable: false),
                    TaxCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShipmentTypeId = table.Column<int>(type: "int", nullable: true),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    DistrictId = table.Column<int>(type: "int", nullable: true),
                    PostalCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BuildingNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CommercialRegister = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Street1 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Street2 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdditionalNumber = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CountryCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address1 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address2 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address3 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address4 = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstResponsibleName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstResponsiblePhone = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FirstResponsibleEmail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecondResponsibleName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecondResponsiblePhone = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SecondResponsibleEmail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsCredit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InActiveReasons = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierId);
                    table.ForeignKey(
                        name: "FK_Suppliers_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Suppliers_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Suppliers_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Suppliers_ShipmentTypes_ShipmentTypeId",
                        column: x => x.ShipmentTypeId,
                        principalTable: "ShipmentTypes",
                        principalColumn: "ShipmentTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Taxes",
                columns: table => new
                {
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    TaxCode = table.Column<int>(type: "int", nullable: false),
                    TaxNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    DrAccount = table.Column<int>(type: "int", nullable: true),
                    CrAccount = table.Column<int>(type: "int", nullable: true),
                    IsVatTax = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TaxAfterVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Taxes", x => x.TaxId);
                    table.ForeignKey(
                        name: "FK_Taxes_Accounts_CrAccount",
                        column: x => x.CrAccount,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Taxes_Accounts_DrAccount",
                        column: x => x.DrAccount,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Taxes_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Taxes_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemSubSections",
                columns: table => new
                {
                    ItemSubSectionId = table.Column<int>(type: "int", nullable: false),
                    ItemSubSectionCode = table.Column<int>(type: "int", nullable: false),
                    ItemSectionId = table.Column<int>(type: "int", nullable: false),
                    SubSectionNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SubSectionNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemSubSections", x => x.ItemSubSectionId);
                    table.ForeignKey(
                        name: "FK_ItemSubSections_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemSubSections_ItemSections_ItemSectionId",
                        column: x => x.ItemSectionId,
                        principalTable: "ItemSections",
                        principalColumn: "ItemSectionId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Stores",
                columns: table => new
                {
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    StoreNameAr = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreNameEn = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreClassificationId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    CountryId = table.Column<int>(type: "int", nullable: true),
                    StateId = table.Column<int>(type: "int", nullable: true),
                    CityId = table.Column<int>(type: "int", nullable: true),
                    DistrictId = table.Column<int>(type: "int", nullable: true),
                    PostalCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BuildingNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CommercialRegister = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Street1 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Street2 = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AdditionalNumber = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CountryCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address1 = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address2 = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address3 = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address4 = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StockDebitAccountId = table.Column<int>(type: "int", nullable: true),
                    StockCreditAccountId = table.Column<int>(type: "int", nullable: true),
                    ExpenseAccountId = table.Column<int>(type: "int", nullable: true),
                    Long = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Lat = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    GMap = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NoReplenishment = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsReservedStore = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReservedParentStoreId = table.Column<int>(type: "int", nullable: true),
                    InActiveReasons = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CanAcceptDirectInternalTransfer = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stores", x => x.StoreId);
                    table.ForeignKey(
                        name: "FK_Stores_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stores_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "CityId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stores_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "CountryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stores_Districts_DistrictId",
                        column: x => x.DistrictId,
                        principalTable: "Districts",
                        principalColumn: "DistrictId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stores_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "StateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Stores_StoreClassifications_StoreClassificationId",
                        column: x => x.StoreClassificationId,
                        principalTable: "StoreClassifications",
                        principalColumn: "StoreClassificationId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaymentMethods",
                columns: table => new
                {
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodCode = table.Column<int>(type: "int", nullable: false),
                    PaymentTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentMethodNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentAccountId = table.Column<int>(type: "int", nullable: false),
                    FixedCommissionValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CommissionPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    MinCommissionValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    MaxCommissionValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CommissionAccountId = table.Column<int>(type: "int", nullable: true),
                    TaxId = table.Column<int>(type: "int", nullable: true),
                    CommissionTaxAccountId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InActiveReasons = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsPaymentMethod = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsReceivingMethod = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FixedCommissionHasVat = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    FixedCommissionVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CommissionHasVat = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CommissionVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentMethods", x => x.PaymentMethodId);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_Accounts_CommissionAccountId",
                        column: x => x.CommissionAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_Accounts_CommissionTaxAccountId",
                        column: x => x.CommissionTaxAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_Accounts_PaymentAccountId",
                        column: x => x.PaymentAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_PaymentTypes_PaymentTypeId",
                        column: x => x.PaymentTypeId,
                        principalTable: "PaymentTypes",
                        principalColumn: "PaymentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentMethods_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TaxPercents",
                columns: table => new
                {
                    TaxPercentId = table.Column<int>(type: "int", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    FromDate = table.Column<DateTime>(type: "Date", nullable: false),
                    Percent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaxPercents", x => x.TaxPercentId);
                    table.ForeignKey(
                        name: "FK_TaxPercents_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MainItems",
                columns: table => new
                {
                    MainItemId = table.Column<int>(type: "int", nullable: false),
                    MainItemCode = table.Column<int>(type: "int", nullable: false),
                    ItemSubSectionId = table.Column<int>(type: "int", nullable: false),
                    MainItemNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MainItemNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainItems", x => x.MainItemId);
                    table.ForeignKey(
                        name: "FK_MainItems_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MainItems_ItemSubSections_ItemSubSectionId",
                        column: x => x.ItemSubSectionId,
                        principalTable: "ItemSubSections",
                        principalColumn: "ItemSubSectionId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AccountStore",
                columns: table => new
                {
                    AccountStoreId = table.Column<int>(type: "int", nullable: false),
                    AccountStoreTypeId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountStore", x => x.AccountStoreId);
                    table.ForeignKey(
                        name: "FK_AccountStore_AccountStoreTypes_AccountStoreTypeId",
                        column: x => x.AccountStoreTypeId,
                        principalTable: "AccountStoreTypes",
                        principalColumn: "AccountStoreTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountStore_Accounts_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AccountStore_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApproveRequests",
                columns: table => new
                {
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    RequestCode = table.Column<int>(type: "int", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "Date", nullable: false),
                    ApproveRequestTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ReferenceId = table.Column<int>(type: "int", nullable: false),
                    ReferenceCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FromUserName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsApproved = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    ApproveId = table.Column<int>(type: "int", nullable: false),
                    CurrentStepId = table.Column<int>(type: "int", nullable: false),
                    CurrentStatusId = table.Column<int>(type: "int", nullable: false),
                    LastStepId = table.Column<int>(type: "int", nullable: false),
                    LastStatusId = table.Column<int>(type: "int", nullable: false),
                    CurrentStepCount = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    BranchId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproveRequests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_ApproveRequests_ApproveRequestTypes_ApproveRequestTypeId",
                        column: x => x.ApproveRequestTypeId,
                        principalTable: "ApproveRequestTypes",
                        principalColumn: "ApproveRequestTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApproveRequests_ApproveStatuses_CurrentStatusId",
                        column: x => x.CurrentStatusId,
                        principalTable: "ApproveStatuses",
                        principalColumn: "ApproveStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApproveRequests_ApproveStatuses_LastStatusId",
                        column: x => x.LastStatusId,
                        principalTable: "ApproveStatuses",
                        principalColumn: "ApproveStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApproveRequests_ApproveStep_CurrentStepId",
                        column: x => x.CurrentStepId,
                        principalTable: "ApproveStep",
                        principalColumn: "ApproveStepId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApproveRequests_ApproveStep_LastStepId",
                        column: x => x.LastStepId,
                        principalTable: "ApproveStep",
                        principalColumn: "ApproveStepId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApproveRequests_Approves_ApproveId",
                        column: x => x.ApproveId,
                        principalTable: "Approves",
                        principalColumn: "ApproveId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApproveRequests_Branches_BranchId",
                        column: x => x.BranchId,
                        principalTable: "Branches",
                        principalColumn: "BranchId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApproveRequests_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApproveRequests_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApproveRequests_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClientPriceRequestHeaders",
                columns: table => new
                {
                    ClientPriceRequestHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: true),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientPriceRequestHeaders", x => x.ClientPriceRequestHeaderId);
                    table.ForeignKey(
                        name: "FK_ClientPriceRequestHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientPriceRequestHeaders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientPriceRequestHeaders_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "SellerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientPriceRequestHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FixedAssetMovementHeaders",
                columns: table => new
                {
                    FixedAssetMovementHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    CostCenterToId = table.Column<int>(type: "int", nullable: false),
                    MovementDate = table.Column<DateTime>(type: "Date", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedAssetMovementHeaders", x => x.FixedAssetMovementHeaderId);
                    table.ForeignKey(
                        name: "FK_FixedAssetMovementHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssetMovementHeaders_CostCenters_CostCenterToId",
                        column: x => x.CostCenterToId,
                        principalTable: "CostCenters",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssetMovementHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InternalTransferHeaders",
                columns: table => new
                {
                    InternalTransferId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InternalTransferCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FromStoreId = table.Column<int>(type: "int", nullable: false),
                    ToStoreId = table.Column<int>(type: "int", nullable: false),
                    Reference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsReturned = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ReturnReason = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MenuCode = table.Column<short>(type: "smallint", nullable: true),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalTransferHeaders", x => x.InternalTransferId);
                    table.ForeignKey(
                        name: "FK_InternalTransferHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternalTransferHeaders_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternalTransferHeaders_Stores_FromStoreId",
                        column: x => x.FromStoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternalTransferHeaders_Stores_ToStoreId",
                        column: x => x.ToStoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventoryInHeaders",
                columns: table => new
                {
                    InventoryInHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InventoryInCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryInHeaders", x => x.InventoryInHeaderId);
                    table.ForeignKey(
                        name: "FK_InventoryInHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryInHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventoryOutHeaders",
                columns: table => new
                {
                    InventoryOutHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InventoryOutCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryOutHeaders", x => x.InventoryOutHeaderId);
                    table.ForeignKey(
                        name: "FK_InventoryOutHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryOutHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemCostUpdateHeaders",
                columns: table => new
                {
                    ItemCostUpdateHeaderId = table.Column<int>(type: "int", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCostUpdateHeaders", x => x.ItemCostUpdateHeaderId);
                    table.ForeignKey(
                        name: "FK_ItemCostUpdateHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemDisassembleHeaders",
                columns: table => new
                {
                    ItemDisassembleHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemDisassembleCode = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    IsAutomatic = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: true),
                    ReferenceHeaderId = table.Column<int>(type: "int", nullable: true),
                    ReferenceDetailId = table.Column<int>(type: "int", nullable: true),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDisassembleHeaders", x => x.ItemDisassembleHeaderId);
                    table.ForeignKey(
                        name: "FK_ItemDisassembleHeaders_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemDisassembleHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JournalHeaders",
                columns: table => new
                {
                    JournalHeaderId = table.Column<int>(type: "int", nullable: false),
                    JournalId = table.Column<int>(type: "int", nullable: false),
                    JournalTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TicketId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JournalCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TicketDate = table.Column<DateTime>(type: "Date", nullable: false),
                    PeerReference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalDebitValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalDebitValueAccount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCreditValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCreditValueAccount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsCancelled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsSystematic = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: true),
                    MenuReferenceId = table.Column<int>(type: "int", nullable: true),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalHeaders", x => x.JournalHeaderId);
                    table.ForeignKey(
                        name: "FK_JournalHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalHeaders_JournalTypes_JournalTypeId",
                        column: x => x.JournalTypeId,
                        principalTable: "JournalTypes",
                        principalColumn: "JournalTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalHeaders_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "MenuEncodings",
                columns: table => new
                {
                    MenuEncodingId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuEncodings", x => x.MenuEncodingId);
                    table.ForeignKey(
                        name: "FK_MenuEncodings_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuEncodings_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductRequestHeaders",
                columns: table => new
                {
                    ProductRequestHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRequestHeaders", x => x.ProductRequestHeaderId);
                    table.ForeignKey(
                        name: "FK_ProductRequestHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockTakingCarryOverHeaders",
                columns: table => new
                {
                    StockTakingCarryOverHeaderId = table.Column<int>(type: "int", nullable: false),
                    StockTakingList = table.Column<string>(type: "varchar(10000)", maxLength: 10000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StockTakingCarryOverCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StockTakingCarryOverNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StockTakingCarryOverNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsOpenBalance = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsAllItemsAffected = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalCurrentBalanceConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalStockTakingConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCurrentBalanceCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalStockTakingCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTakingCarryOverHeaders", x => x.StockTakingCarryOverHeaderId);
                    table.ForeignKey(
                        name: "FK_StockTakingCarryOverHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockTakingHeaders",
                columns: table => new
                {
                    StockTakingHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StockTakingCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    StockTakingNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StockTakingNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsOpenBalance = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    StockDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsCarriedOver = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTakingHeaders", x => x.StockTakingHeaderId);
                    table.ForeignKey(
                        name: "FK_StockTakingHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Items",
                columns: table => new
                {
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ItemNameAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ItemNameEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    ItemCategoryId = table.Column<int>(type: "int", nullable: true),
                    ItemSubCategoryId = table.Column<int>(type: "int", nullable: true),
                    ItemSectionId = table.Column<int>(type: "int", nullable: true),
                    ItemSubSectionId = table.Column<int>(type: "int", nullable: true),
                    MainItemId = table.Column<int>(type: "int", nullable: true),
                    VendorId = table.Column<int>(type: "int", nullable: true),
                    ItemTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    SingularPackageId = table.Column<int>(type: "int", nullable: false),
                    PurchasingPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    InternalPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    MaxDiscountPercent = table.Column<decimal>(type: "decimal(4,2)", nullable: false),
                    SalesAccountId = table.Column<int>(type: "int", nullable: true),
                    PurchaseAccountId = table.Column<int>(type: "int", nullable: true),
                    MinBuyQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    MinSellQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    MaxBuyQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    MaxSellQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ReorderPointQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CoverageQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    InActiveReasons = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NoReplenishment = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsUnderSelling = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsNoStock = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsUntradeable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsDeficit = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsPos = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsOnline = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsPoints = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsGifts = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsPromoted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsExpired = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsBatched = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ItemLocation = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items", x => x.ItemId);
                    table.ForeignKey(
                        name: "FK_Items_Accounts_PurchaseAccountId",
                        column: x => x.PurchaseAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Accounts_SalesAccountId",
                        column: x => x.SalesAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_ItemCategories_ItemCategoryId",
                        column: x => x.ItemCategoryId,
                        principalTable: "ItemCategories",
                        principalColumn: "ItemCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_ItemPackages_SingularPackageId",
                        column: x => x.SingularPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_ItemSections_ItemSectionId",
                        column: x => x.ItemSectionId,
                        principalTable: "ItemSections",
                        principalColumn: "ItemSectionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_ItemSubCategories_ItemSubCategoryId",
                        column: x => x.ItemSubCategoryId,
                        principalTable: "ItemSubCategories",
                        principalColumn: "ItemSubCategoryId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_ItemSubSections_ItemSubSectionId",
                        column: x => x.ItemSubSectionId,
                        principalTable: "ItemSubSections",
                        principalColumn: "ItemSubSectionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_ItemTypes_ItemTypeId",
                        column: x => x.ItemTypeId,
                        principalTable: "ItemTypes",
                        principalColumn: "ItemTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_MainItems_MainItemId",
                        column: x => x.MainItemId,
                        principalTable: "MainItems",
                        principalColumn: "MainItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Items_Vendors_VendorId",
                        column: x => x.VendorId,
                        principalTable: "Vendors",
                        principalColumn: "VendorId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApproveRequestDetails",
                columns: table => new
                {
                    ApproveRequestDetailId = table.Column<int>(type: "int", nullable: false),
                    ApproveRequestId = table.Column<int>(type: "int", nullable: false),
                    OldValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NewValue = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Changes = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproveRequestDetails", x => x.ApproveRequestDetailId);
                    table.ForeignKey(
                        name: "FK_ApproveRequestDetails_ApproveRequests_ApproveRequestId",
                        column: x => x.ApproveRequestId,
                        principalTable: "ApproveRequests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApproveRequestUsers",
                columns: table => new
                {
                    ApproveRequestUserId = table.Column<int>(type: "int", nullable: false),
                    RequestId = table.Column<int>(type: "int", nullable: false),
                    StepId = table.Column<int>(type: "int", nullable: false),
                    StatusId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Remarks = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApproveRequestUsers", x => x.ApproveRequestUserId);
                    table.ForeignKey(
                        name: "FK_ApproveRequestUsers_ApproveRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ApproveRequests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApproveRequestUsers_ApproveStatuses_StatusId",
                        column: x => x.StatusId,
                        principalTable: "ApproveStatuses",
                        principalColumn: "ApproveStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApproveRequestUsers_ApproveStep_StepId",
                        column: x => x.StepId,
                        principalTable: "ApproveStep",
                        principalColumn: "ApproveStepId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClientQuotationHeaders",
                columns: table => new
                {
                    ClientQuotationHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientPriceRequestHeaderId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: true),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalItemDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValueBeforeAdditionalDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    AdditionalDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ValidInDays = table.Column<int>(type: "int", nullable: false),
                    ValidUntilDate = table.Column<DateTime>(type: "Date", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientQuotationHeaders", x => x.ClientQuotationHeaderId);
                    table.ForeignKey(
                        name: "FK_ClientQuotationHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationHeaders_ClientPriceRequestHeaders_ClientPrice~",
                        column: x => x.ClientPriceRequestHeaderId,
                        principalTable: "ClientPriceRequestHeaders",
                        principalColumn: "ClientPriceRequestHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationHeaders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationHeaders_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "SellerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationHeaders_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FixedAssetMovementDetails",
                columns: table => new
                {
                    FixedAssetMovementDetailId = table.Column<int>(type: "int", nullable: false),
                    FixedAssetMovementHeaderId = table.Column<int>(type: "int", nullable: false),
                    FixedAssetId = table.Column<int>(type: "int", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedAssetMovementDetails", x => x.FixedAssetMovementDetailId);
                    table.ForeignKey(
                        name: "FK_FixedAssetMovementDetails_FixedAssetMovementHeaders_FixedAss~",
                        column: x => x.FixedAssetMovementHeaderId,
                        principalTable: "FixedAssetMovementHeaders",
                        principalColumn: "FixedAssetMovementHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssetMovementDetails_FixedAssets_FixedAssetId",
                        column: x => x.FixedAssetId,
                        principalTable: "FixedAssets",
                        principalColumn: "FixedAssetId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InternalTransferReceiveHeaders",
                columns: table => new
                {
                    InternalTransferReceiveHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InternalTransferReceiveCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InternalTransferHeaderId = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    FromStoreId = table.Column<int>(type: "int", nullable: false),
                    ToStoreId = table.Column<int>(type: "int", nullable: false),
                    Reference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MenuCode = table.Column<short>(type: "smallint", nullable: true),
                    ReferenceId = table.Column<int>(type: "int", nullable: true),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalTransferReceiveHeaders", x => x.InternalTransferReceiveHeaderId);
                    table.ForeignKey(
                        name: "FK_InternalTransferReceiveHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternalTransferReceiveHeaders_InternalTransferHeaders_Inter~",
                        column: x => x.InternalTransferHeaderId,
                        principalTable: "InternalTransferHeaders",
                        principalColumn: "InternalTransferId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternalTransferReceiveHeaders_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternalTransferReceiveHeaders_Stores_FromStoreId",
                        column: x => x.FromStoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternalTransferReceiveHeaders_Stores_ToStoreId",
                        column: x => x.ToStoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FixedAssetVoucherHeaders",
                columns: table => new
                {
                    FixedAssetVoucherHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FixedAssetVoucherTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    PeerReference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FixedAssetReference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SellerId = table.Column<int>(type: "int", nullable: true),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JournalHeaderId = table.Column<int>(type: "int", nullable: true),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedAssetVoucherHeaders", x => x.FixedAssetVoucherHeaderId);
                    table.ForeignKey(
                        name: "FK_FixedAssetVoucherHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssetVoucherHeaders_FixedAssetVoucherTypes_FixedAssetVo~",
                        column: x => x.FixedAssetVoucherTypeId,
                        principalTable: "FixedAssetVoucherTypes",
                        principalColumn: "FixedAssetVoucherTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssetVoucherHeaders_JournalHeaders_JournalHeaderId",
                        column: x => x.JournalHeaderId,
                        principalTable: "JournalHeaders",
                        principalColumn: "JournalHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssetVoucherHeaders_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "SellerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssetVoucherHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JournalDetails",
                columns: table => new
                {
                    JournalDetailId = table.Column<int>(type: "int", nullable: false),
                    JournalHeaderId = table.Column<int>(type: "int", nullable: false),
                    Serial = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    CurrencyRate = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DebitValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DebitValueAccount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreditValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreditValueAccount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    IsTax = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: true),
                    TaxId = table.Column<int>(type: "int", nullable: true),
                    TaxParentId = table.Column<int>(type: "int", nullable: true),
                    TaxPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AutomaticRemarks = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: true),
                    EntityId = table.Column<int>(type: "int", nullable: true),
                    EntityNameAr = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityNameEn = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityEmail = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxReference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TaxDate = table.Column<DateTime>(type: "Date", nullable: true),
                    IsLinkedToCostCenters = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    IsCostCenterDistributed = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    IsSystematic = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsCostRelated = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalDetails", x => x.JournalDetailId);
                    table.ForeignKey(
                        name: "FK_JournalDetails_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalDetails_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalDetails_EntityTypes_EntityTypeId",
                        column: x => x.EntityTypeId,
                        principalTable: "EntityTypes",
                        principalColumn: "EntityTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalDetails_JournalHeaders_JournalHeaderId",
                        column: x => x.JournalHeaderId,
                        principalTable: "JournalHeaders",
                        principalColumn: "JournalHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalDetails_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_JournalDetails_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaymentVoucherHeaders",
                columns: table => new
                {
                    PaymentVoucherHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentVoucherCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: true),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    PeerReference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentReference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SellerId = table.Column<int>(type: "int", nullable: true),
                    TotalDebitValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalDebitValueAccount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCreditValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCreditValueAccount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JournalHeaderId = table.Column<int>(type: "int", nullable: true),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentVoucherHeaders", x => x.PaymentVoucherHeaderId);
                    table.ForeignKey(
                        name: "FK_PaymentVoucherHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentVoucherHeaders_JournalHeaders_JournalHeaderId",
                        column: x => x.JournalHeaderId,
                        principalTable: "JournalHeaders",
                        principalColumn: "JournalHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentVoucherHeaders_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "SellerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentVoucherHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentVoucherHeaders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReceiptVoucherHeaders",
                columns: table => new
                {
                    ReceiptVoucherHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReceiptVoucherCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: true),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    PeerReference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentReference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SellerId = table.Column<int>(type: "int", nullable: true),
                    TotalDebitValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalDebitValueAccount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCreditValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCreditValueAccount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    JournalHeaderId = table.Column<int>(type: "int", nullable: true),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptVoucherHeaders", x => x.ReceiptVoucherHeaderId);
                    table.ForeignKey(
                        name: "FK_ReceiptVoucherHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiptVoucherHeaders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiptVoucherHeaders_JournalHeaders_JournalHeaderId",
                        column: x => x.JournalHeaderId,
                        principalTable: "JournalHeaders",
                        principalColumn: "JournalHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiptVoucherHeaders_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "SellerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiptVoucherHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductRequestPriceHeaders",
                columns: table => new
                {
                    ProductRequestPriceHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductRequestHeaderId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalItemDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValueBeforeAdditionalDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    AdditionalDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRequestPriceHeaders", x => x.ProductRequestPriceHeaderId);
                    table.ForeignKey(
                        name: "FK_ProductRequestPriceHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestPriceHeaders_ProductRequestHeaders_ProductRequ~",
                        column: x => x.ProductRequestHeaderId,
                        principalTable: "ProductRequestHeaders",
                        principalColumn: "ProductRequestHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestPriceHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestPriceHeaders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestPriceHeaders_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClientPriceRequestDetails",
                columns: table => new
                {
                    ClientPriceRequestDetailId = table.Column<int>(type: "int", nullable: false),
                    ClientPriceRequestHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientPriceRequestDetails", x => x.ClientPriceRequestDetailId);
                    table.ForeignKey(
                        name: "FK_ClientPriceRequestDetails_ClientPriceRequestHeaders_ClientPr~",
                        column: x => x.ClientPriceRequestHeaderId,
                        principalTable: "ClientPriceRequestHeaders",
                        principalColumn: "ClientPriceRequestHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientPriceRequestDetails_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientPriceRequestDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientPriceRequestDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InternalTransferDetails",
                columns: table => new
                {
                    InternalTransferDetailId = table.Column<int>(type: "int", nullable: false),
                    InternalTransferHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalTransferDetails", x => x.InternalTransferDetailId);
                    table.ForeignKey(
                        name: "FK_InternalTransferDetails_InternalTransferHeaders_InternalTran~",
                        column: x => x.InternalTransferHeaderId,
                        principalTable: "InternalTransferHeaders",
                        principalColumn: "InternalTransferId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternalTransferDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternalTransferDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventoryInDetails",
                columns: table => new
                {
                    InventoryInDetailId = table.Column<int>(type: "int", nullable: false),
                    InventoryInHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsLinkedToCostCenters = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    IsCostCenterDistributed = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryInDetails", x => x.InventoryInDetailId);
                    table.ForeignKey(
                        name: "FK_InventoryInDetails_InventoryInHeaders_InventoryInHeaderId",
                        column: x => x.InventoryInHeaderId,
                        principalTable: "InventoryInHeaders",
                        principalColumn: "InventoryInHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryInDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryInDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InventoryOutDetails",
                columns: table => new
                {
                    InventoryOutDetailId = table.Column<int>(type: "int", nullable: false),
                    InventoryOutHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsLinkedToCostCenters = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    IsCostCenterDistributed = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryOutDetails", x => x.InventoryOutDetailId);
                    table.ForeignKey(
                        name: "FK_InventoryOutDetails_InventoryOutHeaders_InventoryOutHeaderId",
                        column: x => x.InventoryOutHeaderId,
                        principalTable: "InventoryOutHeaders",
                        principalColumn: "InventoryOutHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryOutDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InventoryOutDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemAttributes",
                columns: table => new
                {
                    ItemAttributeId = table.Column<int>(type: "int", nullable: false),
                    ItemAttributeTypeId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemAttributeNameAr = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ItemAttributeNameEn = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemAttributes", x => x.ItemAttributeId);
                    table.ForeignKey(
                        name: "FK_ItemAttributes_ItemAttributeTypes_ItemAttributeTypeId",
                        column: x => x.ItemAttributeTypeId,
                        principalTable: "ItemAttributeTypes",
                        principalColumn: "ItemAttributeTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemAttributes_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemBarCodes",
                columns: table => new
                {
                    ItemBarCodeId = table.Column<int>(type: "int", nullable: false),
                    FromPackageId = table.Column<int>(type: "int", nullable: false),
                    ToPackageId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    IsSingularPackage = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemBarCodes", x => x.ItemBarCodeId);
                    table.ForeignKey(
                        name: "FK_ItemBarCodes_ItemPackages_FromPackageId",
                        column: x => x.FromPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemBarCodes_ItemPackages_ToPackageId",
                        column: x => x.ToPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemBarCodes_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemCosts",
                columns: table => new
                {
                    ItemCostId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastPurchasePrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastCostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCosts", x => x.ItemCostId);
                    table.ForeignKey(
                        name: "FK_ItemCosts_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemCosts_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemCosts_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemCostUpdateDetails",
                columns: table => new
                {
                    ItemCostUpdateDetailId = table.Column<int>(type: "int", nullable: false),
                    ItemCostUpdateHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    OldCostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NewCostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OldLastPurchasePrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NewLastPurchasePrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OldLastCostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NewLastCostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCostUpdateDetails", x => x.ItemCostUpdateDetailId);
                    table.ForeignKey(
                        name: "FK_ItemCostUpdateDetails_ItemCostUpdateHeaders_ItemCostUpdateHe~",
                        column: x => x.ItemCostUpdateHeaderId,
                        principalTable: "ItemCostUpdateHeaders",
                        principalColumn: "ItemCostUpdateHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemCostUpdateDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemCostUpdateDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemCurrentBalances",
                columns: table => new
                {
                    ItemCurrentBalanceId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OpenDate = table.Column<DateTime>(type: "Date", nullable: true),
                    OpenQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    InQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OutQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    PendingInQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    PendingOutQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemCurrentBalances", x => x.ItemCurrentBalanceId);
                    table.ForeignKey(
                        name: "FK_ItemCurrentBalances_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemCurrentBalances_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemCurrentBalances_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemDisassembleDetails",
                columns: table => new
                {
                    ItemDisassembleDetailId = table.Column<int>(type: "int", nullable: false),
                    ItemDisassembleHeaderId = table.Column<int>(type: "int", nullable: false),
                    IsSerialConversion = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    FromPackageId = table.Column<int>(type: "int", nullable: false),
                    ToPackageId = table.Column<int>(type: "int", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    FromPackageQuantityBefore = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ToPackageQuantityBefore = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    FromPackageQuantityAfter = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ToPackageQuantityAfter = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDisassembleDetails", x => x.ItemDisassembleDetailId);
                    table.ForeignKey(
                        name: "FK_ItemDisassembleDetails_ItemDisassembleHeaders_ItemDisassembl~",
                        column: x => x.ItemDisassembleHeaderId,
                        principalTable: "ItemDisassembleHeaders",
                        principalColumn: "ItemDisassembleHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemDisassembleDetails_ItemPackages_FromPackageId",
                        column: x => x.FromPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemDisassembleDetails_ItemPackages_ToPackageId",
                        column: x => x.ToPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemDisassembleDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemDisassembles",
                columns: table => new
                {
                    ItemDisassembleId = table.Column<int>(type: "int", nullable: false),
                    ItemDisassembleHeaderId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    InQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OutQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDisassembles", x => x.ItemDisassembleId);
                    table.ForeignKey(
                        name: "FK_ItemDisassembles_ItemDisassembleHeaders_ItemDisassembleHeade~",
                        column: x => x.ItemDisassembleHeaderId,
                        principalTable: "ItemDisassembleHeaders",
                        principalColumn: "ItemDisassembleHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemDisassembles_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemDisassembles_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemDisassembles_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemDisassembleSerials",
                columns: table => new
                {
                    ItemDisassembleSerialId = table.Column<int>(type: "int", nullable: false),
                    ItemDisassembleHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    MainItemPackageId = table.Column<int>(type: "int", nullable: true),
                    ItemPackageBalanceBefore = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemPackageBalanceAfter = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemDisassembleSerials", x => x.ItemDisassembleSerialId);
                    table.ForeignKey(
                        name: "FK_ItemDisassembleSerials_ItemDisassembleHeaders_ItemDisassembl~",
                        column: x => x.ItemDisassembleHeaderId,
                        principalTable: "ItemDisassembleHeaders",
                        principalColumn: "ItemDisassembleHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemDisassembleSerials_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemDisassembleSerials_ItemPackages_MainItemPackageId",
                        column: x => x.MainItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemDisassembleSerials_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemNegativeSalesHeaders",
                columns: table => new
                {
                    ItemNegativeSalesHeaderId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ItemPreviousQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NegativeSalesQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SettledQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    IsSettled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemNegativeSalesHeaders", x => x.ItemNegativeSalesHeaderId);
                    table.ForeignKey(
                        name: "FK_ItemNegativeSalesHeaders_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemNegativeSalesHeaders_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemNegativeSalesHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemPacking",
                columns: table => new
                {
                    ItemPackingId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    FromPackageId = table.Column<int>(type: "int", nullable: false),
                    ToPackageId = table.Column<int>(type: "int", nullable: false),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemPacking", x => x.ItemPackingId);
                    table.ForeignKey(
                        name: "FK_ItemPacking_ItemPackages_FromPackageId",
                        column: x => x.FromPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemPacking_ItemPackages_ToPackageId",
                        column: x => x.ToPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemPacking_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemTaxes",
                columns: table => new
                {
                    ItemTaxId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemTaxes", x => x.ItemTaxId);
                    table.ForeignKey(
                        name: "FK_ItemTaxes_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemTaxes_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductRequestDetails",
                columns: table => new
                {
                    ProductRequestDetailId = table.Column<int>(type: "int", nullable: false),
                    ProductRequestHeaderId = table.Column<int>(type: "int", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRequestDetails", x => x.ProductRequestDetailId);
                    table.ForeignKey(
                        name: "FK_ProductRequestDetails_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestDetails_ProductRequestHeaders_ProductRequestHe~",
                        column: x => x.ProductRequestHeaderId,
                        principalTable: "ProductRequestHeaders",
                        principalColumn: "ProductRequestHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockTakingCarryOverDetails",
                columns: table => new
                {
                    StockTakingCarryOverDetailId = table.Column<int>(type: "int", nullable: false),
                    StockTakingCarryOverHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    StockTakingQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CurrentQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    StockTakingConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CurrentConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    StockTakingConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CurrentConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    StockTakingCostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CurrentCostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    StockTakingCostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CurrentCostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    StockTakingCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CurrentCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OpenQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OldOpenQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    InQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OutQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTakingCarryOverDetails", x => x.StockTakingCarryOverDetailId);
                    table.ForeignKey(
                        name: "FK_StockTakingCarryOverDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTakingCarryOverDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTakingCarryOverDetails_StockTakingCarryOverHeaders_Stoc~",
                        column: x => x.StockTakingCarryOverHeaderId,
                        principalTable: "StockTakingCarryOverHeaders",
                        principalColumn: "StockTakingCarryOverHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockTakingCarryOverEffectDetails",
                columns: table => new
                {
                    StockTakingCarryOverEffectDetailId = table.Column<int>(type: "int", nullable: false),
                    StockTakingCarryOverHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OpenQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OldOpenQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    InQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OutQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTakingCarryOverEffectDetails", x => x.StockTakingCarryOverEffectDetailId);
                    table.ForeignKey(
                        name: "FK_StockTakingCarryOverEffectDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTakingCarryOverEffectDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTakingCarryOverEffectDetails_StockTakingCarryOverHeader~",
                        column: x => x.StockTakingCarryOverHeaderId,
                        principalTable: "StockTakingCarryOverHeaders",
                        principalColumn: "StockTakingCarryOverHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockTakingDetails",
                columns: table => new
                {
                    StockTakingDetailId = table.Column<int>(type: "int", nullable: false),
                    StockTakingHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockTakingDetails", x => x.StockTakingDetailId);
                    table.ForeignKey(
                        name: "FK_StockTakingDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTakingDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockTakingDetails_StockTakingHeaders_StockTakingHeaderId",
                        column: x => x.StockTakingHeaderId,
                        principalTable: "StockTakingHeaders",
                        principalColumn: "StockTakingHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClientQuotationApprovalHeaders",
                columns: table => new
                {
                    ClientQuotationHeaderApprovalId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientQuotationHeaderId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: true),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalItemDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValueBeforeAdditionalDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    AdditionalDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientQuotationApprovalHeaders", x => x.ClientQuotationHeaderApprovalId);
                    table.ForeignKey(
                        name: "FK_ClientQuotationApprovalHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationApprovalHeaders_ClientQuotationHeaders_Client~",
                        column: x => x.ClientQuotationHeaderId,
                        principalTable: "ClientQuotationHeaders",
                        principalColumn: "ClientQuotationHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationApprovalHeaders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationApprovalHeaders_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "SellerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationApprovalHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationApprovalHeaders_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClientQuotationDetails",
                columns: table => new
                {
                    ClientQuotationDetailId = table.Column<int>(type: "int", nullable: false),
                    ClientQuotationHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValueAfterDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    HeaderDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastSalesPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientQuotationDetails", x => x.ClientQuotationDetailId);
                    table.ForeignKey(
                        name: "FK_ClientQuotationDetails_ClientQuotationHeaders_ClientQuotatio~",
                        column: x => x.ClientQuotationHeaderId,
                        principalTable: "ClientQuotationHeaders",
                        principalColumn: "ClientQuotationHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationDetails_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InternalTransferReceiveDetails",
                columns: table => new
                {
                    InternalTransferReceiveDetailId = table.Column<int>(type: "int", nullable: false),
                    InternalTransferReceiveHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ConsumerValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InternalTransferReceiveDetails", x => x.InternalTransferReceiveDetailId);
                    table.ForeignKey(
                        name: "FK_InternalTransferReceiveDetails_InternalTransferReceiveHeader~",
                        column: x => x.InternalTransferReceiveHeaderId,
                        principalTable: "InternalTransferReceiveHeaders",
                        principalColumn: "InternalTransferReceiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternalTransferReceiveDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InternalTransferReceiveDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FixedAssetVoucherDetails",
                columns: table => new
                {
                    FixedAssetVoucherDetailId = table.Column<int>(type: "int", nullable: false),
                    FixedAssetVoucherHeaderId = table.Column<int>(type: "int", nullable: false),
                    FixedAssetId = table.Column<int>(type: "int", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DetailValue = table.Column<decimal>(type: "decimal(65,30)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedAssetVoucherDetails", x => x.FixedAssetVoucherDetailId);
                    table.ForeignKey(
                        name: "FK_FixedAssetVoucherDetails_FixedAssetVoucherHeaders_FixedAsset~",
                        column: x => x.FixedAssetVoucherHeaderId,
                        principalTable: "FixedAssetVoucherHeaders",
                        principalColumn: "FixedAssetVoucherHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssetVoucherDetails_FixedAssets_FixedAssetId",
                        column: x => x.FixedAssetId,
                        principalTable: "FixedAssets",
                        principalColumn: "FixedAssetId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "CostCenterJournalDetails",
                columns: table => new
                {
                    CostCenterJournalDetailId = table.Column<int>(type: "int", nullable: false),
                    JournalDetailId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: true),
                    CostCenterId = table.Column<int>(type: "int", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: true),
                    ReferenceHeaderId = table.Column<int>(type: "int", nullable: true),
                    ReferenceDetailId = table.Column<int>(type: "int", nullable: true),
                    Serial = table.Column<int>(type: "int", nullable: false),
                    DebitValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreditValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    IsCostCenterDistributed = table.Column<bool>(type: "tinyint(1)", nullable: true),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostCenterJournalDetails", x => x.CostCenterJournalDetailId);
                    table.ForeignKey(
                        name: "FK_CostCenterJournalDetails_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostCenterJournalDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostCenterJournalDetails_JournalDetails_JournalDetailId",
                        column: x => x.JournalDetailId,
                        principalTable: "JournalDetails",
                        principalColumn: "JournalDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CostCenterJournalDetails_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaymentVoucherDetails",
                columns: table => new
                {
                    PaymentVoucherDetailId = table.Column<int>(type: "int", nullable: false),
                    PaymentVoucherHeaderId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    CurrencyRate = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DebitValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreditValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DebitValueAccount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreditValueAccount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentVoucherDetails", x => x.PaymentVoucherDetailId);
                    table.ForeignKey(
                        name: "FK_PaymentVoucherDetails_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentVoucherDetails_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentVoucherDetails_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PaymentVoucherDetails_PaymentVoucherHeaders_PaymentVoucherHe~",
                        column: x => x.PaymentVoucherHeaderId,
                        principalTable: "PaymentVoucherHeaders",
                        principalColumn: "PaymentVoucherHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PaymentVoucherInvoices",
                columns: table => new
                {
                    PaymentVoucherInvoiceId = table.Column<int>(type: "int", nullable: false),
                    PaymentVoucherHeaderId = table.Column<int>(type: "int", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "Date", nullable: false),
                    InvoiceTotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    InvoicePaidValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    InvoiceDueValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    InvoiceInstallmentValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentVoucherInvoices", x => x.PaymentVoucherInvoiceId);
                    table.ForeignKey(
                        name: "FK_PaymentVoucherInvoices_PaymentVoucherHeaders_PaymentVoucherH~",
                        column: x => x.PaymentVoucherHeaderId,
                        principalTable: "PaymentVoucherHeaders",
                        principalColumn: "PaymentVoucherHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReceiptVoucherDetails",
                columns: table => new
                {
                    ReceiptVoucherDetailId = table.Column<int>(type: "int", nullable: false),
                    ReceiptVoucherHeaderId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    CurrencyRate = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DebitValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreditValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DebitValueAccount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreditValueAccount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptVoucherDetails", x => x.ReceiptVoucherDetailId);
                    table.ForeignKey(
                        name: "FK_ReceiptVoucherDetails_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiptVoucherDetails_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiptVoucherDetails_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReceiptVoucherDetails_ReceiptVoucherHeaders_ReceiptVoucherHe~",
                        column: x => x.ReceiptVoucherHeaderId,
                        principalTable: "ReceiptVoucherHeaders",
                        principalColumn: "ReceiptVoucherHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReceiptVoucherInvoices",
                columns: table => new
                {
                    ReceiptVoucherInvoiceId = table.Column<int>(type: "int", nullable: false),
                    ReceiptVoucherHeaderId = table.Column<int>(type: "int", nullable: false),
                    InvoiceId = table.Column<int>(type: "int", nullable: false),
                    InvoiceDate = table.Column<DateTime>(type: "Date", nullable: false),
                    InvoiceTotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    InvoicePaidValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    InvoiceDueValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    InvoiceInstallmentValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReceiptVoucherInvoices", x => x.ReceiptVoucherInvoiceId);
                    table.ForeignKey(
                        name: "FK_ReceiptVoucherInvoices_ReceiptVoucherHeaders_ReceiptVoucherH~",
                        column: x => x.ReceiptVoucherHeaderId,
                        principalTable: "ReceiptVoucherHeaders",
                        principalColumn: "ReceiptVoucherHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductRequestPriceDetails",
                columns: table => new
                {
                    ProductRequestPriceDetailId = table.Column<int>(type: "int", nullable: false),
                    ProductRequestPriceHeaderId = table.Column<int>(type: "int", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RequestedPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValueAfterDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    HeaderDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastPurchasePrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRequestPriceDetails", x => x.ProductRequestPriceDetailId);
                    table.ForeignKey(
                        name: "FK_ProductRequestPriceDetails_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestPriceDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestPriceDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestPriceDetails_ProductRequestPriceHeaders_Produc~",
                        column: x => x.ProductRequestPriceHeaderId,
                        principalTable: "ProductRequestPriceHeaders",
                        principalColumn: "ProductRequestPriceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SupplierQuotationHeaders",
                columns: table => new
                {
                    SupplierQuotationHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProductRequestPriceHeaderId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalItemDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValueBeforeAdditionalDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    AdditionalDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierQuotationHeaders", x => x.SupplierQuotationHeaderId);
                    table.ForeignKey(
                        name: "FK_SupplierQuotationHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuotationHeaders_ProductRequestPriceHeaders_ProductR~",
                        column: x => x.ProductRequestPriceHeaderId,
                        principalTable: "ProductRequestPriceHeaders",
                        principalColumn: "ProductRequestPriceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuotationHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuotationHeaders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuotationHeaders_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemBarCodeDetails",
                columns: table => new
                {
                    ItemBarCodeDetailId = table.Column<int>(type: "int", nullable: false),
                    ItemBarCodeId = table.Column<int>(type: "int", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemBarCodeDetails", x => x.ItemBarCodeDetailId);
                    table.ForeignKey(
                        name: "FK_ItemBarCodeDetails_ItemBarCodes_ItemBarCodeId",
                        column: x => x.ItemBarCodeId,
                        principalTable: "ItemBarCodes",
                        principalColumn: "ItemBarCodeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ItemNegativeSalesDetails",
                columns: table => new
                {
                    ItemNegativeSalesDetailId = table.Column<int>(type: "int", nullable: false),
                    ItemNegativeSalesHeaderId = table.Column<int>(type: "int", nullable: false),
                    SettledQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: true),
                    ReferenceHeaderId = table.Column<int>(type: "int", nullable: true),
                    ReferenceDetailId = table.Column<int>(type: "int", nullable: true),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ItemNegativeSalesDetails", x => x.ItemNegativeSalesDetailId);
                    table.ForeignKey(
                        name: "FK_ItemNegativeSalesDetails_ItemNegativeSalesHeaders_ItemNegati~",
                        column: x => x.ItemNegativeSalesHeaderId,
                        principalTable: "ItemNegativeSalesHeaders",
                        principalColumn: "ItemNegativeSalesHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ItemNegativeSalesDetails_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClientQuotationApprovalDetails",
                columns: table => new
                {
                    ClientQuotationApprovalDetailId = table.Column<int>(type: "int", nullable: false),
                    ClientQuotationApprovalHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValueAfterDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    HeaderDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastSalesPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientQuotationApprovalDetails", x => x.ClientQuotationApprovalDetailId);
                    table.ForeignKey(
                        name: "FK_ClientQuotationApprovalDetails_ClientQuotationApprovalHeader~",
                        column: x => x.ClientQuotationApprovalHeaderId,
                        principalTable: "ClientQuotationApprovalHeaders",
                        principalColumn: "ClientQuotationHeaderApprovalId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationApprovalDetails_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationApprovalDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationApprovalDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProformaInvoiceHeaders",
                columns: table => new
                {
                    ProformaInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientQuotationApprovalHeaderId = table.Column<int>(type: "int", nullable: true),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Reference = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreditPayment = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ShippingDate = table.Column<DateTime>(type: "Date", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "Date", nullable: true),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: true),
                    ShipmentTypeId = table.Column<int>(type: "int", nullable: true),
                    ClientName = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientPhone = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientAddress = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientTaxCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DriverName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DriverPhone = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientResponsibleName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientResponsiblePhone = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShipTo = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BillTo = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShippingRemarks = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalItemDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValueBeforeAdditionalDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    AdditionalDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsCancelled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsEnded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsBlocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DocumentStatusId = table.Column<int>(type: "int", nullable: false),
                    ShippingStatusId = table.Column<int>(type: "int", nullable: true),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProformaInvoiceHeaders", x => x.ProformaInvoiceHeaderId);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceHeaders_ClientQuotationApprovalHeaders_Client~",
                        column: x => x.ClientQuotationApprovalHeaderId,
                        principalTable: "ClientQuotationApprovalHeaders",
                        principalColumn: "ClientQuotationHeaderApprovalId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceHeaders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceHeaders_DocumentStatuses_DocumentStatusId",
                        column: x => x.DocumentStatusId,
                        principalTable: "DocumentStatuses",
                        principalColumn: "DocumentStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceHeaders_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "SellerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceHeaders_ShipmentTypes_ShipmentTypeId",
                        column: x => x.ShipmentTypeId,
                        principalTable: "ShipmentTypes",
                        principalColumn: "ShipmentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceHeaders_ShippingStatuses_ShippingStatusId",
                        column: x => x.ShippingStatusId,
                        principalTable: "ShippingStatuses",
                        principalColumn: "ShippingStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceHeaders_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClientQuotationDetailTaxes",
                columns: table => new
                {
                    ClientQuotationDetailTaxId = table.Column<int>(type: "int", nullable: false),
                    ClientQuotationDetailId = table.Column<int>(type: "int", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    TaxAfterVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientQuotationDetailTaxes", x => x.ClientQuotationDetailTaxId);
                    table.ForeignKey(
                        name: "FK_ClientQuotationDetailTaxes_Accounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationDetailTaxes_ClientQuotationDetails_ClientQuot~",
                        column: x => x.ClientQuotationDetailId,
                        principalTable: "ClientQuotationDetails",
                        principalColumn: "ClientQuotationDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationDetailTaxes_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FixedAssetVoucherDetailPayments",
                columns: table => new
                {
                    FixedAssetVoucherDetailPaymentId = table.Column<int>(type: "int", nullable: false),
                    FixedAssetVoucherDetailId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: true),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    CurrencyRate = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    DebitValue = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    CreditValue = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    DebitValueAccount = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    CreditValueAccount = table.Column<decimal>(type: "decimal(18,6)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FixedAssetVoucherDetailPayments", x => x.FixedAssetVoucherDetailPaymentId);
                    table.ForeignKey(
                        name: "FK_FixedAssetVoucherDetailPayments_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssetVoucherDetailPayments_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssetVoucherDetailPayments_FixedAssetVoucherDetails_Fix~",
                        column: x => x.FixedAssetVoucherDetailId,
                        principalTable: "FixedAssetVoucherDetails",
                        principalColumn: "FixedAssetVoucherDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FixedAssetVoucherDetailPayments_PaymentMethods_PaymentMethod~",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProductRequestPriceDetailTaxes",
                columns: table => new
                {
                    ProductRequestPriceDetailTaxId = table.Column<int>(type: "int", nullable: false),
                    ProductRequestPriceDetailId = table.Column<int>(type: "int", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    TaxAfterVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRequestPriceDetailTaxes", x => x.ProductRequestPriceDetailTaxId);
                    table.ForeignKey(
                        name: "FK_ProductRequestPriceDetailTaxes_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestPriceDetailTaxes_ProductRequestPriceDetails_Pr~",
                        column: x => x.ProductRequestPriceDetailId,
                        principalTable: "ProductRequestPriceDetails",
                        principalColumn: "ProductRequestPriceDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductRequestPriceDetailTaxes_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PurchaseOrderHeaders",
                columns: table => new
                {
                    PurchaseOrderHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SupplierQuotationHeaderId = table.Column<int>(type: "int", nullable: true),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreditPayment = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    PaymentPeriodDays = table.Column<int>(type: "int", nullable: true),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: true),
                    ShipmentTypeId = table.Column<int>(type: "int", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "Date", nullable: true),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalItemDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValueBeforeAdditionalDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    AdditionalDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsCancelled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsEnded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsBlocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DocumentStatusId = table.Column<int>(type: "int", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderHeaders", x => x.PurchaseOrderHeaderId);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderHeaders_DocumentStatuses_DocumentStatusId",
                        column: x => x.DocumentStatusId,
                        principalTable: "DocumentStatuses",
                        principalColumn: "DocumentStatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderHeaders_ShipmentTypes_ShipmentTypeId",
                        column: x => x.ShipmentTypeId,
                        principalTable: "ShipmentTypes",
                        principalColumn: "ShipmentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderHeaders_SupplierQuotationHeaders_SupplierQuotat~",
                        column: x => x.SupplierQuotationHeaderId,
                        principalTable: "SupplierQuotationHeaders",
                        principalColumn: "SupplierQuotationHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderHeaders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderHeaders_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SupplierQuotationDetails",
                columns: table => new
                {
                    SupplierQuotationDetailId = table.Column<int>(type: "int", nullable: false),
                    SupplierQuotationHeaderId = table.Column<int>(type: "int", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ReceivedPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValueAfterDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    HeaderDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastPurchasePrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierQuotationDetails", x => x.SupplierQuotationDetailId);
                    table.ForeignKey(
                        name: "FK_SupplierQuotationDetails_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuotationDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuotationDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuotationDetails_SupplierQuotationHeaders_SupplierQu~",
                        column: x => x.SupplierQuotationHeaderId,
                        principalTable: "SupplierQuotationHeaders",
                        principalColumn: "SupplierQuotationHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClientQuotationDetailApprovalTaxes",
                columns: table => new
                {
                    ClientQuotationApprovalDetailTaxId = table.Column<int>(type: "int", nullable: false),
                    ClientQuotationApprovalDetailId = table.Column<int>(type: "int", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    TaxAfterVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientQuotationDetailApprovalTaxes", x => x.ClientQuotationApprovalDetailTaxId);
                    table.ForeignKey(
                        name: "FK_ClientQuotationDetailApprovalTaxes_Accounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationDetailApprovalTaxes_ClientQuotationApprovalDe~",
                        column: x => x.ClientQuotationApprovalDetailId,
                        principalTable: "ClientQuotationApprovalDetails",
                        principalColumn: "ClientQuotationApprovalDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientQuotationDetailApprovalTaxes_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProformaInvoiceDetails",
                columns: table => new
                {
                    ProformaInvoiceDetailId = table.Column<int>(type: "int", nullable: false),
                    ProformaInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    BonusQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValueAfterDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    HeaderDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastSalesPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProformaInvoiceDetails", x => x.ProformaInvoiceDetailId);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceDetails_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceDetails_ProformaInvoiceHeaders_ProformaInvoic~",
                        column: x => x.ProformaInvoiceHeaderId,
                        principalTable: "ProformaInvoiceHeaders",
                        principalColumn: "ProformaInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SalesInvoiceHeaders",
                columns: table => new
                {
                    SalesInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ProformaInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDirectInvoice = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreditPayment = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ShippingDate = table.Column<DateTime>(type: "Date", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "Date", nullable: true),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: true),
                    ShipmentTypeId = table.Column<int>(type: "int", nullable: true),
                    ClientName = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientPhone = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientAddress = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientTaxCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DriverName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DriverPhone = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientResponsibleName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientResponsiblePhone = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShipTo = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BillTo = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShippingRemarks = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalItemDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValueBeforeAdditionalDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    AdditionalDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DebitAccountId = table.Column<int>(type: "int", nullable: true),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    JournalHeaderId = table.Column<int>(type: "int", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CanReturnInDays = table.Column<int>(type: "int", nullable: false),
                    CanReturnUntilDate = table.Column<DateTime>(type: "Date", nullable: false),
                    IsOnTheWay = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsEnded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsBlocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HasSettlement = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsSettlementCompleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: true),
                    InvoiceTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ClientBalance = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreditLimitDays = table.Column<int>(type: "int", nullable: false),
                    CreditLimitValues = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DebitLimitDays = table.Column<int>(type: "int", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoiceHeaders", x => x.SalesInvoiceHeaderId);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceHeaders_Accounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceHeaders_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceHeaders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceHeaders_InvoiceTypes_InvoiceTypeId",
                        column: x => x.InvoiceTypeId,
                        principalTable: "InvoiceTypes",
                        principalColumn: "InvoiceTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceHeaders_JournalHeaders_JournalHeaderId",
                        column: x => x.JournalHeaderId,
                        principalTable: "JournalHeaders",
                        principalColumn: "JournalHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceHeaders_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceHeaders_ProformaInvoiceHeaders_ProformaInvoiceHe~",
                        column: x => x.ProformaInvoiceHeaderId,
                        principalTable: "ProformaInvoiceHeaders",
                        principalColumn: "ProformaInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceHeaders_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "SellerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceHeaders_ShipmentTypes_ShipmentTypeId",
                        column: x => x.ShipmentTypeId,
                        principalTable: "ShipmentTypes",
                        principalColumn: "ShipmentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceHeaders_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PurchaseInvoiceHeaders",
                columns: table => new
                {
                    PurchaseInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PurchaseOrderHeaderId = table.Column<int>(type: "int", nullable: false),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDirectInvoice = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreditPayment = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalItemDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValueBeforeAdditionalDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    AdditionalDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalInvoiceExpense = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    JournalHeaderId = table.Column<int>(type: "int", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsOnTheWay = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsEnded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsBlocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    HasSettlement = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsSettlementCompleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: true),
                    InvoiceTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    SupplierBalance = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreditLimitDays = table.Column<int>(type: "int", nullable: false),
                    CreditLimitValues = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DebitLimitDays = table.Column<int>(type: "int", nullable: false),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: true),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoiceHeaders", x => x.PurchaseInvoiceHeaderId);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceHeaders_Accounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceHeaders_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceHeaders_InvoiceTypes_InvoiceTypeId",
                        column: x => x.InvoiceTypeId,
                        principalTable: "InvoiceTypes",
                        principalColumn: "InvoiceTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceHeaders_JournalHeaders_JournalHeaderId",
                        column: x => x.JournalHeaderId,
                        principalTable: "JournalHeaders",
                        principalColumn: "JournalHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceHeaders_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceHeaders_PurchaseOrderHeaders_PurchaseOrderHea~",
                        column: x => x.PurchaseOrderHeaderId,
                        principalTable: "PurchaseOrderHeaders",
                        principalColumn: "PurchaseOrderHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceHeaders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceHeaders_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PurchaseOrderDetails",
                columns: table => new
                {
                    PurchaseOrderDetailId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderHeaderId = table.Column<int>(type: "int", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    BonusQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValueAfterDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    HeaderDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastPurchasePrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderDetails", x => x.PurchaseOrderDetailId);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetails_CostCenters_CostCenterId",
                        column: x => x.CostCenterId,
                        principalTable: "CostCenters",
                        principalColumn: "CostCenterId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetails_PurchaseOrderHeaders_PurchaseOrderHeade~",
                        column: x => x.PurchaseOrderHeaderId,
                        principalTable: "PurchaseOrderHeaders",
                        principalColumn: "PurchaseOrderHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SupplierQuotationDetailTaxes",
                columns: table => new
                {
                    SupplierQuotationDetailTaxId = table.Column<int>(type: "int", nullable: false),
                    SupplierQuotationDetailId = table.Column<int>(type: "int", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    TaxAfterVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierQuotationDetailTaxes", x => x.SupplierQuotationDetailTaxId);
                    table.ForeignKey(
                        name: "FK_SupplierQuotationDetailTaxes_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuotationDetailTaxes_SupplierQuotationDetails_Suppli~",
                        column: x => x.SupplierQuotationDetailId,
                        principalTable: "SupplierQuotationDetails",
                        principalColumn: "SupplierQuotationDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierQuotationDetailTaxes_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ProformaInvoiceDetailTaxes",
                columns: table => new
                {
                    ProformaInvoiceDetailTaxId = table.Column<int>(type: "int", nullable: false),
                    ProformaInvoiceDetailId = table.Column<int>(type: "int", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    TaxAfterVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProformaInvoiceDetailTaxes", x => x.ProformaInvoiceDetailTaxId);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceDetailTaxes_Accounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceDetailTaxes_ProformaInvoiceDetails_ProformaIn~",
                        column: x => x.ProformaInvoiceDetailId,
                        principalTable: "ProformaInvoiceDetails",
                        principalColumn: "ProformaInvoiceDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProformaInvoiceDetailTaxes_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClientCreditMemos",
                columns: table => new
                {
                    ClientCreditMemoId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SalesInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    MemoValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    JournalHeaderId = table.Column<int>(type: "int", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientCreditMemos", x => x.ClientCreditMemoId);
                    table.ForeignKey(
                        name: "FK_ClientCreditMemos_Accounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientCreditMemos_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientCreditMemos_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientCreditMemos_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientCreditMemos_JournalHeaders_JournalHeaderId",
                        column: x => x.JournalHeaderId,
                        principalTable: "JournalHeaders",
                        principalColumn: "JournalHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientCreditMemos_SalesInvoiceHeaders_SalesInvoiceHeaderId",
                        column: x => x.SalesInvoiceHeaderId,
                        principalTable: "SalesInvoiceHeaders",
                        principalColumn: "SalesInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientCreditMemos_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "SellerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientCreditMemos_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClientDebitMemos",
                columns: table => new
                {
                    ClientDebitMemoId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SalesInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    MemoValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    JournalHeaderId = table.Column<int>(type: "int", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientDebitMemos", x => x.ClientDebitMemoId);
                    table.ForeignKey(
                        name: "FK_ClientDebitMemos_Accounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientDebitMemos_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientDebitMemos_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientDebitMemos_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientDebitMemos_JournalHeaders_JournalHeaderId",
                        column: x => x.JournalHeaderId,
                        principalTable: "JournalHeaders",
                        principalColumn: "JournalHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientDebitMemos_SalesInvoiceHeaders_SalesInvoiceHeaderId",
                        column: x => x.SalesInvoiceHeaderId,
                        principalTable: "SalesInvoiceHeaders",
                        principalColumn: "SalesInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientDebitMemos_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "SellerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientDebitMemos_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SalesInvoiceCollection",
                columns: table => new
                {
                    SalesInvoiceCollectionId = table.Column<int>(type: "int", nullable: false),
                    SalesInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    CurrencyRate = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CollectedValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CollectedValueAccount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoiceCollection", x => x.SalesInvoiceCollectionId);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceCollection_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceCollection_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceCollection_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceCollection_SalesInvoiceHeaders_SalesInvoiceHeade~",
                        column: x => x.SalesInvoiceHeaderId,
                        principalTable: "SalesInvoiceHeaders",
                        principalColumn: "SalesInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SalesInvoiceDetails",
                columns: table => new
                {
                    SalesInvoiceDetailId = table.Column<int>(type: "int", nullable: false),
                    SalesInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    BonusQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValueAfterDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    HeaderDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastSalesPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VatTaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    VatTaxId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoiceDetails", x => x.SalesInvoiceDetailId);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceDetails_SalesInvoiceHeaders_SalesInvoiceHeaderId",
                        column: x => x.SalesInvoiceHeaderId,
                        principalTable: "SalesInvoiceHeaders",
                        principalColumn: "SalesInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceDetails_TaxTypes_VatTaxTypeId",
                        column: x => x.VatTaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceDetails_Taxes_VatTaxId",
                        column: x => x.VatTaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SalesInvoiceReturnHeaders",
                columns: table => new
                {
                    SalesInvoiceReturnHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SalesInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDirectInvoice = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreditPayment = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ShippingDate = table.Column<DateTime>(type: "Date", nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "Date", nullable: true),
                    DueDate = table.Column<DateTime>(type: "Date", nullable: true),
                    ShipmentTypeId = table.Column<int>(type: "int", nullable: true),
                    ClientName = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientPhone = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientAddress = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientTaxCode = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DriverName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DriverPhone = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientResponsibleName = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ClientResponsiblePhone = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShipTo = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BillTo = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShippingRemarks = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalItemDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValueBeforeAdditionalDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    AdditionalDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    CreditAccountId = table.Column<int>(type: "int", nullable: true),
                    JournalHeaderId = table.Column<int>(type: "int", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsOnTheWay = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsEnded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsBlocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: true),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoiceReturnHeaders", x => x.SalesInvoiceReturnHeaderId);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnHeaders_Accounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnHeaders_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnHeaders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnHeaders_JournalHeaders_JournalHeaderId",
                        column: x => x.JournalHeaderId,
                        principalTable: "JournalHeaders",
                        principalColumn: "JournalHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnHeaders_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnHeaders_SalesInvoiceHeaders_SalesInvoiceHe~",
                        column: x => x.SalesInvoiceHeaderId,
                        principalTable: "SalesInvoiceHeaders",
                        principalColumn: "SalesInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnHeaders_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "SellerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnHeaders_ShipmentTypes_ShipmentTypeId",
                        column: x => x.ShipmentTypeId,
                        principalTable: "ShipmentTypes",
                        principalColumn: "ShipmentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnHeaders_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SalesInvoiceSettlements",
                columns: table => new
                {
                    SalesInvoiceSettlementId = table.Column<int>(type: "int", nullable: false),
                    SalesInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    ReceiptVoucherHeaderId = table.Column<int>(type: "int", nullable: false),
                    SettleValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoiceSettlements", x => x.SalesInvoiceSettlementId);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceSettlements_ReceiptVoucherHeaders_ReceiptVoucher~",
                        column: x => x.ReceiptVoucherHeaderId,
                        principalTable: "ReceiptVoucherHeaders",
                        principalColumn: "ReceiptVoucherHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceSettlements_SalesInvoiceHeaders_SalesInvoiceHead~",
                        column: x => x.SalesInvoiceHeaderId,
                        principalTable: "SalesInvoiceHeaders",
                        principalColumn: "SalesInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockOutHeaders",
                columns: table => new
                {
                    StockOutHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StockTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    ProformaInvoiceHeaderId = table.Column<int>(type: "int", nullable: true),
                    SalesInvoiceHeaderId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalItemDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValueBeforeAdditionalDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    AdditionalDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsEnded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsBlocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: true),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockOutHeaders", x => x.StockOutHeaderId);
                    table.ForeignKey(
                        name: "FK_StockOutHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutHeaders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutHeaders_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutHeaders_ProformaInvoiceHeaders_ProformaInvoiceHeader~",
                        column: x => x.ProformaInvoiceHeaderId,
                        principalTable: "ProformaInvoiceHeaders",
                        principalColumn: "ProformaInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutHeaders_SalesInvoiceHeaders_SalesInvoiceHeaderId",
                        column: x => x.SalesInvoiceHeaderId,
                        principalTable: "SalesInvoiceHeaders",
                        principalColumn: "SalesInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutHeaders_StockTypes_StockTypeId",
                        column: x => x.StockTypeId,
                        principalTable: "StockTypes",
                        principalColumn: "StockTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PurchaseInvoiceDetails",
                columns: table => new
                {
                    PurchaseInvoiceDetailId = table.Column<int>(type: "int", nullable: false),
                    PurchaseInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    BonusQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValueAfterDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    HeaderDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemExpensePercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemExpenseValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastPurchasePrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VatTaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    VatTaxId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoiceDetails", x => x.PurchaseInvoiceDetailId);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceDetails_PurchaseInvoiceHeaders_PurchaseInvoic~",
                        column: x => x.PurchaseInvoiceHeaderId,
                        principalTable: "PurchaseInvoiceHeaders",
                        principalColumn: "PurchaseInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceDetails_TaxTypes_VatTaxTypeId",
                        column: x => x.VatTaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceDetails_Taxes_VatTaxId",
                        column: x => x.VatTaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PurchaseInvoiceExpenses",
                columns: table => new
                {
                    PurchaseInvoiceExpenseId = table.Column<int>(type: "int", nullable: false),
                    PurchaseInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    InvoiceExpenseTypeId = table.Column<int>(type: "int", nullable: false),
                    ExpenseValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoiceExpenses", x => x.PurchaseInvoiceExpenseId);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceExpenses_InvoiceExpenseTypes_InvoiceExpenseTy~",
                        column: x => x.InvoiceExpenseTypeId,
                        principalTable: "InvoiceExpenseTypes",
                        principalColumn: "InvoiceExpenseTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceExpenses_PurchaseInvoiceHeaders_PurchaseInvoi~",
                        column: x => x.PurchaseInvoiceHeaderId,
                        principalTable: "PurchaseInvoiceHeaders",
                        principalColumn: "PurchaseInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PurchaseInvoiceReturnHeaders",
                columns: table => new
                {
                    PurchaseInvoiceReturnHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PurchaseInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsDirectInvoice = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreditPayment = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalItemDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValueBeforeAdditionalDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    AdditionalDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    JournalHeaderId = table.Column<int>(type: "int", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsOnTheWay = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsEnded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsBlocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: true),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoiceReturnHeaders", x => x.PurchaseInvoiceReturnHeaderId);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnHeaders_Accounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnHeaders_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnHeaders_JournalHeaders_JournalHeaderId",
                        column: x => x.JournalHeaderId,
                        principalTable: "JournalHeaders",
                        principalColumn: "JournalHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnHeaders_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnHeaders_PurchaseInvoiceHeaders_Purchase~",
                        column: x => x.PurchaseInvoiceHeaderId,
                        principalTable: "PurchaseInvoiceHeaders",
                        principalColumn: "PurchaseInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnHeaders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnHeaders_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PurchaseInvoiceSettlements",
                columns: table => new
                {
                    PurchaseInvoiceSettlementId = table.Column<int>(type: "int", nullable: false),
                    PurchaseInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    PaymentVoucherHeaderId = table.Column<int>(type: "int", nullable: false),
                    SettleValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoiceSettlements", x => x.PurchaseInvoiceSettlementId);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceSettlements_PaymentVoucherHeaders_PaymentVouc~",
                        column: x => x.PaymentVoucherHeaderId,
                        principalTable: "PaymentVoucherHeaders",
                        principalColumn: "PaymentVoucherHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceSettlements_PurchaseInvoiceHeaders_PurchaseIn~",
                        column: x => x.PurchaseInvoiceHeaderId,
                        principalTable: "PurchaseInvoiceHeaders",
                        principalColumn: "PurchaseInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockInHeaders",
                columns: table => new
                {
                    StockInHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StockTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    PurchaseOrderHeaderId = table.Column<int>(type: "int", nullable: true),
                    PurchaseInvoiceHeaderId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalItemDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValueBeforeAdditionalDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    AdditionalDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsEnded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsBlocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: true),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockInHeaders", x => x.StockInHeaderId);
                    table.ForeignKey(
                        name: "FK_StockInHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInHeaders_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInHeaders_PurchaseInvoiceHeaders_PurchaseInvoiceHeaderId",
                        column: x => x.PurchaseInvoiceHeaderId,
                        principalTable: "PurchaseInvoiceHeaders",
                        principalColumn: "PurchaseInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInHeaders_PurchaseOrderHeaders_PurchaseOrderHeaderId",
                        column: x => x.PurchaseOrderHeaderId,
                        principalTable: "PurchaseOrderHeaders",
                        principalColumn: "PurchaseOrderHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInHeaders_StockTypes_StockTypeId",
                        column: x => x.StockTypeId,
                        principalTable: "StockTypes",
                        principalColumn: "StockTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInHeaders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SupplierCreditMemos",
                columns: table => new
                {
                    SupplierCreditMemoId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PurchaseInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    MemoValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    JournalHeaderId = table.Column<int>(type: "int", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierCreditMemos", x => x.SupplierCreditMemoId);
                    table.ForeignKey(
                        name: "FK_SupplierCreditMemos_Accounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCreditMemos_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCreditMemos_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCreditMemos_JournalHeaders_JournalHeaderId",
                        column: x => x.JournalHeaderId,
                        principalTable: "JournalHeaders",
                        principalColumn: "JournalHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCreditMemos_PurchaseInvoiceHeaders_PurchaseInvoiceHe~",
                        column: x => x.PurchaseInvoiceHeaderId,
                        principalTable: "PurchaseInvoiceHeaders",
                        principalColumn: "PurchaseInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCreditMemos_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierCreditMemos_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SupplierDebitMemos",
                columns: table => new
                {
                    SupplierDebitMemoId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PurchaseInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    MemoValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    JournalHeaderId = table.Column<int>(type: "int", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupplierDebitMemos", x => x.SupplierDebitMemoId);
                    table.ForeignKey(
                        name: "FK_SupplierDebitMemos_Accounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierDebitMemos_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierDebitMemos_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierDebitMemos_JournalHeaders_JournalHeaderId",
                        column: x => x.JournalHeaderId,
                        principalTable: "JournalHeaders",
                        principalColumn: "JournalHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierDebitMemos_PurchaseInvoiceHeaders_PurchaseInvoiceHea~",
                        column: x => x.PurchaseInvoiceHeaderId,
                        principalTable: "PurchaseInvoiceHeaders",
                        principalColumn: "PurchaseInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierDebitMemos_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SupplierDebitMemos_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PurchaseOrderDetailTaxes",
                columns: table => new
                {
                    PurchaseOrderDetailTaxId = table.Column<int>(type: "int", nullable: false),
                    PurchaseOrderDetailId = table.Column<int>(type: "int", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    TaxAfterVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderDetailTaxes", x => x.PurchaseOrderDetailTaxId);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetailTaxes_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetailTaxes_PurchaseOrderDetails_PurchaseOrderD~",
                        column: x => x.PurchaseOrderDetailId,
                        principalTable: "PurchaseOrderDetails",
                        principalColumn: "PurchaseOrderDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderDetailTaxes_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SalesInvoiceDetailTaxes",
                columns: table => new
                {
                    SalesInvoiceDetailTaxId = table.Column<int>(type: "int", nullable: false),
                    SalesInvoiceDetailId = table.Column<int>(type: "int", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    TaxAfterVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoiceDetailTaxes", x => x.SalesInvoiceDetailTaxId);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceDetailTaxes_SalesInvoiceDetails_SalesInvoiceDeta~",
                        column: x => x.SalesInvoiceDetailId,
                        principalTable: "SalesInvoiceDetails",
                        principalColumn: "SalesInvoiceDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceDetailTaxes_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceDetailTaxes_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SalesInvoiceReturnDetails",
                columns: table => new
                {
                    SalesInvoiceReturnDetailId = table.Column<int>(type: "int", nullable: false),
                    SalesInvoiceReturnHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    BonusQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValueAfterDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    HeaderDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastSalesPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VatTaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    VatTaxId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoiceReturnDetails", x => x.SalesInvoiceReturnDetailId);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnDetails_SalesInvoiceReturnHeaders_SalesInv~",
                        column: x => x.SalesInvoiceReturnHeaderId,
                        principalTable: "SalesInvoiceReturnHeaders",
                        principalColumn: "SalesInvoiceReturnHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnDetails_TaxTypes_VatTaxTypeId",
                        column: x => x.VatTaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnDetails_Taxes_VatTaxId",
                        column: x => x.VatTaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SalesInvoiceReturnPayments",
                columns: table => new
                {
                    SalesInvoiceReturnPaymentId = table.Column<int>(type: "int", nullable: false),
                    SalesInvoiceReturnHeaderId = table.Column<int>(type: "int", nullable: false),
                    PaymentMethodId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    CurrencyId = table.Column<short>(type: "smallint", nullable: false),
                    CurrencyRate = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    PaidValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    PaidValueAccount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoiceReturnPayments", x => x.SalesInvoiceReturnPaymentId);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnPayments_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnPayments_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnPayments_PaymentMethods_PaymentMethodId",
                        column: x => x.PaymentMethodId,
                        principalTable: "PaymentMethods",
                        principalColumn: "PaymentMethodId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnPayments_SalesInvoiceReturnHeaders_SalesIn~",
                        column: x => x.SalesInvoiceReturnHeaderId,
                        principalTable: "SalesInvoiceReturnHeaders",
                        principalColumn: "SalesInvoiceReturnHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InvoiceStockOuts",
                columns: table => new
                {
                    InvoiceStockOutId = table.Column<int>(type: "int", nullable: false),
                    SalesInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    StockOutHeaderId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceStockOuts", x => x.InvoiceStockOutId);
                    table.ForeignKey(
                        name: "FK_InvoiceStockOuts_SalesInvoiceHeaders_SalesInvoiceHeaderId",
                        column: x => x.SalesInvoiceHeaderId,
                        principalTable: "SalesInvoiceHeaders",
                        principalColumn: "SalesInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceStockOuts_StockOutHeaders_StockOutHeaderId",
                        column: x => x.StockOutHeaderId,
                        principalTable: "StockOutHeaders",
                        principalColumn: "StockOutHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockOutDetails",
                columns: table => new
                {
                    StockOutDetailId = table.Column<int>(type: "int", nullable: false),
                    StockOutHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    BonusQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValueAfterDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    HeaderDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastSalesPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockOutDetails", x => x.StockOutDetailId);
                    table.ForeignKey(
                        name: "FK_StockOutDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutDetails_StockOutHeaders_StockOutHeaderId",
                        column: x => x.StockOutHeaderId,
                        principalTable: "StockOutHeaders",
                        principalColumn: "StockOutHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockOutReturnHeaders",
                columns: table => new
                {
                    StockOutReturnHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StockTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    StockOutHeaderId = table.Column<int>(type: "int", nullable: true),
                    SalesInvoiceHeaderId = table.Column<int>(type: "int", nullable: true),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    SellerId = table.Column<int>(type: "int", nullable: true),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalItemDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValueBeforeAdditionalDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    AdditionalDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsEnded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsBlocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: true),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockOutReturnHeaders", x => x.StockOutReturnHeaderId);
                    table.ForeignKey(
                        name: "FK_StockOutReturnHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutReturnHeaders_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "ClientId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutReturnHeaders_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutReturnHeaders_SalesInvoiceHeaders_SalesInvoiceHeader~",
                        column: x => x.SalesInvoiceHeaderId,
                        principalTable: "SalesInvoiceHeaders",
                        principalColumn: "SalesInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutReturnHeaders_Sellers_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Sellers",
                        principalColumn: "SellerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutReturnHeaders_StockOutHeaders_StockOutHeaderId",
                        column: x => x.StockOutHeaderId,
                        principalTable: "StockOutHeaders",
                        principalColumn: "StockOutHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutReturnHeaders_StockTypes_StockTypeId",
                        column: x => x.StockTypeId,
                        principalTable: "StockTypes",
                        principalColumn: "StockTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutReturnHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PurchaseInvoiceDetailTaxes",
                columns: table => new
                {
                    PurchaseInvoiceDetailTaxId = table.Column<int>(type: "int", nullable: false),
                    PurchaseInvoiceDetailId = table.Column<int>(type: "int", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    TaxAfterVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoiceDetailTaxes", x => x.PurchaseInvoiceDetailTaxId);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceDetailTaxes_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceDetailTaxes_PurchaseInvoiceDetails_PurchaseIn~",
                        column: x => x.PurchaseInvoiceDetailId,
                        principalTable: "PurchaseInvoiceDetails",
                        principalColumn: "PurchaseInvoiceDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceDetailTaxes_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceDetailTaxes_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PurchaseInvoiceReturnDetails",
                columns: table => new
                {
                    PurchaseInvoiceReturnDetailId = table.Column<int>(type: "int", nullable: false),
                    PurchaseInvoiceReturnHeaderId = table.Column<int>(type: "int", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    BonusQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValueAfterDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    HeaderDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastPurchasePrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    VatTaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    VatTaxId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoiceReturnDetails", x => x.PurchaseInvoiceReturnDetailId);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnDetails_PurchaseInvoiceReturnHeaders_Pu~",
                        column: x => x.PurchaseInvoiceReturnHeaderId,
                        principalTable: "PurchaseInvoiceReturnHeaders",
                        principalColumn: "PurchaseInvoiceReturnHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnDetails_TaxTypes_VatTaxTypeId",
                        column: x => x.VatTaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnDetails_Taxes_VatTaxId",
                        column: x => x.VatTaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InvoiceStockIns",
                columns: table => new
                {
                    InvoiceStockInId = table.Column<int>(type: "int", nullable: false),
                    PurchaseInvoiceHeaderId = table.Column<int>(type: "int", nullable: false),
                    StockInHeaderId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceStockIns", x => x.InvoiceStockInId);
                    table.ForeignKey(
                        name: "FK_InvoiceStockIns_PurchaseInvoiceHeaders_PurchaseInvoiceHeader~",
                        column: x => x.PurchaseInvoiceHeaderId,
                        principalTable: "PurchaseInvoiceHeaders",
                        principalColumn: "PurchaseInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceStockIns_StockInHeaders_StockInHeaderId",
                        column: x => x.StockInHeaderId,
                        principalTable: "StockInHeaders",
                        principalColumn: "StockInHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockInDetails",
                columns: table => new
                {
                    StockInDetailId = table.Column<int>(type: "int", nullable: false),
                    StockInHeaderId = table.Column<int>(type: "int", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    BonusQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValueAfterDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    HeaderDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastPurchasePrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockInDetails", x => x.StockInDetailId);
                    table.ForeignKey(
                        name: "FK_StockInDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInDetails_StockInHeaders_StockInHeaderId",
                        column: x => x.StockInHeaderId,
                        principalTable: "StockInHeaders",
                        principalColumn: "StockInHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockInReturnHeaders",
                columns: table => new
                {
                    StockInReturnHeaderId = table.Column<int>(type: "int", nullable: false),
                    Prefix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentCode = table.Column<int>(type: "int", nullable: false),
                    Suffix = table.Column<string>(type: "varchar(10)", maxLength: 10, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    DocumentReference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StockTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    StockInHeaderId = table.Column<int>(type: "int", nullable: true),
                    PurchaseInvoiceHeaderId = table.Column<int>(type: "int", nullable: true),
                    SupplierId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    DocumentDate = table.Column<DateTime>(type: "Date", nullable: false),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reference = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalItemDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValueBeforeAdditionalDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    AdditionalDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalCostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    RemarksAr = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RemarksEn = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsClosed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsEnded = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsBlocked = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    MenuCode = table.Column<short>(type: "smallint", nullable: true),
                    ArchiveHeaderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockInReturnHeaders", x => x.StockInReturnHeaderId);
                    table.ForeignKey(
                        name: "FK_StockInReturnHeaders_ArchiveHeaders_ArchiveHeaderId",
                        column: x => x.ArchiveHeaderId,
                        principalTable: "ArchiveHeaders",
                        principalColumn: "ArchiveHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInReturnHeaders_Menus_MenuCode",
                        column: x => x.MenuCode,
                        principalTable: "Menus",
                        principalColumn: "MenuCode",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInReturnHeaders_PurchaseInvoiceHeaders_PurchaseInvoiceH~",
                        column: x => x.PurchaseInvoiceHeaderId,
                        principalTable: "PurchaseInvoiceHeaders",
                        principalColumn: "PurchaseInvoiceHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInReturnHeaders_StockInHeaders_StockInHeaderId",
                        column: x => x.StockInHeaderId,
                        principalTable: "StockInHeaders",
                        principalColumn: "StockInHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInReturnHeaders_StockTypes_StockTypeId",
                        column: x => x.StockTypeId,
                        principalTable: "StockTypes",
                        principalColumn: "StockTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInReturnHeaders_Stores_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Stores",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInReturnHeaders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "SupplierId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SalesInvoiceReturnDetailTaxes",
                columns: table => new
                {
                    SalesInvoiceReturnDetailTaxId = table.Column<int>(type: "int", nullable: false),
                    SalesInvoiceReturnDetailId = table.Column<int>(type: "int", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    TaxAfterVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesInvoiceReturnDetailTaxes", x => x.SalesInvoiceReturnDetailTaxId);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnDetailTaxes_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnDetailTaxes_SalesInvoiceReturnDetails_Sale~",
                        column: x => x.SalesInvoiceReturnDetailId,
                        principalTable: "SalesInvoiceReturnDetails",
                        principalColumn: "SalesInvoiceReturnDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnDetailTaxes_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SalesInvoiceReturnDetailTaxes_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockOutDetailTaxes",
                columns: table => new
                {
                    StockOutDetailTaxId = table.Column<int>(type: "int", nullable: false),
                    StockOutDetailId = table.Column<int>(type: "int", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    TaxAfterVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockOutDetailTaxes", x => x.StockOutDetailTaxId);
                    table.ForeignKey(
                        name: "FK_StockOutDetailTaxes_StockOutDetails_StockOutDetailId",
                        column: x => x.StockOutDetailId,
                        principalTable: "StockOutDetails",
                        principalColumn: "StockOutDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutDetailTaxes_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InvoiceStockOutReturns",
                columns: table => new
                {
                    InvoiceStockOutReturnId = table.Column<int>(type: "int", nullable: false),
                    StockOutReturnHeaderId = table.Column<int>(type: "int", nullable: false),
                    SalesInvoiceReturnHeaderId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceStockOutReturns", x => x.InvoiceStockOutReturnId);
                    table.ForeignKey(
                        name: "FK_InvoiceStockOutReturns_SalesInvoiceReturnHeaders_SalesInvoic~",
                        column: x => x.SalesInvoiceReturnHeaderId,
                        principalTable: "SalesInvoiceReturnHeaders",
                        principalColumn: "SalesInvoiceReturnHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceStockOutReturns_StockOutReturnHeaders_StockOutReturnH~",
                        column: x => x.StockOutReturnHeaderId,
                        principalTable: "StockOutReturnHeaders",
                        principalColumn: "StockOutReturnHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockOutReturnDetails",
                columns: table => new
                {
                    StockOutReturnDetailId = table.Column<int>(type: "int", nullable: false),
                    StockOutReturnHeaderId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    BonusQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SellingPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValueAfterDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    HeaderDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastSalesPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockOutReturnDetails", x => x.StockOutReturnDetailId);
                    table.ForeignKey(
                        name: "FK_StockOutReturnDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutReturnDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutReturnDetails_StockOutReturnHeaders_StockOutReturnHe~",
                        column: x => x.StockOutReturnHeaderId,
                        principalTable: "StockOutReturnHeaders",
                        principalColumn: "StockOutReturnHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PurchaseInvoiceReturnDetailTaxes",
                columns: table => new
                {
                    PurchaseInvoiceReturnDetailTaxId = table.Column<int>(type: "int", nullable: false),
                    PurchaseInvoiceReturnDetailId = table.Column<int>(type: "int", nullable: false),
                    TaxTypeId = table.Column<byte>(type: "tinyint unsigned", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    TaxAfterVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseInvoiceReturnDetailTaxes", x => x.PurchaseInvoiceReturnDetailTaxId);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnDetailTaxes_Accounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnDetailTaxes_PurchaseInvoiceReturnDetail~",
                        column: x => x.PurchaseInvoiceReturnDetailId,
                        principalTable: "PurchaseInvoiceReturnDetails",
                        principalColumn: "PurchaseInvoiceReturnDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnDetailTaxes_TaxTypes_TaxTypeId",
                        column: x => x.TaxTypeId,
                        principalTable: "TaxTypes",
                        principalColumn: "TaxTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PurchaseInvoiceReturnDetailTaxes_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockInDetailTaxes",
                columns: table => new
                {
                    StockInDetailTaxId = table.Column<int>(type: "int", nullable: false),
                    StockInDetailId = table.Column<int>(type: "int", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    TaxAfterVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockInDetailTaxes", x => x.StockInDetailTaxId);
                    table.ForeignKey(
                        name: "FK_StockInDetailTaxes_Accounts_DebitAccountId",
                        column: x => x.DebitAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInDetailTaxes_StockInDetails_StockInDetailId",
                        column: x => x.StockInDetailId,
                        principalTable: "StockInDetails",
                        principalColumn: "StockInDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInDetailTaxes_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "InvoiceStockInReturns",
                columns: table => new
                {
                    InvoiceStockInReturnId = table.Column<int>(type: "int", nullable: false),
                    StockInReturnHeaderId = table.Column<int>(type: "int", nullable: false),
                    PurchaseInvoiceReturnHeaderId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceStockInReturns", x => x.InvoiceStockInReturnId);
                    table.ForeignKey(
                        name: "FK_InvoiceStockInReturns_PurchaseInvoiceReturnHeaders_PurchaseI~",
                        column: x => x.PurchaseInvoiceReturnHeaderId,
                        principalTable: "PurchaseInvoiceReturnHeaders",
                        principalColumn: "PurchaseInvoiceReturnHeaderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InvoiceStockInReturns_StockInReturnHeaders_StockInReturnHead~",
                        column: x => x.StockInReturnHeaderId,
                        principalTable: "StockInReturnHeaders",
                        principalColumn: "StockInReturnHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockInReturnDetails",
                columns: table => new
                {
                    StockInReturnDetailId = table.Column<int>(type: "int", nullable: false),
                    StockInReturnHeaderId = table.Column<int>(type: "int", nullable: false),
                    CostCenterId = table.Column<int>(type: "int", nullable: true),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    ItemPackageId = table.Column<int>(type: "int", nullable: false),
                    IsItemVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    BarCode = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Packing = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ExpireDate = table.Column<DateTime>(type: "Date", nullable: true),
                    BatchNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Quantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    BonusQuantity = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TotalValueAfterDiscount = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    HeaderDiscountValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    GrossValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    VatValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    SubNetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    OtherTaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    NetValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ConsumerPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostPackage = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CostValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    LastPurchasePrice = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    ItemNote = table.Column<string>(type: "varchar(2000)", maxLength: 2000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockInReturnDetails", x => x.StockInReturnDetailId);
                    table.ForeignKey(
                        name: "FK_StockInReturnDetails_ItemPackages_ItemPackageId",
                        column: x => x.ItemPackageId,
                        principalTable: "ItemPackages",
                        principalColumn: "ItemPackageId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInReturnDetails_Items_ItemId",
                        column: x => x.ItemId,
                        principalTable: "Items",
                        principalColumn: "ItemId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInReturnDetails_StockInReturnHeaders_StockInReturnHeade~",
                        column: x => x.StockInReturnHeaderId,
                        principalTable: "StockInReturnHeaders",
                        principalColumn: "StockInReturnHeaderId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockOutReturnDetailTaxes",
                columns: table => new
                {
                    StockOutReturnDetailTaxId = table.Column<int>(type: "int", nullable: false),
                    StockOutReturnDetailId = table.Column<int>(type: "int", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    TaxAfterVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    DebitAccountId = table.Column<int>(type: "int", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockOutReturnDetailTaxes", x => x.StockOutReturnDetailTaxId);
                    table.ForeignKey(
                        name: "FK_StockOutReturnDetailTaxes_StockOutReturnDetails_StockOutRetu~",
                        column: x => x.StockOutReturnDetailId,
                        principalTable: "StockOutReturnDetails",
                        principalColumn: "StockOutReturnDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockOutReturnDetailTaxes_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StockInReturnDetailTaxes",
                columns: table => new
                {
                    StockInReturnDetailTaxId = table.Column<int>(type: "int", nullable: false),
                    StockInReturnDetailId = table.Column<int>(type: "int", nullable: false),
                    TaxId = table.Column<int>(type: "int", nullable: false),
                    TaxAfterVatInclusive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreditAccountId = table.Column<int>(type: "int", nullable: false),
                    TaxPercent = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    TaxValue = table.Column<decimal>(type: "decimal(30,15)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UserNameCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserNameModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressCreated = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddressModified = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Hide = table.Column<bool>(type: "tinyint(1)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockInReturnDetailTaxes", x => x.StockInReturnDetailTaxId);
                    table.ForeignKey(
                        name: "FK_StockInReturnDetailTaxes_Accounts_CreditAccountId",
                        column: x => x.CreditAccountId,
                        principalTable: "Accounts",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInReturnDetailTaxes_StockInReturnDetails_StockInReturnD~",
                        column: x => x.StockInReturnDetailId,
                        principalTable: "StockInReturnDetails",
                        principalColumn: "StockInReturnDetailId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockInReturnDetailTaxes_Taxes_TaxId",
                        column: x => x.TaxId,
                        principalTable: "Taxes",
                        principalColumn: "TaxId",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AccountLedgers",
                columns: new[] { "AccountLedgerId", "AccountLedgerNameAr", "AccountLedgerNameEn", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, "مركز مالي", "Balance Sheet", null, null, null, null, null, null, null },
                    { (byte)2, "قائمة دخل", "Income Statement", null, null, null, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "AccountStoreTypes",
                columns: new[] { "AccountStoreTypeId", "AccountStoreTypeNameAr", "AccountStoreTypeNameEn", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { 1, "مخزون أول مدة", "Beginning Inventory", null, null, null, null, null, null, null },
                    { 2, "مخزون آخر مدة", "Ending Inventory", null, null, null, null, null, null, null },
                    { 3, "المخزون", "Inventory", null, null, null, null, null, null, null },
                    { 4, "تكلفة الإيرادات", "Cost of Revenue", null, null, null, null, null, null, null },
                    { 5, "أرباح وخسائر", "Profit and Loss", null, null, null, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "AccountTypes",
                columns: new[] { "AccountTypeId", "AccountTypeNameAr", "AccountTypeNameEn", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "IsInternalSystem", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, "النقدية", "Cash", null, null, null, null, false, null, null, null },
                    { (byte)2, "العملاء", "Clients", null, null, null, null, false, null, null, null },
                    { (byte)3, "الموردين", "Suppliers", null, null, null, null, false, null, null, null },
                    { (byte)4, "البنوك", "Banks", null, null, null, null, false, null, null, null },
                    { (byte)5, "فرق تقريب كسور", "Fractional Approximation Difference", null, null, null, null, true, null, null, null },
                    { (byte)6, "أصول ثابتة", "Fixed Assets", null, null, null, null, false, null, null, null },
                    { (byte)7, "مجمع الاهلاك", "Accumulated Depreciation", null, null, null, null, false, null, null, null },
                    { (byte)8, "الاهلاكات", "Depreciation", null, null, null, null, false, null, null, null },
                    { (byte)9, "حقوق الملكية", "Ownership Equity", null, null, null, null, false, null, null, null },
                    { (byte)10, "المشتريات", "Purchases", null, null, null, null, false, null, null, null },
                    { (byte)11, "تكلفة الايرادات", "Revenues Cost", null, null, null, null, false, null, null, null },
                    { (byte)12, "المبيعات", "Sales", null, null, null, null, false, null, null, null },
                    { (byte)13, "مصروفات عمومية وادارية", "Miscellaneous Expenses", null, null, null, null, false, null, null, null },
                    { (byte)14, "ايرادات متنوعة", "Miscellaneous Income", null, null, null, null, false, null, null, null },
                    { (byte)15, "خصم مسموح به", "Allowed Discount", null, null, null, null, true, null, null, null },
                    { (byte)16, "المخزون", "Inventory", null, null, null, null, false, null, null, null },
                    { (byte)17, "حساب المخزون", "Inventory Account", null, null, null, null, true, null, null, null },
                    { (byte)18, "حساب تكلفة المبيعات", "Revenues Cost Account", null, null, null, null, true, null, null, null },
                    { (byte)50, "أخري", "Other", null, null, null, null, false, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "ApplicationFlagTabs",
                columns: new[] { "ApplicationFlagTabId", "ApplicationFlagTabNameAr", "ApplicationFlagTabNameEn", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "Order", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { 1, "تقسيمات الأصناف", "Items Categories", null, null, null, null, null, (short)4, null, null },
                    { 2, "ترميز الدليل المحاسبي", "Charts of Account Encoding", null, null, null, null, null, (short)5, null, null },
                    { 3, "ترميز مراكز التكلفة", "Cost Centers Encoding", null, null, null, null, null, (short)6, null, null },
                    { 4, "أسعار التكلفة", "Item Cost", null, null, null, null, null, (short)7, null, null },
                    { 5, "الأصناف", "Items", null, null, null, null, null, (short)3, null, null },
                    { 6, "فاتورة المشتريات", "Purchase Invoice", null, null, null, null, null, (short)8, null, null },
                    { 7, "فاتورة مرتجع المشتريات", "Purchase Invoice Return", null, null, null, null, null, (short)9, null, null },
                    { 8, "عرض أسعار العميل", "Client Quotation", null, null, null, null, null, (short)10, null, null },
                    { 9, "فاتورة المبيعات", "Sales Invoice", null, null, null, null, null, (short)11, null, null },
                    { 10, "فاتورة مرتجع المبيعات", "Sales Invoice Return", null, null, null, null, null, (short)12, null, null },
                    { 11, "إعدادات عامة", "General Settings", null, null, null, null, null, (short)1, null, null },
                    { 12, "إعدادات الطباعة", "Print Settings", null, null, null, null, null, (short)2, null, null }
                });

            migrationBuilder.InsertData(
                table: "ApplicationFlagTypes",
                columns: new[] { "ApplicationFlagTypeId", "ApplicationFlagTypeNameAr", "ApplicationFlagTypeNameEn", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, "نص", "Text", null, null, null, null, null, null, null },
                    { (byte)2, "رقم", "Number", null, null, null, null, null, null, null },
                    { (byte)3, "تاريخ", "Date", null, null, null, null, null, null, null },
                    { (byte)4, "وقت", "Time", null, null, null, null, null, null, null },
                    { (byte)5, "تاريخ محدد بوقت", "DateTime", null, null, null, null, null, null, null },
                    { (byte)6, "نعم/لا", "Boolean", null, null, null, null, null, null, null },
                    { (byte)7, "اختيار", "Select", null, null, null, null, null, null, null },
                    { (byte)8, "رفع صورة", "Upload Image", null, null, null, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "ApproveRequestTypes",
                columns: new[] { "ApproveRequestTypeId", "ApproveRequestTypeNameAr", "ApproveRequestTypeNameEn", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, "إضافة جديد", "Create New", null, null, null, null, null, null, null },
                    { (byte)2, "تعديل البيانات", "Edit Data", null, null, null, null, null, null, null },
                    { (byte)3, "حذف", "Delete", null, null, null, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "ColumnIdentifiers",
                columns: new[] { "ColumnIdentifierId", "ColumnIdentifierNameAr", "ColumnIdentifierNameEn", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, "نص", "Text", null, null, null, null, null, null, null },
                    { (byte)2, "رقم", "Number", null, null, null, null, null, null, null },
                    { (byte)3, "تاريخ", "Date", null, null, null, null, null, null, null },
                    { (byte)4, "نعم/لا", "true/false", null, null, null, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "DocumentStatuses",
                columns: new[] { "DocumentStatusId", "CreatedAt", "DocumentStatusNameAr", "DocumentStatusNameEn", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { 1, null, "تم إنشاء إتفاقية شراء", "Purchase order has been created", null, null, null, null, null, null },
                    { 2, null, "تم إستلام الكمية جزئياً", "The quantity has been partially received", null, null, null, null, null, null },
                    { 3, null, "تم إستلام الكمية بالكامل", "The entire quantity has been received", null, null, null, null, null, null },
                    { 4, null, "تم عمل فاتورة المشتريات", "Purchase invoice has been created", null, null, null, null, null, null },
                    { 5, null, "تم عمل فاتورة المشتريات لبعض البضائع", "Purchase invoice has been created for some of the items", null, null, null, null, null, null },
                    { 6, null, "تم عمل فاتورة المشتريات لكل البضائع", "Purchase invoice has been created for all of the items", null, null, null, null, null, null },
                    { 7, null, "تم عمل فاتورة المشتريات وفي انتظار الاستلام", "purchase invoice has been created and is awaiting receipt", null, null, null, null, null, null },
                    { 8, null, "تم عمل مرتجع للكمية جزئياً وفي انتظار فاتورة مرتجع المشتريات", "The quantity has been partially returned and the purchase return invoice is awaited", null, null, null, null, null, null },
                    { 9, null, "تم عمل مرتجع بالكمية بالكامل وفي انتظار فاتورة مرتجع المشتريات", "The entire quantity has been returned and the purchase return invoice is awaited", null, null, null, null, null, null },
                    { 10, null, "تم عمل فاتورة مرتجع المشتريات", "Purchase return invoice has been created", null, null, null, null, null, null },
                    { 11, null, "تم عمل فاتورة مرتجع المشتريات لبعض البضائع", "Purchase return invoice has been created for some of the items", null, null, null, null, null, null },
                    { 12, null, "تم عمل فاتورة مرتجع المشتريات لكل البضائع", "Purchase return invoice has been created for all of the items", null, null, null, null, null, null },
                    { 13, null, "تم إنشاء الفاتورة الأولية", "Proforma invoice has been created", null, null, null, null, null, null },
                    { 14, null, "تم صرف الكمية جزئياً", "The quantity has been partially disbursed", null, null, null, null, null, null },
                    { 15, null, "تم صرف الكمية بالكامل", "The entire quantity has been disbursed", null, null, null, null, null, null },
                    { 16, null, "تم عمل فاتورة المبيعات", "Sales invoice has been created", null, null, null, null, null, null },
                    { 17, null, "تم عمل فاتورة المبيعات لبعض البضائع", "Sales invoice has been created for some of the items", null, null, null, null, null, null },
                    { 18, null, "تم عمل فاتورة المبيعات لكل البضائع", "Sales invoice has been created for all of the items", null, null, null, null, null, null },
                    { 19, null, "تم عمل مرتجع للكمية المصروفة جزئياً وفي انتظار فاتورة مرتجع المبيعات", "The partially disbursed quantity has been returned and the sales return invoice is awaited", null, null, null, null, null, null },
                    { 20, null, "تم عمل مرتجع للكمية المصروفة بالكامل وفي انتظار فاتورة مرتجع المبيعات", "The entire quantity disbursed has been returned and the sales return invoice is awaited", null, null, null, null, null, null },
                    { 21, null, "تم عمل فاتورة مرتجع المبيعات", "Sales return invoice has been created", null, null, null, null, null, null },
                    { 22, null, "تم عمل فاتورة مرتجع المبيعات لبعض البضائع", "Sales return invoice has been created for some of the items", null, null, null, null, null, null },
                    { 23, null, "تم عمل فاتورة مرتجع المبيعات لكل البضائع", "Sales return invoice has been created for all of the items", null, null, null, null, null, null },
                    { 24, null, "تم عمل إشعار مدين المورد", "Supplier debit memo has been created", null, null, null, null, null, null },
                    { 25, null, "تم عمل إشعار دائن المورد", "Supplier credit memo has been created", null, null, null, null, null, null },
                    { 26, null, "تم عمل إشعار مدين للعميل", "Client debit memo has been created", null, null, null, null, null, null },
                    { 27, null, "تم عمل إشعار دائن للعميل", "Client credit memo has been created", null, null, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "DocumentTypes",
                columns: new[] { "DocumentTypeId", "CreatedAt", "DocumentTypeNameAr", "DocumentTypeNameEn", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (short)1, null, "الجرد والأرصدة الافتتاحية", "StockTaking And Open Balances", null, null, null, null, null, null },
                    { (short)2, null, "اعتماد الجرد كرصيد إفتتاحي", "Approval of StockTaking as Open Balance", null, null, null, null, null, null },
                    { (short)3, null, "الجرد والأرصدة الحالية", "StockTaking And Current Balances", null, null, null, null, null, null },
                    { (short)4, null, "اعتماد الجرد كرصيد حالي", "Approval of StockTaking as Current Balance", null, null, null, null, null, null },
                    { (short)5, null, "سند استلام", "Inventory In", null, null, null, null, null, null },
                    { (short)6, null, "سند تسليم", "Inventory Out", null, null, null, null, null, null },
                    { (short)7, null, "نقل بضاعة داخلي", "Internal Transfer Items", null, null, null, null, null, null },
                    { (short)8, null, "استلام بضاعة داخلي", "Internal Receive Items", null, null, null, null, null, null },
                    { (short)9, null, "قيد يومية", "Journal Entry", null, null, null, null, null, null },
                    { (short)10, null, "سند قبض", "Receipt Voucher", null, null, null, null, null, null },
                    { (short)11, null, "سند صرف", "Payment Voucher", null, null, null, null, null, null },
                    { (short)12, null, "طلب بضاعة", "Product Request", null, null, null, null, null, null },
                    { (short)13, null, "طلب تسعير بضاعة", "Product Request Price", null, null, null, null, null, null },
                    { (short)14, null, "عرض أسعار المورد", "Supplier Quotation", null, null, null, null, null, null },
                    { (short)15, null, "فاتورة شراء نقداً", "Cash Purchase Invoice", null, null, null, null, null, null },
                    { (short)16, null, "فاتورة شراء آجل", "Credit Purchase Invoice", null, null, null, null, null, null },
                    { (short)17, null, "فاتورة مرتجع شراء نقداً", "Cash Purchase Invoice Return", null, null, null, null, null, null },
                    { (short)18, null, "فاتورة مرتجع شراء آجل", "Credit Purchase Invoice Return", null, null, null, null, null, null },
                    { (short)19, null, "اتفاقية شراء", "Purchase Order", null, null, null, null, null, null },
                    { (short)20, null, "بيان استلام", "Receipt Statement", null, null, null, null, null, null },
                    { (short)21, null, "مرتجع من بيان استلام", "Receipt Statement Return", null, null, null, null, null, null },
                    { (short)22, null, "فاتورة مشتريات مرحلية", "Purchase Invoice Interim ", null, null, null, null, null, null },
                    { (short)23, null, "فاتورة مشتريات بضاعة بالطريق نقداً", "Purchase Invoice On The Way Cash", null, null, null, null, null, null },
                    { (short)24, null, "فاتورة مشتريات بضاعة بالطريق آجل", "Purchase Invoice On The Way Credit", null, null, null, null, null, null },
                    { (short)25, null, "استلام من فاتورة بضاعة بالطريق", "Receipt From Purchase Invoice On The Way ", null, null, null, null, null, null },
                    { (short)26, null, "فاتورة مرتجع بضاعة بالطريق", "Purchase Invoice Return On The Way", null, null, null, null, null, null },
                    { (short)27, null, "مرتجع من فاتورة المشتريات", "Return From Purchase Invoice", null, null, null, null, null, null },
                    { (short)28, null, "فاتورة مرتجع المشتريات", "Purchase Invoice Return", null, null, null, null, null, null },
                    { (short)29, null, "إشعار مدين", "Supplier Debit Memo", null, null, null, null, null, null },
                    { (short)30, null, "إشعار دائن", "Supplier Credit Memo", null, null, null, null, null, null },
                    { (short)31, null, "طلب تسعير", "Client Price Request", null, null, null, null, null, null },
                    { (short)32, null, "عرض أسعار", "Client Quotation", null, null, null, null, null, null },
                    { (short)33, null, "اعتماد عرض أسعار", "Client Quotation Approval", null, null, null, null, null, null },
                    { (short)34, null, "فاتورة بيع نقداً", "Cash Sales Invoice", null, null, null, null, null, null },
                    { (short)35, null, "فاتورة بيع آجل", "Credit Sales Invoice", null, null, null, null, null, null },
                    { (short)36, null, "فاتورة مرتجع بيع نقداً", "Cash Sales Invoice Return", null, null, null, null, null, null },
                    { (short)37, null, "فاتورة مرتجع بيع آجل", "Credit Sales Invoice Return", null, null, null, null, null, null },
                    { (short)38, null, "اتفاقية بيع", "Proforma Invoice", null, null, null, null, null, null },
                    { (short)39, null, "بيان تسليم", "Stock Out From Proforma Invoice", null, null, null, null, null, null },
                    { (short)40, null, "مرتجع من بيان تسليم", "Stock Out Return From Stock Out", null, null, null, null, null, null },
                    { (short)41, null, "فاتورة مرحلية", "Sales Invoice Interim", null, null, null, null, null, null },
                    { (short)42, null, "فاتورة حجز نقدي", "Cash Reservation Invoice", null, null, null, null, null, null },
                    { (short)43, null, "فاتورة حجز آجل", "Credit Reservation Invoice", null, null, null, null, null, null },
                    { (short)44, null, "تسليم محجوز", "Stock Out From Reservation", null, null, null, null, null, null },
                    { (short)45, null, "مرتجع من تسليم محجوز", "Stock Out Return From Reservation", null, null, null, null, null, null },
                    { (short)46, null, "فاتورة تصفية حجز", "Reservation Invoice Close Out", null, null, null, null, null, null },
                    { (short)47, null, "مرتجع من فاتورة", "Stock Out Return From Invoice", null, null, null, null, null, null },
                    { (short)48, null, "فاتورة مرتجع", "Sales Invoice Return", null, null, null, null, null, null },
                    { (short)49, null, "إشعار مدين للعميل", "Debit Memo", null, null, null, null, null, null },
                    { (short)50, null, "إشعار دائن للعميل", "Credit Memo", null, null, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "EntityTypes",
                columns: new[] { "EntityTypeId", "CreatedAt", "EntityTypeNameAr", "EntityTypeNameEn", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "Order", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, null, "مورد", "Supplier", null, null, null, null, (byte)1, null, null },
                    { (byte)2, null, "عميل", "Client", null, null, null, null, (byte)2, null, null },
                    { (byte)3, null, "مندوب مبيعات", "Seller", null, null, null, null, (byte)3, null, null },
                    { (byte)4, null, "بنك", "Bank", null, null, null, null, (byte)4, null, null }
                });

            migrationBuilder.InsertData(
                table: "FixedAssetVoucherTypes",
                columns: new[] { "FixedAssetVoucherTypeId", "CreatedAt", "FixedAssetVoucherTypeNameAr", "FixedAssetVoucherTypeNameEn", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, null, "إضافة", "Addition", null, null, null, null, null, null },
                    { (byte)2, null, "إستبعاد", "Exclusion", null, null, null, null, null, null },
                    { (byte)3, null, "إهلاك", "Depreciation", null, null, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "InvoiceTypes",
                columns: new[] { "InvoiceTypeId", "CreatedAt", "Hide", "InvoiceTypeNameAr", "InvoiceTypeNameEn", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)0, null, null, "غير معرف", "unknown", null, null, null, null, null },
                    { (byte)1, null, null, "فاتورة ضريبية ", "Tax Invoice", null, null, null, null, null },
                    { (byte)2, null, null, "فاتورة ضريبية مبسطة", "Simplified Tax Invoice", null, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "ItemCostCalculationType",
                columns: new[] { "ItemCostCalculationTypeId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ItemCostCalculationTypeNameAr", "ItemCostCalculationTypeNameEn", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, null, null, null, null, "المتوسط الفعلي", "Actual Average", null, null, null },
                    { (byte)2, null, null, null, null, "السعر الإفتتاحي", "Opening Price", null, null, null },
                    { (byte)3, null, null, null, null, "آخر سعر شراء", "last Purchase Price", null, null, null },
                    { (byte)4, null, null, null, null, "آخر تكلفة شراء", "Last Cost Price", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "ItemPackages",
                columns: new[] { "ItemPackageId", "CompanyId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ItemPackageCode", "ModifiedAt", "PackageCode", "PackageNameAr", "PackageNameEn", "UserNameCreated", "UserNameModified" },
                values: new object[] { 1, null, null, null, null, null, 1, null, null, "حبة", "each", null, null });

            migrationBuilder.InsertData(
                table: "ItemTypes",
                columns: new[] { "ItemTypeId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ItemTypeNameAr", "ItemTypeNameEn", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, null, null, null, null, "بضاعة", "Goods", null, null, null },
                    { (byte)2, null, null, null, null, "مجمع", "Combined", null, null, null },
                    { (byte)3, null, null, null, null, "مصنع", "Manufactured", null, null, null },
                    { (byte)4, null, null, null, null, "مركب", "Assembled", null, null, null },
                    { (byte)5, null, null, null, null, "خدمة", "Service", null, null, null },
                    { (byte)6, null, null, null, null, "ملاحظة", "Note", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "JournalTypes",
                columns: new[] { "JournalTypeId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "JournalTypeNameAr", "JournalTypeNameEn", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, null, null, null, null, "رصيد افتتاحي", "Opening Balance", null, null, null },
                    { (byte)2, null, null, null, null, "فاتورة بيع نقداً", "Cash Sales Invoice", null, null, null },
                    { (byte)3, null, null, null, null, "فاتورة بيع آجل", "Credit Sales Invoice", null, null, null },
                    { (byte)4, null, null, null, null, "فاتورة مرتجع بيع نقداً", "Cash Invoice Return", null, null, null },
                    { (byte)5, null, null, null, null, "فاتورة مرتجع بيع آجل", "Credit Sales Invoice Return", null, null, null },
                    { (byte)6, null, null, null, null, "فاتورة شراء نقداً", "Cash Purchase Invoice", null, null, null },
                    { (byte)7, null, null, null, null, "فاتورة شراء آجل", "Credit Purchase Invoice", null, null, null },
                    { (byte)8, null, null, null, null, "فاتورة مرتجع شراء نقداً", "Cash Purchase Invoice Return", null, null, null },
                    { (byte)9, null, null, null, null, "فاتورة مرتجع شراء آجل", "Credit Purchase Invoice Return", null, null, null },
                    { (byte)10, null, null, null, null, "سند صرف شيكات", "Check Cashing Voucher", null, null, null },
                    { (byte)11, null, null, null, null, "قيد يومية", "Journal Entry", null, null, null },
                    { (byte)12, null, null, null, null, "سند قبض", "Receipt Voucher", null, null, null },
                    { (byte)13, null, null, null, null, "سند قبض إيجار", "Rent Receipt Voucher", null, null, null },
                    { (byte)14, null, null, null, null, "سند صرف", "Cashing Voucher", null, null, null },
                    { (byte)15, null, null, null, null, "فاتورة صرف", "Cashing Invoice", null, null, null },
                    { (byte)16, null, null, null, null, "فاتورة استلام", "Receipt Invoice", null, null, null },
                    { (byte)17, null, null, null, null, "قيد ايجار", "Rent Entry", null, null, null },
                    { (byte)18, null, null, null, null, "إشعار مدين المورد", "Supplier Debit Notice", null, null, null },
                    { (byte)19, null, null, null, null, "إشعار دائن المورد", "Supplier Credit Notice", null, null, null },
                    { (byte)20, null, null, null, null, "إشعار مدين للعميل", "Client Debit Notice", null, null, null },
                    { (byte)21, null, null, null, null, "إشعار دائن للعميل", "Client Credit Notice", null, null, null },
                    { (byte)22, null, null, null, null, "أصل ثابت", "Fixed Asset", null, null, null },
                    { (byte)23, null, null, null, null, "فاتورة مشتريات بضاعة بالطريق نقداً", "Purchase Invoice On The Way Cash", null, null, null },
                    { (byte)24, null, null, null, null, "فاتورة مشتريات بضاعة بالطريق آجل", "Purchase Invoice On The Way Credit", null, null, null },
                    { (byte)25, null, null, null, null, "فاتورة مشتريات مرحلية", "Purchase Invoice Interim", null, null, null },
                    { (byte)26, null, null, null, null, "فاتورة بيع حجز نقدي", "Cash Reservation Sales Invoice", null, null, null },
                    { (byte)27, null, null, null, null, "فاتورة بيع حجز آجل", "Credit Reservation Sales Invoice", null, null, null },
                    { (byte)28, null, null, null, null, "فاتورة بيع مرحلية", "Sales Invoice Interim", null, null, null },
                    { (byte)29, null, null, null, null, "فاتورة مرتجع شراء بضاعة بالطريق", "Purchase Invoice Return On The Way", null, null, null },
                    { (byte)30, null, null, null, null, "فاتورة مرتجع ما بعد الشراء", "Purchase Invoice Return", null, null, null },
                    { (byte)31, null, null, null, null, "فاتورة مرتجع بيع لتصفية الحجز", "Reservation Sales Invoice Close Out", null, null, null },
                    { (byte)32, null, null, null, null, "فاتورة مرتجع ما بعد البيع", "Sales Invoice Return", null, null, null },
                    { (byte)33, null, null, null, null, "اهلاك اصل ثابت", "Fixed Asset Depreciation", null, null, null },
                    { (byte)34, null, null, null, null, "استبعاد اصل ثابت", "Fixed Asset Exclusion", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "Menus",
                columns: new[] { "MenuCode", "CreatedAt", "HasApprove", "HasEncoding", "HasNotes", "Hide", "IpAddressCreated", "IpAddressModified", "IsFavorite", "IsReport", "MenuNameAr", "MenuNameEn", "MenuUrl", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (short)1, null, false, false, false, null, null, null, false, false, "الطلبات", "Requests", "/request", null, null, null },
                    { (short)2, null, false, false, false, null, null, null, false, false, "تعريف الموافقات ومراحلها", "Approvals Steps", "/approve", null, null, null },
                    { (short)3, null, false, false, false, null, null, null, false, false, "الموافقة علي المستندات", "Documents Approvals", "/approverequest", null, null, null },
                    { (short)4, null, false, false, false, null, null, null, false, false, "استيراد البيانات الأساسية", "Import Basic Data", "/task/importfiles", null, null, null },
                    { (short)5, null, false, false, false, null, null, null, false, false, "ترميز أكواد المستندات", "Menu Encoding", "/menuencoding", null, null, null },
                    { (short)6, null, false, false, false, null, null, null, false, false, "إعدادات البرنامج", "Application Settings", "/setting", null, null, null },
                    { (short)7, null, false, false, false, null, null, null, false, false, "الدول", "Countries", "/country", null, null, null },
                    { (short)8, null, false, false, false, null, null, null, false, false, "المناطق الإدارية", "States", "/state", null, null, null },
                    { (short)9, null, false, false, false, null, null, null, false, false, "المدن", "Cities", "/city", null, null, null },
                    { (short)10, null, false, false, false, null, null, null, false, false, "الأحياء", "Districts", "/district", null, null, null },
                    { (short)11, null, false, false, false, null, null, null, false, false, "الأنشطة", "Business", "/company", null, null, null },
                    { (short)12, null, false, false, false, null, null, null, false, false, "الفروع", "Branches", "/branch", null, null, null },
                    { (short)13, null, false, false, false, null, null, null, false, false, "المواقع", "Stores", "/store", null, null, null },
                    { (short)14, null, false, false, false, null, null, null, false, false, "الموردين", "Suppliers", "/supplier", null, null, null },
                    { (short)15, null, false, false, false, null, null, null, false, false, "العملاء", "Clients", "/client", null, null, null },
                    { (short)16, null, false, false, false, null, null, null, false, false, "مندوبي المبيعات", "Sellers", "/seller", null, null, null },
                    { (short)17, null, false, false, false, null, null, null, false, false, "البنوك", "Banks", "/bank", null, null, null },
                    { (short)18, null, false, false, false, null, null, null, false, false, "معرفات الملاحظات", "Notes Identifiers", "/noteidentifier", null, null, null },
                    { (short)19, null, false, false, false, null, null, null, false, false, "اسماء شرائح عمولة المناديب", "Seller Commission Names", "/sellercommissionmethod", null, null, null },
                    { (short)20, null, false, false, false, null, null, null, false, false, "أنواع مصاريف الفاتورة", "Invoice Expense Types", "/invoiceexpensetype", null, null, null },
                    { (short)21, null, false, false, false, null, null, null, false, false, "طرق الشحن", "Shipment Types", "/shipmenttype", null, null, null },
                    { (short)22, null, false, false, false, null, null, null, false, false, "أنواع حالات الشحن", "Shipping Status Types", "/shippingstatus", null, null, null },
                    { (short)23, null, false, false, false, null, null, null, false, false, "شرائح عمولات مندوبي المبيعات", "Sellers Commision Methods", "/sellercommission", null, null, null },
                    { (short)24, null, false, false, false, null, null, null, false, false, "الضرائب", "Taxes", "/tax", null, null, null },
                    { (short)25, null, false, false, false, null, null, null, false, false, "طرق القبض/الدفع", "Payment/Receiving Methods", "/paymentmethod", null, null, null },
                    { (short)26, null, false, false, false, null, null, null, false, false, "العملات", "Currencies", "/currency", null, null, null },
                    { (short)27, null, false, false, false, null, null, null, false, false, "أسعار العملات", "Currencies Rates", "/currencyrate", null, null, null },
                    { (short)28, null, false, false, true, null, null, null, false, false, "الأصناف", "Items", "/item", null, null, null },
                    { (short)29, null, false, false, false, null, null, null, false, false, "وحدات الأصناف", "Items Packages", "/itempackage", null, null, null },
                    { (short)30, null, false, false, false, null, null, null, false, false, "تقسيمات الأصناف", "Items Categories", "/itemdivision", null, null, null },
                    { (short)31, null, false, false, false, null, null, null, false, false, "الشركات المصنعة", "Vendors", "/vendor", null, null, null },
                    { (short)32, null, false, false, false, null, null, null, false, false, "سمات الأصناف", "Item Attributes", "/itemattribute", null, null, null },
                    { (short)33, null, false, false, false, null, null, null, false, false, "الرصيد الافتتاحي", "Open Balance", "/openbalance", null, null, null },
                    { (short)34, null, false, false, false, null, null, null, false, false, "رصيد المخزن", "Current Balance", "/currentbalance", null, null, null },
                    { (short)35, null, false, false, false, null, null, null, false, false, "أسعار التكلفة", "Item Cost", "/itemcost", null, null, null },
                    { (short)36, null, true, true, true, null, null, null, false, false, "الجرد والأرصدة الافتتاحية", "StockTaking And Open Balances", "/stocktakingopenbalance", null, null, null },
                    { (short)37, null, true, true, true, null, null, null, false, false, "اعتماد الجرد كرصيد إفتتاحي", "Approval of StockTaking as Open Balance", "/stocktakingcarryoveropenbalance", null, null, null },
                    { (short)38, null, true, true, true, null, null, null, false, false, "الجرد والأرصدة الحالية", "StockTaking And Current Balances", "/stocktakingcurrentbalance", null, null, null },
                    { (short)39, null, true, true, true, null, null, null, false, false, "اعتماد الجرد كرصيد حالي", "Approval of StockTaking as Current Balance", "/stocktakingcarryovercurrentbalance", null, null, null },
                    { (short)40, null, true, true, true, null, null, null, false, false, "سند استلام", "Inventory In", "/inventoryin", null, null, null },
                    { (short)41, null, true, true, true, null, null, null, false, false, "سند تسليم", "Inventory Out", "/inventoryout", null, null, null },
                    { (short)42, null, true, true, true, null, null, null, false, false, "نقل بضاعة داخلي", "Internal Transfer Items", "/internaltransfer", null, null, null },
                    { (short)43, null, true, true, true, null, null, null, false, false, "استلام بضاعة داخلي", "Internal Receive Items", "/internaltransferreceive", null, null, null },
                    { (short)44, null, true, true, true, null, null, null, false, false, "الدليل المحاسبي", "Charts of Account", "/account", null, null, null },
                    { (short)45, null, true, true, true, null, null, null, false, false, "قيد يومية", "Journal Entry", "/journalentry", null, null, null },
                    { (short)46, null, true, true, true, null, null, null, false, false, "سند قبض", "Receipt Voucher", "/receiptvoucher", null, null, null },
                    { (short)47, null, true, true, true, null, null, null, false, false, "سند صرف", "Payment Voucher", "/paymentvoucher", null, null, null },
                    { (short)48, null, true, true, true, null, null, null, false, false, "مراكز التكلفة", "Cost Center", "/costcenter", null, null, null },
                    { (short)49, null, true, true, true, null, null, null, false, false, "طلب بضاعة", "Product Request", "/productrequest", null, null, null },
                    { (short)50, null, true, true, true, null, null, null, false, false, "طلب تسعير بضاعة", "Product Request Price", "/productrequestprice", null, null, null },
                    { (short)51, null, true, true, true, null, null, null, false, false, "عرض أسعار المورد", "Supplier Quotation", "/supplierquotation", null, null, null },
                    { (short)52, null, true, true, true, null, null, null, false, false, "فاتورة شراء نقداً", "Cash Purchase Invoice", "/purchaseinvoicecash", null, null, null },
                    { (short)53, null, true, true, true, null, null, null, false, false, "فاتورة شراء آجل", "Credit Purchase Invoice", "/purchaseinvoicecredit", null, null, null },
                    { (short)54, null, true, true, true, null, null, null, false, false, "فاتورة مرتجع شراء نقداً", "Cash Purchase Invoice Return", "/purchaseinvoicereturncash", null, null, null },
                    { (short)55, null, true, true, true, null, null, null, false, false, "فاتورة مرتجع شراء آجل", "Credit Purchase Invoice Return", "/purchaseinvoicereturncredit", null, null, null },
                    { (short)56, null, true, true, true, null, null, null, false, false, "اتفاقية شراء", "Purchase Order", "/purchaseorder", null, null, null },
                    { (short)57, null, true, true, true, null, null, null, false, false, "بيان استلام", "Receipt Statement", "/stockinfrompurchaseorder", null, null, null },
                    { (short)58, null, true, true, true, null, null, null, false, false, "مرتجع من بيان استلام", "Receipt Statement Return", "/stockinreturnfromstockin", null, null, null },
                    { (short)59, null, true, true, true, null, null, null, false, false, "فاتورة مشتريات مرحلية", "Purchase Invoice Interim ", "/purchaseinvoice", null, null, null },
                    { (short)60, null, true, true, true, null, null, null, false, false, "فاتورة مشتريات بضاعة بالطريق نقداً", "Purchase Invoice On The Way Cash", "/purchaseinvoicecashonway", null, null, null },
                    { (short)61, null, true, true, true, null, null, null, false, false, "فاتورة مشتريات بضاعة بالطريق آجل", "Purchase Invoice On The Way Credit", "/purchaseinvoicecreditonway", null, null, null },
                    { (short)62, null, true, true, true, null, null, null, false, false, "استلام من فاتورة بضاعة بالطريق", "Receipt From Purchase Invoice On The Way ", "/stockinfrompurchaseinvoiceonway", null, null, null },
                    { (short)63, null, true, true, true, null, null, null, false, false, "مرتجع من استلام فاتورة بضاعة بالطريق", "Receipt From Purchase Invoice On The Way Return", "/stockinreturnfromstockinonway", null, null, null },
                    { (short)64, null, true, true, true, null, null, null, false, false, "فاتورة مرتجع بضاعة بالطريق", "Purchase Invoice Return On The Way", "/purchaseinvoicereturnonway", null, null, null },
                    { (short)65, null, true, true, true, null, null, null, false, false, "مرتجع من فاتورة المشتريات", "Return From Purchase Invoice", "/stockinreturnfrompurchaseinvoice", null, null, null },
                    { (short)66, null, true, true, true, null, null, null, false, false, "فاتورة مرتجع المشتريات", "Purchase Invoice Return", "/purchaseinvoicereturn", null, null, null },
                    { (short)67, null, true, true, true, null, null, null, false, false, "إشعار مدين المورد", "Supplier Debit Memo", "/supplierdebitmemo", null, null, null },
                    { (short)68, null, true, true, true, null, null, null, false, false, "إشعار دائن المورد", "Supplier Credit Memo", "/suppliercreditmemo", null, null, null },
                    { (short)69, null, true, true, true, null, null, null, false, false, "طلب تسعير", "Client Price Request", "/clientpricerequest", null, null, null },
                    { (short)70, null, true, true, true, null, null, null, false, false, "عرض أسعار", "Client Quotation", "/clientquotation", null, null, null },
                    { (short)71, null, true, true, true, null, null, null, false, false, "اعتماد عرض أسعار", "Client Quotation Approval", "/clientquotationapproval", null, null, null },
                    { (short)72, null, true, true, true, null, null, null, false, false, "فاتورة بيع نقداً", "Cash Sales Invoice", "/cashsalesinvoice", null, null, null },
                    { (short)73, null, true, true, true, null, null, null, false, false, "فاتورة بيع آجل", "Credit Sales Invoice", "/creditsalesinvoice", null, null, null },
                    { (short)74, null, true, true, true, null, null, null, false, false, "فاتورة مرتجع بيع نقداً", "Cash Sales Invoice Return", "/cashsalesinvoicereturn", null, null, null },
                    { (short)75, null, true, true, true, null, null, null, false, false, "فاتورة مرتجع بيع آجل", "Credit Sales Invoice Return", "/creditsalesinvoicereturn", null, null, null },
                    { (short)76, null, true, true, true, null, null, null, false, false, "اتفاقية بيع", "Proforma Invoice", "/proformainvoice", null, null, null },
                    { (short)77, null, true, true, true, null, null, null, false, false, "بيان تسليم", "Stock Out From Proforma Invoice", "/stockoutfromproformainvoice", null, null, null },
                    { (short)78, null, true, true, true, null, null, null, false, false, "مرتجع من بيان تسليم", "Stock Out Return From Stock Out", "/stockoutreturnfromstockout", null, null, null },
                    { (short)79, null, true, true, true, null, null, null, false, false, "فاتورة مرحلية", "Sales Invoice Interim", "/salesinvoice", null, null, null },
                    { (short)80, null, true, true, true, null, null, null, false, false, "فاتورة حجز نقدي", "Cash Reservation Invoice", "/cashreservationinvoice", null, null, null },
                    { (short)81, null, true, true, true, null, null, null, false, false, "فاتورة حجز آجل", "Credit Reservation Invoice", "/creditreservationinvoice", null, null, null },
                    { (short)82, null, true, true, true, null, null, null, false, false, "تسليم محجوز", "Stock Out From Reservation", "/stockoutfromreservationinvoice", null, null, null },
                    { (short)83, null, true, true, true, null, null, null, false, false, "مرتجع من تسليم محجوز", "Stock Out Return From Reservation", "/stockoutreturnfromreservationinvoice", null, null, null },
                    { (short)84, null, true, true, true, null, null, null, false, false, "فاتورة تصفية حجز", "Reservation Invoice Close Out", "/reservationinvoicecloseout", null, null, null },
                    { (short)85, null, true, true, true, null, null, null, false, false, "مرتجع من فاتورة المبيعات", "Stock Out Return From Invoice", "/stockoutreturnfromsalesinvoice", null, null, null },
                    { (short)86, null, true, true, true, null, null, null, false, false, "فاتورة مرتجع المبيعات", "Sales Invoice Return", "/salesinvoicereturn", null, null, null },
                    { (short)87, null, true, true, true, null, null, null, false, false, "إشعار مدين للعميل", "Client Debit Memo", "/clientdebitmemo", null, null, null },
                    { (short)88, null, true, true, true, null, null, null, false, false, "إشعار دائن للعميل", "Client Credit Memo", "/clientcreditmemo", null, null, null },
                    { (short)89, null, false, false, false, null, null, null, false, false, "فك وحدات الأصناف", "Disassemble Item Packages", "/disassembleitempackages", null, null, null },
                    { (short)90, null, false, false, false, null, null, null, false, false, "بيان فك وحدات الأصناف", "Disassemble Item Packages Statement", "/disassemblestatement", null, null, null },
                    { (short)91, null, false, false, false, null, null, null, false, true, "ميزان مراجعة أساسي", "Basic Trial Balance", "/reports/accounting/basictrialbalance", null, null, null },
                    { (short)92, null, false, false, false, null, null, null, false, true, "ميزان مراجعة فرعي", "Sub-Trial Balance", "/reports/accounting/subtrialbalance", null, null, null },
                    { (short)93, null, false, false, false, null, null, null, false, true, "أرصدة الحسابات  ", "Account Balances", "/reports/accounting/accountbalances", null, null, null },
                    { (short)94, null, false, false, false, null, null, null, false, true, "قائمة الدخل", "Income Statement", "/reports/accounting/incomestatement", null, null, null },
                    { (short)95, null, false, false, false, null, null, null, false, true, "ميزانية عمومية", "Balance Sheet", "/reports/accounting/balancesheet", null, null, null },
                    { (short)96, null, false, false, false, null, null, null, false, true, "بيانات الحسابات الجزئية", "Individual Account Statement", "/reports/accounting/individualaccountstatement", null, null, null },
                    { (short)97, null, false, false, false, null, null, null, false, true, "كشف حساب مفصل", "Detailed Account Statement", "/reports/accounting/accountstatementdetailed", null, null, null },
                    { (short)98, null, false, false, false, null, null, null, false, true, "كشف حساب لمجموعة حسابات", "Account Statement for Group of Accounts", "/reports/accounting/accountstatementgrouped", null, null, null },
                    { (short)99, null, false, false, false, null, null, null, false, true, "دفتر قيود اليومية", "General Journal", "/reports/accounting/generaljournal", null, null, null },
                    { (short)100, null, false, false, false, null, null, null, false, true, "أرقام السندات المفقودة", "Missing Documents Numbers", "/reports/accounting/missingdocumentsnumbers", null, null, null },
                    { (short)101, null, false, false, false, null, null, null, false, true, "تقرير اجمالي الدخل للموقع", "Gross Income Report On Store", "/reports/accounting/grossincome", null, null, null },
                    { (short)102, null, false, false, false, null, null, null, false, true, "تقرير طرق التحصيلات النقدية", "Cash Collections Methods Report", "/reports/accounting/cashmethodsreport", null, null, null },
                    { (short)103, null, false, false, false, null, null, null, false, true, "التقرير الضريبي النموذجي", "Typical Tax Report", "/reports/tax/typicaltaxreport", null, null, null },
                    { (short)104, null, false, false, false, null, null, null, false, true, "تقرير تفصيلي الضريبة", "Detailed Tax Report", "/reports/tax/detailedtaxreport", null, null, null },
                    { (short)105, null, false, false, false, null, null, null, false, true, "تقرير الضرائب الأخرى", "Other Tax Report", "/reports/tax/othertaxreport", null, null, null },
                    { (short)106, null, false, false, false, null, null, null, false, true, "تفصيلي الضرائب الأخرى", "Detailed Other Tax Report", "/reports/tax/detailedothertaxreport", null, null, null },
                    { (short)107, null, false, false, false, null, null, null, false, true, "مجمع مراكز التكلفة الأساسي", "Main Cost Center Consolidated", "/reports/costcenter/maincenterconsolidated", null, null, null },
                    { (short)108, null, false, false, false, null, null, null, false, true, "مجمع مراكز التكلفة الجزئي", "Individual Cost Center Consolidated", "/reports/costcenter/individualcenterconsolidated", null, null, null },
                    { (short)109, null, false, false, false, null, null, null, false, true, "دفتر قيود يومية لمركز تكلفة", "Cost Center Journal", "/reports/costcenter/costcenterjournal", null, null, null },
                    { (short)110, null, false, false, false, null, null, null, false, true, "حسابات مراكز التكلفة", "Cost Center Accounts", "/reports/costcenter/costcenteraccounts", null, null, null },
                    { (short)111, null, false, false, false, null, null, null, false, true, "استهلاك الأصول الثابتة", "Fixed Assets Depreciation", "/reports/fixedasset/fixedassetsdepreciation", null, null, null },
                    { (short)112, null, false, false, false, null, null, null, false, true, "الحركة الأصلية في مركز التكلفة (المشروع)", "Original Movement in Cost Center (Project)", "/reports/fixedasset/originalmovementincostcenter", null, null, null },
                    { (short)113, null, false, false, false, null, null, null, false, true, "جرد البضاعة", "Inventory Of Goods", "/reports/inventory/inventoryofgoods", null, null, null },
                    { (short)114, null, false, false, false, null, null, null, false, true, "قيمة البضاعة", "Value Of Goods", "/reports/inventory/valueofgoods", null, null, null },
                    { (short)115, null, false, false, false, null, null, null, false, true, "حركة تفصيلية لصنف", "Detailed Transaction Of An Item", "/reports/inventory/transactionofitem", null, null, null },
                    { (short)116, null, false, false, false, null, null, null, false, true, "حركة الأصناف", "Transaction Of Items", "/reports/inventory/transactionofitems", null, null, null },
                    { (short)117, null, false, false, false, null, null, null, false, true, "الحركة التجارية للمشتريات", "Commercial Movement Of Purchases", "/reports/purchases/commercialmovementpurchases", null, null, null },
                    { (short)118, null, false, false, false, null, null, null, false, true, "الأصناف منتهية الصلاحية", "Expired Items", "/reports/inventory/expireditems", null, null, null },
                    { (short)119, null, false, false, false, null, null, null, false, true, "الأصناف التي ستنتهي صلاحيتها خلال مدة", "Items That Will Expire Within A Period", "/reports/inventory/itemswillexpire", null, null, null },
                    { (short)120, null, false, false, false, null, null, null, false, true, "الأصناف الراكدة لم ينفذ لها بيع منذ مدة", "Stagnant Items That Haven't Been Sold For A Period", "/reports/inventory/stagnantitems", null, null, null },
                    { (short)121, null, false, false, false, null, null, null, false, true, "الأصناف الأكثر ربيحة", "Most Profitable Items", "/reports/inventory/mostprofitableitems", null, null, null },
                    { (short)122, null, false, false, false, null, null, null, false, true, "الأصناف الأكثر دوران", "Most Circulating Items", "/reports/inventory/mostcirculatingitems", null, null, null },
                    { (short)123, null, false, false, false, null, null, null, false, true, "الأصناف التي وصلت لحد الطلب", "Items That Have Reached The Demand Limit", "/reports/inventory/itemsdemandlimit", null, null, null },
                    { (short)124, null, false, false, false, null, null, null, false, true, "فواتير البيع المعمرة", "Outstanding Sales Invoices", "/reports/client/outstandingsalesinvoices", null, null, null },
                    { (short)125, null, false, false, false, null, null, null, false, true, "فواتير البيع الغيرة مسددة", "Unpaid Sales Invoices", "/reports/client/unpaidsalesinvoices", null, null, null },
                    { (short)126, null, false, false, false, null, null, null, false, true, "عملاء تجاوز رصيدهم حد الائتمان", "Clients with Credit Limit Exceeded", "/reports/client/clientslimitexceeded", null, null, null },
                    { (short)127, null, false, false, false, null, null, null, false, true, "فواتير بيع سيحل موعد سدادها خلال فترة", "Sales Invoices Due in a Period", "/reports/client/salesinaperiod", null, null, null },
                    { (short)128, null, false, false, false, null, null, null, false, true, "فواتير مبيعات نسبة الربح فيها أقل من", "Sales Invoices with a Profit Rate Less than", "/reports/client/saleslessthan", null, null, null },
                    { (short)129, null, false, false, false, null, null, null, false, true, "فواتير مبيعات نسبة الخصم فيها أكبر من", "Sales Invoices with a Discount Rate Greater than", "/reports/client/salesgreaterthan", null, null, null },
                    { (short)130, null, false, false, false, null, null, null, false, true, "تقرير النشاط التجاري", "Business Activity Report", "/reports/client/businessactivityreport", null, null, null },
                    { (short)131, null, false, false, false, null, null, null, false, true, "العملاء الأكثر مبيعاً (مبالغ)", "Top Selling Clients (Amounts)", "/reports/client/topsellingclients", null, null, null },
                    { (short)132, null, false, false, false, null, null, null, false, true, "العملاء الأكثر دورية (عدد فواتير)", "Top Frequent Clients (No of Invoices)", "/reports/client/topfrequentclients", null, null, null },
                    { (short)133, null, false, false, false, null, null, null, false, true, "العملاء الأكثر ربحية", "Top Profitable Clients", "/reports/client/topprofitableclients", null, null, null },
                    { (short)134, null, false, false, false, null, null, null, false, true, "فواتير الشراء المعمرة", "Outstanding Purchase Invoices", "/reports/supplier/outstandingpurchaseinvoices", null, null, null },
                    { (short)135, null, false, false, false, null, null, null, false, true, "فواتير الشراء الغير مسددة", "Unpaid Purchase Invoices", "/reports/supplier/unpaidpurchaseinvoices", null, null, null },
                    { (short)136, null, false, false, false, null, null, null, false, true, "موردين تجاوز رصيدهم حد الائتمان", "Suppliers Over Credit Limit", "/reports/supplier/supplierscreditlimit", null, null, null },
                    { (short)137, null, false, false, false, null, null, null, false, true, "فواتير شراء سيحل موعد سدادها خلال فترة", "Purchase Invoices Due Within a Period", "/reports/supplier/purchaseinaperiod", null, null, null },
                    { (short)138, null, false, false, false, null, null, null, false, true, "تقرير النشاط التجاري لفواتير الشراء", "Business Activity Report for Purchase Invoices", "/reports/supplier/businesspurchaseinvoices", null, null, null },
                    { (short)139, null, false, false, false, null, null, null, false, true, "تقرير عمولات المناديب", "Sellers Commission Report", "/reports/seller/sellerscommissionreport", null, null, null },
                    { (short)140, null, false, false, false, null, null, null, false, true, "متابعة طلبات البضاعة", "Products Request Follow-up", "/reports/purchases/productrequestfollowup", null, null, null },
                    { (short)141, null, false, false, false, null, null, null, false, true, "متابعة طلبات التسعير للموردين", "Products Requests Price Follow-up", "/reports/purchases/productrequestpricefollowup", null, null, null },
                    { (short)142, null, false, false, false, null, null, null, false, true, "متابعة عروض أسعار الموردين", "Suppliers Quotations Follow-up", "/reports/purchases/supplierquotationfollowup", null, null, null },
                    { (short)143, null, false, false, false, null, null, null, false, true, "متابعة اتفاقيات شراء", "Purchase Order Follow-up", "/reports/purchases/purchaseorderfollowup", null, null, null },
                    { (short)144, null, false, false, false, null, null, null, false, true, "متابعة بيانات استلام", "Receipt Statements Follow-up", "/reports/purchases/receiptstatementfollowup", null, null, null },
                    { (short)145, null, false, false, false, null, null, null, false, true, "متابعة مرتجع من بيانات استلام", "Receipt Statements Return Follow-up", "/reports/purchases/receiptstatementreturnfollowup", null, null, null },
                    { (short)146, null, false, false, false, null, null, null, false, true, "متابعة فاتورة مشتريات مرحلية", "Purchase Invoices Interim Follow-up", "/reports/purchases/purchaseinvoicefollowup", null, null, null },
                    { (short)147, null, false, false, false, null, null, null, false, true, "متابعة مرتجع كميات من فاتورة مشتريات مرحلية", "Purchase Invoices Interim Return Follow-up", "/reports/purchases/purchaseinvoiceinterimfollowup", null, null, null },
                    { (short)148, null, false, false, false, null, null, null, false, true, "متابعة بضاعة بالطريق", "Products On The Way Follow-up", "/reports/purchases/productonthewayfollowup", null, null, null },
                    { (short)149, null, false, false, false, null, null, null, false, true, "متابعة طلبات التسعير للعملاء", "Client Price Requests Follow-up", "/reports/sales/clientpricerequestfollowup", null, null, null },
                    { (short)150, null, false, false, false, null, null, null, false, true, "متابعة عروض الأسعار للعملاء", "Client Quotations Follow-up", "/reports/sales/clientquotationfollowup", null, null, null },
                    { (short)151, null, false, false, false, null, null, null, false, true, "متابعة اعتمادات عروض الأسعار للعملاء", "Client Quotations Approval Follow-up", "/reports/sales/clientquotationapprovalfollowup", null, null, null },
                    { (short)152, null, false, false, false, null, null, null, false, true, "متابعة اتفاقيات بيع", "Proforma Invoices Follow-up", "/reports/sales/proformainvoicefollowup", null, null, null },
                    { (short)153, null, false, false, false, null, null, null, false, true, "متابعة بيان تسليم", "Stock Out Follow-up", "/reports/sales/stockoutfollowup", null, null, null },
                    { (short)154, null, false, false, false, null, null, null, false, true, "متابعة مرتجع من بيان تسليم", "Stock Out Return Follow-up", "/reports/sales/stockoutreturnfollowup", null, null, null },
                    { (short)155, null, false, false, false, null, null, null, false, true, "متابعة فاتورة  بيع مرحلية", "Sales Invoice Interim Follow-up", "/reports/sales/salesinvoiceinterimfollowup", null, null, null },
                    { (short)156, null, false, false, false, null, null, null, false, true, "متابعة مرتجع كميات من فاتورة مرحلية", "Sales Invoice Interim Return Follow-up", "/reports/sales/salesinvoiceinterimreturnfollowup", null, null, null },
                    { (short)157, null, false, false, false, null, null, null, false, true, "متابعة بضاعة محجوزة", "Reservation Invoice Follow-up", "/reports/sales/reservationinvoicefollowup", null, null, null },
                    { (short)158, null, true, true, true, null, null, null, false, false, "الأصول الثابتة", "Fixed Assets", "/fixedasset", null, null, null },
                    { (short)159, null, true, true, true, null, null, null, false, false, "نقل أصل ثابت", "Fixed Asset Movement", "/fixedassetmovement", null, null, null },
                    { (short)160, null, true, true, true, null, null, null, false, false, "إضافة أصل ثابت", "Fixed Asset Addition", "/fixedassetaddition", null, null, null },
                    { (short)161, null, true, true, true, null, null, null, false, false, "استبعاد أصل ثابت", "Fixed Asset Exclusion", "/fixedassetexclusion", null, null, null },
                    { (short)162, null, true, true, true, null, null, null, false, false, "إهلاك أصل ثابت", "Fixed Asset Depreciation", "/fixedassetdepreciation", null, null, null },
                    { (short)163, null, false, false, false, null, null, null, false, true, "الحركة التجارية للمبيعات", "Commercial Movement Of Sales", "/reports/purchases/commercialmovementsales", null, null, null }
                });

            migrationBuilder.InsertData(
                table: "PaymentTypes",
                columns: new[] { "PaymentTypeId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "PaymentTypeCode", "PaymentTypeNameAr", "PaymentTypeNameEn", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, null, null, null, null, null, "10", "نقدي", "Cash", null, null },
                    { (byte)2, null, null, null, null, null, "42", "حساب بنكي", "Bank Account", null, null },
                    { (byte)3, null, null, null, null, null, "48", "بطاقة مصرفية", "Bank Card", null, null },
                    { (byte)4, null, null, null, null, null, "", "تقسيط", "Installment", null, null },
                    { (byte)5, null, null, null, null, null, "30", "تحويل رصيد", "Credit Transfer", null, null }
                });

            migrationBuilder.InsertData(
                table: "ReportPrintForms",
                columns: new[] { "ReportPrintFormId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "ReportPrintFormNameAr", "ReportPrintFormNameEn", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { 1, null, null, null, null, null, "رسمي", "Formal", null, null },
                    { 2, null, null, null, null, null, "شبه رسمي", "Semi-Formal", null, null }
                });

            migrationBuilder.InsertData(
                table: "SellerCommissionTypes",
                columns: new[] { "SellerCommissionTypeId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "SellerCommissionTypeNameAr", "SellerCommissionTypeNameEn", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, null, null, null, null, null, "حسب الدخل النقدي", "According To Cash Income", null, null },
                    { (byte)2, null, null, null, null, null, "حسب عمر الدين المحصل", "According To Age Of Debt Collected", null, null }
                });

            migrationBuilder.InsertData(
                table: "SellerTypes",
                columns: new[] { "SellerTypeId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "SellerTypeNameAr", "SellerTypeNameEn", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, null, null, null, null, null, "موظف", "Employee", null, null },
                    { (byte)2, null, null, null, null, null, "توظيف خارجي", "OutSourcing", null, null },
                    { (byte)3, null, null, null, null, null, "بالعمولة", "With a Commission", null, null },
                    { (byte)4, null, null, null, null, null, "أخري", "Other", null, null }
                });

            migrationBuilder.InsertData(
                table: "ShipmentTypes",
                columns: new[] { "ShipmentTypeId", "CompanyId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "ShipmentTypeCode", "ShipmentTypeNameAr", "ShipmentTypeNameEn", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { 1, null, null, null, null, null, null, 1, "سيارة", "Car", null, null },
                    { 2, null, null, null, null, null, null, 2, "دراجة نارية", "Motorcycle", null, null },
                    { 3, null, null, null, null, null, null, 3, "شاحنة", "Truck", null, null },
                    { 4, null, null, null, null, null, null, 4, "الطائرة", "Plane", null, null },
                    { 5, null, null, null, null, null, null, 5, "الباخرة", "Ship", null, null }
                });

            migrationBuilder.InsertData(
                table: "StockTypes",
                columns: new[] { "StockTypeId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "StockTypeNameAr", "StockTypeNameEn", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, null, null, null, null, null, "بيان استلام", "Receipt Statement", null, null },
                    { (byte)2, null, null, null, null, null, "استلام من فاتورة بضاعة بالطريق", "Receipt From Purchase Invoice On The Way", null, null },
                    { (byte)3, null, null, null, null, null, "مرتجع من بيان استلام", "Receipt Statement Return", null, null },
                    { (byte)4, null, null, null, null, null, "مرتجع من استلام فاتورة بضاعة بالطريق", "Receipt From Purchase Invoice On The Way Return", null, null },
                    { (byte)5, null, null, null, null, null, "مرتجع من فاتورة المشتريات", "StockIn Return From Purchase Invoice", null, null },
                    { (byte)6, null, null, null, null, null, "بيان تسليم", "StockOut From Proforma Invoice", null, null },
                    { (byte)7, null, null, null, null, null, "تسليم محجوز", "Stock Out From Reservation", null, null },
                    { (byte)8, null, null, null, null, null, "مرتجع من بيان تسليم", "Stock Out Return From Stock Out", null, null },
                    { (byte)9, null, null, null, null, null, "مرتجع من تسليم محجوز", "Stock Out Return From Reservation", null, null },
                    { (byte)10, null, null, null, null, null, "مرتجع من فاتورة المبيعات", "StockOut Return From Sales Invoice", null, null }
                });

            migrationBuilder.InsertData(
                table: "StoreClassifications",
                columns: new[] { "StoreClassificationId", "ClassificationNameAr", "ClassificationNameEn", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, "تجاري", "Commercial", null, null, null, null, null, null, null },
                    { (byte)2, "صناعي", "Industrial", null, null, null, null, null, null, null },
                    { (byte)3, "خدمي", "Services", null, null, null, null, null, null, null },
                    { (byte)4, "إداري", "Managerial", null, null, null, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "SystemTasks",
                columns: new[] { "TaskId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "IsCompleted", "ModifiedAt", "TaskNameAr", "TaskNameEn", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { 1, null, null, null, null, false, null, "استيراد الدول", "Import Countries", null, null },
                    { 2, null, null, null, null, false, null, "استيراد المناطق الإدارية", "Import States", null, null },
                    { 3, null, null, null, null, false, null, "استيراد المدن", "Import Cities", null, null },
                    { 4, null, null, null, null, false, null, "استيراد الأحياء", "Import Districts", null, null },
                    { 5, null, null, null, null, false, null, "استيراد العملات", "Import Currencies", null, null }
                });

            migrationBuilder.InsertData(
                table: "TaxTypes",
                columns: new[] { "TaxTypeId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "TaxTypeNameAr", "TaxTypeNameEn", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)0, null, null, null, null, null, "غير محدد", "unknown", null, null },
                    { (byte)1, null, null, null, null, null, "ضريبي", "Taxable", null, null },
                    { (byte)2, null, null, null, null, null, "معفي", "Exempted", null, null },
                    { (byte)3, null, null, null, null, null, "صفري", "Zero", null, null },
                    { (byte)4, null, null, null, null, null, "اتفاقيات خاصة", "Private Contracts", null, null },
                    { (byte)5, null, null, null, null, null, "صادرات", "Exports", null, null },
                    { (byte)6, null, null, null, null, null, "استيراد من الخارج", "Imports", null, null },
                    { (byte)7, null, null, null, null, null, "احتساب عكسي", "Reverse Calculation", null, null }
                });

            migrationBuilder.InsertData(
                table: "TransactionTypes",
                columns: new[] { "TransactionTypeId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "TransactionTypeNameAr", "TransactionTypeNameEn", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, null, null, null, null, null, "مدين", "Debit", null, null },
                    { (byte)2, null, null, null, null, null, "دائن", "Credit", null, null }
                });

            migrationBuilder.InsertData(
                table: "AccountCategories",
                columns: new[] { "AccountCategoryId", "AccountCategoryNameAr", "AccountCategoryNameEn", "AccountLedgerId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { (byte)1, "الأصول", "Assets", (byte)1, null, null, null, null, null, null, null },
                    { (byte)2, "الخصوم", "Liabilities", (byte)1, null, null, null, null, null, null, null },
                    { (byte)3, "المصروفات", "Expenses", (byte)2, null, null, null, null, null, null, null },
                    { (byte)4, "الايرادات", "Revenues", (byte)2, null, null, null, null, null, null, null }
                });

            migrationBuilder.InsertData(
                table: "ApplicationFlagHeaders",
                columns: new[] { "ApplicationFlagHeaderId", "ApplicationFlagHeaderNameAr", "ApplicationFlagHeaderNameEn", "ApplicationFlagTabId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "Order", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { 1, "تقسيمات الأصناف", "Items Categories", 1, null, null, null, null, null, (short)1, null, null },
                    { 2, "ترميز الدليل المحاسبي", "Charts of Account Encoding", 2, null, null, null, null, null, (short)1, null, null },
                    { 3, "ترميز مراكز التكلفة", "Cost Centers Encoding", 3, null, null, null, null, null, (short)1, null, null },
                    { 4, "أسعار التكلفة", "Item Cost", 4, null, null, null, null, null, (short)1, null, null },
                    { 5, "الأصناف", "Items", 5, null, null, null, null, null, (short)1, null, null },
                    { 6, "فاتورة المشتريات", "Purchase Invoice", 6, null, null, null, null, null, (short)1, null, null },
                    { 7, "فاتورة مرتجع المشتريات", "Purchase Invoice Return", 7, null, null, null, null, null, (short)1, null, null },
                    { 8, "عرض أسعار العميل", "Client Quotation", 8, null, null, null, null, null, (short)1, null, null },
                    { 9, "فاتورة المبيعات", "Sales Invoice", 9, null, null, null, null, null, (short)1, null, null },
                    { 10, "فاتورة مرتجع المبيعات", "Sales Invoice Return", 10, null, null, null, null, null, (short)1, null, null },
                    { 11, "إعدادات عامة", "General Settings", 11, null, null, null, null, null, (short)1, null, null },
                    { 12, "إعدادات أساسية", "Basic Settings", 12, null, null, null, null, null, (short)1, null, null },
                    { 13, "ملاحظات إضافية", "Additional Notes", 12, null, null, null, null, null, (short)2, null, null },
                    { 14, "إضافات أخري", "Other Additions", 12, null, null, null, null, null, (short)3, null, null },
                    { 15, "حجم الخط", "Font Size", 12, null, null, null, null, null, (short)4, null, null },
                    { 16, "تقريب الكسر العشري", "Rounding a decimal", 12, null, null, null, null, null, (short)5, null, null },
                    { 17, "هيئة الطباعة", "Printing Form", 12, null, null, null, null, null, (short)6, null, null },
                    { 18, "صور مرفقة", "Images", 12, null, null, null, null, null, (short)7, null, null },
                    { 19, "حجم الهوامش", "margin Size", 12, null, null, null, null, null, (short)8, null, null }
                });

            migrationBuilder.InsertData(
                table: "ApplicationFlagDetails",
                columns: new[] { "ApplicationFlagDetailId", "ApplicationFlagHeaderId", "ApplicationFlagTypeId", "CreatedAt", "FlagNameAr", "FlagNameEn", "FlagValue", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "Order", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { 1, 1, (byte)1, null, "المستوى الأول بالعربية", "First Level Ar", "المستوى الأول", null, null, null, null, (short)1, null, null },
                    { 2, 1, (byte)1, null, "المستوى الأول بالانجليزية", "First Level En", "First Level", null, null, null, null, (short)2, null, null },
                    { 3, 1, (byte)1, null, "المستوى الثاني بالعربية", "Second Level Ar", "المستوى الثاني", null, null, null, null, (short)3, null, null },
                    { 4, 1, (byte)1, null, "المستوى الثاني بالانجليزية", "Second Level En", "Second Level", null, null, null, null, (short)4, null, null },
                    { 5, 1, (byte)1, null, "المستوى الثالث بالعربية", "Third Level Ar", "المستوى الثالث", null, null, null, null, (short)5, null, null },
                    { 6, 1, (byte)1, null, "المستوى الثالث بالانجليزية", "Third Level En", "Third Level", null, null, null, null, (short)6, null, null },
                    { 7, 1, (byte)1, null, "المستوى الرابع بالعربية", "Forth Level Ar", "المستوى الرابع", null, null, null, null, (short)7, null, null },
                    { 8, 1, (byte)1, null, "المستوى الرابع بالانجليزية", "Forth Level En", "Forth Level", null, null, null, null, (short)8, null, null },
                    { 9, 1, (byte)1, null, "المستوى الخامس بالعربية", "Fifth Level Ar", "المستوى الخامس", null, null, null, null, (short)9, null, null },
                    { 10, 1, (byte)1, null, "المستوى الخامس بالانجليزية", "Fifth Level En", "Fifth Level", null, null, null, null, (short)10, null, null },
                    { 11, 2, (byte)2, null, "عدد خانات التسلسل للحسابات العامة", "Number of Code Width for Main Accounts", "2", null, null, null, null, (short)1, null, null },
                    { 12, 2, (byte)2, null, "عدد خانات التسلسل للحسابات الجزئية", "Number of Code Width for Individual Accounts", "4", null, null, null, null, (short)2, null, null },
                    { 13, 3, (byte)2, null, "عدد خانات التسلسل للمراكز العامة", "Number of Code Width for Main Centers", "2", null, null, null, null, (short)1, null, null },
                    { 14, 3, (byte)2, null, "عدد خانات التسلسل للمراكز الجزئية", "Number of Code Width for Individual Centers", "4", null, null, null, null, (short)2, null, null },
                    { 15, 4, (byte)7, null, "طريقة احتساب سعر التكلفة للصنف", "Item Cost Price Calculation Method", "1", null, null, null, null, (short)1, null, null },
                    { 16, 5, (byte)6, null, "الأسعار تشمل ضريبة القيمة المضافة", "Prices VAT Inclusive", "0", null, null, null, null, (short)1, null, null },
                    { 17, 6, (byte)6, null, "فصل تسلسل النقدي عن الآجل", "Separating the sequence of cash from credit", "1", null, null, null, null, (short)1, null, null },
                    { 18, 6, (byte)2, null, "عدد الأيام لإرتجاع الفاتورة", "No of days to return the invoice", "0", null, null, null, null, (short)2, null, null },
                    { 19, 7, (byte)6, null, "فصل تسلسل النقدي عن الآجل", "Separating the sequence of cash from credit", "1", null, null, null, null, (short)1, null, null },
                    { 20, 8, (byte)2, null, "عدد أيام صلاحية العرض", "No of days the quotation is valid", "0", null, null, null, null, (short)1, null, null },
                    { 21, 9, (byte)6, null, "فصل تسلسل النقدي عن الآجل", "Separating the sequence of cash from credit", "1", null, null, null, null, (short)1, null, null },
                    { 22, 9, (byte)2, null, "عدد الأيام لإرتجاع الفاتورة", "No of days to return the invoice", "0", null, null, null, null, (short)3, null, null },
                    { 23, 10, (byte)6, null, "فصل تسلسل النقدي عن الآجل", "Separating the sequence of cash from credit", "1", null, null, null, null, (short)1, null, null },
                    { 24, 9, (byte)6, null, "فصل تسلسل الفواتير لكل مندوب", "Separating the sequence of invoices for each seller", "0", null, null, null, null, (short)2, null, null },
                    { 25, 11, (byte)6, null, "عرض الأصناف مجمعة", "Show Items Grouped", "0", null, null, null, null, (short)2, null, null },
                    { 26, 11, (byte)6, null, "فصل تسلسل أرقام السندات لكل سنة", "Separating the sequence of documents every year", "1", null, null, null, null, (short)1, null, null },
                    { 27, 12, (byte)1, null, "اسم المؤسسة بالعربية", "Name of the institution Ar", "", null, null, null, null, (short)1, null, null },
                    { 28, 12, (byte)1, null, "اسم المؤسسة بالانجليزية", "Name of the institution En", "", null, null, null, null, (short)2, null, null },
                    { 29, 12, (byte)1, null, "اسم آخر أسفل اسم المؤسسة بالعربية", "Another name below the name of the institution Ar", "", null, null, null, null, (short)3, null, null },
                    { 30, 12, (byte)1, null, "اسم آخر أسفل اسم المؤسسة بالانجليزية", "Another name below the name of the institution En", "", null, null, null, null, (short)4, null, null },
                    { 31, 12, (byte)1, null, "عنوان 1 بالعربية", "Address 1 Ar", "", null, null, null, null, (short)5, null, null },
                    { 32, 12, (byte)1, null, "عنوان 1 بالانجليزية", "Address 1 En", "", null, null, null, null, (short)6, null, null },
                    { 33, 12, (byte)1, null, "عنوان 2 بالعربية", "Address 2 Ar", "", null, null, null, null, (short)7, null, null },
                    { 34, 12, (byte)1, null, "عنوان 2 بالانجليزية", "Address 2 En", "", null, null, null, null, (short)8, null, null },
                    { 35, 12, (byte)1, null, "عنوان 3 بالعربية", "Address 3 Ar", "", null, null, null, null, (short)9, null, null },
                    { 36, 12, (byte)1, null, "عنوان 3 بالانجليزية", "Address 3 En", "", null, null, null, null, (short)10, null, null },
                    { 37, 13, (byte)1, null, "ملاحظات علوية في أول صفحة فقط بالعربية", "Top notes on first page only Ar", "", null, null, null, null, (short)11, null, null },
                    { 38, 13, (byte)1, null, "ملاحظات علوية في أول صفحة فقط بالانجليزية", "Top notes on first page only En", "", null, null, null, null, (short)12, null, null },
                    { 39, 13, (byte)1, null, "ملاحظات سفلية في آخر صفحة فقط بالعربية", "Footnotes on last page only Ar", "", null, null, null, null, (short)13, null, null },
                    { 40, 13, (byte)1, null, "ملاحظات سفلية في آخر صفحة فقط بالانجليزية", "Footnotes on last page only En", "", null, null, null, null, (short)14, null, null },
                    { 41, 13, (byte)1, null, "ملاحظات علوية متكررة في كل الصفحات بالعربية", "Repeated top notes on all pages Ar", "", null, null, null, null, (short)15, null, null },
                    { 42, 13, (byte)1, null, "ملاحظات علوية متكررة في كل الصفحات بالانجليزية", "Repeated top notes on all pages En", "", null, null, null, null, (short)16, null, null },
                    { 43, 13, (byte)1, null, "ملاحظات سفلية متكررة في كل الصفحات بالعربية", "Repeated footnotes on all pages Ar", "", null, null, null, null, (short)17, null, null },
                    { 44, 13, (byte)1, null, "ملاحظات سفلية متكررة في كل الصفحات بالانجليزية", "Repeated footnotes on all pages En", "", null, null, null, null, (short)18, null, null },
                    { 45, 14, (byte)6, null, "طباعة اسم النشاط والموقع؟", "Print Business Name And Store?", "1", null, null, null, null, (short)1, null, null },
                    { 46, 14, (byte)6, null, "طباعة اسم المستخدم؟", "Print User Name?", "0", null, null, null, null, (short)2, null, null },
                    { 47, 14, (byte)6, null, "طباعة تاريخ اليوم؟", "Print datetime?", "1", null, null, null, null, (short)3, null, null },
                    { 48, 15, (byte)2, null, "حجم الخط لاسم المؤسسة", "Font size for the institution name", "16", null, null, null, null, (short)1, null, null },
                    { 49, 15, (byte)2, null, "حجم الخط للاسم الآخر للمؤسسة", "Font size for other name of the institution", "14", null, null, null, null, (short)2, null, null },
                    { 50, 15, (byte)2, null, "حجم الخط للنشاط والموقع", "Font size for business and store name", "14", null, null, null, null, (short)3, null, null },
                    { 51, 15, (byte)2, null, "حجم الخط للعنوان الأول", "Font size for first address", "14", null, null, null, null, (short)4, null, null },
                    { 52, 15, (byte)2, null, "حجم الخط للعنوان الثاني", "Font size for second address", "14", null, null, null, null, (short)5, null, null },
                    { 53, 15, (byte)2, null, "حجم الخط للعنوان الثالث", "Font size for third address", "14", null, null, null, null, (short)6, null, null },
                    { 54, 15, (byte)2, null, "حجم الخط لعنوان التقرير", "Font size for report title", "16", null, null, null, null, (short)7, null, null },
                    { 55, 15, (byte)2, null, "حجم الخط للجدول", "Font size for table", "12", null, null, null, null, (short)9, null, null },
                    { 56, 15, (byte)2, null, "حجم الخط للملاحظات العلوية الثابتة", "Font size for fixed top notes", "12", null, null, null, null, (short)10, null, null },
                    { 57, 15, (byte)2, null, "حجم الخط للملاحظات العلوية المتكررة", "Font size for repeated top notes", "12", null, null, null, null, (short)11, null, null },
                    { 58, 15, (byte)2, null, "حجم الخط للملاحظات السفلية الثابتة", "Font size for fixed footnotes", "12", null, null, null, null, (short)12, null, null },
                    { 59, 15, (byte)2, null, "حجم الخط للملاحظات السفلية المتكررة", "Font size for repeated footnotes", "12", null, null, null, null, (short)13, null, null },
                    { 60, 16, (byte)2, null, "الأرقام في الجدول", "Numbers in grid", "2", null, null, null, null, (short)1, null, null },
                    { 61, 16, (byte)2, null, "الأرقام في المجاميع", "Numbers in summation", "2", null, null, null, null, (short)2, null, null },
                    { 62, 17, (byte)7, null, "هيئة الورقة أثناء الطباعة", "Paper Form During Printing", "1", null, null, null, null, (short)1, null, null },
                    { 63, 18, (byte)8, null, "شعار المؤسسة", "Institution Logo", "", null, null, null, null, (short)1, null, null },
                    { 64, 15, (byte)2, null, "حجم الخط لفترة التقرير", "Font size for report period", "14", null, null, null, null, (short)8, null, null },
                    { 65, 19, (byte)2, null, "الهامش العلوي", "Top Margin", "40", null, null, null, null, (short)1, null, null },
                    { 66, 19, (byte)2, null, "هامش الجانب الأيمن", "Right Margin", "10", null, null, null, null, (short)2, null, null },
                    { 67, 19, (byte)2, null, "الهامش السفلي", "Bottom Margin", "40", null, null, null, null, (short)3, null, null },
                    { 68, 19, (byte)2, null, "هامش الجانب الأيسر", "Left Margin", "10", null, null, null, null, (short)4, null, null },
                    { 69, 14, (byte)6, null, "طباعة شعار المؤسسة؟", "Print Institution Logo?", "1", null, null, null, null, (short)4, null, null }
                });

            migrationBuilder.InsertData(
                table: "ApplicationFlagDetailSelects",
                columns: new[] { "ApplicationFlagDetailSelectId", "ApplicationFlagDetailId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "Order", "SelectId", "SelectNameAr", "SelectNameEn", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { 1, 15, null, null, null, null, null, (short)1, (short)1, "المتوسط الفعلي", "Actual average", null, null },
                    { 2, 15, null, null, null, null, null, (short)2, (short)2, "آخر سعر شراء", "last Purchasing Price", null, null },
                    { 3, 15, null, null, null, null, null, (short)3, (short)3, "آخر سعر تكلفة", "last Cost Price", null, null },
                    { 4, 62, null, null, null, null, null, (short)1, (short)1, "رسمي", "Formal", null, null },
                    { 5, 62, null, null, null, null, null, (short)2, (short)2, "شبه رسمي", "Semi-Formal", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountCategories_AccountLedgerId",
                table: "AccountCategories",
                column: "AccountLedgerId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountCategoryId",
                table: "Accounts",
                column: "AccountCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AccountTypeId",
                table: "Accounts",
                column: "AccountTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ArchiveHeaderId",
                table: "Accounts",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CompanyId",
                table: "Accounts",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CostCenterId",
                table: "Accounts",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_CurrencyId",
                table: "Accounts",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountStore_AccountStoreTypeId",
                table: "AccountStore",
                column: "AccountStoreTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_AccountStore_StoreId",
                table: "AccountStore",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationFlagDetailCompanies_ApplicationFlagDetailId",
                table: "ApplicationFlagDetailCompanies",
                column: "ApplicationFlagDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationFlagDetailCompanies_CompanyId",
                table: "ApplicationFlagDetailCompanies",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationFlagDetailImages_ApplicationFlagDetailCompanyId",
                table: "ApplicationFlagDetailImages",
                column: "ApplicationFlagDetailCompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationFlagDetails_ApplicationFlagHeaderId",
                table: "ApplicationFlagDetails",
                column: "ApplicationFlagHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationFlagDetails_ApplicationFlagTypeId",
                table: "ApplicationFlagDetails",
                column: "ApplicationFlagTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationFlagDetailSelects_ApplicationFlagDetailId",
                table: "ApplicationFlagDetailSelects",
                column: "ApplicationFlagDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationFlagHeaders_ApplicationFlagTabId",
                table: "ApplicationFlagHeaders",
                column: "ApplicationFlagTabId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRequestDetails_ApproveRequestId",
                table: "ApproveRequestDetails",
                column: "ApproveRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRequests_ApproveId",
                table: "ApproveRequests",
                column: "ApproveId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRequests_ApproveRequestTypeId",
                table: "ApproveRequests",
                column: "ApproveRequestTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRequests_BranchId",
                table: "ApproveRequests",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRequests_CompanyId",
                table: "ApproveRequests",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRequests_CurrentStatusId",
                table: "ApproveRequests",
                column: "CurrentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRequests_CurrentStepId",
                table: "ApproveRequests",
                column: "CurrentStepId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRequests_LastStatusId",
                table: "ApproveRequests",
                column: "LastStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRequests_LastStepId",
                table: "ApproveRequests",
                column: "LastStepId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRequests_MenuCode",
                table: "ApproveRequests",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRequests_StoreId",
                table: "ApproveRequests",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRequestUsers_RequestId",
                table: "ApproveRequestUsers",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRequestUsers_StatusId",
                table: "ApproveRequestUsers",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveRequestUsers_StepId",
                table: "ApproveRequestUsers",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_Approves_CompanyId",
                table: "Approves",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Approves_MenuCode",
                table: "Approves",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveStatuses_ApproveStepId",
                table: "ApproveStatuses",
                column: "ApproveStepId");

            migrationBuilder.CreateIndex(
                name: "IX_ApproveStep_ApproveId",
                table: "ApproveStep",
                column: "ApproveId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveDetails_ArchiveHeaderId",
                table: "ArchiveDetails",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ArchiveHeaders_MenuCode",
                table: "ArchiveHeaders",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_AccountId",
                table: "Banks",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_ArchiveHeaderId",
                table: "Banks",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Banks_CompanyId",
                table: "Banks",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Branches_CompanyId",
                table: "Branches",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_StateId",
                table: "Cities",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCreditMemos_ArchiveHeaderId",
                table: "ClientCreditMemos",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCreditMemos_ClientId",
                table: "ClientCreditMemos",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCreditMemos_CreditAccountId",
                table: "ClientCreditMemos",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCreditMemos_DebitAccountId",
                table: "ClientCreditMemos",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCreditMemos_JournalHeaderId",
                table: "ClientCreditMemos",
                column: "JournalHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCreditMemos_SalesInvoiceHeaderId",
                table: "ClientCreditMemos",
                column: "SalesInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCreditMemos_SellerId",
                table: "ClientCreditMemos",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientCreditMemos_StoreId",
                table: "ClientCreditMemos",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDebitMemos_ArchiveHeaderId",
                table: "ClientDebitMemos",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDebitMemos_ClientId",
                table: "ClientDebitMemos",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDebitMemos_CreditAccountId",
                table: "ClientDebitMemos",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDebitMemos_DebitAccountId",
                table: "ClientDebitMemos",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDebitMemos_JournalHeaderId",
                table: "ClientDebitMemos",
                column: "JournalHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDebitMemos_SalesInvoiceHeaderId",
                table: "ClientDebitMemos",
                column: "SalesInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDebitMemos_SellerId",
                table: "ClientDebitMemos",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientDebitMemos_StoreId",
                table: "ClientDebitMemos",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPriceRequestDetails_ClientPriceRequestHeaderId",
                table: "ClientPriceRequestDetails",
                column: "ClientPriceRequestHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPriceRequestDetails_CostCenterId",
                table: "ClientPriceRequestDetails",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPriceRequestDetails_ItemId",
                table: "ClientPriceRequestDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPriceRequestDetails_ItemPackageId",
                table: "ClientPriceRequestDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPriceRequestHeaders_ArchiveHeaderId",
                table: "ClientPriceRequestHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPriceRequestHeaders_ClientId",
                table: "ClientPriceRequestHeaders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPriceRequestHeaders_SellerId",
                table: "ClientPriceRequestHeaders",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientPriceRequestHeaders_StoreId",
                table: "ClientPriceRequestHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationApprovalDetails_ClientQuotationApprovalHeader~",
                table: "ClientQuotationApprovalDetails",
                column: "ClientQuotationApprovalHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationApprovalDetails_CostCenterId",
                table: "ClientQuotationApprovalDetails",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationApprovalDetails_ItemId",
                table: "ClientQuotationApprovalDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationApprovalDetails_ItemPackageId",
                table: "ClientQuotationApprovalDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationApprovalHeaders_ArchiveHeaderId",
                table: "ClientQuotationApprovalHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationApprovalHeaders_ClientId",
                table: "ClientQuotationApprovalHeaders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationApprovalHeaders_ClientQuotationHeaderId",
                table: "ClientQuotationApprovalHeaders",
                column: "ClientQuotationHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationApprovalHeaders_SellerId",
                table: "ClientQuotationApprovalHeaders",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationApprovalHeaders_StoreId",
                table: "ClientQuotationApprovalHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationApprovalHeaders_TaxTypeId",
                table: "ClientQuotationApprovalHeaders",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationDetailApprovalTaxes_ClientQuotationApprovalDe~",
                table: "ClientQuotationDetailApprovalTaxes",
                column: "ClientQuotationApprovalDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationDetailApprovalTaxes_CreditAccountId",
                table: "ClientQuotationDetailApprovalTaxes",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationDetailApprovalTaxes_TaxId",
                table: "ClientQuotationDetailApprovalTaxes",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationDetails_ClientQuotationHeaderId",
                table: "ClientQuotationDetails",
                column: "ClientQuotationHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationDetails_CostCenterId",
                table: "ClientQuotationDetails",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationDetails_ItemId",
                table: "ClientQuotationDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationDetails_ItemPackageId",
                table: "ClientQuotationDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationDetailTaxes_ClientQuotationDetailId",
                table: "ClientQuotationDetailTaxes",
                column: "ClientQuotationDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationDetailTaxes_CreditAccountId",
                table: "ClientQuotationDetailTaxes",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationDetailTaxes_TaxId",
                table: "ClientQuotationDetailTaxes",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationHeaders_ArchiveHeaderId",
                table: "ClientQuotationHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationHeaders_ClientId",
                table: "ClientQuotationHeaders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationHeaders_ClientPriceRequestHeaderId",
                table: "ClientQuotationHeaders",
                column: "ClientPriceRequestHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationHeaders_SellerId",
                table: "ClientQuotationHeaders",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationHeaders_StoreId",
                table: "ClientQuotationHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientQuotationHeaders_TaxTypeId",
                table: "ClientQuotationHeaders",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_AccountId",
                table: "Clients",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_ArchiveHeaderId",
                table: "Clients",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_CompanyId",
                table: "Clients",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_SellerId",
                table: "Clients",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_CurrencyId",
                table: "Companies",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenterJournalDetails_CostCenterId",
                table: "CostCenterJournalDetails",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenterJournalDetails_ItemId",
                table: "CostCenterJournalDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenterJournalDetails_JournalDetailId",
                table: "CostCenterJournalDetails",
                column: "JournalDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenterJournalDetails_MenuCode",
                table: "CostCenterJournalDetails",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenters_ArchiveHeaderId",
                table: "CostCenters",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_CostCenters_CompanyId",
                table: "CostCenters",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Countries_CurrencyId",
                table: "Countries",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRates_FromCurrencyId",
                table: "CurrencyRates",
                column: "FromCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrencyRates_ToCurrencyId",
                table: "CurrencyRates",
                column: "ToCurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_CityId",
                table: "Districts",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetMovementDetails_FixedAssetId",
                table: "FixedAssetMovementDetails",
                column: "FixedAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetMovementDetails_FixedAssetMovementHeaderId",
                table: "FixedAssetMovementDetails",
                column: "FixedAssetMovementHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetMovementHeaders_ArchiveHeaderId",
                table: "FixedAssetMovementHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetMovementHeaders_CostCenterToId",
                table: "FixedAssetMovementHeaders",
                column: "CostCenterToId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetMovementHeaders_StoreId",
                table: "FixedAssetMovementHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssets_AccountId",
                table: "FixedAssets",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssets_ArchiveHeaderId",
                table: "FixedAssets",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssets_CompanyId",
                table: "FixedAssets",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssets_CumulativeDepreciationAccountId",
                table: "FixedAssets",
                column: "CumulativeDepreciationAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssets_DepreciationAccountId",
                table: "FixedAssets",
                column: "DepreciationAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetVoucherDetailPayments_AccountId",
                table: "FixedAssetVoucherDetailPayments",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetVoucherDetailPayments_CurrencyId",
                table: "FixedAssetVoucherDetailPayments",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetVoucherDetailPayments_FixedAssetVoucherDetailId",
                table: "FixedAssetVoucherDetailPayments",
                column: "FixedAssetVoucherDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetVoucherDetailPayments_PaymentMethodId",
                table: "FixedAssetVoucherDetailPayments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetVoucherDetails_FixedAssetId",
                table: "FixedAssetVoucherDetails",
                column: "FixedAssetId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetVoucherDetails_FixedAssetVoucherHeaderId",
                table: "FixedAssetVoucherDetails",
                column: "FixedAssetVoucherHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetVoucherHeaders_ArchiveHeaderId",
                table: "FixedAssetVoucherHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetVoucherHeaders_FixedAssetVoucherTypeId",
                table: "FixedAssetVoucherHeaders",
                column: "FixedAssetVoucherTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetVoucherHeaders_JournalHeaderId",
                table: "FixedAssetVoucherHeaders",
                column: "JournalHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetVoucherHeaders_SellerId",
                table: "FixedAssetVoucherHeaders",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_FixedAssetVoucherHeaders_StoreId",
                table: "FixedAssetVoucherHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransferDetails_InternalTransferHeaderId",
                table: "InternalTransferDetails",
                column: "InternalTransferHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransferDetails_ItemId",
                table: "InternalTransferDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransferDetails_ItemPackageId",
                table: "InternalTransferDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransferHeaders_ArchiveHeaderId",
                table: "InternalTransferHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransferHeaders_FromStoreId",
                table: "InternalTransferHeaders",
                column: "FromStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransferHeaders_MenuCode",
                table: "InternalTransferHeaders",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransferHeaders_ToStoreId",
                table: "InternalTransferHeaders",
                column: "ToStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransferReceiveDetails_InternalTransferReceiveHeader~",
                table: "InternalTransferReceiveDetails",
                column: "InternalTransferReceiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransferReceiveDetails_ItemId",
                table: "InternalTransferReceiveDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransferReceiveDetails_ItemPackageId",
                table: "InternalTransferReceiveDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransferReceiveHeaders_ArchiveHeaderId",
                table: "InternalTransferReceiveHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransferReceiveHeaders_FromStoreId",
                table: "InternalTransferReceiveHeaders",
                column: "FromStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransferReceiveHeaders_InternalTransferHeaderId",
                table: "InternalTransferReceiveHeaders",
                column: "InternalTransferHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransferReceiveHeaders_MenuCode",
                table: "InternalTransferReceiveHeaders",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_InternalTransferReceiveHeaders_ToStoreId",
                table: "InternalTransferReceiveHeaders",
                column: "ToStoreId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryInDetails_InventoryInHeaderId",
                table: "InventoryInDetails",
                column: "InventoryInHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryInDetails_ItemId",
                table: "InventoryInDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryInDetails_ItemPackageId",
                table: "InventoryInDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryInHeaders_ArchiveHeaderId",
                table: "InventoryInHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryInHeaders_StoreId",
                table: "InventoryInHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryOutDetails_InventoryOutHeaderId",
                table: "InventoryOutDetails",
                column: "InventoryOutHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryOutDetails_ItemId",
                table: "InventoryOutDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryOutDetails_ItemPackageId",
                table: "InventoryOutDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryOutHeaders_ArchiveHeaderId",
                table: "InventoryOutHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryOutHeaders_StoreId",
                table: "InventoryOutHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceExpenseTypes_CompanyId",
                table: "InvoiceExpenseTypes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceStockInReturns_PurchaseInvoiceReturnHeaderId",
                table: "InvoiceStockInReturns",
                column: "PurchaseInvoiceReturnHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceStockInReturns_StockInReturnHeaderId",
                table: "InvoiceStockInReturns",
                column: "StockInReturnHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceStockIns_PurchaseInvoiceHeaderId",
                table: "InvoiceStockIns",
                column: "PurchaseInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceStockIns_StockInHeaderId",
                table: "InvoiceStockIns",
                column: "StockInHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceStockOutReturns_SalesInvoiceReturnHeaderId",
                table: "InvoiceStockOutReturns",
                column: "SalesInvoiceReturnHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceStockOutReturns_StockOutReturnHeaderId",
                table: "InvoiceStockOutReturns",
                column: "StockOutReturnHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceStockOuts_SalesInvoiceHeaderId",
                table: "InvoiceStockOuts",
                column: "SalesInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceStockOuts_StockOutHeaderId",
                table: "InvoiceStockOuts",
                column: "StockOutHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAttributes_ItemAttributeTypeId",
                table: "ItemAttributes",
                column: "ItemAttributeTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAttributes_ItemId",
                table: "ItemAttributes",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemAttributeTypes_CompanyId",
                table: "ItemAttributeTypes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemBarCodeDetails_ItemBarCodeId",
                table: "ItemBarCodeDetails",
                column: "ItemBarCodeId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemBarCodes_FromPackageId",
                table: "ItemBarCodes",
                column: "FromPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemBarCodes_ItemId",
                table: "ItemBarCodes",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemBarCodes_ToPackageId",
                table: "ItemBarCodes",
                column: "ToPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCategories_CompanyId",
                table: "ItemCategories",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCosts_ItemId",
                table: "ItemCosts",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCosts_ItemPackageId",
                table: "ItemCosts",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCosts_StoreId",
                table: "ItemCosts",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCostUpdateDetails_ItemCostUpdateHeaderId",
                table: "ItemCostUpdateDetails",
                column: "ItemCostUpdateHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCostUpdateDetails_ItemId",
                table: "ItemCostUpdateDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCostUpdateDetails_ItemPackageId",
                table: "ItemCostUpdateDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCostUpdateHeaders_StoreId",
                table: "ItemCostUpdateHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCurrentBalances_ItemId",
                table: "ItemCurrentBalances",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCurrentBalances_ItemPackageId",
                table: "ItemCurrentBalances",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemCurrentBalances_StoreId",
                table: "ItemCurrentBalances",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDisassembleDetails_FromPackageId",
                table: "ItemDisassembleDetails",
                column: "FromPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDisassembleDetails_ItemDisassembleHeaderId",
                table: "ItemDisassembleDetails",
                column: "ItemDisassembleHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDisassembleDetails_ItemId",
                table: "ItemDisassembleDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDisassembleDetails_ToPackageId",
                table: "ItemDisassembleDetails",
                column: "ToPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDisassembleHeaders_MenuCode",
                table: "ItemDisassembleHeaders",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDisassembleHeaders_StoreId",
                table: "ItemDisassembleHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDisassembles_ItemDisassembleHeaderId",
                table: "ItemDisassembles",
                column: "ItemDisassembleHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDisassembles_ItemId",
                table: "ItemDisassembles",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDisassembles_ItemPackageId",
                table: "ItemDisassembles",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDisassembles_StoreId",
                table: "ItemDisassembles",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDisassembleSerials_ItemDisassembleHeaderId",
                table: "ItemDisassembleSerials",
                column: "ItemDisassembleHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDisassembleSerials_ItemId",
                table: "ItemDisassembleSerials",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDisassembleSerials_ItemPackageId",
                table: "ItemDisassembleSerials",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemDisassembleSerials_MainItemPackageId",
                table: "ItemDisassembleSerials",
                column: "MainItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemImportExcelHistories_ArchiveHeaderId",
                table: "ItemImportExcelHistories",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemNegativeSalesDetails_ItemNegativeSalesHeaderId",
                table: "ItemNegativeSalesDetails",
                column: "ItemNegativeSalesHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemNegativeSalesDetails_MenuCode",
                table: "ItemNegativeSalesDetails",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_ItemNegativeSalesHeaders_ItemId",
                table: "ItemNegativeSalesHeaders",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemNegativeSalesHeaders_ItemPackageId",
                table: "ItemNegativeSalesHeaders",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemNegativeSalesHeaders_StoreId",
                table: "ItemNegativeSalesHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPackages_CompanyId",
                table: "ItemPackages",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPacking_FromPackageId",
                table: "ItemPacking",
                column: "FromPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPacking_ItemId",
                table: "ItemPacking",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemPacking_ToPackageId",
                table: "ItemPacking",
                column: "ToPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ArchiveHeaderId",
                table: "Items",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_CompanyId",
                table: "Items",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemCategoryId",
                table: "Items",
                column: "ItemCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemSectionId",
                table: "Items",
                column: "ItemSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemSubCategoryId",
                table: "Items",
                column: "ItemSubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemSubSectionId",
                table: "Items",
                column: "ItemSubSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_ItemTypeId",
                table: "Items",
                column: "ItemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_MainItemId",
                table: "Items",
                column: "MainItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_PurchaseAccountId",
                table: "Items",
                column: "PurchaseAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_SalesAccountId",
                table: "Items",
                column: "SalesAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_SingularPackageId",
                table: "Items",
                column: "SingularPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_TaxTypeId",
                table: "Items",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Items_VendorId",
                table: "Items",
                column: "VendorId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSections_CompanyId",
                table: "ItemSections",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSections_ItemSubCategoryId",
                table: "ItemSections",
                column: "ItemSubCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSubCategories_CompanyId",
                table: "ItemSubCategories",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSubCategories_ItemCategoryId",
                table: "ItemSubCategories",
                column: "ItemCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSubSections_CompanyId",
                table: "ItemSubSections",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemSubSections_ItemSectionId",
                table: "ItemSubSections",
                column: "ItemSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTaxes_ItemId",
                table: "ItemTaxes",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ItemTaxes_TaxId",
                table: "ItemTaxes",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetails_AccountId",
                table: "JournalDetails",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetails_CurrencyId",
                table: "JournalDetails",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetails_EntityTypeId",
                table: "JournalDetails",
                column: "EntityTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetails_JournalHeaderId",
                table: "JournalDetails",
                column: "JournalHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetails_TaxId",
                table: "JournalDetails",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalDetails_TaxTypeId",
                table: "JournalDetails",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalHeaders_ArchiveHeaderId",
                table: "JournalHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalHeaders_JournalTypeId",
                table: "JournalHeaders",
                column: "JournalTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalHeaders_MenuCode",
                table: "JournalHeaders",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_JournalHeaders_StoreId",
                table: "JournalHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_MainItems_CompanyId",
                table: "MainItems",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MainItems_ItemSubSectionId",
                table: "MainItems",
                column: "ItemSubSectionId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCompanies_CompanyId",
                table: "MenuCompanies",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuCompanies_MenuCode",
                table: "MenuCompanies",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_MenuEncodings_MenuCode",
                table: "MenuEncodings",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_MenuEncodings_StoreId",
                table: "MenuEncodings",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuNoteIdentifiers_ColumnIdentifierId",
                table: "MenuNoteIdentifiers",
                column: "ColumnIdentifierId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuNoteIdentifiers_CompanyId",
                table: "MenuNoteIdentifiers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuNoteIdentifiers_MenuCode",
                table: "MenuNoteIdentifiers",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_MenuNotes_MenuCode",
                table: "MenuNotes",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_MenuNotes_MenuNoteIdentifierId",
                table: "MenuNotes",
                column: "MenuNoteIdentifierId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationDetails_NotificationHeaderId",
                table: "NotificationDetails",
                column: "NotificationHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_NotificationHeaders_NotificationTypeId",
                table: "NotificationHeaders",
                column: "NotificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_CommissionAccountId",
                table: "PaymentMethods",
                column: "CommissionAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_CommissionTaxAccountId",
                table: "PaymentMethods",
                column: "CommissionTaxAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_CompanyId",
                table: "PaymentMethods",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_PaymentAccountId",
                table: "PaymentMethods",
                column: "PaymentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_PaymentTypeId",
                table: "PaymentMethods",
                column: "PaymentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentMethods_TaxId",
                table: "PaymentMethods",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVoucherDetails_AccountId",
                table: "PaymentVoucherDetails",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVoucherDetails_CurrencyId",
                table: "PaymentVoucherDetails",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVoucherDetails_PaymentMethodId",
                table: "PaymentVoucherDetails",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVoucherDetails_PaymentVoucherHeaderId",
                table: "PaymentVoucherDetails",
                column: "PaymentVoucherHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVoucherHeaders_ArchiveHeaderId",
                table: "PaymentVoucherHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVoucherHeaders_JournalHeaderId",
                table: "PaymentVoucherHeaders",
                column: "JournalHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVoucherHeaders_SellerId",
                table: "PaymentVoucherHeaders",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVoucherHeaders_StoreId",
                table: "PaymentVoucherHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVoucherHeaders_SupplierId",
                table: "PaymentVoucherHeaders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentVoucherInvoices_PaymentVoucherHeaderId",
                table: "PaymentVoucherInvoices",
                column: "PaymentVoucherHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestDetails_CostCenterId",
                table: "ProductRequestDetails",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestDetails_ItemId",
                table: "ProductRequestDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestDetails_ItemPackageId",
                table: "ProductRequestDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestDetails_ProductRequestHeaderId",
                table: "ProductRequestDetails",
                column: "ProductRequestHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestHeaders_ArchiveHeaderId",
                table: "ProductRequestHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestHeaders_StoreId",
                table: "ProductRequestHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestPriceDetails_CostCenterId",
                table: "ProductRequestPriceDetails",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestPriceDetails_ItemId",
                table: "ProductRequestPriceDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestPriceDetails_ItemPackageId",
                table: "ProductRequestPriceDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestPriceDetails_ProductRequestPriceHeaderId",
                table: "ProductRequestPriceDetails",
                column: "ProductRequestPriceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestPriceDetailTaxes_DebitAccountId",
                table: "ProductRequestPriceDetailTaxes",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestPriceDetailTaxes_ProductRequestPriceDetailId",
                table: "ProductRequestPriceDetailTaxes",
                column: "ProductRequestPriceDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestPriceDetailTaxes_TaxId",
                table: "ProductRequestPriceDetailTaxes",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestPriceHeaders_ArchiveHeaderId",
                table: "ProductRequestPriceHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestPriceHeaders_ProductRequestHeaderId",
                table: "ProductRequestPriceHeaders",
                column: "ProductRequestHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestPriceHeaders_StoreId",
                table: "ProductRequestPriceHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestPriceHeaders_SupplierId",
                table: "ProductRequestPriceHeaders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductRequestPriceHeaders_TaxTypeId",
                table: "ProductRequestPriceHeaders",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceDetails_CostCenterId",
                table: "ProformaInvoiceDetails",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceDetails_ItemId",
                table: "ProformaInvoiceDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceDetails_ItemPackageId",
                table: "ProformaInvoiceDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceDetails_ProformaInvoiceHeaderId",
                table: "ProformaInvoiceDetails",
                column: "ProformaInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceDetailTaxes_CreditAccountId",
                table: "ProformaInvoiceDetailTaxes",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceDetailTaxes_ProformaInvoiceDetailId",
                table: "ProformaInvoiceDetailTaxes",
                column: "ProformaInvoiceDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceDetailTaxes_TaxId",
                table: "ProformaInvoiceDetailTaxes",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceHeaders_ArchiveHeaderId",
                table: "ProformaInvoiceHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceHeaders_ClientId",
                table: "ProformaInvoiceHeaders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceHeaders_ClientQuotationApprovalHeaderId",
                table: "ProformaInvoiceHeaders",
                column: "ClientQuotationApprovalHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceHeaders_DocumentStatusId",
                table: "ProformaInvoiceHeaders",
                column: "DocumentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceHeaders_SellerId",
                table: "ProformaInvoiceHeaders",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceHeaders_ShipmentTypeId",
                table: "ProformaInvoiceHeaders",
                column: "ShipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceHeaders_ShippingStatusId",
                table: "ProformaInvoiceHeaders",
                column: "ShippingStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceHeaders_StoreId",
                table: "ProformaInvoiceHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ProformaInvoiceHeaders_TaxTypeId",
                table: "ProformaInvoiceHeaders",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceDetails_ItemId",
                table: "PurchaseInvoiceDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceDetails_ItemPackageId",
                table: "PurchaseInvoiceDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceDetails_PurchaseInvoiceHeaderId",
                table: "PurchaseInvoiceDetails",
                column: "PurchaseInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceDetails_VatTaxId",
                table: "PurchaseInvoiceDetails",
                column: "VatTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceDetails_VatTaxTypeId",
                table: "PurchaseInvoiceDetails",
                column: "VatTaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceDetailTaxes_DebitAccountId",
                table: "PurchaseInvoiceDetailTaxes",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceDetailTaxes_PurchaseInvoiceDetailId",
                table: "PurchaseInvoiceDetailTaxes",
                column: "PurchaseInvoiceDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceDetailTaxes_TaxId",
                table: "PurchaseInvoiceDetailTaxes",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceDetailTaxes_TaxTypeId",
                table: "PurchaseInvoiceDetailTaxes",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceExpenses_InvoiceExpenseTypeId",
                table: "PurchaseInvoiceExpenses",
                column: "InvoiceExpenseTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceExpenses_PurchaseInvoiceHeaderId",
                table: "PurchaseInvoiceExpenses",
                column: "PurchaseInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceHeaders_ArchiveHeaderId",
                table: "PurchaseInvoiceHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceHeaders_CreditAccountId",
                table: "PurchaseInvoiceHeaders",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceHeaders_DebitAccountId",
                table: "PurchaseInvoiceHeaders",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceHeaders_InvoiceTypeId",
                table: "PurchaseInvoiceHeaders",
                column: "InvoiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceHeaders_JournalHeaderId",
                table: "PurchaseInvoiceHeaders",
                column: "JournalHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceHeaders_MenuCode",
                table: "PurchaseInvoiceHeaders",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceHeaders_PurchaseOrderHeaderId",
                table: "PurchaseInvoiceHeaders",
                column: "PurchaseOrderHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceHeaders_StoreId",
                table: "PurchaseInvoiceHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceHeaders_SupplierId",
                table: "PurchaseInvoiceHeaders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceHeaders_TaxTypeId",
                table: "PurchaseInvoiceHeaders",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnDetails_ItemId",
                table: "PurchaseInvoiceReturnDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnDetails_ItemPackageId",
                table: "PurchaseInvoiceReturnDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnDetails_PurchaseInvoiceReturnHeaderId",
                table: "PurchaseInvoiceReturnDetails",
                column: "PurchaseInvoiceReturnHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnDetails_VatTaxId",
                table: "PurchaseInvoiceReturnDetails",
                column: "VatTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnDetails_VatTaxTypeId",
                table: "PurchaseInvoiceReturnDetails",
                column: "VatTaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnDetailTaxes_CreditAccountId",
                table: "PurchaseInvoiceReturnDetailTaxes",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnDetailTaxes_PurchaseInvoiceReturnDetail~",
                table: "PurchaseInvoiceReturnDetailTaxes",
                column: "PurchaseInvoiceReturnDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnDetailTaxes_TaxId",
                table: "PurchaseInvoiceReturnDetailTaxes",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnDetailTaxes_TaxTypeId",
                table: "PurchaseInvoiceReturnDetailTaxes",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnHeaders_ArchiveHeaderId",
                table: "PurchaseInvoiceReturnHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnHeaders_CreditAccountId",
                table: "PurchaseInvoiceReturnHeaders",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnHeaders_DebitAccountId",
                table: "PurchaseInvoiceReturnHeaders",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnHeaders_JournalHeaderId",
                table: "PurchaseInvoiceReturnHeaders",
                column: "JournalHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnHeaders_MenuCode",
                table: "PurchaseInvoiceReturnHeaders",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnHeaders_PurchaseInvoiceHeaderId",
                table: "PurchaseInvoiceReturnHeaders",
                column: "PurchaseInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnHeaders_StoreId",
                table: "PurchaseInvoiceReturnHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnHeaders_SupplierId",
                table: "PurchaseInvoiceReturnHeaders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceReturnHeaders_TaxTypeId",
                table: "PurchaseInvoiceReturnHeaders",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceSettlements_PaymentVoucherHeaderId",
                table: "PurchaseInvoiceSettlements",
                column: "PaymentVoucherHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseInvoiceSettlements_PurchaseInvoiceHeaderId",
                table: "PurchaseInvoiceSettlements",
                column: "PurchaseInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_CostCenterId",
                table: "PurchaseOrderDetails",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_ItemId",
                table: "PurchaseOrderDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_ItemPackageId",
                table: "PurchaseOrderDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetails_PurchaseOrderHeaderId",
                table: "PurchaseOrderDetails",
                column: "PurchaseOrderHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetailTaxes_DebitAccountId",
                table: "PurchaseOrderDetailTaxes",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetailTaxes_PurchaseOrderDetailId",
                table: "PurchaseOrderDetailTaxes",
                column: "PurchaseOrderDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderDetailTaxes_TaxId",
                table: "PurchaseOrderDetailTaxes",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderHeaders_ArchiveHeaderId",
                table: "PurchaseOrderHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderHeaders_DocumentStatusId",
                table: "PurchaseOrderHeaders",
                column: "DocumentStatusId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderHeaders_ShipmentTypeId",
                table: "PurchaseOrderHeaders",
                column: "ShipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderHeaders_StoreId",
                table: "PurchaseOrderHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderHeaders_SupplierId",
                table: "PurchaseOrderHeaders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderHeaders_SupplierQuotationHeaderId",
                table: "PurchaseOrderHeaders",
                column: "SupplierQuotationHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderHeaders_TaxTypeId",
                table: "PurchaseOrderHeaders",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVoucherDetails_AccountId",
                table: "ReceiptVoucherDetails",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVoucherDetails_CurrencyId",
                table: "ReceiptVoucherDetails",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVoucherDetails_PaymentMethodId",
                table: "ReceiptVoucherDetails",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVoucherDetails_ReceiptVoucherHeaderId",
                table: "ReceiptVoucherDetails",
                column: "ReceiptVoucherHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVoucherHeaders_ArchiveHeaderId",
                table: "ReceiptVoucherHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVoucherHeaders_ClientId",
                table: "ReceiptVoucherHeaders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVoucherHeaders_JournalHeaderId",
                table: "ReceiptVoucherHeaders",
                column: "JournalHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVoucherHeaders_SellerId",
                table: "ReceiptVoucherHeaders",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVoucherHeaders_StoreId",
                table: "ReceiptVoucherHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_ReceiptVoucherInvoices_ReceiptVoucherHeaderId",
                table: "ReceiptVoucherInvoices",
                column: "ReceiptVoucherHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportPrintSettings_CompanyId",
                table: "ReportPrintSettings",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportPrintSettings_MenuCode",
                table: "ReportPrintSettings",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_ReportPrintSettings_PrintFormId",
                table: "ReportPrintSettings",
                column: "PrintFormId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceCollection_AccountId",
                table: "SalesInvoiceCollection",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceCollection_CurrencyId",
                table: "SalesInvoiceCollection",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceCollection_PaymentMethodId",
                table: "SalesInvoiceCollection",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceCollection_SalesInvoiceHeaderId",
                table: "SalesInvoiceCollection",
                column: "SalesInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceDetails_ItemId",
                table: "SalesInvoiceDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceDetails_ItemPackageId",
                table: "SalesInvoiceDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceDetails_SalesInvoiceHeaderId",
                table: "SalesInvoiceDetails",
                column: "SalesInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceDetails_VatTaxId",
                table: "SalesInvoiceDetails",
                column: "VatTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceDetails_VatTaxTypeId",
                table: "SalesInvoiceDetails",
                column: "VatTaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceDetailTaxes_SalesInvoiceDetailId",
                table: "SalesInvoiceDetailTaxes",
                column: "SalesInvoiceDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceDetailTaxes_TaxId",
                table: "SalesInvoiceDetailTaxes",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceDetailTaxes_TaxTypeId",
                table: "SalesInvoiceDetailTaxes",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceHeaders_ArchiveHeaderId",
                table: "SalesInvoiceHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceHeaders_ClientId",
                table: "SalesInvoiceHeaders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceHeaders_CreditAccountId",
                table: "SalesInvoiceHeaders",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceHeaders_DebitAccountId",
                table: "SalesInvoiceHeaders",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceHeaders_InvoiceTypeId",
                table: "SalesInvoiceHeaders",
                column: "InvoiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceHeaders_JournalHeaderId",
                table: "SalesInvoiceHeaders",
                column: "JournalHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceHeaders_MenuCode",
                table: "SalesInvoiceHeaders",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceHeaders_ProformaInvoiceHeaderId",
                table: "SalesInvoiceHeaders",
                column: "ProformaInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceHeaders_SellerId",
                table: "SalesInvoiceHeaders",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceHeaders_ShipmentTypeId",
                table: "SalesInvoiceHeaders",
                column: "ShipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceHeaders_StoreId",
                table: "SalesInvoiceHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceHeaders_TaxTypeId",
                table: "SalesInvoiceHeaders",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnDetails_ItemId",
                table: "SalesInvoiceReturnDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnDetails_ItemPackageId",
                table: "SalesInvoiceReturnDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnDetails_SalesInvoiceReturnHeaderId",
                table: "SalesInvoiceReturnDetails",
                column: "SalesInvoiceReturnHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnDetails_VatTaxId",
                table: "SalesInvoiceReturnDetails",
                column: "VatTaxId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnDetails_VatTaxTypeId",
                table: "SalesInvoiceReturnDetails",
                column: "VatTaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnDetailTaxes_DebitAccountId",
                table: "SalesInvoiceReturnDetailTaxes",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnDetailTaxes_SalesInvoiceReturnDetailId",
                table: "SalesInvoiceReturnDetailTaxes",
                column: "SalesInvoiceReturnDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnDetailTaxes_TaxId",
                table: "SalesInvoiceReturnDetailTaxes",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnDetailTaxes_TaxTypeId",
                table: "SalesInvoiceReturnDetailTaxes",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnHeaders_ArchiveHeaderId",
                table: "SalesInvoiceReturnHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnHeaders_ClientId",
                table: "SalesInvoiceReturnHeaders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnHeaders_CreditAccountId",
                table: "SalesInvoiceReturnHeaders",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnHeaders_DebitAccountId",
                table: "SalesInvoiceReturnHeaders",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnHeaders_JournalHeaderId",
                table: "SalesInvoiceReturnHeaders",
                column: "JournalHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnHeaders_MenuCode",
                table: "SalesInvoiceReturnHeaders",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnHeaders_SalesInvoiceHeaderId",
                table: "SalesInvoiceReturnHeaders",
                column: "SalesInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnHeaders_SellerId",
                table: "SalesInvoiceReturnHeaders",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnHeaders_ShipmentTypeId",
                table: "SalesInvoiceReturnHeaders",
                column: "ShipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnHeaders_StoreId",
                table: "SalesInvoiceReturnHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnHeaders_TaxTypeId",
                table: "SalesInvoiceReturnHeaders",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnPayments_AccountId",
                table: "SalesInvoiceReturnPayments",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnPayments_CurrencyId",
                table: "SalesInvoiceReturnPayments",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnPayments_PaymentMethodId",
                table: "SalesInvoiceReturnPayments",
                column: "PaymentMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceReturnPayments_SalesInvoiceReturnHeaderId",
                table: "SalesInvoiceReturnPayments",
                column: "SalesInvoiceReturnHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceSettlements_ReceiptVoucherHeaderId",
                table: "SalesInvoiceSettlements",
                column: "ReceiptVoucherHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesInvoiceSettlements_SalesInvoiceHeaderId",
                table: "SalesInvoiceSettlements",
                column: "SalesInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerCommissionMethod_CompanyId",
                table: "SellerCommissionMethod",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerCommissionMethod_SellerCommissionTypeId",
                table: "SellerCommissionMethod",
                column: "SellerCommissionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SellerCommissions_SellerCommissionMethodId",
                table: "SellerCommissions",
                column: "SellerCommissionMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_ArchiveHeaderId",
                table: "Sellers",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_CompanyId",
                table: "Sellers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_SellerCommissionMethodId",
                table: "Sellers",
                column: "SellerCommissionMethodId");

            migrationBuilder.CreateIndex(
                name: "IX_Sellers_SellerTypeId",
                table: "Sellers",
                column: "SellerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ShipmentTypes_CompanyId",
                table: "ShipmentTypes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingStatuses_CompanyId",
                table: "ShippingStatuses",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_ShippingStatuses_MenuCode",
                table: "ShippingStatuses",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_States_CountryId",
                table: "States",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInDetails_ItemId",
                table: "StockInDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInDetails_ItemPackageId",
                table: "StockInDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInDetails_StockInHeaderId",
                table: "StockInDetails",
                column: "StockInHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInDetailTaxes_DebitAccountId",
                table: "StockInDetailTaxes",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInDetailTaxes_StockInDetailId",
                table: "StockInDetailTaxes",
                column: "StockInDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInDetailTaxes_TaxId",
                table: "StockInDetailTaxes",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInHeaders_ArchiveHeaderId",
                table: "StockInHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInHeaders_MenuCode",
                table: "StockInHeaders",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_StockInHeaders_PurchaseInvoiceHeaderId",
                table: "StockInHeaders",
                column: "PurchaseInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInHeaders_PurchaseOrderHeaderId",
                table: "StockInHeaders",
                column: "PurchaseOrderHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInHeaders_StockTypeId",
                table: "StockInHeaders",
                column: "StockTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInHeaders_StoreId",
                table: "StockInHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInHeaders_SupplierId",
                table: "StockInHeaders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInReturnDetails_ItemId",
                table: "StockInReturnDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInReturnDetails_ItemPackageId",
                table: "StockInReturnDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInReturnDetails_StockInReturnHeaderId",
                table: "StockInReturnDetails",
                column: "StockInReturnHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInReturnDetailTaxes_CreditAccountId",
                table: "StockInReturnDetailTaxes",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInReturnDetailTaxes_StockInReturnDetailId",
                table: "StockInReturnDetailTaxes",
                column: "StockInReturnDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInReturnDetailTaxes_TaxId",
                table: "StockInReturnDetailTaxes",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInReturnHeaders_ArchiveHeaderId",
                table: "StockInReturnHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInReturnHeaders_MenuCode",
                table: "StockInReturnHeaders",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_StockInReturnHeaders_PurchaseInvoiceHeaderId",
                table: "StockInReturnHeaders",
                column: "PurchaseInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInReturnHeaders_StockInHeaderId",
                table: "StockInReturnHeaders",
                column: "StockInHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInReturnHeaders_StockTypeId",
                table: "StockInReturnHeaders",
                column: "StockTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInReturnHeaders_StoreId",
                table: "StockInReturnHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StockInReturnHeaders_SupplierId",
                table: "StockInReturnHeaders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutDetails_ItemId",
                table: "StockOutDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutDetails_ItemPackageId",
                table: "StockOutDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutDetails_StockOutHeaderId",
                table: "StockOutDetails",
                column: "StockOutHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutDetailTaxes_StockOutDetailId",
                table: "StockOutDetailTaxes",
                column: "StockOutDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutDetailTaxes_TaxId",
                table: "StockOutDetailTaxes",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutHeaders_ArchiveHeaderId",
                table: "StockOutHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutHeaders_ClientId",
                table: "StockOutHeaders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutHeaders_MenuCode",
                table: "StockOutHeaders",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutHeaders_ProformaInvoiceHeaderId",
                table: "StockOutHeaders",
                column: "ProformaInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutHeaders_SalesInvoiceHeaderId",
                table: "StockOutHeaders",
                column: "SalesInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutHeaders_StockTypeId",
                table: "StockOutHeaders",
                column: "StockTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutHeaders_StoreId",
                table: "StockOutHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutReturnDetails_ItemId",
                table: "StockOutReturnDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutReturnDetails_ItemPackageId",
                table: "StockOutReturnDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutReturnDetails_StockOutReturnHeaderId",
                table: "StockOutReturnDetails",
                column: "StockOutReturnHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutReturnDetailTaxes_StockOutReturnDetailId",
                table: "StockOutReturnDetailTaxes",
                column: "StockOutReturnDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutReturnDetailTaxes_TaxId",
                table: "StockOutReturnDetailTaxes",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutReturnHeaders_ArchiveHeaderId",
                table: "StockOutReturnHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutReturnHeaders_ClientId",
                table: "StockOutReturnHeaders",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutReturnHeaders_MenuCode",
                table: "StockOutReturnHeaders",
                column: "MenuCode");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutReturnHeaders_SalesInvoiceHeaderId",
                table: "StockOutReturnHeaders",
                column: "SalesInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutReturnHeaders_SellerId",
                table: "StockOutReturnHeaders",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutReturnHeaders_StockOutHeaderId",
                table: "StockOutReturnHeaders",
                column: "StockOutHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutReturnHeaders_StockTypeId",
                table: "StockOutReturnHeaders",
                column: "StockTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_StockOutReturnHeaders_StoreId",
                table: "StockOutReturnHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakingCarryOverDetails_ItemId",
                table: "StockTakingCarryOverDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakingCarryOverDetails_ItemPackageId",
                table: "StockTakingCarryOverDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakingCarryOverDetails_StockTakingCarryOverHeaderId",
                table: "StockTakingCarryOverDetails",
                column: "StockTakingCarryOverHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakingCarryOverEffectDetails_ItemId",
                table: "StockTakingCarryOverEffectDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakingCarryOverEffectDetails_ItemPackageId",
                table: "StockTakingCarryOverEffectDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakingCarryOverEffectDetails_StockTakingCarryOverHeader~",
                table: "StockTakingCarryOverEffectDetails",
                column: "StockTakingCarryOverHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakingCarryOverHeaders_StoreId",
                table: "StockTakingCarryOverHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakingDetails_ItemId",
                table: "StockTakingDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakingDetails_ItemPackageId",
                table: "StockTakingDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakingDetails_StockTakingHeaderId",
                table: "StockTakingDetails",
                column: "StockTakingHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTakingHeaders_StoreId",
                table: "StockTakingHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_BranchId",
                table: "Stores",
                column: "BranchId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_CityId",
                table: "Stores",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_CountryId",
                table: "Stores",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_DistrictId",
                table: "Stores",
                column: "DistrictId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_StateId",
                table: "Stores",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Stores_StoreClassificationId",
                table: "Stores",
                column: "StoreClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditMemos_ArchiveHeaderId",
                table: "SupplierCreditMemos",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditMemos_CreditAccountId",
                table: "SupplierCreditMemos",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditMemos_DebitAccountId",
                table: "SupplierCreditMemos",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditMemos_JournalHeaderId",
                table: "SupplierCreditMemos",
                column: "JournalHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditMemos_PurchaseInvoiceHeaderId",
                table: "SupplierCreditMemos",
                column: "PurchaseInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditMemos_StoreId",
                table: "SupplierCreditMemos",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierCreditMemos_SupplierId",
                table: "SupplierCreditMemos",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierDebitMemos_ArchiveHeaderId",
                table: "SupplierDebitMemos",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierDebitMemos_CreditAccountId",
                table: "SupplierDebitMemos",
                column: "CreditAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierDebitMemos_DebitAccountId",
                table: "SupplierDebitMemos",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierDebitMemos_JournalHeaderId",
                table: "SupplierDebitMemos",
                column: "JournalHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierDebitMemos_PurchaseInvoiceHeaderId",
                table: "SupplierDebitMemos",
                column: "PurchaseInvoiceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierDebitMemos_StoreId",
                table: "SupplierDebitMemos",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierDebitMemos_SupplierId",
                table: "SupplierDebitMemos",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationDetails_CostCenterId",
                table: "SupplierQuotationDetails",
                column: "CostCenterId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationDetails_ItemId",
                table: "SupplierQuotationDetails",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationDetails_ItemPackageId",
                table: "SupplierQuotationDetails",
                column: "ItemPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationDetails_SupplierQuotationHeaderId",
                table: "SupplierQuotationDetails",
                column: "SupplierQuotationHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationDetailTaxes_DebitAccountId",
                table: "SupplierQuotationDetailTaxes",
                column: "DebitAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationDetailTaxes_SupplierQuotationDetailId",
                table: "SupplierQuotationDetailTaxes",
                column: "SupplierQuotationDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationDetailTaxes_TaxId",
                table: "SupplierQuotationDetailTaxes",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationHeaders_ArchiveHeaderId",
                table: "SupplierQuotationHeaders",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationHeaders_ProductRequestPriceHeaderId",
                table: "SupplierQuotationHeaders",
                column: "ProductRequestPriceHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationHeaders_StoreId",
                table: "SupplierQuotationHeaders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationHeaders_SupplierId",
                table: "SupplierQuotationHeaders",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierQuotationHeaders_TaxTypeId",
                table: "SupplierQuotationHeaders",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_AccountId",
                table: "Suppliers",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_ArchiveHeaderId",
                table: "Suppliers",
                column: "ArchiveHeaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_CompanyId",
                table: "Suppliers",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Suppliers_ShipmentTypeId",
                table: "Suppliers",
                column: "ShipmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_CompanyId",
                table: "Taxes",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_CrAccount",
                table: "Taxes",
                column: "CrAccount");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_DrAccount",
                table: "Taxes",
                column: "DrAccount");

            migrationBuilder.CreateIndex(
                name: "IX_Taxes_TaxTypeId",
                table: "Taxes",
                column: "TaxTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaxPercents_TaxId",
                table: "TaxPercents",
                column: "TaxId");

            migrationBuilder.CreateIndex(
                name: "IX_Vendors_CompanyId",
                table: "Vendors",
                column: "CompanyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountStore");

            migrationBuilder.DropTable(
                name: "ApplicationFlagDetailImages");

            migrationBuilder.DropTable(
                name: "ApplicationFlagDetailSelects");

            migrationBuilder.DropTable(
                name: "ApproveRequestDetails");

            migrationBuilder.DropTable(
                name: "ApproveRequestUsers");

            migrationBuilder.DropTable(
                name: "ArchiveDetails");

            migrationBuilder.DropTable(
                name: "Banks");

            migrationBuilder.DropTable(
                name: "ClientCreditMemos");

            migrationBuilder.DropTable(
                name: "ClientDebitMemos");

            migrationBuilder.DropTable(
                name: "ClientPriceRequestDetails");

            migrationBuilder.DropTable(
                name: "ClientQuotationDetailApprovalTaxes");

            migrationBuilder.DropTable(
                name: "ClientQuotationDetailTaxes");

            migrationBuilder.DropTable(
                name: "CostCenterJournalDetails");

            migrationBuilder.DropTable(
                name: "CurrencyRates");

            migrationBuilder.DropTable(
                name: "DocumentTypes");

            migrationBuilder.DropTable(
                name: "FixedAssetMovementDetails");

            migrationBuilder.DropTable(
                name: "FixedAssetVoucherDetailPayments");

            migrationBuilder.DropTable(
                name: "InternalTransferDetails");

            migrationBuilder.DropTable(
                name: "InternalTransferReceiveDetails");

            migrationBuilder.DropTable(
                name: "InventoryInDetails");

            migrationBuilder.DropTable(
                name: "InventoryOutDetails");

            migrationBuilder.DropTable(
                name: "InvoiceStockInReturns");

            migrationBuilder.DropTable(
                name: "InvoiceStockIns");

            migrationBuilder.DropTable(
                name: "InvoiceStockOutReturns");

            migrationBuilder.DropTable(
                name: "InvoiceStockOuts");

            migrationBuilder.DropTable(
                name: "ItemAttributes");

            migrationBuilder.DropTable(
                name: "ItemBarCodeDetails");

            migrationBuilder.DropTable(
                name: "ItemCostCalculationType");

            migrationBuilder.DropTable(
                name: "ItemCosts");

            migrationBuilder.DropTable(
                name: "ItemCostUpdateDetails");

            migrationBuilder.DropTable(
                name: "ItemCurrentBalances");

            migrationBuilder.DropTable(
                name: "ItemDisassembleDetails");

            migrationBuilder.DropTable(
                name: "ItemDisassembles");

            migrationBuilder.DropTable(
                name: "ItemDisassembleSerials");

            migrationBuilder.DropTable(
                name: "ItemImportExcelHistories");

            migrationBuilder.DropTable(
                name: "ItemNegativeSalesDetails");

            migrationBuilder.DropTable(
                name: "ItemPacking");

            migrationBuilder.DropTable(
                name: "ItemTaxes");

            migrationBuilder.DropTable(
                name: "MenuCompanies");

            migrationBuilder.DropTable(
                name: "MenuEncodings");

            migrationBuilder.DropTable(
                name: "MenuNotes");

            migrationBuilder.DropTable(
                name: "NotificationDetails");

            migrationBuilder.DropTable(
                name: "PaymentVoucherDetails");

            migrationBuilder.DropTable(
                name: "PaymentVoucherInvoices");

            migrationBuilder.DropTable(
                name: "ProductRequestDetails");

            migrationBuilder.DropTable(
                name: "ProductRequestPriceDetailTaxes");

            migrationBuilder.DropTable(
                name: "ProformaInvoiceDetailTaxes");

            migrationBuilder.DropTable(
                name: "PurchaseInvoiceDetailTaxes");

            migrationBuilder.DropTable(
                name: "PurchaseInvoiceExpenses");

            migrationBuilder.DropTable(
                name: "PurchaseInvoiceReturnDetailTaxes");

            migrationBuilder.DropTable(
                name: "PurchaseInvoiceSettlements");

            migrationBuilder.DropTable(
                name: "PurchaseOrderDetailTaxes");

            migrationBuilder.DropTable(
                name: "ReceiptVoucherDetails");

            migrationBuilder.DropTable(
                name: "ReceiptVoucherInvoices");

            migrationBuilder.DropTable(
                name: "ReportPrintSettings");

            migrationBuilder.DropTable(
                name: "SalesInvoiceCollection");

            migrationBuilder.DropTable(
                name: "SalesInvoiceDetailTaxes");

            migrationBuilder.DropTable(
                name: "SalesInvoiceReturnDetailTaxes");

            migrationBuilder.DropTable(
                name: "SalesInvoiceReturnPayments");

            migrationBuilder.DropTable(
                name: "SalesInvoiceSettlements");

            migrationBuilder.DropTable(
                name: "SellerCommissions");

            migrationBuilder.DropTable(
                name: "StockInDetailTaxes");

            migrationBuilder.DropTable(
                name: "StockInReturnDetailTaxes");

            migrationBuilder.DropTable(
                name: "StockOutDetailTaxes");

            migrationBuilder.DropTable(
                name: "StockOutReturnDetailTaxes");

            migrationBuilder.DropTable(
                name: "StockTakingCarryOverDetails");

            migrationBuilder.DropTable(
                name: "StockTakingCarryOverEffectDetails");

            migrationBuilder.DropTable(
                name: "StockTakingDetails");

            migrationBuilder.DropTable(
                name: "SupplierCreditMemos");

            migrationBuilder.DropTable(
                name: "SupplierDebitMemos");

            migrationBuilder.DropTable(
                name: "SupplierQuotationDetailTaxes");

            migrationBuilder.DropTable(
                name: "SystemTasks");

            migrationBuilder.DropTable(
                name: "TaxPercents");

            migrationBuilder.DropTable(
                name: "TransactionTypes");

            migrationBuilder.DropTable(
                name: "AccountStoreTypes");

            migrationBuilder.DropTable(
                name: "ApplicationFlagDetailCompanies");

            migrationBuilder.DropTable(
                name: "ApproveRequests");

            migrationBuilder.DropTable(
                name: "ClientQuotationApprovalDetails");

            migrationBuilder.DropTable(
                name: "ClientQuotationDetails");

            migrationBuilder.DropTable(
                name: "JournalDetails");

            migrationBuilder.DropTable(
                name: "FixedAssetMovementHeaders");

            migrationBuilder.DropTable(
                name: "FixedAssetVoucherDetails");

            migrationBuilder.DropTable(
                name: "InternalTransferReceiveHeaders");

            migrationBuilder.DropTable(
                name: "InventoryInHeaders");

            migrationBuilder.DropTable(
                name: "InventoryOutHeaders");

            migrationBuilder.DropTable(
                name: "ItemAttributeTypes");

            migrationBuilder.DropTable(
                name: "ItemBarCodes");

            migrationBuilder.DropTable(
                name: "ItemCostUpdateHeaders");

            migrationBuilder.DropTable(
                name: "ItemDisassembleHeaders");

            migrationBuilder.DropTable(
                name: "ItemNegativeSalesHeaders");

            migrationBuilder.DropTable(
                name: "MenuNoteIdentifiers");

            migrationBuilder.DropTable(
                name: "NotificationHeaders");

            migrationBuilder.DropTable(
                name: "ProductRequestPriceDetails");

            migrationBuilder.DropTable(
                name: "ProformaInvoiceDetails");

            migrationBuilder.DropTable(
                name: "PurchaseInvoiceDetails");

            migrationBuilder.DropTable(
                name: "InvoiceExpenseTypes");

            migrationBuilder.DropTable(
                name: "PurchaseInvoiceReturnDetails");

            migrationBuilder.DropTable(
                name: "PaymentVoucherHeaders");

            migrationBuilder.DropTable(
                name: "PurchaseOrderDetails");

            migrationBuilder.DropTable(
                name: "ReportPrintForms");

            migrationBuilder.DropTable(
                name: "SalesInvoiceDetails");

            migrationBuilder.DropTable(
                name: "SalesInvoiceReturnDetails");

            migrationBuilder.DropTable(
                name: "PaymentMethods");

            migrationBuilder.DropTable(
                name: "ReceiptVoucherHeaders");

            migrationBuilder.DropTable(
                name: "StockInDetails");

            migrationBuilder.DropTable(
                name: "StockInReturnDetails");

            migrationBuilder.DropTable(
                name: "StockOutDetails");

            migrationBuilder.DropTable(
                name: "StockOutReturnDetails");

            migrationBuilder.DropTable(
                name: "StockTakingCarryOverHeaders");

            migrationBuilder.DropTable(
                name: "StockTakingHeaders");

            migrationBuilder.DropTable(
                name: "SupplierQuotationDetails");

            migrationBuilder.DropTable(
                name: "ApplicationFlagDetails");

            migrationBuilder.DropTable(
                name: "ApproveRequestTypes");

            migrationBuilder.DropTable(
                name: "ApproveStatuses");

            migrationBuilder.DropTable(
                name: "EntityTypes");

            migrationBuilder.DropTable(
                name: "FixedAssetVoucherHeaders");

            migrationBuilder.DropTable(
                name: "FixedAssets");

            migrationBuilder.DropTable(
                name: "InternalTransferHeaders");

            migrationBuilder.DropTable(
                name: "ColumnIdentifiers");

            migrationBuilder.DropTable(
                name: "NotificationTypes");

            migrationBuilder.DropTable(
                name: "PurchaseInvoiceReturnHeaders");

            migrationBuilder.DropTable(
                name: "SalesInvoiceReturnHeaders");

            migrationBuilder.DropTable(
                name: "PaymentTypes");

            migrationBuilder.DropTable(
                name: "Taxes");

            migrationBuilder.DropTable(
                name: "StockInReturnHeaders");

            migrationBuilder.DropTable(
                name: "StockOutReturnHeaders");

            migrationBuilder.DropTable(
                name: "Items");

            migrationBuilder.DropTable(
                name: "ApplicationFlagHeaders");

            migrationBuilder.DropTable(
                name: "ApplicationFlagTypes");

            migrationBuilder.DropTable(
                name: "ApproveStep");

            migrationBuilder.DropTable(
                name: "FixedAssetVoucherTypes");

            migrationBuilder.DropTable(
                name: "StockInHeaders");

            migrationBuilder.DropTable(
                name: "StockOutHeaders");

            migrationBuilder.DropTable(
                name: "ItemPackages");

            migrationBuilder.DropTable(
                name: "ItemTypes");

            migrationBuilder.DropTable(
                name: "MainItems");

            migrationBuilder.DropTable(
                name: "Vendors");

            migrationBuilder.DropTable(
                name: "ApplicationFlagTabs");

            migrationBuilder.DropTable(
                name: "Approves");

            migrationBuilder.DropTable(
                name: "PurchaseInvoiceHeaders");

            migrationBuilder.DropTable(
                name: "SalesInvoiceHeaders");

            migrationBuilder.DropTable(
                name: "StockTypes");

            migrationBuilder.DropTable(
                name: "ItemSubSections");

            migrationBuilder.DropTable(
                name: "PurchaseOrderHeaders");

            migrationBuilder.DropTable(
                name: "InvoiceTypes");

            migrationBuilder.DropTable(
                name: "JournalHeaders");

            migrationBuilder.DropTable(
                name: "ProformaInvoiceHeaders");

            migrationBuilder.DropTable(
                name: "ItemSections");

            migrationBuilder.DropTable(
                name: "SupplierQuotationHeaders");

            migrationBuilder.DropTable(
                name: "JournalTypes");

            migrationBuilder.DropTable(
                name: "ClientQuotationApprovalHeaders");

            migrationBuilder.DropTable(
                name: "DocumentStatuses");

            migrationBuilder.DropTable(
                name: "ShippingStatuses");

            migrationBuilder.DropTable(
                name: "ItemSubCategories");

            migrationBuilder.DropTable(
                name: "ProductRequestPriceHeaders");

            migrationBuilder.DropTable(
                name: "ClientQuotationHeaders");

            migrationBuilder.DropTable(
                name: "ItemCategories");

            migrationBuilder.DropTable(
                name: "ProductRequestHeaders");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropTable(
                name: "ClientPriceRequestHeaders");

            migrationBuilder.DropTable(
                name: "TaxTypes");

            migrationBuilder.DropTable(
                name: "ShipmentTypes");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Stores");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.DropTable(
                name: "Sellers");

            migrationBuilder.DropTable(
                name: "Branches");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "StoreClassifications");

            migrationBuilder.DropTable(
                name: "AccountCategories");

            migrationBuilder.DropTable(
                name: "AccountTypes");

            migrationBuilder.DropTable(
                name: "CostCenters");

            migrationBuilder.DropTable(
                name: "SellerCommissionMethod");

            migrationBuilder.DropTable(
                name: "SellerTypes");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "AccountLedgers");

            migrationBuilder.DropTable(
                name: "ArchiveHeaders");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "SellerCommissionTypes");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Currencies");
        }
    }
}
