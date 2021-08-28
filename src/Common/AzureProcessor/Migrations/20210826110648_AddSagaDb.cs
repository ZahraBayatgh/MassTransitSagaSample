using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AzureProcessor.Migrations
{
    public partial class AddSagaDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductDto",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitialOnHand = table.Column<int>(type: "int", nullable: false),
                    ProductStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCatalogState",
                columns: table => new
                {
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CurrentState = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCatalogState", x => x.CorrelationId);
                    table.ForeignKey(
                        name: "FK_ProductCatalogState_ProductDto_ProductId",
                        column: x => x.ProductId,
                        principalTable: "ProductDto",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductCatalogState_ProductId",
                table: "ProductCatalogState",
                column: "ProductId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductCatalogState");

            migrationBuilder.DropTable(
                name: "ProductDto");
        }
    }
}
