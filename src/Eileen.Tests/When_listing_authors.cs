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
        public async Task With_default_paging_and_database_is_empty_list_is_empty()
        {
            var controller = new AuthorsController(CurrentDbContext);
            var viewResult = await controller.List() as ViewResult;
            var pagedResult = viewResult?.Model as PagedResults<AuthorsWithBooksCount>;
            
            Assert.NotNull(viewResult);
            Assert.NotNull(pagedResult);
            Assert.Empty(pagedResult.Results);
        }
    }
}