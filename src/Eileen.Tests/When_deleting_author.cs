using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eileen.Controllers;
using Eileen.Data;
using Eileen.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Eileen.Tests
{
    public class When_deleting_author : AbstractDatabaseTest
    {
        public When_deleting_author(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            CurrentDbContext.Database.Migrate();
        }

        [Fact]
        public async Task Should_successfully_delete_author_with_no_books()
        {
            var entityEntry = await CurrentDbContext.Authors.AddAsync(new Author
            {
                Name = "author name"
            });
            await CurrentDbContext.SaveChangesAsync();

            var controller = new AuthorsController(CurrentDbContext);
            await controller.Delete(entityEntry.Entity.Id, new DeleteAuthorRequest() {DeleteBooks = false});

            var author = await CurrentDbContext.Authors.FindAsync(entityEntry.Entity.Id);

            Assert.Null(author);
        }

        [Fact]
        public async Task Should_successfully_delete_author_and_redirect_to_list()
        {
            var entityEntry = await CurrentDbContext.Authors.AddAsync(new Author
            {
                Name = "author name"
            });
            await CurrentDbContext.SaveChangesAsync();

            var controller = new AuthorsController(CurrentDbContext);
            var actionResult = await controller.Delete(entityEntry.Entity.Id, new DeleteAuthorRequest() {DeleteBooks = false});

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.Equal("List", redirectToActionResult.ActionName, ignoreCase: true);
        }

        [Fact]
        public async Task Should_successfully_delete_author_and_keep_books()
        {
            var entityEntry = await CurrentDbContext.Authors.AddAsync(new Author {Name = "author name", Books = new List<Book>
            {
                new()
                {
                    Title = "book title"
                }
            }});

            await CurrentDbContext.SaveChangesAsync();

            var controller = new AuthorsController(CurrentDbContext);
            await controller.Delete(entityEntry.Entity.Id, new DeleteAuthorRequest() {DeleteBooks = false});

            var author = await CurrentDbContext.Authors.FindAsync(entityEntry.Entity.Id);
            var book = await CurrentDbContext.Books.FindAsync(entityEntry.Entity.Books.Single().Id);

            Assert.Null(author);
            Assert.NotNull(book);
            Assert.Null(book.AuthorId);
        }

        [Fact]
        public async Task Should_successfully_delete_author_and_books()
        {
            var entityEntry = await CurrentDbContext.Authors.AddAsync(new Author {Name = "author name", Books = new List<Book>
            {
                new()
                {
                    Title = "book title"
                }
            }});

            await CurrentDbContext.SaveChangesAsync();

            var controller = new AuthorsController(CurrentDbContext);
            await controller.Delete(entityEntry.Entity.Id, new DeleteAuthorRequest() {DeleteBooks = true});

            var author = await CurrentDbContext.Authors.FindAsync(entityEntry.Entity.Id);
            var book = await CurrentDbContext.Books.FindAsync(entityEntry.Entity.Books.Single().Id);

            Assert.Null(author);
            Assert.Null(book);
        }
    }
}