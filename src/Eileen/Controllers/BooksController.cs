using System.Linq;
using System.Threading.Tasks;
using Eileen.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eileen.Controllers
{
    [Authorize, Route("Books")]
    public class BooksController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public BooksController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, int pageSize = 25)
        {
            if (page <= 0)
            {
                page = 1;
            }

            if (pageSize <= 0)
            {
                pageSize = 25;
            }

            var authors = await _dbContext.Books
                .OrderBy(a => a.Title)
                .ToPagedResultsAsync(page, pageSize);

            return View(authors);
        }
    }
}