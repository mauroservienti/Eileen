using System;
using Eileen.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Eileen.Tests
{
    public abstract class AbstractDatabaseTest : IDisposable
    {
        const string DefaultConnectionString = "Data Source=(localdb)\\Eileen;Integrated Security=True";
        protected string DatabaseName { get; }
        protected ITestOutputHelper OutputHelper { get; }
        protected ApplicationDbContext CurrentDbContext { get; }
        protected ServiceProvider ServiceProvider { get; }

        public virtual void Dispose()
        {
            CurrentDbContext?.Database.EnsureDeleted();

            CurrentDbContext?.Dispose();
            ServiceProvider?.Dispose();
        }

        protected AbstractDatabaseTest(ITestOutputHelper outputHelper)
        {
            OutputHelper = outputHelper;
            DatabaseName = $"tests_db_{Guid.NewGuid()}";

            OutputHelper.WriteLine($"Current test database name: {DatabaseName}");

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder => builder.AddXUnit(OutputHelper));
            serviceCollection.AddEntityFrameworkSqlServer();
            serviceCollection.AddDbContext<ApplicationDbContext>((provider, dbContextOptionsBuilder) =>
            {
                var connectionStringToUse = Environment.GetEnvironmentVariable("EILEEN_TESTS_CONNECTION_STRING") ?? DefaultConnectionString;

                var connectionStringBuilder = new SqlConnectionStringBuilder(connectionStringToUse) {InitialCatalog = DatabaseName};

                var connectionString = connectionStringBuilder.ToString();
                OutputHelper.WriteLine($"Current test connection string: {connectionString}");

                dbContextOptionsBuilder.UseSqlServer(connectionString)
                    .UseInternalServiceProvider(provider);
            });

            ServiceProvider = serviceCollection.BuildServiceProvider();
            CurrentDbContext = ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }
    }
}