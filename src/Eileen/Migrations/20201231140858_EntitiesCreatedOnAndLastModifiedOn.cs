using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Eileen.Migrations
{
    public partial class EntitiesCreatedOnAndLastModifiedOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "Publishers",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedOn",
                table: "Publishers",
                type: "datetimeoffset",
                nullable: true);
            
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "Books",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedOn",
                table: "Books",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "Authors",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "LastModifiedOn",
                table: "Authors",
                type: "datetimeoffset",
                nullable: true);
            
            migrationBuilder.Sql(@"UPDATE [dbo].[Publishers] SET CreatedOn = SYSUTCDATETIME(), LastModifiedOn = SYSUTCDATETIME()");
            migrationBuilder.Sql(@"UPDATE [dbo].[Books] SET CreatedOn = SYSUTCDATETIME(), LastModifiedOn = SYSUTCDATETIME()");
            migrationBuilder.Sql(@"UPDATE [dbo].[Authors] SET CreatedOn = SYSUTCDATETIME(), LastModifiedOn = SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "Publishers",
                nullable: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastModifiedOn",
                table: "Publishers",
                nullable: false,
                oldNullable: true);
            
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "Books",
                nullable: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastModifiedOn",
                table: "Books",
                nullable: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "CreatedOn",
                table: "Authors",
                nullable: false,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastModifiedOn",
                table: "Authors",
                nullable: false,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Publishers");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Publishers");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Authors");
        }
    }
}
