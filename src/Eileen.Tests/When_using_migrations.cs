using System.Collections.Generic;
using System.Threading.Tasks;
using Eileen.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Eileen.Tests
{
    public class When_using_migrations : AbstractDatabaseTest
    {
        public When_using_migrations(ITestOutputHelper outputHelper)
            : base(outputHelper)
        {
        }

        [Fact]
        public async Task Run_up_and_down_succeeds()
        {
            await CurrentDbContext.Database.MigrateAsync();

            var migrator = CurrentDbContext.GetInfrastructure().GetRequiredService<IMigrator>();
            await migrator.MigrateAsync("0");
        }

        [Fact]
        public async Task Add_CreatedOn_and_LastModifiedOn_correctly_even_with_existing_data()
        {
            var migrator = CurrentDbContext.GetInfrastructure().GetRequiredService<IMigrator>();

            //Migrate up to right before CreatedOn and LastModifiedOn breaking changes 
            await migrator.MigrateAsync("ChangeBooksReferentialActionToSetNull");

            //Add some data using raw SQL since C# classes and DbContext are already using te new schema
            await CurrentDbContext.Database.ExecuteSqlRawAsync(
                "INSERT INTO [dbo].[Authors] ([Name]) VALUES ('Andrea Camilleri')");
            await CurrentDbContext.Database.ExecuteSqlRawAsync(
                "INSERT INTO [dbo].[Books] ([Title]) VALUES ('Gita a Tindari')");
            await CurrentDbContext.Database.ExecuteSqlRawAsync(
                "INSERT INTO [dbo].[Publishers] ([Name]) VALUES ('Sellerio')");

            //Migrate up to CreatedOn and LastModifiedOn schema change
            await migrator.MigrateAsync("EntitiesCreatedOnAndLastModifiedOn");

            //Go back to previous
            await migrator.MigrateAsync("ChangeBooksReferentialActionToSetNull");
            
            DoNotDeleteTestDatabase();
        }

        [Fact]
        public async Task Run_up_and_down_with_data_succeeds()
        {
            await CurrentDbContext.Database.MigrateAsync();

            //setting publisher (or author) to null causes down to fail
            //when turning AuthorId or PublisherId to not null
            await CurrentDbContext.Books.AddAsync(new Book()
            {
                Title = "Gita a Tindari",
                Author = new Author() {Name = "Andrea Camilleri"},
                Publisher = null
            });
            await CurrentDbContext.SaveChangesAsync();

            var migrator = CurrentDbContext.GetInfrastructure().GetRequiredService<IMigrator>();
            await migrator.MigrateAsync("0");
        }
    }
}