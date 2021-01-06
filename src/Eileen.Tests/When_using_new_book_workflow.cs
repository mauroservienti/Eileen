using System;
using System.Net;
using System.Text.Encodings.Web;
using Eileen.Controllers;
using Eileen.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public void New_returns_expected_book_view_model()
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
            var expectedSelectedAuthorId = 3;
            var expectedSelectedAuthorName = "this is the author name";

            var cookies = new CookieContainer();
            cookies.AddCookie("selected-author-id", expectedSelectedAuthorId.ToString());
            cookies.AddCookie("selected-author-name", UrlEncoder.Default.Encode(expectedSelectedAuthorName));
            
            var httpContext = new DefaultHttpContext();
            httpContext.Request.SetCookies(cookies);
            
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
        
        [Fact]
        public void ClearSelectedAuthor_removes_cookies()
        {
            var selectedAuthorIdCookieName = "selected-author-id";
            var selectedAuthorNameCookieName = "selected-author-name";
            
            var expectedSelectedAuthorId = 3;
            var expectedSelectedAuthorName = "this is the author name";

            var cookies = new CookieContainer();
            cookies.AddCookie(selectedAuthorIdCookieName, expectedSelectedAuthorId.ToString());
            cookies.AddCookie(selectedAuthorNameCookieName, UrlEncoder.Default.Encode(expectedSelectedAuthorName));
            
            var httpContext = new DefaultHttpContext();
            httpContext.Request.SetCookies(cookies);
            
            var controller = new BooksController(CurrentDbContext)
            {
                ControllerContext = new ControllerContext() {HttpContext = httpContext}
            };
            var viewResult = controller.ClearSelectedAuthor(new NewBookViewModel()) as RedirectToActionResult;

            var rawSelectedAuthorIdCookie = httpContext.Response.GetRawCookie(selectedAuthorIdCookieName);
            var rawSelectedAuthorNameCookie = httpContext.Response.GetRawCookie(selectedAuthorNameCookieName);

            Assert.NotNull(viewResult);
            
            Assert.NotNull(rawSelectedAuthorIdCookie);
            Assert.Equal(string.Empty, rawSelectedAuthorIdCookie[selectedAuthorIdCookieName]);
            Assert.Equal(DateTime.UnixEpoch, DateTimeOffset.Parse(rawSelectedAuthorIdCookie["expires"]));
            
            Assert.NotNull(rawSelectedAuthorNameCookie);
            Assert.Equal(string.Empty, rawSelectedAuthorNameCookie[selectedAuthorNameCookieName]);
            Assert.Equal(DateTime.UnixEpoch, DateTimeOffset.Parse(rawSelectedAuthorNameCookie["expires"]));
        }
    }
}