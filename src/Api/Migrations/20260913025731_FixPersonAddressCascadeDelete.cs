using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Migrations
{
    /// <inheritdoc />
    public partial class FixPersonAddressCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Persons_PersonAddresses_AddressId",
                table: "Persons");

            migrationBuilder.DropIndex(
                name: "IX_Persons_AddressId",
                table: "Persons");

            migrationBuilder.DropColumn(
                name: "AddressId",
                table: "Persons");

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "PersonAddresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_PersonAddresses_PersonId",
                table: "PersonAddresses",
                column: "PersonId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PersonAddresses_Persons_PersonId",
                table: "PersonAddresses",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonAddresses_Persons_PersonId",
                table: "PersonAddresses");

            migrationBuilder.DropIndex(
                name: "IX_PersonAddresses_PersonId",
                table: "PersonAddresses");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "PersonAddresses");

            migrationBuilder.AddColumn<int>(
                name: "AddressId",
                table: "Persons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Persons_AddressId",
                table: "Persons",
                column: "AddressId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Persons_PersonAddresses_AddressId",
                table: "Persons",
                column: "AddressId",
                principalTable: "PersonAddresses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
