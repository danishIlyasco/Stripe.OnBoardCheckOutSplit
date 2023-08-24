using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stripe.OnBoardCheckOutSplit.Data.Migrations
{
    /// <inheritdoc />
    public partial class Necessaryfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTransfered",
                table: "ContractUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "StripeTranferId",
                table: "ContractUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LatestCahrgeId",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsTransfered",
                table: "ContractUsers");

            migrationBuilder.DropColumn(
                name: "StripeTranferId",
                table: "ContractUsers");

            migrationBuilder.DropColumn(
                name: "LatestCahrgeId",
                table: "Contracts");
        }
    }
}
