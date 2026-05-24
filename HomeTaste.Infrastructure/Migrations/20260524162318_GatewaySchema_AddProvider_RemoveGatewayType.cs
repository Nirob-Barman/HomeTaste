using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTaste.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class GatewaySchema_AddProvider_RemoveGatewayType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GatewayType",
                table: "PaymentGateways");

            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "PaymentGateways",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            // Migrate existing slugs to underscore convention and set Provider
            migrationBuilder.Sql("UPDATE PaymentGateways SET Slug = 'stripe_payment_intents', Provider = 'stripe'       WHERE Slug = 'stripe'");
            migrationBuilder.Sql("UPDATE PaymentGateways SET Slug = 'bkash_manual',           Provider = 'bkash'        WHERE Slug = 'bkash'");
            migrationBuilder.Sql("UPDATE PaymentGateways SET Slug = 'bkash_checkout',         Provider = 'bkash'        WHERE Slug = 'bkash-checkout'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Provider",
                table: "PaymentGateways");

            migrationBuilder.AddColumn<string>(
                name: "GatewayType",
                table: "PaymentGateways",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "card");
        }
    }
}
