using System.Threading.Tasks;
using Eileen.Controllers;
using Eileen.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Eileen.Tests
{
    public class When_viewing_book : AbstractDatabaseTest
    {
        public When_viewing_book(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            CurrentDbContext.Database.Migrate();
        }

        [Fact]
        public async Task Details_are_returned_as_expected()
        {
            var expectedBook = "Spillover";
            
            //Arrange
            var bookEntry = await CurrentDbContext.Books.AddAsync(new Book
            {
                Title = expectedBook
            });
            await CurrentDbContext.SaveChangesAsync();
            
            //Act
            var controller = new BookController(CurrentDbContext);
            var viewResult = await controller.Index(bookEntry.Entity.Id) as ViewResult;
            var book = viewResult?.Model as Book;
            
            Assert.NotNull(book);
            Assert.Equal(expectedBook, book.Title);
        }
        
        [Fact]
        public async Task Details_include_author()
        {
            var expectedBook = "IT";
            var expectedAuthor = "Stephen King";
            
            //Arrange
            var bookEntry = await CurrentDbContext.Books.AddAsync(new Book
            {
                Title = expectedBook,
                Author = new ()
                {
                    Name = expectedAuthor
                }
            });
            await CurrentDbContext.SaveChangesAsync();
            
            //Act
            var controller = new BookController(CurrentDbContext);
            var viewResult = await controller.Index(bookEntry.Entity.Id) as ViewResult;
            var book = viewResult?.Model as Book;
            
            Assert.NotNull(book);
            Assert.NotNull(book.Author);
            Assert.Equal(expectedAuthor, book.Author.Name);
        }
        
        [Fact]
        public async Task Details_include_publisher()
        {
            var expectedBook = "IT";
            var expectedPublisher = "Some publisher";
            
            //Arrange
            var bookEntry = await CurrentDbContext.Books.AddAsync(new Book
            {
                Title = expectedBook,
                Publisher = new ()
                {
                    Name = expectedPublisher
                }
            });
            await CurrentDbContext.SaveChangesAsync();
            
            //Act
            var controller = new BookController(CurrentDbContext);
            var viewResult = await controller.Index(bookEntry.Entity.Id) as ViewResult;
            var book = viewResult?.Model as Book;
            
            Assert.NotNull(book);
            Assert.NotNull(book.Publisher);
            Assert.Equal(expectedPublisher, book.Publisher.Name);
        }
    }
}