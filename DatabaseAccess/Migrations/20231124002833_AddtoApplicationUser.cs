using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DatabaseAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddtoApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderDetails_orderHeaders_OrderHeaderId",
                table: "orderDetails");

            migrationBuilder.AlterColumn<int>(
                name: "OrderHeaderId",
                table: "orderDetails",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactNumber",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LatestShopDate",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ShoppingActivityCounter",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_orderDetails_orderHeaders_OrderHeaderId",
                table: "orderDetails",
                column: "OrderHeaderId",
                principalTable: "orderHeaders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_orderDetails_orderHeaders_OrderHeaderId",
                table: "orderDetails");

            migrationBuilder.DropColumn(
                name: "ContactNumber",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LatestShopDate",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ShoppingActivityCounter",
                table: "AspNetUsers");

            migrationBuilder.AlterColumn<int>(
                name: "OrderHeaderId",
                table: "orderDetails",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_orderDetails_orderHeaders_OrderHeaderId",
                table: "orderDetails",
                column: "OrderHeaderId",
                principalTable: "orderHeaders",
                principalColumn: "Id");
        }
    }
}
