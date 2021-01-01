using System.Linq;
using System.Threading.Tasks;
using Eileen.Controllers;
using Eileen.Data;
using Eileen.Data.Views;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace Eileen.Tests
{
    public class When_listing_books : AbstractDatabaseTest
    {
        public When_listing_books(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            CurrentDbContext.Database.Migrate();
        }
        
        [Fact]
        public async Task With_empty_database_and_default_paging_list_is_empty()
        {
            var controller = new BooksController(CurrentDbContext);
            var viewResult = await controller.Index() as ViewResult;
            var pagedResults = viewResult?.Model as PagedResults<Book>;
            
            Assert.NotNull(viewResult);
            Assert.NotNull(pagedResults);
            Assert.Equal(0, pagedResults.PagesCount);
            Assert.Equal(0, pagedResults.TotalItemsCount);
            Assert.Empty(pagedResults.Results);
        }
        
        [Fact]
        public async Task With_empty_database_and_invalid_paging_list_is_empty()
        {
            var controller = new BooksController(CurrentDbContext);
            var viewResult = await controller.Index(page: -1, pageSize: -5) as ViewResult;
            var pagedResults = viewResult?.Model as PagedResults<Book>;
            
            Assert.NotNull(viewResult);
            Assert.NotNull(pagedResults);
            Assert.Equal(1, pagedResults.Page);
            Assert.Equal(25, pagedResults.PageSize);
            Assert.Equal(0, pagedResults.PagesCount);
            Assert.Equal(0, pagedResults.TotalItemsCount);;
            Assert.Empty(pagedResults.Results);
        }
        
        [Fact]
        public async Task With_pre_filled_database_list_contains_expected_data()
        {
            var expectedBooks = new[] {"IT", "Bosch", "Spillover"};
            
            //Arrange
            foreach (var expectedBook in expectedBooks)
            {
                await CurrentDbContext.Books.AddAsync(new Book
                {
                    Title = expectedBook
                });
            }
            await CurrentDbContext.SaveChangesAsync();
            
            //Act
            var controller = new BooksController(CurrentDbContext);
            var viewResult = await controller.Index() as ViewResult;
            var pagedResults = viewResult?.Model as PagedResults<Book>;
            
            //Assert
            Assert.NotNull(pagedResults);
            Assert.Contains(pagedResults.Results, result => expectedBooks.Contains(result.Title));
        }
        
        [Fact]
        public async Task With_pre_filled_database_list_is_sorted_alphabetically()
        {
            var expectedBooks = new[] {"IT", "Bosch", "Spillover"};
            
            //Arrange
            foreach (var expectedBook in expectedBooks)
            {
                await CurrentDbContext.Books.AddAsync(new Book
                {
                    Title = expectedBook
                });
            }
            await CurrentDbContext.SaveChangesAsync();
            
            //Act
            var controller = new BooksController(CurrentDbContext);
            var viewResult = await controller.Index() as ViewResult;
            var pagedResults = viewResult?.Model as PagedResults<Book>;
            
            //Assert
            Assert.NotNull(pagedResults);
            Assert.Equal(expectedBooks.OrderBy(a=> a), pagedResults.Results.Select(b=>b.Title));
        }
        
        [Fact]
        public async Task With_pre_filled_database_first_page_contains_expected_results()
        {
            var expectedBooks = new[]
            {
                "IT",
                "Bosch",
                "Spillover",
                "Factfulness",
                "The Open Organization",
                "Open",
                "Serve to win",
                "Lord of the rings"
            };
            
            //Arrange
            foreach (var expectedBook in expectedBooks)
            {
                await CurrentDbContext.Books.AddAsync(new Book
                {
                    Title = expectedBook
                });
            }
            await CurrentDbContext.SaveChangesAsync();
            
            //Act
            var controller = new BooksController(CurrentDbContext);
            var viewResult = await controller.Index(pageSize: 5) as ViewResult;
            var pagedResults = viewResult?.Model as PagedResults<Book>;
            
            //Assert
            Assert.NotNull(pagedResults);
            Assert.Equal(2, pagedResults.PagesCount);
            Assert.Equal(expectedBooks.Length, pagedResults.TotalItemsCount);
            Assert.Equal
            (
                expectedBooks.OrderBy(b => b).Take(5),
                pagedResults.Results.Select(a => a.Title)
            );
        }
        
        [Fact]
        public async Task With_pre_filled_database_second_page_contains_expected_results()
        {
            var expectedBooks = new[]
            {
                "IT",
                "Bosch",
                "Spillover",
                "Factfulness",
                "The Open Organization",
                "Open",
                "Serve to win",
                "Lord of the rings"
            };
            
            //Arrange
            foreach (var expectedBook in expectedBooks)
            {
                await CurrentDbContext.Books.AddAsync(new Book
                {
                    Title = expectedBook
                });
            }
            await CurrentDbContext.SaveChangesAsync();
            
            //Act
            var controller = new BooksController(CurrentDbContext);
            var viewResult = await controller.Index(page: 2, pageSize: 5) as ViewResult;
            var pagedResults = viewResult?.Model as PagedResults<Book>;
            
            //Assert
            Assert.NotNull(pagedResults);
            Assert.Equal(2, pagedResults.PagesCount);
            Assert.Equal(2, pagedResults.Page);
            Assert.Equal(expectedBooks.Length, pagedResults.TotalItemsCount);
            Assert.Equal
            (
                expectedBooks.OrderBy(b => b).Skip(5).Take(5),
                pagedResults.Results.Select(b => b.Title)
            );
        }
    }
}