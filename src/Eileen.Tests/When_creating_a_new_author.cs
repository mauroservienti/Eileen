using System;
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
    public class When_creating_a_new_author : AbstractDatabaseTest
    {
        public When_creating_a_new_author(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            CurrentDbContext.Database.Migrate();
        }

        [Fact]
        public async Task Values_should_be_set_as_expected()
        {
            var newAuthorModel = new NewAuthorViewModel
            {
                Name = "Author name"
            };

            var controller = new AuthorsController(CurrentDbContext);
            var redirectToActionResult = await controller.New(newAuthorModel) as RedirectToActionResult;
            var newAuthorId = Convert.ToInt32(redirectToActionResult?.RouteValues["id"]);

            var viewAuthorResult = await controller.View(newAuthorId) as ViewResult;
            var author = viewAuthorResult?.Model as Author;

            Assert.NotNull(author);
            Assert.Equal(newAuthorModel.Name, author.Name);
        }
        
        [Fact]
        public async Task CreatedOn_and_LastModifiedOn_are_set_correctly()
        {
            var expectedNow = DateTimeOffset.Now.AddDays(-1);
            CurrentDbContext.NowGetter = () => expectedNow;
            
            var newAuthor = new Author
            {
                Name = "Author name"
            };

            await CurrentDbContext.Authors.AddAsync(newAuthor);
            await CurrentDbContext.SaveChangesAsync();
            CurrentDbContext.ChangeTracker.Clear();
            
            var author = await CurrentDbContext.Authors.FindAsync(newAuthor.Id);

            Assert.Equal(expectedNow, author.CreatedOn);
            Assert.Equal(expectedNow, author.LastModifiedOn);
        }

        [Fact]
        public Task Using_null_arguments_should_throw()
        {
            return Assert.ThrowsAsync<ArgumentNullException>(async () =>
            {
                var controller = new AuthorsController(CurrentDbContext);
                await controller.New(null);
            });
        }

        [Fact]
        public async Task With_null_name_should_report_validation_error()
        {
            var newAuthorModel = new NewAuthorViewModel
            {
                Name = null
            };

            var controller = new AuthorsController(CurrentDbContext);
            controller.ModelState.AddModelError("Name", "Name is required");

            var actionResult = await controller.New(newAuthorModel);

            var viewResult = Assert.IsType<BadRequestObjectResult>(actionResult);
            var responseModel = Assert.IsType<NewAuthorViewModel>(viewResult.Value);
            Assert.Equal(newAuthorModel, responseModel);
        }
    }
}