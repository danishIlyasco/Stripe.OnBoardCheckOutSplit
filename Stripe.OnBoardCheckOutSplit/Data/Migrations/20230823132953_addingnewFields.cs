using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Stripe.OnBoardCheckOutSplit.Data.Migrations
{
    /// <inheritdoc />
    public partial class addingnewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientUserId",
                table: "Contracts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SessionId",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransactionId",
                table: "Contracts",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ClientUserId",
                table: "Contracts",
                column: "ClientUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contracts_AspNetUsers_ClientUserId",
                table: "Contracts",
                column: "ClientUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contracts_AspNetUsers_ClientUserId",
                table: "Contracts");

            migrationBuilder.DropIndex(
                name: "IX_Contracts_ClientUserId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "ClientUserId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "Contracts");

            migrationBuilder.DropColumn(
                name: "TransactionId",
                table: "Contracts");
        }
    }
}
