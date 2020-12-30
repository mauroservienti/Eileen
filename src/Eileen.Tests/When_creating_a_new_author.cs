using System;
using System.Threading.Tasks;
using Eileen.Controllers;
using Eileen.Data;
using Eileen.Data.Views;
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
    }
}