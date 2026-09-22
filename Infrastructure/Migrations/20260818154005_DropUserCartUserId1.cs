using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Xenon.Infrastructure.Migrations.StoreDb
{
    /// <inheritdoc />
    public partial class DropUserCartUserId1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartLine_UserCarts_UserCartUserId1",
                table: "CartLine");

            migrationBuilder.DropIndex(
                name: "IX_CartLine_UserCartUserId1",
                table: "CartLine");

            migrationBuilder.DropColumn(
                name: "UserCartUserId1",
                table: "CartLine");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserCartUserId1",
                table: "CartLine",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CartLine_UserCartUserId1",
                table: "CartLine",
                column: "UserCartUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CartLine_UserCarts_UserCartUserId1",
                table: "CartLine",
                column: "UserCartUserId1",
                principalTable: "UserCarts",
                principalColumn: "UserId");
        }
    }
}
