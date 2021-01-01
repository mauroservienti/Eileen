using Microsoft.EntityFrameworkCore.Migrations;

namespace Eileen.Migrations
{
    public partial class ChangeBooksReferentialActionToSetNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books");

            migrationBuilder.AlterColumn<int>(
                name: "PublisherId",
                table: "Books",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Books",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books",
                column: "PublisherId",
                principalTable: "Publishers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //look for books with a null AuthorId
            //if any, create an unknown-author
            //(or look for an existing one)
            //assign the unknown-author id to books
            //with a null author
            //do the same for publishers
            var booksAuthorsFix = @"print 'going to fix books with null AuthorId'
declare @unknownAuthorId int
select @unknownAuthorId = [dbo].[Authors].[Id] from [dbo].[Authors] where [dbo].[Authors].[Name] = 'unknown-author'
if @unknownAuthorId is null
begin
   print 'unknown-author not fund, creating it'
   insert into [dbo].[Authors] ([Name]) values ('unknown-author')
   select @unknownAuthorId = SCOPE_IDENTITY()
end
print concat('unknown-author ID: ', @unknownAuthorId)
update [dbo].[Books] set [dbo].[Books].[AuthorId] = @unknownAuthorId where [dbo].[Books].[AuthorId] is null";

            migrationBuilder.Sql(booksAuthorsFix);
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books");
            
            var booksPublishersFix = @"print 'going to fix books with null PublisherId'
declare @unknownPublisherId int
select @unknownPublisherId = [dbo].[Publishers].[Id] from [dbo].[Publishers] where [dbo].[Publishers].[Name] = 'unknown-publisher'
if @unknownPublisherId is null
begin
   print 'unknown-publisher not fund, creating it'
   insert into [dbo].[Publishers] ([Name]) values ('unknown-publisher')
   select @unknownPublisherId = SCOPE_IDENTITY()
end
print concat('unknown-publisher ID: ', @unknownPublisherId)
update [dbo].[Books] set [dbo].[Books].[PublisherId] = @unknownPublisherId where [dbo].[Books].[PublisherId] is null";
            
            migrationBuilder.Sql(booksPublishersFix);
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books");

            migrationBuilder.AlterColumn<int>(
                name: "PublisherId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Publishers_PublisherId",
                table: "Books",
                column: "PublisherId",
                principalTable: "Publishers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
