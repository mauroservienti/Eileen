using Eileen.Data.Views;
using Microsoft.EntityFrameworkCore;

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
    }
}