using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookManagement.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBorrowIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "IX_BookBorrowedDetails_BookId_ReturnedAt",
                table: "BookBorrowedDetails",
                newName: "IX_BookBorrowedDetails_ActiveBorrow_BookId");

            migrationBuilder.CreateIndex(
                name: "IX_BookBorrowedDetails_BookId_BorrowedAt",
                table: "BookBorrowedDetails",
                columns: new[] { "BookId", "BorrowedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookBorrowedDetails_BookId_BorrowedAt",
                table: "BookBorrowedDetails");

            migrationBuilder.RenameIndex(
                name: "IX_BookBorrowedDetails_ActiveBorrow_BookId",
                table: "BookBorrowedDetails",
                newName: "IX_BookBorrowedDetails_BookId_ReturnedAt");
        }
    }
}
