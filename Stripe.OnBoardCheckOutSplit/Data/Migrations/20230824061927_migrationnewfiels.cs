using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stripe.OnBoardCheckOutSplit.Data.Migrations
{
    /// <inheritdoc />
    public partial class migrationnewfiels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SessionStatus",
                table: "Contracts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SessionStatus",
                table: "Contracts");
        }
    }
}
