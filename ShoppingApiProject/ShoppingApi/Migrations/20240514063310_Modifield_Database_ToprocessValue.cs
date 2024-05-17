using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingApi.Migrations
{
    /// <inheritdoc />
    public partial class Modifield_Database_ToprocessValue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ShipmentDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "CartItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Paymemt_Status",
                table: "CartDetails",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "Items_Payed_For",
                columns: table => new
                {
                    Payment_Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Items_Payed = table.Column<int>(type: "int", nullable: false),
                    User_Id = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<bool>(type: "bit", nullable: false),
                    PaymentDetailsPayment_ID = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Items_Payed_For", x => x.Payment_Id);
                    table.ForeignKey(
                        name: "FK_Items_Payed_For_PaymentDetails_PaymentDetailsPayment_ID",
                        column: x => x.PaymentDetailsPayment_ID,
                        principalTable: "PaymentDetails",
                        principalColumn: "Payment_ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Items_Payed_For_PaymentDetailsPayment_ID",
                table: "Items_Payed_For",
                column: "PaymentDetailsPayment_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Items_Payed_For");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ShipmentDetails");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "CartItems");

            migrationBuilder.DropColumn(
                name: "Paymemt_Status",
                table: "CartDetails");
        }
    }
}
