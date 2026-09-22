using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Xenon.Infrastructure.Migrations.StoreDb
{
    /// <inheritdoc />
    public partial class AddRecentlyViewed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecentlyViewed",
                columns: table => new
                {
                    RecentlyViewedId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    SessionId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecentlyViewed", x => x.RecentlyViewedId);
                    table.ForeignKey(
                        name: "FK_RecentlyViewed_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecentlyViewed_ProductId",
                table: "RecentlyViewed",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_RecentlyViewed_SessionId_ProductId",
                table: "RecentlyViewed",
                columns: new[] { "SessionId", "ProductId" },
                unique: true,
                filter: "[SessionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RecentlyViewed_SessionId_ViewedAt",
                table: "RecentlyViewed",
                columns: new[] { "SessionId", "ViewedAt" },
                filter: "[SessionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RecentlyViewed_UserId_ProductId",
                table: "RecentlyViewed",
                columns: new[] { "UserId", "ProductId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_RecentlyViewed_UserId_ViewedAt",
                table: "RecentlyViewed",
                columns: new[] { "UserId", "ViewedAt" },
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecentlyViewed");
        }
    }
}