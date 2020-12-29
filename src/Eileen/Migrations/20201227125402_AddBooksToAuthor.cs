using Microsoft.EntityFrameworkCore.Migrations;

namespace Eileen.Migrations
{
    public partial class AddBooksToAuthor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Author",
                table: "Books",
                newName: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            var sql = @"CREATE OR ALTER VIEW [dbo].[vwAuthorsWithBooksCount] AS
                        SELECT dbo.Authors.Id, dbo.Authors.Name, COUNT(dbo.Books.Id) AS BooksCount
                        FROM dbo.Authors LEFT OUTER JOIN
                         dbo.Books ON dbo.Authors.Id = dbo.Books.AuthorId
						 Group By dbo.Authors.Id, dbo.Authors.Name";

            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_AuthorId",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "AuthorId",
                table: "Books",
                newName: "Author");

            var sql = @"CREATE OR ALTER VIEW [dbo].[vwAuthorsWithBooksCount] AS
                        SELECT dbo.Authors.Id, dbo.Authors.Name, COUNT(dbo.Books.Id) AS BooksCount
                        FROM dbo.Authors LEFT OUTER JOIN
                         dbo.Books ON dbo.Authors.Id = dbo.Books.Author
						 Group By dbo.Authors.Id, dbo.Authors.Name";

            migrationBuilder.Sql(sql);
        }
    }
}
