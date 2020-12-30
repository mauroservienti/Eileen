using System;
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
    public class When_deleting_books_from_existing_author : AbstractDatabaseTest
    {
        public When_deleting_books_from_existing_author(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            CurrentDbContext.Database.Migrate();
        }

        [Fact]
        public async Task Should_successfully_delete_the_book()
        {
            var entityEntry = await CurrentDbContext.Authors.AddAsync(new Author
            {
                Name = "author name",
                Books = new ()
                {
                    new ()
                    {
                        Title = "book title"
                    }
                }
            });
            await CurrentDbContext.SaveChangesAsync();
            CurrentDbContext.ChangeTracker.Clear();

            var controller = new AuthorsController(CurrentDbContext);
            await controller.DeleteAuthorBook(entityEntry.Entity.Id, entityEntry.Entity.Books.Single().Id);

            var book = await CurrentDbContext.Books.FindAsync(entityEntry.Entity.Books.Single().Id);

            Assert.Null(book);
        }
        
        [Fact]
        public async Task Should_redirect_to_author_view()
        {
            var entityEntry = await CurrentDbContext.Authors.AddAsync(new Author
            {
                Name = "author name",
                Books = new ()
                {
                    new ()
                    {
                        Title = "book title"
                    }
                }
            });
            await CurrentDbContext.SaveChangesAsync();
            CurrentDbContext.ChangeTracker.Clear();
            
            var controller = new AuthorsController(CurrentDbContext);
            var actionResult = await controller.DeleteAuthorBook(entityEntry.Entity.Id, entityEntry.Entity.Books.Single().Id);

            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(actionResult);
            Assert.Equal("View", redirectToActionResult.ActionName);
            Assert.Equal(entityEntry.Entity.Id, redirectToActionResult.RouteValues["id"]);
        }
    }
}