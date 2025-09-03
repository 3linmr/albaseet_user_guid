using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Shared.Repository.Migrations
{
    /// <inheritdoc />
    public partial class AddPrintInvoiceSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 1,
                column: "Order",
                value: (short)5);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 2,
                column: "Order",
                value: (short)6);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 3,
                column: "Order",
                value: (short)7);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 4,
                column: "Order",
                value: (short)8);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 5,
                column: "Order",
                value: (short)4);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 6,
                column: "Order",
                value: (short)9);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 7,
                column: "Order",
                value: (short)10);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 8,
                column: "Order",
                value: (short)11);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 9,
                column: "Order",
                value: (short)12);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 10,
                column: "Order",
                value: (short)13);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 12,
                columns: new[] { "ApplicationFlagTabNameAr", "ApplicationFlagTabNameEn" },
                values: new object[] { "طباعة التقارير", "Print Reports" });

            migrationBuilder.InsertData(
                table: "ApplicationFlagTabs",
                columns: new[] { "ApplicationFlagTabId", "ApplicationFlagTabNameAr", "ApplicationFlagTabNameEn", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "Order", "UserNameCreated", "UserNameModified" },
                values: new object[] { 13, "طباعة الفواتير", "Print Invoices", null, null, null, null, null, (short)3, null, null });

            migrationBuilder.InsertData(
                table: "ApplicationFlagHeaders",
                columns: new[] { "ApplicationFlagHeaderId", "ApplicationFlagHeaderNameAr", "ApplicationFlagHeaderNameEn", "ApplicationFlagTabId", "CreatedAt", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "Order", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { 20, "إعدادات أساسية", "Basic Settings", 13, null, null, null, null, null, (short)1, null, null },
                    { 21, "إضافات أخري", "Other Additions", 13, null, null, null, null, null, (short)2, null, null },
                    { 22, "تقريب الكسر العشري", "Rounding a decimal", 13, null, null, null, null, null, (short)3, null, null },
                    { 23, "صور مرفقة", "Images", 13, null, null, null, null, null, (short)4, null, null },
                    { 24, "إضافة ملاحظات", "Notes Addition", 13, null, null, null, null, null, (short)5, null, null }
                });

            migrationBuilder.InsertData(
                table: "ApplicationFlagDetails",
                columns: new[] { "ApplicationFlagDetailId", "ApplicationFlagHeaderId", "ApplicationFlagTypeId", "CreatedAt", "FlagNameAr", "FlagNameEn", "FlagValue", "Hide", "IpAddressCreated", "IpAddressModified", "ModifiedAt", "Order", "UserNameCreated", "UserNameModified" },
                values: new object[,]
                {
                    { 70, 20, (byte)1, null, "اسم المؤسسة بالعربية", "Institution Name Ar", "", null, null, null, null, (short)1, null, null },
                    { 71, 20, (byte)1, null, "اسم المؤسسة بالانجليزية", "Institution Name En", "", null, null, null, null, (short)2, null, null },
                    { 72, 20, (byte)1, null, "عنوان المؤسسة بالعربية", "Institution address Ar", "", null, null, null, null, (short)3, null, null },
                    { 73, 20, (byte)1, null, "عنوان المؤسسة بالانجليزية", "Institution address En", "", null, null, null, null, (short)4, null, null },
                    { 74, 20, (byte)1, null, "بيانات التواصل للمؤسسة بالعربية", "Institution contact info Ar", "", null, null, null, null, (short)5, null, null },
                    { 75, 20, (byte)1, null, "بيانات التواصل للمؤسسة بالانجليزية", "Institution contact info En", "", null, null, null, null, (short)6, null, null },
                    { 76, 20, (byte)1, null, "اسم المتجر بالعربية", "Store Name Ar", "", null, null, null, null, (short)7, null, null },
                    { 77, 20, (byte)1, null, "اسم المتجر بالانجليزية", "Store Name En", "", null, null, null, null, (short)8, null, null },
                    { 78, 20, (byte)1, null, "عنوان المتجر بالعربية", "Store Address Ar", "", null, null, null, null, (short)9, null, null },
                    { 79, 20, (byte)1, null, "عنوان المتجر بالانجليزية", "Store Address En", "", null, null, null, null, (short)10, null, null },
                    { 80, 20, (byte)1, null, "الرقم الضريبي", "VAT no", "", null, null, null, null, (short)11, null, null },
                    { 81, 21, (byte)6, null, "عرض طريقة التحصيل؟", "Show collection method?", "1", null, null, null, null, (short)1, null, null },
                    { 82, 21, (byte)6, null, "عرض تاريخ الاستحقاق؟", "Show due date?", "1", null, null, null, null, (short)2, null, null },
                    { 83, 21, (byte)6, null, "عرض وقت الطباعة؟", "Show print time?", "1", null, null, null, null, (short)2, null, null },
                    { 84, 22, (byte)2, null, "الأرقام في الجدول", "Numbers in grid", "2", null, null, null, null, (short)1, null, null },
                    { 85, 22, (byte)2, null, "الأرقام في المجاميع", "Numbers in summation", "2", null, null, null, null, (short)2, null, null },
                    { 86, 23, (byte)8, null, "شعار المؤسسة", "Institution Logo", "", null, null, null, null, (short)1, null, null },
                    { 87, 24, (byte)1, null, "ملاحظة أساسية 1", "Basic Note 1", "", null, null, null, null, (short)1, null, null },
                    { 88, 24, (byte)1, null, "ملاحظة أساسية 2", "Basic Note 2", "", null, null, null, null, (short)2, null, null },
                    { 89, 24, (byte)1, null, "ملاحظة أساسية 3", "Basic Note 3", "", null, null, null, null, (short)3, null, null },
                    { 90, 24, (byte)1, null, "ملاحظة جانبية 1", "Side Note 1", "", null, null, null, null, (short)4, null, null },
                    { 91, 24, (byte)1, null, "ملاحظة جانبية 2", "Side Note 2", "", null, null, null, null, (short)5, null, null },
                    { 92, 24, (byte)1, null, "ملاحظة جانبية 3", "Side Note 3", "", null, null, null, null, (short)6, null, null },
                    { 93, 24, (byte)1, null, "ملاحظة جانبية 4", "Side Note 4", "", null, null, null, null, (short)7, null, null },
                    { 94, 24, (byte)1, null, "ملاحظة جانبية 5", "Side Note 5", "", null, null, null, null, (short)8, null, null },
                    { 95, 24, (byte)1, null, "ملاحظة جانبية 6", "Side Note 6", "", null, null, null, null, (short)9, null, null },
                    { 96, 24, (byte)1, null, "ملاحظة جانبية 7", "Side Note 7", "", null, null, null, null, (short)10, null, null },
                    { 97, 24, (byte)1, null, "ملاحظة جانبية 8", "Side Note 8", "", null, null, null, null, (short)11, null, null },
                    { 98, 24, (byte)1, null, "ماحظة جانبية 9", "Side Note 9", "", null, null, null, null, (short)12, null, null },
                    { 99, 24, (byte)1, null, "ملاحظة جانبية 10", "Side Note 10", "", null, null, null, null, (short)13, null, null },
                    { 100, 24, (byte)1, null, "سطر ختامي 1", "Closing line 1", "", null, null, null, null, (short)14, null, null },
                    { 101, 24, (byte)1, null, "سطر ختامي 2", "Closing line 2", "", null, null, null, null, (short)15, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagDetails",
                keyColumn: "ApplicationFlagDetailId",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagHeaders",
                keyColumn: "ApplicationFlagHeaderId",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagHeaders",
                keyColumn: "ApplicationFlagHeaderId",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagHeaders",
                keyColumn: "ApplicationFlagHeaderId",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagHeaders",
                keyColumn: "ApplicationFlagHeaderId",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagHeaders",
                keyColumn: "ApplicationFlagHeaderId",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 13);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 1,
                column: "Order",
                value: (short)4);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 2,
                column: "Order",
                value: (short)5);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 3,
                column: "Order",
                value: (short)6);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 4,
                column: "Order",
                value: (short)7);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 5,
                column: "Order",
                value: (short)3);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 6,
                column: "Order",
                value: (short)8);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 7,
                column: "Order",
                value: (short)9);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 8,
                column: "Order",
                value: (short)10);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 9,
                column: "Order",
                value: (short)11);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 10,
                column: "Order",
                value: (short)12);

            migrationBuilder.UpdateData(
                table: "ApplicationFlagTabs",
                keyColumn: "ApplicationFlagTabId",
                keyValue: 12,
                columns: new[] { "ApplicationFlagTabNameAr", "ApplicationFlagTabNameEn" },
                values: new object[] { "إعدادات الطباعة", "Print Settings" });
        }
    }
}
