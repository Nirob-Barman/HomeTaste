using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeTaste.Infrastructure.Migrations
{
    public partial class UseConfigJsonForGatewayCredentials : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Config",
                table: "PaymentGateways",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "{}");

            // Migrate existing credentials into Config JSON
            migrationBuilder.Sql(@"
                UPDATE [PaymentGateways]
                SET [Config] = (
                    SELECT pg2.[PublishableKey], pg2.[SecretKey], pg2.[MerchantNumber]
                    FROM [PaymentGateways] pg2
                    WHERE pg2.[Id] = [PaymentGateways].[Id]
                    FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
                )
                WHERE [PublishableKey] IS NOT NULL
                   OR [SecretKey]      IS NOT NULL
                   OR [MerchantNumber] IS NOT NULL;
            ");

            migrationBuilder.DropColumn(name: "PublishableKey", table: "PaymentGateways");
            migrationBuilder.DropColumn(name: "SecretKey",      table: "PaymentGateways");
            migrationBuilder.DropColumn(name: "MerchantNumber", table: "PaymentGateways");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PublishableKey",
                table: "PaymentGateways",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecretKey",
                table: "PaymentGateways",
                type: "nvarchar(300)",
                maxLength: 300,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MerchantNumber",
                table: "PaymentGateways",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.DropColumn(name: "Config", table: "PaymentGateways");
        }
    }
}
