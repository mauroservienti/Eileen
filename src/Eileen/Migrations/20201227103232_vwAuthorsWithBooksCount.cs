using Microsoft.EntityFrameworkCore.Migrations;

namespace Eileen.Migrations
{
    public partial class vwAuthorsWithBooksCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var sql = @"CREATE OR ALTER VIEW [dbo].[vwAuthorsWithBooksCount] AS
                        SELECT dbo.Authors.Id, dbo.Authors.Name, COUNT(dbo.Books.Id) AS BooksCount
                        FROM dbo.Authors LEFT OUTER JOIN
                         dbo.Books ON dbo.Authors.Id = dbo.Books.Author
						 Group By dbo.Authors.Id, dbo.Authors.Name";

            migrationBuilder.Sql(sql);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW vwAuthorsWithBooksCount");
        }
    }
}
