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
    public class When_adding_new_book_to_existing_author : AbstractDatabaseTest
    {
        public When_adding_new_book_to_existing_author(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            CurrentDbContext.Database.Migrate();
        }

        [Fact]
        public async Task Should_successfully_add_the_book()
        {
            var expectedBookTitle = "Expected book title";
            var entityEntry = await CurrentDbContext.Authors.AddAsync(new Author {Name = "author name"});
            await CurrentDbContext.SaveChangesAsync();

            var controller = new AuthorsController(CurrentDbContext);
            await controller.NewAuthorBook(entityEntry.Entity.Id, new NewAuthorBookRequest {BookTitle = expectedBookTitle});

            var book = await CurrentDbContext.Books.SingleOrDefaultAsync(b=>b.AuthorId==entityEntry.Entity.Id);

            Assert.NotNull(book);
            Assert.Equal(expectedBookTitle, book.Title);
        }

        [Fact]
        public async Task With_null_title_should_report_validation_error()
        {
            var newAuthorBookModel = new NewAuthorBookRequest
            {
                BookTitle = null
            };

            var controller = new AuthorsController(CurrentDbContext);
            controller.ModelState.AddModelError(nameof(NewAuthorBookRequest.BookTitle), "BookTitle is required");

            var actionResult = await controller.NewAuthorBook(0, newAuthorBookModel);

            var viewResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            var responseModel = Assert.IsType<NewAuthorBookRequest>(viewResult.Value);
            Assert.Equal(newAuthorBookModel, responseModel);
        }

        [Fact]
        public Task Using_null_arguments_should_throw()
        {
            return Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                var controller = new AuthorsController(CurrentDbContext);
                await controller.NewAuthorBook(0, null);
            });
        }

        [Fact]
        public Task Using_non_existing_author_should_throw()
        {
            return Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                var controller = new AuthorsController(CurrentDbContext);
                await controller.NewAuthorBook(0, new NewAuthorBookRequest {BookTitle = "something"});
            });
        }
    }
}