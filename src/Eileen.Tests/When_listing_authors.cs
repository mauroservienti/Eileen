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
    public class When_listing_authors : AbstractDatabaseTest
    {
        public When_listing_authors(ITestOutputHelper outputHelper) : base(outputHelper)
        {
            CurrentDbContext.Database.Migrate();
        }
        
        [Fact]
        public async Task With_empty_database_and_default_paging_list_is_empty()
        {
            var controller = new AuthorsController(CurrentDbContext);
            var viewResult = await controller.List() as ViewResult;
            var pagedResults = viewResult?.Model as PagedResults<AuthorsWithBooksCount>;
            
            Assert.NotNull(viewResult);
            Assert.NotNull(pagedResults);
            Assert.Equal(0, pagedResults.PageCount);
            Assert.Equal(0, pagedResults.RowCount);
            Assert.Empty(pagedResults.Results);
        }
        
        [Fact]
        public async Task With_empty_database_and_invalid_paging_list_is_empty()
        {
            var controller = new AuthorsController(CurrentDbContext);
            var viewResult = await controller.List(page: -1, pageSize: -5) as ViewResult;
            var pagedResults = viewResult?.Model as PagedResults<AuthorsWithBooksCount>;
            
            Assert.NotNull(viewResult);
            Assert.NotNull(pagedResults);
            Assert.Equal(1, pagedResults.CurrentPage);
            Assert.Equal(25, pagedResults.PageSize);
            Assert.Equal(0, pagedResults.PageCount);
            Assert.Equal(0, pagedResults.RowCount);;
            Assert.Empty(pagedResults.Results);
        }
        
        [Fact]
        public async Task With_pre_filled_database_list_contains_expected_data()
        {
            var expectedAuthors = new[] {"Stephen King", "Michael Connelly", "John le Carré"};
            
            //Arrange
            foreach (var expectedAuthor in expectedAuthors)
            {
                await CurrentDbContext.Authors.AddAsync(new Author
                {
                    Name = expectedAuthor
                });
            }
            await CurrentDbContext.SaveChangesAsync();
            
            //Act
            var controller = new AuthorsController(CurrentDbContext);
            var viewResult = await controller.List() as ViewResult;
            var pagedResults = viewResult?.Model as PagedResults<AuthorsWithBooksCount>;
            
            //Assert
            Assert.NotNull(pagedResults);
            Assert.Contains(pagedResults.Results, result => expectedAuthors.Contains(result.Name));
        }
        
        [Fact]
        public async Task With_pre_filled_database_list_is_sorted_alphabetically()
        {
            var expectedAuthors = new[] {"Stephen King", "Michael Connelly", "John le Carré"};
            
            //Arrange
            foreach (var expectedAuthor in expectedAuthors)
            {
                await CurrentDbContext.Authors.AddAsync(new Author
                {
                    Name = expectedAuthor
                });
            }
            await CurrentDbContext.SaveChangesAsync();
            
            //Act
            var controller = new AuthorsController(CurrentDbContext);
            var viewResult = await controller.List() as ViewResult;
            var pagedResults = viewResult?.Model as PagedResults<AuthorsWithBooksCount>;
            
            //Assert
            Assert.NotNull(pagedResults);
            Assert.Equal(expectedAuthors.OrderBy(a=> a), pagedResults.Results.Select(a=>a.Name));
        }
    }
}