using System.Threading.Tasks;
using Eileen.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Eileen.Controllers
{
    [Authorize, Route("Book")]
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _dbContext;

        public BookController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Index(int id)
        {
            var book = await _dbContext.Books
                .Include(b => b.Author)
                .Include(b => b.Publisher)
                .SingleOrDefaultAsync(b => b.Id == id);

            //TODO handle book not found
            return View(book);
        }
    }
}