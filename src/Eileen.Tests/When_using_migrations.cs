using System.Threading.Tasks;
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
            : base(outputHelper, runMigrations: false)
        {
            
        }

        [Fact]
        public async Task Run_up_and_down_succeed()
        {
            await CurrentDbContext.Database.MigrateAsync();

            var migrator = CurrentDbContext.GetInfrastructure().GetRequiredService<IMigrator>();
            await migrator.MigrateAsync("0");
        }
    }
}