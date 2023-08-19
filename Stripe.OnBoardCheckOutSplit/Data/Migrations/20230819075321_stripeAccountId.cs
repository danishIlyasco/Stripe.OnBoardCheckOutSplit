using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stripe.OnBoardCheckOutSplit.Data.Migrations
{
    /// <inheritdoc />
    public partial class stripeAccountId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StripeAccountStatus",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "StripeConnectedId",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StripeAccountStatus",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StripeConnectedId",
                table: "AspNetUsers");
        }
    }
}
