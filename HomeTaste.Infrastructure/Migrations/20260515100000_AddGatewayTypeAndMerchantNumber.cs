using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTaste.Infrastructure.Migrations
{
    public partial class AddGatewayTypeAndMerchantNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GatewayType",
                table: "PaymentGateways",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "card");

            migrationBuilder.AddColumn<string>(
                name: "MerchantNumber",
                table: "PaymentGateways",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GatewayType",
                table: "PaymentGateways");

            migrationBuilder.DropColumn(
                name: "MerchantNumber",
                table: "PaymentGateways");
        }
    }
}
