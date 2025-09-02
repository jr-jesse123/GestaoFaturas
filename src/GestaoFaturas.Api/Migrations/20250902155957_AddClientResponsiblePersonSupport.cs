using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoFaturas.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddClientResponsiblePersonSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_responsible_persons_cost_center_primary",
                table: "responsible_persons");

            migrationBuilder.AlterColumn<string>(
                name: "position",
                table: "responsible_persons",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "full_name",
                table: "responsible_persons",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "cost_center_id",
                table: "responsible_persons",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "client_id",
                table: "responsible_persons",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "responsible_persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "role",
                table: "responsible_persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_responsible_persons_client_id",
                table: "responsible_persons",
                column: "client_id",
                filter: "client_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_responsible_persons_client_primary",
                table: "responsible_persons",
                columns: new[] { "client_id", "is_primary" },
                filter: "client_id IS NOT NULL AND is_primary = true");

            migrationBuilder.CreateIndex(
                name: "ix_responsible_persons_cost_center_primary",
                table: "responsible_persons",
                columns: new[] { "cost_center_id", "is_primary" },
                filter: "cost_center_id IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "fk_responsible_persons_clients_client_id",
                table: "responsible_persons",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_responsible_persons_clients_client_id",
                table: "responsible_persons");

            migrationBuilder.DropIndex(
                name: "ix_responsible_persons_client_id",
                table: "responsible_persons");

            migrationBuilder.DropIndex(
                name: "ix_responsible_persons_client_primary",
                table: "responsible_persons");

            migrationBuilder.DropIndex(
                name: "ix_responsible_persons_cost_center_primary",
                table: "responsible_persons");

            migrationBuilder.DropColumn(
                name: "client_id",
                table: "responsible_persons");

            migrationBuilder.DropColumn(
                name: "name",
                table: "responsible_persons");

            migrationBuilder.DropColumn(
                name: "role",
                table: "responsible_persons");

            migrationBuilder.AlterColumn<string>(
                name: "position",
                table: "responsible_persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "full_name",
                table: "responsible_persons",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "cost_center_id",
                table: "responsible_persons",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_responsible_persons_cost_center_primary",
                table: "responsible_persons",
                columns: new[] { "cost_center_id", "is_primary" });
        }
    }
}
