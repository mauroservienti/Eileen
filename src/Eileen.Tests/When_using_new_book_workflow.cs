using System;
using System.Net;
using System.Text.Encodings.Web;
using Eileen.Controllers;
using Eileen.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Xunit;
using Xunit.Abstractions;

namespace Eileen.Tests
{
    public class When_using_new_book_workflow : AbstractDatabaseTest
    {
        public When_using_new_book_workflow(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            
        }

        [Fact]
        public void New_create_expected_book_view_model()
        {
            var httpContext = new DefaultHttpContext();

            var controller = new BooksController(CurrentDbContext)
            {
                ControllerContext = new ControllerContext() {HttpContext = httpContext}
            };
            var viewResult = controller.New() as ViewResult;
            var viewModel = viewResult?.Model as NewBookViewModel;

            Assert.NotNull(viewModel);
            Assert.Null(viewModel.Title);
            Assert.False(viewModel.IsAuthorSelected);
            Assert.Null(viewModel.SelectedAuthorName);
            Assert.Null(viewModel.SelectedAuthorId);
        }

        [Fact]
        public void Selected_author_is_presented_as_expected()
        {
            var domain = "localhost";
            var expectedSelectedAuthorId = 3;
            var expectedSelectedAuthorName = "this is the author name";

            var cookies = new CookieContainer();
            cookies.Add(new Cookie("selected-author-id", expectedSelectedAuthorId.ToString())
            {
                Domain = domain
            });
            cookies.Add(new Cookie("selected-author-name", UrlEncoder.Default.Encode(expectedSelectedAuthorName))
            {
                Domain = domain
            });
            
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers.Add(HeaderNames.Cookie, cookies.GetCookieHeader(new Uri("http://" + domain)));
            
            var controller = new BooksController(CurrentDbContext)
            {
                ControllerContext = new ControllerContext() {HttpContext = httpContext}
            };
            var viewResult = controller.New() as ViewResult;
            var viewModel = viewResult?.Model as NewBookViewModel;

            Assert.NotNull(viewModel);
            Assert.Null(viewModel.Title);
            Assert.True(viewModel.IsAuthorSelected);
            Assert.Equal(expectedSelectedAuthorName, viewModel.SelectedAuthorName);
            Assert.Equal(expectedSelectedAuthorId, viewModel.SelectedAuthorId);
        }
    }
}