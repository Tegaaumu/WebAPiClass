using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingApi.Migrations
{
    /// <inheritdoc />
    public partial class Transaction_data_Updated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "userId_from_cart",
                table: "PaymentDetails",
                newName: "UserId_From_Cart");

            migrationBuilder.AlterColumn<string>(
                name: "UserId_From_Cart",
                table: "PaymentDetails",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "PaymentDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "PaymentDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "PaymentDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "PaymentDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "TrxRef",
                table: "PaymentDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "PaymentDetails");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "PaymentDetails");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "PaymentDetails");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "PaymentDetails");

            migrationBuilder.DropColumn(
                name: "TrxRef",
                table: "PaymentDetails");

            migrationBuilder.RenameColumn(
                name: "UserId_From_Cart",
                table: "PaymentDetails",
                newName: "userId_from_cart");

            migrationBuilder.AlterColumn<int>(
                name: "userId_from_cart",
                table: "PaymentDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
