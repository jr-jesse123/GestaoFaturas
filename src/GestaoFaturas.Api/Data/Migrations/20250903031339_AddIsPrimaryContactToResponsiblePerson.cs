using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GestaoFaturas.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPrimaryContactToResponsiblePerson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_primary_contact",
                table: "responsible_persons",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_primary_contact",
                table: "responsible_persons");
        }
    }
}
