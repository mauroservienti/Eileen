using Eileen.Controllers;
using Eileen.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Eileen.Tests
{
    public class When_using_new_book_workflow : AbstractDatabaseTest
    {
        public When_using_new_book_workflow(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            CurrentDbContext.Database.Migrate();
        }

        [Fact]
        public void New_create_expected_book_view_model()
        {
            var httpContext = new DefaultHttpContext();
            
            var controller = new BooksController(CurrentDbContext)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext
                }
            };
            var viewResult = controller.New() as ViewResult;
            var viewModel = viewResult?.Model as NewBookViewModel;
            
            Assert.NotNull(viewModel);
            Assert.Null(viewModel.Title);
            Assert.False(viewModel.IsAuthorSelected);
            Assert.Null(viewModel.SelectedAuthorName);
            Assert.Null(viewModel.SelectedAuthorId);
        }
    }
}