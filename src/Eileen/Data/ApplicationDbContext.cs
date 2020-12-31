using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eileen.Data.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Eileen.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<AuthorsWithBooksCount>(eb =>
                {
                    eb.ToView("vwAuthorsWithBooksCount");
                });
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<Publisher> Publishers { get; set; }
        public DbSet<AuthorsWithBooksCount> AuthorsWithBooksCountView { get; set; }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            SetCreatedOnAndLastModifiedOnProperties(ChangeTracker.Entries(), NowGetter());

            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default)
        {
            SetCreatedOnAndLastModifiedOnProperties(ChangeTracker.Entries(), NowGetter());

            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        internal Func<DateTimeOffset> NowGetter {get; set;} = () => DateTimeOffset.Now;

        static void SetCreatedOnAndLastModifiedOnProperties(IEnumerable<EntityEntry> entityEntries, DateTimeOffset now)
        {
            var entities = entityEntries.Where(entry => entry.Entity is IEntity)
                .Select(entry => ((IEntity)entry.Entity, entry.State));
            foreach ((IEntity iEntity, EntityState state) in entities)
            {
                if (state == EntityState.Added)
                {
                    iEntity.CreatedOn = now;
                }

                if (state == EntityState.Added || state == EntityState.Modified)
                {
                    iEntity.LastModifiedOn = now;
                }
            }
        }
    }
}