using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoFaturas.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Insert seed data for InvoiceStatus
            migrationBuilder.InsertData(
                table: "invoice_statuses",
                columns: new[] { "id", "name", "description", "color", "sort_order", "is_active", "is_final", "created_at", "updated_at" },
                values: new object[,]
                {
                    { 1, "Pending", "Invoice received and pending review", "#FFA500", 1, true, false, DateTime.UtcNow, DateTime.UtcNow },
                    { 2, "Under Review", "Invoice is being reviewed", "#2196F3", 2, true, false, DateTime.UtcNow, DateTime.UtcNow },
                    { 3, "Approved", "Invoice has been approved for payment", "#4CAF50", 3, true, false, DateTime.UtcNow, DateTime.UtcNow },
                    { 4, "Rejected", "Invoice has been rejected", "#F44336", 4, true, true, DateTime.UtcNow, DateTime.UtcNow },
                    { 5, "On Hold", "Invoice processing is on hold", "#FF9800", 5, true, false, DateTime.UtcNow, DateTime.UtcNow },
                    { 6, "Scheduled for Payment", "Invoice is scheduled for payment", "#9C27B0", 6, true, false, DateTime.UtcNow, DateTime.UtcNow },
                    { 7, "Paid", "Invoice has been paid", "#8BC34A", 7, true, true, DateTime.UtcNow, DateTime.UtcNow },
                    { 8, "Overdue", "Invoice payment is overdue", "#E91E63", 8, true, false, DateTime.UtcNow, DateTime.UtcNow },
                    { 9, "Cancelled", "Invoice has been cancelled", "#607D8B", 9, true, true, DateTime.UtcNow, DateTime.UtcNow }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Remove seed data for InvoiceStatus
            migrationBuilder.DeleteData(
                table: "invoice_statuses",
                keyColumn: "id",
                keyValues: new object[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 });
        }
    }
}
