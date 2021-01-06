using System;
using System.Net;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Eileen.Controllers;
using Eileen.Data;
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
        public async Task New_creates_expected_book_and_clears_cookies()
        {
            await CurrentDbContext.Database.MigrateAsync();

            var expectedAuthor = new Author() {Name = "the author name"};
            await CurrentDbContext.Authors.AddAsync(expectedAuthor);
            await CurrentDbContext.SaveChangesAsync();
            CurrentDbContext.ChangeTracker.Clear();
            
            var selectedAuthorIdCookieName = "selected-author-id";
            var selectedAuthorNameCookieName = "selected-author-name";
            
            var expectedSelectedAuthorId = expectedAuthor.Id;
            var expectedSelectedAuthorName = expectedAuthor.Name;
            
            var cookies = new CookieContainer();
            cookies.AddCookie(selectedAuthorIdCookieName, expectedSelectedAuthorId.ToString());
            cookies.AddCookie(selectedAuthorNameCookieName, UrlEncoder.Default.Encode(expectedSelectedAuthorName));
            
            var httpContext = new DefaultHttpContext();
            httpContext.Request.SetCookies(cookies);

            var controller = new BooksController(CurrentDbContext)
            {
                ControllerContext = new ControllerContext() {HttpContext = httpContext}
            };

            var newBookViewModel = new NewBookViewModel()
            {
                Title = "Book title",
                IsAuthorSelected = true,
                SelectedAuthorId = expectedSelectedAuthorId
            };
            
            var viewResult = await controller.New(newBookViewModel) as RedirectToActionResult;
            Assert.NotNull(viewResult);
            Assert.Equal("Index", viewResult.ActionName);
            Assert.Equal("Book", viewResult.ControllerName);
            
            var newBookId = viewResult.RouteValues["id"]?.ToString();
            Assert.NotNull(newBookId);
            
            var id = int.Parse(newBookId);

            var newBook = await CurrentDbContext.Books
                .Include(b => b.Author)
                .SingleOrDefaultAsync(b => b.Id == id);
            
            Assert.NotNull(newBook);
            Assert.Equal(newBookViewModel.Title, newBook.Title);
            Assert.Equal(expectedSelectedAuthorId, newBook.AuthorId);
            Assert.Equal(expectedSelectedAuthorName, newBook.Author.Name);
            
            var rawSelectedAuthorIdCookie = httpContext.Response.GetRawCookie(selectedAuthorIdCookieName);
            var rawSelectedAuthorNameCookie = httpContext.Response.GetRawCookie(selectedAuthorNameCookieName);

            Assert.NotNull(rawSelectedAuthorIdCookie);
            Assert.Equal(string.Empty, rawSelectedAuthorIdCookie[selectedAuthorIdCookieName]);
            Assert.Equal(DateTime.UnixEpoch, DateTimeOffset.Parse(rawSelectedAuthorIdCookie["expires"]));
            
            Assert.NotNull(rawSelectedAuthorNameCookie);
            Assert.Equal(string.Empty, rawSelectedAuthorNameCookie[selectedAuthorNameCookieName]);
            Assert.Equal(DateTime.UnixEpoch, DateTimeOffset.Parse(rawSelectedAuthorNameCookie["expires"]));
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